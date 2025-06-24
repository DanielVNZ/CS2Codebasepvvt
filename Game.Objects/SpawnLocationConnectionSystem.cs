using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class SpawnLocationConnectionSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindUpdatedSpawnLocationsJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<SpawnLocation> m_SpawnLocationData;

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
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && m_SpawnLocationData.HasComponent(entity))
				{
					m_ResultQueue.Enqueue(entity);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public ComponentLookup<SpawnLocation> m_SpawnLocationData;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Bounds = MathUtils.Expand(m_Bounds[index], float2.op_Implicit(32f)),
				m_SpawnLocationData = m_SpawnLocationData,
				m_ResultQueue = m_ResultQueue
			};
			m_ObjectSearchTree.Iterate<Iterator>(ref iterator, 0);
		}
	}

	[BurstCompile]
	private struct CheckUpdatedSpawnLocationsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<RoadConnectionUpdated> m_RoadConnectionUpdatedType;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocations;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<RoadConnectionUpdated> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RoadConnectionUpdated>(ref m_RoadConnectionUpdatedType);
			if (nativeArray.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity building = nativeArray[i].m_Building;
					DynamicBuffer<SpawnLocationElement> val = m_SpawnLocations[building];
					for (int j = 0; j < val.Length; j++)
					{
						if (val[j].m_Type == SpawnLocationType.SpawnLocation)
						{
							m_ResultQueue.Enqueue(val[j].m_SpawnLocation);
						}
					}
				}
			}
			else
			{
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int k = 0; k < nativeArray2.Length; k++)
				{
					m_ResultQueue.Enqueue(nativeArray2[k]);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ListUpdatedSpawnLocationsJob : IJob
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

		public NativeQueue<Entity> m_UpdatedQueue4;

		public NativeList<Entity> m_UpdatedList;

		public void Execute()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			int count = m_UpdatedQueue1.Count;
			int num = count + m_UpdatedQueue2.Count;
			int num2 = num + m_UpdatedQueue3.Count;
			int num3 = num2 + m_UpdatedQueue4.Count;
			m_UpdatedList.ResizeUninitialized(num3);
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
			for (int l = num2; l < num3; l++)
			{
				m_UpdatedList[l] = m_UpdatedQueue4.Dequeue();
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_UpdatedList, default(EntityComparer));
			Entity val = Entity.Null;
			int num4 = 0;
			int num5 = 0;
			while (num4 < m_UpdatedList.Length)
			{
				Entity val2 = m_UpdatedList[num4++];
				if (val2 != val)
				{
					m_UpdatedList[num5++] = val2;
					val = val2;
				}
			}
			if (num5 < m_UpdatedList.Length)
			{
				m_UpdatedList.RemoveRangeSwapBack(num5, m_UpdatedList.Length - num5);
			}
		}
	}

	[BurstCompile]
	private struct FindSpawnLocationConnectionJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float3 m_Position;

			public float m_MaxDistance;

			public SpawnLocationData m_SpawnLocationData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

			public ComponentLookup<SlaveLane> m_SlaveLaneData;

			public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

			public ComponentLookup<Game.Areas.Lot> m_LotData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

			public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

			public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

			public BufferLookup<Game.Net.SubLane> m_Lanes;

			public BufferLookup<Game.Areas.Node> m_AreaNodes;

			public BufferLookup<Triangle> m_AreaTriangles;

			public SpawnLocation m_BestLocation;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && TryFindLanes(entity, out var distance, out var spawnLocation) && distance < m_MaxDistance)
				{
					m_Bounds = new Bounds3(m_Position - distance, m_Position + distance);
					m_MaxDistance = distance;
					m_BestLocation = spawnLocation;
				}
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_030a: Unknown result type (might be due to invalid IL or missing references)
				//IL_030f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_031d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0324: Unknown result type (might be due to invalid IL or missing references)
				//IL_032a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0331: Unknown result type (might be due to invalid IL or missing references)
				//IL_0336: Unknown result type (might be due to invalid IL or missing references)
				//IL_033b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_0212: Unknown result type (might be due to invalid IL or missing references)
				//IL_021f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0230: Unknown result type (might be due to invalid IL or missing references)
				//IL_023e: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0260: Unknown result type (might be due to invalid IL or missing references)
				//IL_026e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0273: Unknown result type (might be due to invalid IL or missing references)
				//IL_028e: Unknown result type (might be due to invalid IL or missing references)
				//IL_029d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_Lanes.HasBuffer(item.m_Area))
				{
					return;
				}
				DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[item.m_Area];
				Triangle triangle = m_AreaTriangles[item.m_Area][item.m_Triangle];
				Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
				float2 val = default(float2);
				float num = MathUtils.Distance(triangle2, m_Position, ref val);
				if (num >= m_MaxDistance)
				{
					return;
				}
				float3 val2 = MathUtils.Position(triangle2, val);
				if (m_LotData.HasComponent(item.m_Area))
				{
					bool3 val3 = AreaUtils.IsEdge(nodes, triangle);
					val3 &= (math.cmin(((int3)(ref triangle.m_Indices)).xy) != 0) | (math.cmax(((int3)(ref triangle.m_Indices)).xy) != 1);
					val3 &= (math.cmin(((int3)(ref triangle.m_Indices)).yz) != 0) | (math.cmax(((int3)(ref triangle.m_Indices)).yz) != 1);
					val3 &= (math.cmin(((int3)(ref triangle.m_Indices)).zx) != 0) | (math.cmax(((int3)(ref triangle.m_Indices)).zx) != 1);
					float num2 = default(float);
					if ((val3.x && MathUtils.Distance(((Triangle3)(ref triangle2)).ab, val2, ref num2) < 0.1f) || (val3.y && MathUtils.Distance(((Triangle3)(ref triangle2)).bc, val2, ref num2) < 0.1f) || (val3.z && MathUtils.Distance(((Triangle3)(ref triangle2)).ca, val2, ref num2) < 0.1f))
					{
						return;
					}
				}
				DynamicBuffer<Game.Net.SubLane> val4 = m_Lanes[item.m_Area];
				SpawnLocation bestLocation = default(SpawnLocation);
				float num3 = float.MaxValue;
				bool2 val5 = default(bool2);
				float2 val6 = default(float2);
				float num5 = default(float);
				for (int i = 0; i < val4.Length; i++)
				{
					Entity subLane = val4[i].m_SubLane;
					if (!m_ConnectionLaneData.HasComponent(subLane) || !CheckLaneType(m_ConnectionLaneData[subLane]))
					{
						continue;
					}
					Curve curve = m_CurveData[subLane];
					((bool2)(ref val5))._002Ector(MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val6), MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val6));
					if (math.any(val5))
					{
						float num4 = MathUtils.Distance(curve.m_Bezier, val2, ref num5);
						if (num4 < num3)
						{
							float2 val7 = math.select(new float2(0f, 0.49f), math.select(new float2(0.51f, 1f), new float2(0f, 1f), val5.x), val5.y);
							num3 = num4;
							bestLocation.m_ConnectedLane1 = subLane;
							bestLocation.m_CurvePosition1 = math.clamp(num5, val7.x, val7.y);
						}
					}
				}
				if (bestLocation.m_ConnectedLane1 != Entity.Null)
				{
					m_Bounds = new Bounds3(m_Position - num, m_Position + num);
					m_MaxDistance = num;
					m_BestLocation = bestLocation;
				}
			}

			private bool CheckLaneType(Game.Net.ConnectionLane connectionLane)
			{
				switch (m_SpawnLocationData.m_ConnectionType)
				{
				case RouteConnectionType.Pedestrian:
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						return true;
					}
					return false;
				case RouteConnectionType.Road:
				case RouteConnectionType.Cargo:
				case RouteConnectionType.Parking:
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0 && (connectionLane.m_RoadTypes & m_SpawnLocationData.m_RoadTypes) != RoadTypes.None)
					{
						return true;
					}
					return false;
				case RouteConnectionType.Track:
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Track) != 0 && (connectionLane.m_TrackTypes & m_SpawnLocationData.m_TrackTypes) != TrackTypes.None)
					{
						return true;
					}
					return false;
				default:
					return false;
				}
			}

			private bool CheckLaneType(Entity lane)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				PrefabRef prefabRef = m_PrefabRefData[lane];
				NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
				switch (m_SpawnLocationData.m_ConnectionType)
				{
				case RouteConnectionType.Pedestrian:
					if ((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
					{
						return true;
					}
					return false;
				case RouteConnectionType.Road:
				case RouteConnectionType.Cargo:
				case RouteConnectionType.Parking:
					if ((netLaneData.m_Flags & LaneFlags.Road) != 0 && (m_PrefabCarLaneData[prefabRef.m_Prefab].m_RoadTypes & m_SpawnLocationData.m_RoadTypes) != RoadTypes.None)
					{
						return true;
					}
					return false;
				case RouteConnectionType.Track:
					if ((netLaneData.m_Flags & LaneFlags.Track) != 0 && (m_PrefabTrackLaneData[prefabRef.m_Prefab].m_TrackTypes & m_SpawnLocationData.m_TrackTypes) != TrackTypes.None)
					{
						return true;
					}
					return false;
				default:
					return false;
				}
			}

			public bool TryFindLanes(Entity entity, out float distance, out SpawnLocation spawnLocation)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_015b: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				distance = float.MaxValue;
				spawnLocation = default(SpawnLocation);
				if (!m_Lanes.HasBuffer(entity))
				{
					return false;
				}
				DynamicBuffer<Game.Net.SubLane> val = m_Lanes[entity];
				float curvePosition = default(float);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (CheckLaneType(subLane))
					{
						float num = MathUtils.Distance(m_CurveData[subLane].m_Bezier, m_Position, ref curvePosition);
						if (num < distance)
						{
							distance = num;
							spawnLocation.m_ConnectedLane1 = subLane;
							spawnLocation.m_CurvePosition1 = curvePosition;
						}
					}
				}
				if (m_SlaveLaneData.HasComponent(spawnLocation.m_ConnectedLane1))
				{
					SlaveLane slaveLane = m_SlaveLaneData[spawnLocation.m_ConnectedLane1];
					if (slaveLane.m_MasterIndex < val.Length)
					{
						spawnLocation.m_ConnectedLane1 = val[(int)slaveLane.m_MasterIndex].m_SubLane;
					}
				}
				if (spawnLocation.m_ConnectedLane1 != Entity.Null && (m_SpawnLocationData.m_ConnectionType == RouteConnectionType.Road || m_SpawnLocationData.m_ConnectionType == RouteConnectionType.Cargo || m_SpawnLocationData.m_ConnectionType == RouteConnectionType.Parking))
				{
					Game.Net.CarLane carLane = m_CarLaneData[spawnLocation.m_ConnectedLane1];
					for (int j = 0; j < val.Length; j++)
					{
						Entity subLane2 = val[j].m_SubLane;
						if (!(subLane2 == spawnLocation.m_ConnectedLane1) && m_CarLaneData.HasComponent(subLane2) && !m_SlaveLaneData.HasComponent(subLane2))
						{
							Game.Net.CarLane carLane2 = m_CarLaneData[subLane2];
							if (carLane.m_CarriagewayGroup == carLane2.m_CarriagewayGroup)
							{
								spawnLocation.m_ConnectedLane2 = subLane2;
								spawnLocation.m_CurvePosition2 = math.select(spawnLocation.m_CurvePosition1, 1f - spawnLocation.m_CurvePosition1, ((carLane.m_Flags ^ carLane2.m_Flags) & CarLaneFlags.Invert) != 0);
								break;
							}
						}
					}
				}
				return spawnLocation.m_ConnectedLane1 != Entity.Null;
			}
		}

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<MovedLocation> m_MovedLocationData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_LotData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocations;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_Lanes;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public AirwayHelpers.AirwayData m_AirwayData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			if (!m_SpawnLocationData.HasComponent(val))
			{
				return;
			}
			if (m_UpdatedData.HasComponent(val) && m_MovedLocationData.HasComponent(val))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<MovedLocation>(index, val);
			}
			Transform transform = m_TransformData[val];
			PrefabRef prefabRef = m_PrefabRefData[val];
			SpawnLocationData spawnLocationData = default(SpawnLocationData);
			if (m_PrefabSpawnLocationData.TryGetComponent(prefabRef.m_Prefab, ref spawnLocationData))
			{
				if (spawnLocationData.m_ConnectionType == RouteConnectionType.Air)
				{
					SpawnLocation spawnLocation = default(SpawnLocation);
					float distance = float.MaxValue;
					if ((spawnLocationData.m_RoadTypes & RoadTypes.Helicopter) != RoadTypes.None)
					{
						m_AirwayData.helicopterMap.FindClosestLane(transform.m_Position, m_CurveData, ref spawnLocation.m_ConnectedLane1, ref spawnLocation.m_CurvePosition1, ref distance);
					}
					if ((spawnLocationData.m_RoadTypes & RoadTypes.Airplane) != RoadTypes.None)
					{
						m_AirwayData.airplaneMap.FindClosestLane(transform.m_Position, m_CurveData, ref spawnLocation.m_ConnectedLane1, ref spawnLocation.m_CurvePosition1, ref distance);
					}
					SetSpawnLocation(index, val, spawnLocation);
					return;
				}
				if (m_PrefabRouteConnectionData.HasComponent(prefabRef.m_Prefab))
				{
					RouteConnectionData routeConnectionData = m_PrefabRouteConnectionData[prefabRef.m_Prefab];
					if (spawnLocationData.m_ConnectionType != RouteConnectionType.None && spawnLocationData.m_ConnectionType == routeConnectionData.m_AccessConnectionType && spawnLocationData.m_ActivityMask.m_Mask == 0)
					{
						SetSpawnLocation(index, val, default(SpawnLocation));
						return;
					}
				}
				Iterator iterator = new Iterator
				{
					m_Bounds = new Bounds3(transform.m_Position - 32f, transform.m_Position + 32f),
					m_Position = transform.m_Position,
					m_MaxDistance = 32f,
					m_SpawnLocationData = spawnLocationData,
					m_CurveData = m_CurveData,
					m_CarLaneData = m_CarLaneData,
					m_SlaveLaneData = m_SlaveLaneData,
					m_ConnectionLaneData = m_ConnectionLaneData,
					m_LotData = m_LotData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabNetLaneData = m_PrefabNetLaneData,
					m_PrefabCarLaneData = m_PrefabCarLaneData,
					m_PrefabTrackLaneData = m_PrefabTrackLaneData,
					m_Lanes = m_Lanes,
					m_AreaNodes = m_AreaNodes,
					m_AreaTriangles = m_AreaTriangles
				};
				if (m_AttachedData.HasComponent(val) && iterator.TryFindLanes(m_AttachedData[val].m_Parent, out var distance2, out var spawnLocation2))
				{
					SetSpawnLocation(index, val, spawnLocation2);
					return;
				}
				if (m_OwnerData.HasComponent(val) && iterator.TryFindLanes(m_OwnerData[val].m_Owner, out distance2, out var spawnLocation3))
				{
					SetSpawnLocation(index, val, spawnLocation3);
					return;
				}
				m_NetSearchTree.Iterate<Iterator>(ref iterator, 0);
				m_AreaSearchTree.Iterate<Iterator>(ref iterator, 0);
				if (iterator.m_BestLocation.m_ConnectedLane1 != Entity.Null)
				{
					SetSpawnLocation(index, val, iterator.m_BestLocation);
					return;
				}
				if (m_OwnerData.HasComponent(val))
				{
					Owner owner = m_OwnerData[val];
					if (m_BuildingData.HasComponent(owner.m_Owner) && ShouldConnectToBuildingRoad(spawnLocationData, transform.m_Position, val, owner.m_Owner) && iterator.TryFindLanes(m_BuildingData[owner.m_Owner].m_RoadEdge, out distance2, out var spawnLocation4))
					{
						SetSpawnLocation(index, val, spawnLocation4);
						return;
					}
				}
			}
			SetSpawnLocation(index, val, default(SpawnLocation));
		}

		private bool ShouldConnectToBuildingRoad(SpawnLocationData spawnLocationData, float3 position, Entity entity, Entity owner)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			switch (spawnLocationData.m_ConnectionType)
			{
			case RouteConnectionType.Road:
			case RouteConnectionType.Parking:
				if ((spawnLocationData.m_RoadTypes & RoadTypes.Car) == 0)
				{
					return false;
				}
				break;
			case RouteConnectionType.None:
			case RouteConnectionType.Track:
			case RouteConnectionType.Air:
				return false;
			}
			if (spawnLocationData.m_ActivityMask.m_Mask != 0)
			{
				return false;
			}
			if (m_SpawnLocations.HasBuffer(owner))
			{
				DynamicBuffer<SpawnLocationElement> val = m_SpawnLocations[owner];
				PrefabRef prefabRef = m_PrefabRefData[owner];
				BuildingData buildingData = m_PrefabBuildingData[prefabRef.m_Prefab];
				float3 val2 = BuildingUtils.CalculateFrontPosition(m_TransformData[owner], buildingData.m_LotSize.y);
				float num = math.distance(position, val2);
				for (int i = 0; i < val.Length; i++)
				{
					if (val[i].m_Type != SpawnLocationType.SpawnLocation)
					{
						continue;
					}
					Entity spawnLocation = val[i].m_SpawnLocation;
					if (entity == spawnLocation)
					{
						continue;
					}
					PrefabRef prefabRef2 = m_PrefabRefData[spawnLocation];
					SpawnLocationData spawnLocationData2 = m_PrefabSpawnLocationData[prefabRef2.m_Prefab];
					if (spawnLocationData2.m_ConnectionType != spawnLocationData.m_ConnectionType || ((spawnLocationData.m_ConnectionType == RouteConnectionType.Road || spawnLocationData.m_ConnectionType == RouteConnectionType.Parking) && (spawnLocationData2.m_RoadTypes & spawnLocationData.m_RoadTypes) == 0) || spawnLocationData2.m_ActivityMask.m_Mask != 0)
					{
						continue;
					}
					if (m_PrefabRouteConnectionData.HasComponent(prefabRef2.m_Prefab))
					{
						RouteConnectionData routeConnectionData = m_PrefabRouteConnectionData[prefabRef2.m_Prefab];
						if (spawnLocationData.m_ConnectionType == routeConnectionData.m_AccessConnectionType)
						{
							continue;
						}
					}
					if (m_TransformData.HasComponent(spawnLocation) && math.distance(m_TransformData[spawnLocation].m_Position, val2) < num)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void SetSpawnLocation(int jobIndex, Entity entity, SpawnLocation spawnLocation)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			SpawnLocation spawnLocation2 = m_SpawnLocationData[entity];
			if (spawnLocation2.m_ConnectedLane1 != spawnLocation.m_ConnectedLane1 || spawnLocation2.m_ConnectedLane2 != spawnLocation.m_ConnectedLane2 || spawnLocation2.m_CurvePosition1 != spawnLocation.m_CurvePosition1 || spawnLocation2.m_CurvePosition2 != spawnLocation.m_CurvePosition2 || m_UpdatedData.HasComponent(spawnLocation.m_ConnectedLane1) || m_UpdatedData.HasComponent(spawnLocation.m_ConnectedLane2))
			{
				spawnLocation.m_AccessRestriction = spawnLocation2.m_AccessRestriction;
				spawnLocation.m_GroupIndex = spawnLocation2.m_GroupIndex;
				m_SpawnLocationData[entity] = spawnLocation;
				if (!m_UpdatedData.HasComponent(entity))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, entity, default(PathfindUpdated));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RoadConnectionUpdated> __Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovedLocation> __Game_Objects_MovedLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		public ComponentLookup<SpawnLocation> __Game_Objects_SpawnLocation_RW_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocation>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadConnectionUpdated>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_MovedLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovedLocation>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Objects_SpawnLocation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocation>(false);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
		}
	}

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Net.UpdateCollectSystem m_NetUpdateCollectSystem;

	private AirwaySystem m_AirwaySystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Areas.UpdateCollectSystem m_AreaUpdateCollectSystem;

	private SearchSystem m_ObjectSearchSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_UpdatedQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_NetUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.UpdateCollectSystem>();
		m_AirwaySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirwaySystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_AreaUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.UpdateCollectSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<SpawnLocation>(),
			ComponentType.ReadOnly<Updated>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<RoadConnectionUpdated>(),
			ComponentType.ReadOnly<Event>()
		};
		array[1] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((EntityQuery)(ref m_UpdatedQuery)).IsEmptyIgnoreFilter;
		if (flag || m_NetUpdateCollectSystem.netsUpdated || m_AreaUpdateCollectSystem.lotsUpdated || m_AreaUpdateCollectSystem.spacesUpdated)
		{
			NativeQueue<Entity> updatedQueue = default(NativeQueue<Entity>);
			updatedQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Entity> updatedQueue2 = default(NativeQueue<Entity>);
			updatedQueue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Entity> updatedQueue3 = default(NativeQueue<Entity>);
			updatedQueue3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Entity> updatedQueue4 = default(NativeQueue<Entity>);
			updatedQueue4._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val2 = default(JobHandle);
			if (m_NetUpdateCollectSystem.netsUpdated)
			{
				JobHandle dependencies;
				NativeList<Bounds2> updatedNetBounds = m_NetUpdateCollectSystem.GetUpdatedNetBounds(out dependencies);
				JobHandle dependencies2;
				JobHandle val3 = IJobParallelForDeferExtensions.Schedule<FindUpdatedSpawnLocationsJob, Bounds2>(new FindUpdatedSpawnLocationsJob
				{
					m_Bounds = updatedNetBounds.AsDeferredJobArray(),
					m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies2),
					m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue.AsParallelWriter()
				}, updatedNetBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
				m_NetUpdateCollectSystem.AddNetBoundsReader(val3);
				m_ObjectSearchSystem.AddStaticSearchTreeReader(val3);
				val2 = JobHandle.CombineDependencies(val2, val3);
			}
			if (m_AreaUpdateCollectSystem.lotsUpdated)
			{
				JobHandle dependencies3;
				NativeList<Bounds2> updatedLotBounds = m_AreaUpdateCollectSystem.GetUpdatedLotBounds(out dependencies3);
				JobHandle dependencies4;
				JobHandle val4 = IJobParallelForDeferExtensions.Schedule<FindUpdatedSpawnLocationsJob, Bounds2>(new FindUpdatedSpawnLocationsJob
				{
					m_Bounds = updatedLotBounds.AsDeferredJobArray(),
					m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies4),
					m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue2.AsParallelWriter()
				}, updatedLotBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies3, dependencies4));
				m_AreaUpdateCollectSystem.AddLotBoundsReader(val4);
				m_ObjectSearchSystem.AddStaticSearchTreeReader(val4);
				val2 = JobHandle.CombineDependencies(val2, val4);
			}
			if (m_AreaUpdateCollectSystem.spacesUpdated)
			{
				JobHandle dependencies5;
				NativeList<Bounds2> updatedSpaceBounds = m_AreaUpdateCollectSystem.GetUpdatedSpaceBounds(out dependencies5);
				JobHandle dependencies6;
				JobHandle val5 = IJobParallelForDeferExtensions.Schedule<FindUpdatedSpawnLocationsJob, Bounds2>(new FindUpdatedSpawnLocationsJob
				{
					m_Bounds = updatedSpaceBounds.AsDeferredJobArray(),
					m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies6),
					m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue3.AsParallelWriter()
				}, updatedSpaceBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies5, dependencies6));
				m_AreaUpdateCollectSystem.AddSpaceBoundsReader(val5);
				m_ObjectSearchSystem.AddStaticSearchTreeReader(val5);
				val2 = JobHandle.CombineDependencies(val2, val5);
			}
			if (flag)
			{
				JobHandle val6 = JobChunkExtensions.ScheduleParallel<CheckUpdatedSpawnLocationsJob>(new CheckUpdatedSpawnLocationsJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RoadConnectionUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<RoadConnectionUpdated>(ref __TypeHandle.__Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SpawnLocations = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ResultQueue = updatedQueue4.AsParallelWriter()
				}, m_UpdatedQuery, ((SystemBase)this).Dependency);
				val2 = JobHandle.CombineDependencies(val2, val6);
			}
			ListUpdatedSpawnLocationsJob listUpdatedSpawnLocationsJob = new ListUpdatedSpawnLocationsJob
			{
				m_UpdatedQueue1 = updatedQueue,
				m_UpdatedQueue2 = updatedQueue2,
				m_UpdatedQueue3 = updatedQueue3,
				m_UpdatedQueue4 = updatedQueue4,
				m_UpdatedList = val
			};
			JobHandle dependencies7;
			JobHandle dependencies8;
			FindSpawnLocationConnectionJob findSpawnLocationConnectionJob = new FindSpawnLocationConnectionJob
			{
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MovedLocationData = InternalCompilerInterface.GetComponentLookup<MovedLocation>(ref __TypeHandle.__Game_Objects_MovedLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LotData = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnLocations = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Lanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Entities = val.AsDeferredJobArray(),
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies7),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies8),
				m_AirwayData = m_AirwaySystem.GetAirwayData()
			};
			EntityCommandBuffer val7 = m_ModificationBarrier.CreateCommandBuffer();
			findSpawnLocationConnectionJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val7)).AsParallelWriter();
			FindSpawnLocationConnectionJob findSpawnLocationConnectionJob2 = findSpawnLocationConnectionJob;
			JobHandle val8 = IJobExtensions.Schedule<ListUpdatedSpawnLocationsJob>(listUpdatedSpawnLocationsJob, val2);
			JobHandle val9 = IJobParallelForDeferExtensions.Schedule<FindSpawnLocationConnectionJob, Entity>(findSpawnLocationConnectionJob2, val, 1, JobHandle.CombineDependencies(val8, dependencies7, dependencies8));
			updatedQueue.Dispose(val8);
			updatedQueue2.Dispose(val8);
			updatedQueue3.Dispose(val8);
			updatedQueue4.Dispose(val8);
			val.Dispose(val9);
			m_NetSearchSystem.AddNetSearchTreeReader(val9);
			m_AreaSearchSystem.AddSearchTreeReader(val9);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val9);
			((SystemBase)this).Dependency = val9;
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
	public SpawnLocationConnectionSystem()
	{
	}
}

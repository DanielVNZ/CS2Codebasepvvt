using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GroundHeightSystem : GameSystemBase, IJobSerializable
{
	private enum LoadHeightsState
	{
		Loaded,
		Pending,
		Reading,
		Ready
	}

	[BurstCompile]
	internal struct SerializeJob<TWriter> : IJob where TWriter : struct, IWriter
	{
		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeList<Bounds2> m_NewUpdates;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeList<Bounds2> m_PendingUpdates;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeList<Bounds2> m_ReadingUpdates;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeList<Bounds2> m_ReadyUpdates;

		public EntityWriterData m_WriterData;

		public void Execute()
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			TWriter writer = ((EntityWriterData)(ref m_WriterData)).GetWriter<TWriter>();
			int num = 0;
			if (m_NewUpdates.IsCreated)
			{
				num += m_NewUpdates.Length;
			}
			if (m_PendingUpdates.IsCreated)
			{
				num += m_PendingUpdates.Length;
			}
			if (m_ReadingUpdates.IsCreated)
			{
				num += m_ReadingUpdates.Length;
			}
			if (m_ReadyUpdates.IsCreated)
			{
				num += m_ReadyUpdates.Length;
			}
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
			if (m_NewUpdates.IsCreated)
			{
				for (int i = 0; i < m_NewUpdates.Length; i++)
				{
					Bounds2 val = m_NewUpdates[i];
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val.min);
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val.max);
				}
			}
			if (m_PendingUpdates.IsCreated)
			{
				for (int j = 0; j < m_PendingUpdates.Length; j++)
				{
					Bounds2 val2 = m_PendingUpdates[j];
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val2.min);
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val2.max);
				}
			}
			if (m_ReadingUpdates.IsCreated)
			{
				for (int k = 0; k < m_ReadingUpdates.Length; k++)
				{
					Bounds2 val3 = m_ReadingUpdates[k];
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val3.min);
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val3.max);
				}
			}
			if (m_ReadyUpdates.IsCreated)
			{
				for (int l = 0; l < m_ReadyUpdates.Length; l++)
				{
					Bounds2 val4 = m_ReadyUpdates[l];
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val4.min);
					((IWriter)writer/*cast due to .constrained prefix*/).Write(val4.max);
				}
			}
		}
	}

	[BurstCompile]
	internal struct DeserializeJob<TReader> : IJob where TReader : struct, IReader
	{
		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_NewUpdates;

		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_PendingUpdates;

		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_ReadingUpdates;

		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_ReadyUpdates;

		public EntityReaderData m_ReaderData;

		public void Execute()
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			if (m_NewUpdates.IsCreated)
			{
				m_NewUpdates.Clear();
			}
			if (m_PendingUpdates.IsCreated)
			{
				m_PendingUpdates.Clear();
			}
			if (m_ReadyUpdates.IsCreated)
			{
				m_ReadyUpdates.Clear();
			}
			TReader reader = ((EntityReaderData)(ref m_ReaderData)).GetReader<TReader>();
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_ReadingUpdates.ResizeUninitialized(num);
			Bounds2 val = default(Bounds2);
			for (int i = 0; i < num; i++)
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val.min);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val.max);
				m_ReadingUpdates[i] = val;
			}
		}
	}

	[BurstCompile]
	private struct SetDefaultsJob : IJob
	{
		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_NewUpdates;

		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_PendingUpdates;

		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_ReadingUpdates;

		[NativeDisableContainerSafetyRestriction]
		public NativeList<Bounds2> m_ReadyUpdates;

		public void Execute()
		{
			if (m_NewUpdates.IsCreated)
			{
				m_NewUpdates.Clear();
			}
			if (m_PendingUpdates.IsCreated)
			{
				m_PendingUpdates.Clear();
			}
			if (m_ReadingUpdates.IsCreated)
			{
				m_ReadingUpdates.Clear();
			}
			if (m_ReadyUpdates.IsCreated)
			{
				m_ReadyUpdates.Clear();
			}
		}
	}

	[BurstCompile]
	private struct BoundsFindJob : IJobParallelFor
	{
		private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public ParallelWriter<Entity> m_Queue;

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
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[entity];
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
				{
					if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.HasBase) != Game.Objects.GeometryFlags.None)
					{
						goto IL_007b;
					}
					if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.DeleteOverridden) != Game.Objects.GeometryFlags.None || (objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.Marker)) == 0)
					{
						return;
					}
				}
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				if (m_ElevationData.TryGetComponent(entity, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0)
				{
					return;
				}
				goto IL_007b;
				IL_007b:
				m_Queue.Enqueue(entity);
			}
		}

		private struct LaneIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Game.Objects.Elevation> m_ObjectElevationData;

			public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public ParallelWriter<Entity> m_Queue;

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
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
				{
					return;
				}
				Owner owner = default(Owner);
				if (m_OwnerData.TryGetComponent(entity, ref owner))
				{
					PrefabRef prefabRef = m_PrefabRefData[owner.m_Owner];
					Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
					ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
					if ((m_ObjectElevationData.TryGetComponent(owner.m_Owner, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0) || !m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) || ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.DeleteOverridden) == 0 && (objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.Marker)) != Game.Objects.GeometryFlags.None))
					{
						return;
					}
				}
				Game.Net.Elevation elevation2 = default(Game.Net.Elevation);
				if (!m_NetElevationData.TryGetComponent(entity, ref elevation2) || !math.all(elevation2.m_Elevation != float.MinValue))
				{
					m_Queue.Enqueue(entity);
				}
			}
		}

		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Game.Buildings.Lot> m_BuildingLotData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Game.Net.Elevation> m_ElevationData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

			public ParallelWriter<Entity> m_Queue;

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
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				Edge edge = default(Edge);
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || (m_ElevationData.HasComponent(entity) && (!m_EdgeData.TryGetComponent(entity, ref edge) || (m_ElevationData.HasComponent(edge.m_Start) && m_ElevationData.HasComponent(edge.m_End)))))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[entity];
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_PrefabNetGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData))
				{
					if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.OnWater) != 0)
					{
						return;
					}
					if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.FlattenTerrain) != 0)
					{
						bool flag = false;
						Entity val = entity;
						while (m_OwnerData.HasComponent(val))
						{
							val = m_OwnerData[val].m_Owner;
							if (m_BuildingLotData.HasComponent(val))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return;
						}
					}
				}
				m_Queue.Enqueue(entity);
			}
		}

		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public BufferLookup<Game.Areas.Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public ParallelWriter<Entity> m_Queue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && math.any(AreaUtils.GetElevations(m_Nodes[item.m_Area], m_Triangles[item.m_Area][item.m_Triangle]) == float.MinValue))
				{
					m_Queue.Enqueue(item.m_Area);
				}
			}
		}

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ObjectElevationData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Lot> m_BuildingLotData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public NativeList<Bounds2> m_ReadyUpdates;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		public ParallelWriter<Entity> m_Queue;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			Bounds2 bounds = m_ReadyUpdates[index];
			ObjectIterator objectIterator = new ObjectIterator
			{
				m_Bounds = bounds,
				m_ElevationData = m_ObjectElevationData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
				m_Queue = m_Queue
			};
			m_StaticObjectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
			LaneIterator laneIterator = new LaneIterator
			{
				m_Bounds = bounds,
				m_OwnerData = m_OwnerData,
				m_ObjectElevationData = objectIterator.m_ElevationData,
				m_NetElevationData = m_NetElevationData,
				m_PrefabRefData = objectIterator.m_PrefabRefData,
				m_PrefabObjectGeometryData = objectIterator.m_PrefabObjectGeometryData,
				m_Queue = m_Queue
			};
			m_LaneSearchTree.Iterate<LaneIterator>(ref laneIterator, 0);
			NetIterator netIterator = new NetIterator
			{
				m_Bounds = bounds,
				m_EdgeData = m_EdgeData,
				m_OwnerData = laneIterator.m_OwnerData,
				m_BuildingLotData = m_BuildingLotData,
				m_ElevationData = laneIterator.m_NetElevationData,
				m_PrefabRefData = laneIterator.m_PrefabRefData,
				m_PrefabNetGeometryData = m_PrefabNetGeometryData,
				m_Queue = m_Queue
			};
			m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
			AreaIterator areaIterator = new AreaIterator
			{
				m_Bounds = bounds,
				m_Nodes = m_AreaNodes,
				m_Triangles = m_Triangles,
				m_Queue = m_Queue
			};
			m_AreaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
			m_OwnerData = netIterator.m_OwnerData;
			m_ObjectElevationData = laneIterator.m_ObjectElevationData;
			m_BuildingLotData = netIterator.m_BuildingLotData;
			m_EdgeData = netIterator.m_EdgeData;
			m_NetElevationData = netIterator.m_ElevationData;
			m_PrefabRefData = netIterator.m_PrefabRefData;
			m_PrefabObjectGeometryData = objectIterator.m_PrefabObjectGeometryData;
			m_PrefabNetGeometryData = netIterator.m_PrefabNetGeometryData;
			m_AreaNodes = areaIterator.m_Nodes;
			m_Triangles = areaIterator.m_Triangles;
		}
	}

	[BurstCompile]
	private struct DequeueJob : IJob
	{
		public NativeQueue<Entity> m_Queue;

		public NativeList<Entity> m_List;

		public NativeList<Bounds2> m_ReadyUpdates;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> val = m_Queue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
			m_List.Capacity = val.Length;
			NativeSortExtension.Sort<Entity>(val);
			Entity val2 = Entity.Null;
			for (int i = 0; i < val.Length; i++)
			{
				Entity val3 = val[i];
				if (val3 != val2)
				{
					m_List.Add(ref val3);
					val2 = val3;
				}
			}
			m_ReadyUpdates.Clear();
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct UpdateHeightsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ObjectElevationData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<MovedLocation> m_MovedLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Lot> m_BuildingLotData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PrefabPlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> m_PrefabBuildingTerraformData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Transform> m_TransformData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Curve> m_CurveData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeList<Entity> m_List;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0876: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_List[index];
			Transform transform = default(Transform);
			Curve curve = default(Curve);
			Game.Net.Node node = default(Game.Net.Node);
			if (m_TransformData.TryGetComponent(val, ref transform))
			{
				bool flag = true;
				bool flag2 = false;
				PrefabRef prefabRef = m_PrefabRefData[val];
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				if (m_ObjectElevationData.TryGetComponent(val, ref elevation))
				{
					flag = (elevation.m_Flags & ElevationFlags.OnGround) != 0;
				}
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
				{
					flag2 = (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.HasBase) != 0;
					flag &= (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.DeleteOverridden) == 0 && (objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.Marker)) != 0;
				}
				if (flag)
				{
					bool angledSample;
					Transform transform2 = ObjectUtils.AdjustPosition(transform, ref elevation, prefabRef.m_Prefab, out angledSample, ref m_TerrainHeightData, ref m_WaterSurfaceData, ref m_PrefabPlaceableObjectData, ref m_PrefabObjectGeometryData);
					if (math.abs(transform2.m_Position.y - transform.m_Position.y) >= 0.01f || (angledSample && MathUtils.RotationAngle(transform2.m_Rotation, transform.m_Rotation) >= math.radians(0.1f)))
					{
						m_TransformData[val] = transform2;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
						flag2 = false;
						if (m_SpawnLocationData.HasComponent(val) && !m_MovedLocationData.HasComponent(val))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<MovedLocation>(index, val, new MovedLocation
							{
								m_OldPosition = transform.m_Position
							});
						}
						DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
						if (m_SubObjects.TryGetBuffer(val, ref subObjects) && (m_OwnerData.HasComponent(val) || m_EditorMode))
						{
							HandleSubObjects(index, subObjects, transform, transform2);
						}
					}
				}
				CullingInfo cullingInfo = default(CullingInfo);
				if (flag2 && m_CullingInfoData.TryGetComponent(val, ref cullingInfo))
				{
					float min = TerrainUtils.GetHeightRange(ref m_TerrainHeightData, cullingInfo.m_Bounds).min;
					if (min < cullingInfo.m_Bounds.min.y)
					{
						cullingInfo.m_Bounds.min.y = min;
						m_CullingInfoData[val] = cullingInfo;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(index, val, default(BatchesUpdated));
					}
				}
			}
			else if (m_CurveData.TryGetComponent(val, ref curve))
			{
				Edge edge = default(Edge);
				Bezier4x1 y;
				if (m_EdgeData.TryGetComponent(val, ref edge))
				{
					bool flag3 = m_NetElevationData.HasComponent(edge.m_Start);
					bool flag4 = m_NetElevationData.HasComponent(edge.m_End);
					bool flag5 = false;
					bool flag6 = false;
					PrefabRef prefabRef2 = m_PrefabRefData[val];
					NetGeometryData netGeometryData = default(NetGeometryData);
					if (m_PrefabNetGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref netGeometryData))
					{
						flag5 = (netGeometryData.m_Flags & Game.Net.GeometryFlags.FlattenTerrain) != 0;
					}
					bool flag7 = false;
					bool flag8 = false;
					if (GetLotOwner(val, out var lotOwner))
					{
						flag7 = IsFixedNode(edge.m_Start, val, lotOwner);
						flag8 = IsFixedNode(edge.m_End, val, lotOwner);
						flag3 = flag3 || flag7;
						flag4 = flag4 || flag8;
					}
					else
					{
						flag6 = flag5;
					}
					if (flag6)
					{
						return;
					}
					bool linearMiddle = flag3 || flag4 || m_NetElevationData.HasComponent(val);
					BuildingUtils.LotInfo lotInfo;
					Curve curve2 = ((!flag5) ? NetUtils.AdjustPosition(curve, flag3, linearMiddle, flag4, ref m_TerrainHeightData) : ((!GetOwnerLot(lotOwner, out lotInfo)) ? curve : NetUtils.AdjustPosition(curve, new bool2(flag3, flag7), linearMiddle, new bool2(flag4, flag8), ref lotInfo)));
					y = ((Bezier4x3)(ref curve2.m_Bezier)).y;
					float4 abcd = ((Bezier4x1)(ref y)).abcd;
					y = ((Bezier4x3)(ref curve.m_Bezier)).y;
					bool4 val2 = math.abs(abcd - ((Bezier4x1)(ref y)).abcd) >= 0.01f;
					if (!math.any(val2))
					{
						return;
					}
					m_CurveData[val] = curve2;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge.m_Start, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge.m_End, default(Updated));
					if (val2.x)
					{
						DynamicBuffer<ConnectedEdge> val3 = m_ConnectedEdges[edge.m_Start];
						for (int i = 0; i < val3.Length; i++)
						{
							Entity edge2 = val3[i].m_Edge;
							if (edge2 != val)
							{
								Edge edge3 = m_EdgeData[edge2];
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge2, default(Updated));
								if (edge3.m_Start != edge.m_Start)
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge3.m_Start, default(Updated));
								}
								if (edge3.m_End != edge.m_Start)
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge3.m_End, default(Updated));
								}
							}
						}
					}
					if (!val2.w)
					{
						return;
					}
					DynamicBuffer<ConnectedEdge> val4 = m_ConnectedEdges[edge.m_End];
					for (int j = 0; j < val4.Length; j++)
					{
						Entity edge4 = val4[j].m_Edge;
						if (edge4 != val)
						{
							Edge edge5 = m_EdgeData[edge4];
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge4, default(Updated));
							if (edge5.m_Start != edge.m_End)
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge5.m_Start, default(Updated));
							}
							if (edge5.m_End != edge.m_End)
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, edge5.m_End, default(Updated));
							}
						}
					}
				}
				else
				{
					bool2 val5 = bool2.op_Implicit(false);
					Game.Net.Elevation elevation2 = default(Game.Net.Elevation);
					if (m_NetElevationData.TryGetComponent(val, ref elevation2))
					{
						val5 = elevation2.m_Elevation != float.MinValue;
					}
					Curve curve3 = NetUtils.AdjustPosition(curve, val5.x, math.any(val5), val5.y, ref m_TerrainHeightData);
					y = ((Bezier4x3)(ref curve3.m_Bezier)).y;
					float4 abcd2 = ((Bezier4x1)(ref y)).abcd;
					y = ((Bezier4x3)(ref curve.m_Bezier)).y;
					if (math.any(math.abs(abcd2 - ((Bezier4x1)(ref y)).abcd) >= 0.01f))
					{
						m_CurveData[val] = curve3;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
					}
				}
			}
			else if (m_NodeData.TryGetComponent(val, ref node))
			{
				bool flag9 = false;
				bool flag10 = false;
				PrefabRef prefabRef3 = m_PrefabRefData[val];
				NetGeometryData netGeometryData2 = default(NetGeometryData);
				if (m_PrefabNetGeometryData.TryGetComponent(prefabRef3.m_Prefab, ref netGeometryData2))
				{
					flag9 = (netGeometryData2.m_Flags & Game.Net.GeometryFlags.FlattenTerrain) != 0;
				}
				if (!((!GetLotOwner(val, out var lotOwner2)) ? flag9 : IsFixedNode(val, Entity.Null, lotOwner2)))
				{
					BuildingUtils.LotInfo lotInfo2;
					Game.Net.Node node2 = ((!flag9) ? NetUtils.AdjustPosition(node, ref m_TerrainHeightData) : ((!GetOwnerLot(lotOwner2, out lotInfo2)) ? node : NetUtils.AdjustPosition(node, ref lotInfo2)));
					if (math.abs(node2.m_Position.y - node.m_Position.y) >= 0.01f)
					{
						m_NodeData[val] = node2;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
					}
				}
			}
			else
			{
				DynamicBuffer<Game.Areas.Node> val6 = default(DynamicBuffer<Game.Areas.Node>);
				if (!m_AreaNodes.TryGetBuffer(val, ref val6))
				{
					return;
				}
				bool flag11 = false;
				bool flag12 = false;
				PrefabRef prefabRef4 = m_PrefabRefData[val];
				AreaGeometryData areaGeometryData = default(AreaGeometryData);
				if (m_PrefabAreaGeometryData.TryGetComponent(prefabRef4.m_Prefab, ref areaGeometryData))
				{
					flag12 = (areaGeometryData.m_Flags & Game.Areas.GeometryFlags.OnWaterSurface) != 0;
					if (flag12 && (areaGeometryData.m_Flags & Game.Areas.GeometryFlags.ShiftTerrain) != 0)
					{
						return;
					}
				}
				for (int k = 0; k < val6.Length; k++)
				{
					ref Game.Areas.Node reference = ref val6.ElementAt(k);
					if (reference.m_Elevation == float.MinValue)
					{
						Game.Areas.Node node3 = ((!flag12) ? AreaUtils.AdjustPosition(reference, ref m_TerrainHeightData) : AreaUtils.AdjustPosition(reference, ref m_TerrainHeightData, ref m_WaterSurfaceData));
						bool flag13 = math.abs(node3.m_Position.y - reference.m_Position.y) >= 0.01f;
						reference.m_Position = math.select(reference.m_Position, node3.m_Position, flag13);
						flag11 = flag11 || flag13;
					}
				}
				if (flag11)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
				}
			}
		}

		private bool GetLotOwner(Entity entity, out Entity lotOwner)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = entity;
			PrefabRef prefabRef = default(PrefabRef);
			BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
			while (m_OwnerData.HasComponent(val))
			{
				val = m_OwnerData[val].m_Owner;
				if (m_BuildingLotData.HasComponent(val) && (!m_PrefabRefData.TryGetComponent(val, ref prefabRef) || !m_PrefabBuildingExtensionData.TryGetComponent(prefabRef.m_Prefab, ref buildingExtensionData) || buildingExtensionData.m_External))
				{
					lotOwner = val;
					return true;
				}
			}
			lotOwner = Entity.Null;
			return false;
		}

		private bool GetOwnerLot(Entity lotOwner, out BuildingUtils.LotInfo lotInfo)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			Game.Buildings.Lot lot = m_BuildingLotData[lotOwner];
			Transform transform = default(Transform);
			PrefabRef prefabRef = default(PrefabRef);
			if (m_TransformData.TryGetComponent(lotOwner, ref transform) && m_PrefabRefData.TryGetComponent(lotOwner, ref prefabRef))
			{
				BuildingData buildingData = default(BuildingData);
				bool hasExtensionLots;
				if (m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData))
				{
					float2 extents = new float2(buildingData.m_LotSize) * 4f;
					Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
					m_ObjectElevationData.TryGetComponent(lotOwner, ref elevation);
					DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
					m_InstalledUpgrades.TryGetBuffer(lotOwner, ref upgrades);
					lotInfo = BuildingUtils.CalculateLotInfo(extents, transform, elevation, lot, prefabRef, upgrades, m_TransformData, m_PrefabRefData, m_PrefabObjectGeometryData, m_PrefabBuildingTerraformData, m_PrefabBuildingExtensionData, defaultNoSmooth: false, out hasExtensionLots);
					return true;
				}
				BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
				if (m_PrefabBuildingExtensionData.TryGetComponent(prefabRef.m_Prefab, ref buildingExtensionData))
				{
					float2 extents2 = new float2(buildingExtensionData.m_LotSize) * 4f;
					Game.Objects.Elevation elevation2 = default(Game.Objects.Elevation);
					m_ObjectElevationData.TryGetComponent(lotOwner, ref elevation2);
					lotInfo = BuildingUtils.CalculateLotInfo(extents2, transform, elevation2, lot, prefabRef, default(DynamicBuffer<InstalledUpgrade>), m_TransformData, m_PrefabRefData, m_PrefabObjectGeometryData, m_PrefabBuildingTerraformData, m_PrefabBuildingExtensionData, defaultNoSmooth: false, out hasExtensionLots);
					return true;
				}
			}
			lotInfo = default(BuildingUtils.LotInfo);
			return false;
		}

		private bool IsFixedNode(Entity node, Entity ignoreEdge, Entity lotOwner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (m_ConnectedEdges.TryGetBuffer(node, ref val))
			{
				NetGeometryData netGeometryData = default(NetGeometryData);
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					if (edge == ignoreEdge)
					{
						continue;
					}
					Edge edge2 = m_EdgeData[edge];
					if (!(edge2.m_Start != node) || !(edge2.m_End != node))
					{
						PrefabRef prefabRef = m_PrefabRefData[edge];
						if (m_PrefabNetGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData) && (netGeometryData.m_Flags & Game.Net.GeometryFlags.FlattenTerrain) != 0 && (!GetLotOwner(edge, out var lotOwner2) || lotOwner2 != lotOwner))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void HandleSubObjects(int jobIndex, DynamicBuffer<Game.Objects.SubObject> subObjects, Transform transform, Transform adjusted)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			if (subObjects.Length == 0)
			{
				return;
			}
			quaternion val = math.mul(adjusted.m_Rotation, math.inverse(transform.m_Rotation));
			Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
			Transform transform3 = default(Transform);
			DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_ObjectElevationData.TryGetComponent(subObject, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0)
				{
					Transform transform2 = m_TransformData[subObject];
					transform3.m_Position = adjusted.m_Position + math.mul(val, transform2.m_Position - transform.m_Position);
					transform3.m_Rotation = math.normalize(math.mul(val, transform2.m_Rotation));
					m_TransformData[subObject] = transform3;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, subObject, default(Updated));
					if (m_SpawnLocationData.HasComponent(subObject) && !m_MovedLocationData.HasComponent(subObject))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<MovedLocation>(jobIndex, subObject, new MovedLocation
						{
							m_OldPosition = transform.m_Position
						});
					}
					if (m_SubObjects.TryGetBuffer(subObject, ref subObjects2))
					{
						HandleSubObjects(jobIndex, subObjects2, transform2, transform3);
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Lot> __Game_Buildings_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovedLocation> __Game_Objects_MovedLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingTerraformData> __Game_Prefabs_BuildingTerraformData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

		public ComponentLookup<Curve> __Game_Net_Curve_RW_ComponentLookup;

		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RW_ComponentLookup;

		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RW_ComponentLookup;

		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RW_BufferLookup;

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
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Buildings_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Lot>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_MovedLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovedLocation>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingTerraformData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
			__Game_Net_Curve_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(false);
			__Game_Net_Node_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(false);
			__Game_Rendering_CullingInfo_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(false);
			__Game_Areas_Node_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(false);
		}
	}

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private ModificationBarrier2 m_ModificationBarrier;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private ToolSystem m_ToolSystem;

	private Game.Areas.GeometrySystem m_AreaGeometrySystem;

	private NativeList<Bounds2> m_NewUpdates;

	private NativeList<Bounds2> m_PendingUpdates;

	private NativeList<Bounds2> m_ReadingUpdates;

	private NativeList<Bounds2> m_ReadyUpdates;

	private JobHandle m_UpdateDeps;

	private LoadHeightsState m_LoadHeightsState;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_AreaGeometrySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.GeometrySystem>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_UpdateDeps)).Complete();
		if (m_NewUpdates.IsCreated)
		{
			m_NewUpdates.Dispose();
		}
		if (m_PendingUpdates.IsCreated)
		{
			m_PendingUpdates.Dispose();
		}
		if (m_ReadingUpdates.IsCreated)
		{
			m_ReadingUpdates.Dispose();
		}
		if (m_ReadyUpdates.IsCreated)
		{
			m_ReadyUpdates.Dispose();
		}
		base.OnDestroy();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_LoadHeightsState = LoadHeightsState.Loaded;
	}

	public NativeList<Bounds2> GetUpdateBuffer()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_UpdateDeps)).Complete();
		if (!m_NewUpdates.IsCreated)
		{
			if (m_ReadyUpdates.IsCreated && m_ReadyUpdates.Length == 0)
			{
				m_NewUpdates = m_ReadyUpdates;
				m_ReadyUpdates = default(NativeList<Bounds2>);
			}
			else
			{
				m_NewUpdates = new NativeList<Bounds2>(AllocatorHandle.op_Implicit((Allocator)4));
			}
		}
		return m_NewUpdates;
	}

	public void BeforeUpdateHeights()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_UpdateDeps)).Complete();
		if (m_NewUpdates.IsCreated && m_NewUpdates.Length != 0)
		{
			if (m_PendingUpdates.IsCreated)
			{
				m_PendingUpdates.AddRange(m_NewUpdates.AsArray());
				m_NewUpdates.Clear();
			}
			else
			{
				m_PendingUpdates = m_NewUpdates;
				m_NewUpdates = default(NativeList<Bounds2>);
			}
		}
		if (m_LoadHeightsState == LoadHeightsState.Loaded)
		{
			m_LoadHeightsState = LoadHeightsState.Pending;
		}
	}

	public void BeforeReadHeights()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_UpdateDeps)).Complete();
		if (m_PendingUpdates.IsCreated && m_PendingUpdates.Length != 0)
		{
			if (m_ReadingUpdates.IsCreated)
			{
				m_ReadingUpdates.AddRange(m_PendingUpdates.AsArray());
				m_PendingUpdates.Clear();
			}
			else
			{
				m_ReadingUpdates = m_PendingUpdates;
				m_PendingUpdates = default(NativeList<Bounds2>);
			}
		}
		if (m_LoadHeightsState == LoadHeightsState.Pending)
		{
			m_LoadHeightsState = LoadHeightsState.Reading;
		}
	}

	public void AfterReadHeights()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_UpdateDeps)).Complete();
		if (m_ReadingUpdates.IsCreated && m_ReadingUpdates.Length != 0)
		{
			if (m_ReadyUpdates.IsCreated)
			{
				m_ReadyUpdates.AddRange(m_ReadingUpdates.AsArray());
				m_ReadingUpdates.Clear();
			}
			else
			{
				m_ReadyUpdates = m_ReadingUpdates;
				m_ReadingUpdates = default(NativeList<Bounds2>);
			}
		}
		if (m_LoadHeightsState == LoadHeightsState.Reading)
		{
			m_LoadHeightsState = LoadHeightsState.Ready;
			m_AreaGeometrySystem.TerrainHeightsReadyAfterLoading();
			m_TerrainSystem.TerrainHeightsReadyAfterLoading();
		}
	}

	public JobHandle Serialize<TWriter>(EntityWriterData writerData, JobHandle inputDeps) where TWriter : struct, IWriter
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		SerializeJob<TWriter> serializeJob = new SerializeJob<TWriter>
		{
			m_NewUpdates = m_NewUpdates,
			m_PendingUpdates = m_PendingUpdates,
			m_ReadingUpdates = m_ReadingUpdates,
			m_ReadyUpdates = m_ReadyUpdates,
			m_WriterData = writerData
		};
		m_UpdateDeps = IJobExtensions.Schedule<SerializeJob<TWriter>>(serializeJob, JobHandle.CombineDependencies(inputDeps, m_UpdateDeps));
		return m_UpdateDeps;
	}

	public JobHandle Deserialize<TReader>(EntityReaderData readerData, JobHandle inputDeps) where TReader : struct, IReader
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!m_ReadingUpdates.IsCreated)
		{
			m_ReadingUpdates = new NativeList<Bounds2>(AllocatorHandle.op_Implicit((Allocator)4));
		}
		DeserializeJob<TReader> deserializeJob = new DeserializeJob<TReader>
		{
			m_NewUpdates = m_NewUpdates,
			m_PendingUpdates = m_PendingUpdates,
			m_ReadingUpdates = m_ReadingUpdates,
			m_ReadyUpdates = m_ReadyUpdates,
			m_ReaderData = readerData
		};
		m_UpdateDeps = IJobExtensions.Schedule<DeserializeJob<TReader>>(deserializeJob, JobHandle.CombineDependencies(inputDeps, m_UpdateDeps));
		return m_UpdateDeps;
	}

	public JobHandle SetDefaults(Context context)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		SetDefaultsJob setDefaultsJob = new SetDefaultsJob
		{
			m_NewUpdates = m_NewUpdates,
			m_PendingUpdates = m_PendingUpdates,
			m_ReadingUpdates = m_ReadingUpdates,
			m_ReadyUpdates = m_ReadyUpdates
		};
		m_UpdateDeps = IJobExtensions.Schedule<SetDefaultsJob>(setDefaultsJob, m_UpdateDeps);
		return m_UpdateDeps;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_UpdateDeps)).Complete();
		if (m_ReadyUpdates.IsCreated && m_ReadyUpdates.Length != 0)
		{
			NativeQueue<Entity> queue = default(NativeQueue<Entity>);
			queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle dependencies;
			JobHandle dependencies2;
			JobHandle dependencies3;
			JobHandle dependencies4;
			BoundsFindJob boundsFindJob = new BoundsFindJob
			{
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingLotData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Lot>(ref __TypeHandle.__Game_Buildings_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ReadyUpdates = m_ReadyUpdates,
				m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
				m_LaneSearchTree = m_NetSearchSystem.GetLaneSearchTree(readOnly: true, out dependencies2),
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies3),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies4),
				m_Queue = queue.AsParallelWriter()
			};
			DequeueJob dequeueJob = new DequeueJob
			{
				m_Queue = queue,
				m_List = val,
				m_ReadyUpdates = m_ReadyUpdates
			};
			JobHandle deps;
			UpdateHeightsJob updateHeightsJob = new UpdateHeightsJob
			{
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MovedLocationData = InternalCompilerInterface.GetComponentLookup<MovedLocation>(ref __TypeHandle.__Game_Objects_MovedLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingLotData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Lot>(ref __TypeHandle.__Game_Buildings_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabPlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingTerraformData = InternalCompilerInterface.GetComponentLookup<BuildingTerraformData>(ref __TypeHandle.__Game_Prefabs_BuildingTerraformData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_List = val,
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps)
			};
			EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
			updateHeightsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			UpdateHeightsJob updateHeightsJob2 = updateHeightsJob;
			JobHandle val3 = IJobParallelForExtensions.Schedule<BoundsFindJob>(boundsFindJob, m_ReadyUpdates.Length, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies4, JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3)));
			JobHandle val4 = IJobExtensions.Schedule<DequeueJob>(dequeueJob, val3);
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<UpdateHeightsJob, Entity>(updateHeightsJob2, val, 4, JobHandle.CombineDependencies(val4, deps));
			queue.Dispose(val4);
			val.Dispose(val5);
			m_ObjectSearchSystem.AddStaticSearchTreeReader(val3);
			m_NetSearchSystem.AddLaneSearchTreeReader(val3);
			m_NetSearchSystem.AddNetSearchTreeReader(val3);
			m_AreaSearchSystem.AddSearchTreeReader(val3);
			m_TerrainSystem.AddCPUHeightReader(val5);
			m_WaterSystem.AddSurfaceReader(val5);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val5);
			((SystemBase)this).Dependency = val5;
			m_UpdateDeps = val4;
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
	public GroundHeightSystem()
	{
	}
}

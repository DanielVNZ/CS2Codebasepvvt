using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Objects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Net;

public struct LaneObjectUpdater
{
	[BurstCompile]
	private struct UpdateLaneObjectsJob : IJobParallelFor
	{
		public Reader<LaneObjectAction> m_LaneActions;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LaneObject> m_LaneObjects;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<LaneObjectAction> enumerator = m_LaneActions.GetEnumerator(index);
			DynamicBuffer<LaneObject> buffer = default(DynamicBuffer<LaneObject>);
			while (enumerator.MoveNext())
			{
				LaneObjectAction current = enumerator.Current;
				if (!m_LaneObjects.TryGetBuffer(current.m_Lane, ref buffer))
				{
					continue;
				}
				if (current.m_Add == current.m_Remove)
				{
					if (current.m_Add != Entity.Null)
					{
						NetUtils.UpdateLaneObject(buffer, current.m_Add, current.m_CurvePosition);
					}
					continue;
				}
				if (current.m_Remove != Entity.Null)
				{
					NetUtils.RemoveLaneObject(buffer, current.m_Remove);
				}
				if (current.m_Add != Entity.Null)
				{
					NetUtils.AddLaneObject(buffer, current.m_Add, current.m_CurvePosition);
				}
			}
			enumerator.Dispose();
		}
	}

	[BurstCompile]
	private struct UpdateTreeObjectsJob : IJob
	{
		public NativeQueue<TreeObjectAction> m_TreeActions;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public void Execute()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			TreeObjectAction treeObjectAction = default(TreeObjectAction);
			while (m_TreeActions.TryDequeue(ref treeObjectAction))
			{
				if (treeObjectAction.m_Add == treeObjectAction.m_Remove)
				{
					if (treeObjectAction.m_Add != Entity.Null)
					{
						m_SearchTree.Update(treeObjectAction.m_Add, new QuadTreeBoundsXZ(treeObjectAction.m_Bounds));
					}
					continue;
				}
				if (treeObjectAction.m_Remove != Entity.Null)
				{
					m_SearchTree.TryRemove(treeObjectAction.m_Remove);
				}
				if (treeObjectAction.m_Add != Entity.Null && !m_SearchTree.TryAdd(treeObjectAction.m_Add, new QuadTreeBoundsXZ(treeObjectAction.m_Bounds)))
				{
					float3 val = MathUtils.Center(treeObjectAction.m_Bounds);
					Debug.Log((object)$"Entity already added to search tree ({treeObjectAction.m_Add.Index}: {val.x}, {val.y}, {val.z})");
				}
			}
		}
	}

	private Game.Objects.SearchSystem m_SearchSystem;

	private BufferLookup<LaneObject> m_LaneObjects;

	private NativeParallelQueue<LaneObjectAction> m_LaneActionQueue;

	private NativeQueue<TreeObjectAction> m_TreeActionQueue;

	public LaneObjectUpdater(SystemBase system)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		m_SearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_LaneObjects = system.GetBufferLookup<LaneObject>(false);
		m_LaneActionQueue = default(NativeParallelQueue<LaneObjectAction>);
		m_TreeActionQueue = default(NativeQueue<TreeObjectAction>);
	}

	public LaneObjectCommandBuffer Begin(Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		m_LaneActionQueue = new NativeParallelQueue<LaneObjectAction>(AllocatorHandle.op_Implicit(allocator));
		m_TreeActionQueue = new NativeQueue<TreeObjectAction>(AllocatorHandle.op_Implicit(allocator));
		return new LaneObjectCommandBuffer(m_LaneActionQueue.AsWriter(), m_TreeActionQueue.AsParallelWriter());
	}

	public JobHandle Apply(SystemBase system, JobHandle dependencies)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		m_LaneObjects.Update(system);
		UpdateLaneObjectsJob updateLaneObjectsJob = new UpdateLaneObjectsJob
		{
			m_LaneActions = m_LaneActionQueue.AsReader(),
			m_LaneObjects = m_LaneObjects
		};
		JobHandle dependencies2;
		UpdateTreeObjectsJob obj = new UpdateTreeObjectsJob
		{
			m_TreeActions = m_TreeActionQueue,
			m_SearchTree = m_SearchSystem.GetMovingSearchTree(readOnly: false, out dependencies2)
		};
		JobHandle val = IJobParallelForExtensions.Schedule<UpdateLaneObjectsJob>(updateLaneObjectsJob, m_LaneActionQueue.HashRange, 1, dependencies);
		JobHandle val2 = IJobExtensions.Schedule<UpdateTreeObjectsJob>(obj, JobHandle.CombineDependencies(dependencies, dependencies2));
		m_LaneActionQueue.Dispose(val);
		m_TreeActionQueue.Dispose(val2);
		m_SearchSystem.AddMovingSearchTreeWriter(val2);
		return val;
	}
}

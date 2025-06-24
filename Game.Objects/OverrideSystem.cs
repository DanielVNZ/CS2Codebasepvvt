using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class OverrideSystem : GameSystemBase
{
	private struct TreeAction
	{
		public Entity m_Entity;

		public BoundsMask m_Mask;
	}

	private struct OverridableAction : IComparable<OverridableAction>
	{
		public Entity m_Entity;

		public Entity m_Other;

		public BoundsMask m_Mask;

		public sbyte m_Priority;

		public bool m_OtherOverridden;

		public int CompareTo(OverridableAction other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return math.select(m_Entity.Index - other.m_Entity.Index, m_Priority - other.m_Priority, m_Priority != other.m_Priority);
		}
	}

	[BurstCompile]
	private struct UpdateObjectOverrideJob : IJob
	{
		[ReadOnly]
		public ComponentTypeSet m_OverriddenUpdatedSet;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		public NativeQueue<TreeAction> m_Actions;

		public NativeQueue<OverridableAction> m_OverridableActions;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<OverridableAction> val = m_OverridableActions.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
			if (val.Length != 0)
			{
				NativeSortExtension.Sort<OverridableAction>(val);
				NativeHashMap<Entity, bool> overridden = default(NativeHashMap<Entity, bool>);
				overridden._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
				TreeAction treeAction = default(TreeAction);
				QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
				while (m_Actions.TryDequeue(ref treeAction))
				{
					if (m_ObjectSearchTree.TryGet(treeAction.m_Entity, ref quadTreeBoundsXZ))
					{
						quadTreeBoundsXZ.m_Mask = (quadTreeBoundsXZ.m_Mask & ~(BoundsMask.AllLayers | BoundsMask.NotOverridden)) | treeAction.m_Mask;
						m_ObjectSearchTree.Update(treeAction.m_Entity, quadTreeBoundsXZ);
					}
					overridden.Add(treeAction.m_Entity, treeAction.m_Mask == (BoundsMask)0);
				}
				for (int i = 0; i < val.Length; i++)
				{
					OverridableAction overridableAction = val[i];
					overridden[overridableAction.m_Entity] = overridableAction.m_Other != Entity.Null;
				}
				OverridableAction action = default(OverridableAction);
				bool flag = false;
				bool flag2 = default(bool);
				for (int j = 0; j < val.Length; j++)
				{
					OverridableAction overridableAction2 = val[j];
					if (overridableAction2.m_Entity != action.m_Entity)
					{
						if (action.m_Entity != Entity.Null && action.m_Other != Entity.Null)
						{
							overridden[action.m_Entity] = flag;
							UpdateObject(action, overridden, flag);
						}
						action = overridableAction2;
						flag = false;
					}
					if (!flag && overridableAction2.m_Other != Entity.Null)
					{
						flag = ((!overridden.TryGetValue(overridableAction2.m_Other, ref flag2)) ? (!overridableAction2.m_OtherOverridden) : (!flag2));
					}
				}
				if (action.m_Entity != Entity.Null && action.m_Other != Entity.Null)
				{
					UpdateObject(action, overridden, flag);
				}
				action = default(OverridableAction);
				for (int k = 0; k < val.Length; k++)
				{
					OverridableAction overridableAction3 = val[k];
					if (overridableAction3.m_Entity != action.m_Entity)
					{
						if (action.m_Entity != Entity.Null && action.m_Other == Entity.Null)
						{
							UpdateObject(action, overridden, overridden[action.m_Entity]);
						}
						action = overridableAction3;
					}
				}
				if (action.m_Entity != Entity.Null && action.m_Other == Entity.Null)
				{
					UpdateObject(action, overridden, overridden[action.m_Entity]);
				}
				overridden.Dispose();
				return;
			}
			TreeAction treeAction2 = default(TreeAction);
			QuadTreeBoundsXZ quadTreeBoundsXZ2 = default(QuadTreeBoundsXZ);
			while (m_Actions.TryDequeue(ref treeAction2))
			{
				if (m_ObjectSearchTree.TryGet(treeAction2.m_Entity, ref quadTreeBoundsXZ2))
				{
					quadTreeBoundsXZ2.m_Mask = (quadTreeBoundsXZ2.m_Mask & ~(BoundsMask.AllLayers | BoundsMask.NotOverridden)) | treeAction2.m_Mask;
					m_ObjectSearchTree.Update(treeAction2.m_Entity, quadTreeBoundsXZ2);
				}
			}
		}

		private void UpdateObject(OverridableAction action, NativeHashMap<Entity, bool> overridden, bool collision)
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (action.m_Priority & 2) != 0;
			if (collision != flag)
			{
				if (collision)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(action.m_Entity, ref m_OverriddenUpdatedSet);
					action.m_Mask = (BoundsMask)0;
				}
				else
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(action.m_Entity, default(Updated));
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Overridden>(action.m_Entity);
				}
				QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
				if (m_ObjectSearchTree.TryGet(action.m_Entity, ref quadTreeBoundsXZ))
				{
					quadTreeBoundsXZ.m_Mask = (quadTreeBoundsXZ.m_Mask & ~(BoundsMask.AllLayers | BoundsMask.NotOverridden)) | action.m_Mask;
					m_ObjectSearchTree.Update(action.m_Entity, quadTreeBoundsXZ);
				}
			}
			DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
			if (!m_SubObjects.TryGetBuffer(action.m_Entity, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (overridden.ContainsKey(subObject))
				{
					overridden[subObject] = collision;
				}
			}
		}
	}

	[BurstCompile]
	private struct FindUpdatedObjectsJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
				{
					m_ResultQueue.Enqueue(objectEntity);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Bounds = m_Bounds[index],
				m_ResultQueue = m_ResultQueue
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
		}
	}

	[BurstCompile]
	private struct CollectObjectsJob : IJob
	{
		public NativeQueue<Entity> m_Queue1;

		public NativeQueue<Entity> m_Queue2;

		public NativeQueue<Entity> m_Queue3;

		public NativeList<Entity> m_ResultList;

		public NativeHashSet<Entity> m_ObjectSet;

		public void Execute()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			while (m_Queue1.TryDequeue(ref val))
			{
				if (m_ObjectSet.Add(val))
				{
					m_ResultList.Add(ref val);
				}
			}
			Entity val2 = default(Entity);
			while (m_Queue2.TryDequeue(ref val2))
			{
				if (m_ObjectSet.Add(val2))
				{
					m_ResultList.Add(ref val2);
				}
			}
			Entity val3 = default(Entity);
			while (m_Queue3.TryDequeue(ref val3))
			{
				if (m_ObjectSet.Add(val3))
				{
					m_ResultList.Add(ref val3);
				}
			}
		}
	}

	[BurstCompile]
	private struct CheckObjectOverrideJob : IJobParallelForDefer
	{
		private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_TopLevelEntity;

			public Entity m_ObjectEntity;

			public Bounds3 m_ObjectBounds;

			public Transform m_ObjectTransform;

			public Stack m_ObjectStack;

			public CollisionMask m_CollisionMask;

			public ObjectGeometryData m_PrefabGeometryData;

			public StackData m_ObjectStackData;

			public DynamicBuffer<ConnectedEdge> m_TopLevelEdges;

			public DynamicBuffer<ConnectedNode> m_TopLevelNodes;

			public Edge m_TopLevelEdge;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<Elevation> m_ElevationData;

			public ComponentLookup<Attachment> m_AttachmentData;

			public ComponentLookup<Stack> m_StackData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public ComponentLookup<StackData> m_PrefabStackData;

			public NativeList<Entity> m_OverridableCollisions;

			public bool m_CollisionFound;

			public bool m_EditorMode;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				if (m_CollisionFound)
				{
					return false;
				}
				if ((m_CollisionMask & CollisionMask.OnGround) != 0)
				{
					return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz);
				}
				return MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity)
			{
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_018c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_0206: Unknown result type (might be due to invalid IL or missing references)
				//IL_020b: Unknown result type (might be due to invalid IL or missing references)
				//IL_020f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0226: Unknown result type (might be due to invalid IL or missing references)
				//IL_0237: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0255: Unknown result type (might be due to invalid IL or missing references)
				//IL_0271: Unknown result type (might be due to invalid IL or missing references)
				//IL_0276: Unknown result type (might be due to invalid IL or missing references)
				//IL_027d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0282: Unknown result type (might be due to invalid IL or missing references)
				//IL_0287: Unknown result type (might be due to invalid IL or missing references)
				//IL_028f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0294: Unknown result type (might be due to invalid IL or missing references)
				//IL_0296: Unknown result type (might be due to invalid IL or missing references)
				//IL_029b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_1989: Unknown result type (might be due to invalid IL or missing references)
				//IL_198e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1990: Unknown result type (might be due to invalid IL or missing references)
				//IL_199b: Unknown result type (might be due to invalid IL or missing references)
				//IL_19a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_19b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_19b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_19ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_19bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_19c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_19c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_17a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_17a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_17ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_17b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_17b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f37: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f3e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f45: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f56: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f68: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f7c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
				//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c04: Unknown result type (might be due to invalid IL or missing references)
				//IL_12db: Unknown result type (might be due to invalid IL or missing references)
				//IL_12e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_12e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_12e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f93: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f95: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0321: Unknown result type (might be due to invalid IL or missing references)
				//IL_032b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0330: Unknown result type (might be due to invalid IL or missing references)
				//IL_0335: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b47: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b4c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b4e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b55: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b5b: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b65: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b6a: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b6f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b74: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b78: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b7d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b7f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b81: Unknown result type (might be due to invalid IL or missing references)
				//IL_1afd: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b15: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b1a: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b1c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b21: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b25: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b2f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b31: Unknown result type (might be due to invalid IL or missing references)
				//IL_1936: Unknown result type (might be due to invalid IL or missing references)
				//IL_193b: Unknown result type (might be due to invalid IL or missing references)
				//IL_193d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1944: Unknown result type (might be due to invalid IL or missing references)
				//IL_194a: Unknown result type (might be due to invalid IL or missing references)
				//IL_1954: Unknown result type (might be due to invalid IL or missing references)
				//IL_1959: Unknown result type (might be due to invalid IL or missing references)
				//IL_195e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1963: Unknown result type (might be due to invalid IL or missing references)
				//IL_1967: Unknown result type (might be due to invalid IL or missing references)
				//IL_196c: Unknown result type (might be due to invalid IL or missing references)
				//IL_18eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_1903: Unknown result type (might be due to invalid IL or missing references)
				//IL_1908: Unknown result type (might be due to invalid IL or missing references)
				//IL_190a: Unknown result type (might be due to invalid IL or missing references)
				//IL_190f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1913: Unknown result type (might be due to invalid IL or missing references)
				//IL_191d: Unknown result type (might be due to invalid IL or missing references)
				//IL_191f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1324: Unknown result type (might be due to invalid IL or missing references)
				//IL_0354: Unknown result type (might be due to invalid IL or missing references)
				//IL_037a: Unknown result type (might be due to invalid IL or missing references)
				//IL_037f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0384: Unknown result type (might be due to invalid IL or missing references)
				//IL_038b: Unknown result type (might be due to invalid IL or missing references)
				//IL_038d: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b8e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1b3e: Unknown result type (might be due to invalid IL or missing references)
				//IL_19e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_19e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_19eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_19f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_197c: Unknown result type (might be due to invalid IL or missing references)
				//IL_192d: Unknown result type (might be due to invalid IL or missing references)
				//IL_17d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_17dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_17df: Unknown result type (might be due to invalid IL or missing references)
				//IL_17e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_150f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1527: Unknown result type (might be due to invalid IL or missing references)
				//IL_1531: Unknown result type (might be due to invalid IL or missing references)
				//IL_154d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1557: Unknown result type (might be due to invalid IL or missing references)
				//IL_1561: Unknown result type (might be due to invalid IL or missing references)
				//IL_1569: Unknown result type (might be due to invalid IL or missing references)
				//IL_156e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1575: Unknown result type (might be due to invalid IL or missing references)
				//IL_157a: Unknown result type (might be due to invalid IL or missing references)
				//IL_157f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1584: Unknown result type (might be due to invalid IL or missing references)
				//IL_1588: Unknown result type (might be due to invalid IL or missing references)
				//IL_158d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1248: Unknown result type (might be due to invalid IL or missing references)
				//IL_1250: Unknown result type (might be due to invalid IL or missing references)
				//IL_1252: Unknown result type (might be due to invalid IL or missing references)
				//IL_1254: Unknown result type (might be due to invalid IL or missing references)
				//IL_1259: Unknown result type (might be due to invalid IL or missing references)
				//IL_1260: Unknown result type (might be due to invalid IL or missing references)
				//IL_1262: Unknown result type (might be due to invalid IL or missing references)
				//IL_126c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1271: Unknown result type (might be due to invalid IL or missing references)
				//IL_1276: Unknown result type (might be due to invalid IL or missing references)
				//IL_127f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1284: Unknown result type (might be due to invalid IL or missing references)
				//IL_1289: Unknown result type (might be due to invalid IL or missing references)
				//IL_128b: Unknown result type (might be due to invalid IL or missing references)
				//IL_11b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_11bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_11d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_11da: Unknown result type (might be due to invalid IL or missing references)
				//IL_11df: Unknown result type (might be due to invalid IL or missing references)
				//IL_11e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_11e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_11f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_120a: Unknown result type (might be due to invalid IL or missing references)
				//IL_120f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1216: Unknown result type (might be due to invalid IL or missing references)
				//IL_121b: Unknown result type (might be due to invalid IL or missing references)
				//IL_1224: Unknown result type (might be due to invalid IL or missing references)
				//IL_1229: Unknown result type (might be due to invalid IL or missing references)
				//IL_122e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1230: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ed7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ed9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ee0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ee7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ee9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0efd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f10: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f12: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e3a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e43: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e5b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e60: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e6e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e7e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e80: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e90: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e9c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ea1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0eaa: Unknown result type (might be due to invalid IL or missing references)
				//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
				//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0eb6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0724: Unknown result type (might be due to invalid IL or missing references)
				//IL_0736: Unknown result type (might be due to invalid IL or missing references)
				//IL_0738: Unknown result type (might be due to invalid IL or missing references)
				//IL_0764: Unknown result type (might be due to invalid IL or missing references)
				//IL_076e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0778: Unknown result type (might be due to invalid IL or missing references)
				//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0804: Unknown result type (might be due to invalid IL or missing references)
				//IL_080e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0813: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a08: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a20: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a2a: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a2c: Unknown result type (might be due to invalid IL or missing references)
				//IL_17fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_1814: Unknown result type (might be due to invalid IL or missing references)
				//IL_181e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1820: Unknown result type (might be due to invalid IL or missing references)
				//IL_14aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_14af: Unknown result type (might be due to invalid IL or missing references)
				//IL_14b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_14b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_14be: Unknown result type (might be due to invalid IL or missing references)
				//IL_14c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_14cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_14d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_14d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_14db: Unknown result type (might be due to invalid IL or missing references)
				//IL_14e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_145f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1477: Unknown result type (might be due to invalid IL or missing references)
				//IL_147c: Unknown result type (might be due to invalid IL or missing references)
				//IL_147e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1483: Unknown result type (might be due to invalid IL or missing references)
				//IL_1487: Unknown result type (might be due to invalid IL or missing references)
				//IL_1491: Unknown result type (might be due to invalid IL or missing references)
				//IL_1493: Unknown result type (might be due to invalid IL or missing references)
				//IL_129a: Unknown result type (might be due to invalid IL or missing references)
				//IL_123f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0fc3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0fcd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0fd2: Unknown result type (might be due to invalid IL or missing references)
				//IL_1196: Unknown result type (might be due to invalid IL or missing references)
				//IL_0f24: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ec6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e1c: Unknown result type (might be due to invalid IL or missing references)
				//IL_082a: Unknown result type (might be due to invalid IL or missing references)
				//IL_082c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a56: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a6a: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a74: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a8c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a96: Unknown result type (might be due to invalid IL or missing references)
				//IL_1aa0: Unknown result type (might be due to invalid IL or missing references)
				//IL_1aa4: Unknown result type (might be due to invalid IL or missing references)
				//IL_1aa9: Unknown result type (might be due to invalid IL or missing references)
				//IL_1ab0: Unknown result type (might be due to invalid IL or missing references)
				//IL_1ab5: Unknown result type (might be due to invalid IL or missing references)
				//IL_1aba: Unknown result type (might be due to invalid IL or missing references)
				//IL_1abf: Unknown result type (might be due to invalid IL or missing references)
				//IL_1ac3: Unknown result type (might be due to invalid IL or missing references)
				//IL_1ac8: Unknown result type (might be due to invalid IL or missing references)
				//IL_1aca: Unknown result type (might be due to invalid IL or missing references)
				//IL_1acc: Unknown result type (might be due to invalid IL or missing references)
				//IL_1a3c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1848: Unknown result type (might be due to invalid IL or missing references)
				//IL_185c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1866: Unknown result type (might be due to invalid IL or missing references)
				//IL_187e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1888: Unknown result type (might be due to invalid IL or missing references)
				//IL_1892: Unknown result type (might be due to invalid IL or missing references)
				//IL_1896: Unknown result type (might be due to invalid IL or missing references)
				//IL_189b: Unknown result type (might be due to invalid IL or missing references)
				//IL_18a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_18a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_18ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_18b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_18b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_18ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_182e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1710: Unknown result type (might be due to invalid IL or missing references)
				//IL_1715: Unknown result type (might be due to invalid IL or missing references)
				//IL_1717: Unknown result type (might be due to invalid IL or missing references)
				//IL_171e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1724: Unknown result type (might be due to invalid IL or missing references)
				//IL_172e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1733: Unknown result type (might be due to invalid IL or missing references)
				//IL_1738: Unknown result type (might be due to invalid IL or missing references)
				//IL_173d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1741: Unknown result type (might be due to invalid IL or missing references)
				//IL_1746: Unknown result type (might be due to invalid IL or missing references)
				//IL_1748: Unknown result type (might be due to invalid IL or missing references)
				//IL_174a: Unknown result type (might be due to invalid IL or missing references)
				//IL_16c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_16de: Unknown result type (might be due to invalid IL or missing references)
				//IL_16e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_16e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_16ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_16ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_16f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_16fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_14f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_14a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_1348: Unknown result type (might be due to invalid IL or missing references)
				//IL_134d: Unknown result type (might be due to invalid IL or missing references)
				//IL_134f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1354: Unknown result type (might be due to invalid IL or missing references)
				//IL_0fec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ff5: Unknown result type (might be due to invalid IL or missing references)
				//IL_100d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1012: Unknown result type (might be due to invalid IL or missing references)
				//IL_1017: Unknown result type (might be due to invalid IL or missing references)
				//IL_101e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1020: Unknown result type (might be due to invalid IL or missing references)
				//IL_1031: Unknown result type (might be due to invalid IL or missing references)
				//IL_1041: Unknown result type (might be due to invalid IL or missing references)
				//IL_1046: Unknown result type (might be due to invalid IL or missing references)
				//IL_104d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1052: Unknown result type (might be due to invalid IL or missing references)
				//IL_105b: Unknown result type (might be due to invalid IL or missing references)
				//IL_1060: Unknown result type (might be due to invalid IL or missing references)
				//IL_1065: Unknown result type (might be due to invalid IL or missing references)
				//IL_1067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c95: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cc9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ce3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ce8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
				//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_06de: Unknown result type (might be due to invalid IL or missing references)
				//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_061b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0624: Unknown result type (might be due to invalid IL or missing references)
				//IL_063c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0641: Unknown result type (might be due to invalid IL or missing references)
				//IL_0646: Unknown result type (might be due to invalid IL or missing references)
				//IL_064d: Unknown result type (might be due to invalid IL or missing references)
				//IL_064f: Unknown result type (might be due to invalid IL or missing references)
				//IL_065f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0661: Unknown result type (might be due to invalid IL or missing references)
				//IL_0671: Unknown result type (might be due to invalid IL or missing references)
				//IL_0676: Unknown result type (might be due to invalid IL or missing references)
				//IL_067d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0682: Unknown result type (might be due to invalid IL or missing references)
				//IL_068b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0690: Unknown result type (might be due to invalid IL or missing references)
				//IL_0695: Unknown result type (might be due to invalid IL or missing references)
				//IL_0697: Unknown result type (might be due to invalid IL or missing references)
				//IL_1ad9: Unknown result type (might be due to invalid IL or missing references)
				//IL_18c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_1757: Unknown result type (might be due to invalid IL or missing references)
				//IL_1707: Unknown result type (might be due to invalid IL or missing references)
				//IL_15a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_15ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_15b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_15b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_136c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1384: Unknown result type (might be due to invalid IL or missing references)
				//IL_138e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1390: Unknown result type (might be due to invalid IL or missing references)
				//IL_1093: Unknown result type (might be due to invalid IL or missing references)
				//IL_10a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_10a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_10cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_10d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_10e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_10fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_1122: Unknown result type (might be due to invalid IL or missing references)
				//IL_112c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1136: Unknown result type (might be due to invalid IL or missing references)
				//IL_1148: Unknown result type (might be due to invalid IL or missing references)
				//IL_114d: Unknown result type (might be due to invalid IL or missing references)
				//IL_114f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1154: Unknown result type (might be due to invalid IL or missing references)
				//IL_115d: Unknown result type (might be due to invalid IL or missing references)
				//IL_1162: Unknown result type (might be due to invalid IL or missing references)
				//IL_1167: Unknown result type (might be due to invalid IL or missing references)
				//IL_1169: Unknown result type (might be due to invalid IL or missing references)
				//IL_1079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d19: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d2b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d55: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d69: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d80: Unknown result type (might be due to invalid IL or missing references)
				//IL_0da8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0db2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
				//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
				//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ded: Unknown result type (might be due to invalid IL or missing references)
				//IL_0def: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ae9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aa1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0705: Unknown result type (might be due to invalid IL or missing references)
				//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_042c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0431: Unknown result type (might be due to invalid IL or missing references)
				//IL_0436: Unknown result type (might be due to invalid IL or missing references)
				//IL_043b: Unknown result type (might be due to invalid IL or missing references)
				//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_15cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_15e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_15ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_15f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_13b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_13cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_13d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_13ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_13f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_1402: Unknown result type (might be due to invalid IL or missing references)
				//IL_1406: Unknown result type (might be due to invalid IL or missing references)
				//IL_140b: Unknown result type (might be due to invalid IL or missing references)
				//IL_1412: Unknown result type (might be due to invalid IL or missing references)
				//IL_1417: Unknown result type (might be due to invalid IL or missing references)
				//IL_141c: Unknown result type (might be due to invalid IL or missing references)
				//IL_1421: Unknown result type (might be due to invalid IL or missing references)
				//IL_1425: Unknown result type (might be due to invalid IL or missing references)
				//IL_142a: Unknown result type (might be due to invalid IL or missing references)
				//IL_139e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1178: Unknown result type (might be due to invalid IL or missing references)
				//IL_0dfe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
				//IL_085a: Unknown result type (might be due to invalid IL or missing references)
				//IL_085f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0864: Unknown result type (might be due to invalid IL or missing references)
				//IL_0869: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0455: Unknown result type (might be due to invalid IL or missing references)
				//IL_045e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0476: Unknown result type (might be due to invalid IL or missing references)
				//IL_047b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0480: Unknown result type (might be due to invalid IL or missing references)
				//IL_0487: Unknown result type (might be due to invalid IL or missing references)
				//IL_0489: Unknown result type (might be due to invalid IL or missing references)
				//IL_049a: Unknown result type (might be due to invalid IL or missing references)
				//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_04af: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_161b: Unknown result type (might be due to invalid IL or missing references)
				//IL_162f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1639: Unknown result type (might be due to invalid IL or missing references)
				//IL_1651: Unknown result type (might be due to invalid IL or missing references)
				//IL_165b: Unknown result type (might be due to invalid IL or missing references)
				//IL_1665: Unknown result type (might be due to invalid IL or missing references)
				//IL_1669: Unknown result type (might be due to invalid IL or missing references)
				//IL_166e: Unknown result type (might be due to invalid IL or missing references)
				//IL_1675: Unknown result type (might be due to invalid IL or missing references)
				//IL_167a: Unknown result type (might be due to invalid IL or missing references)
				//IL_167f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1684: Unknown result type (might be due to invalid IL or missing references)
				//IL_1688: Unknown result type (might be due to invalid IL or missing references)
				//IL_168d: Unknown result type (might be due to invalid IL or missing references)
				//IL_168f: Unknown result type (might be due to invalid IL or missing references)
				//IL_1691: Unknown result type (might be due to invalid IL or missing references)
				//IL_1601: Unknown result type (might be due to invalid IL or missing references)
				//IL_1437: Unknown result type (might be due to invalid IL or missing references)
				//IL_0883: Unknown result type (might be due to invalid IL or missing references)
				//IL_088c: Unknown result type (might be due to invalid IL or missing references)
				//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_050c: Unknown result type (might be due to invalid IL or missing references)
				//IL_050e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0536: Unknown result type (might be due to invalid IL or missing references)
				//IL_0540: Unknown result type (might be due to invalid IL or missing references)
				//IL_054a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0561: Unknown result type (might be due to invalid IL or missing references)
				//IL_0589: Unknown result type (might be due to invalid IL or missing references)
				//IL_0593: Unknown result type (might be due to invalid IL or missing references)
				//IL_059d: Unknown result type (might be due to invalid IL or missing references)
				//IL_05af: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_169e: Unknown result type (might be due to invalid IL or missing references)
				//IL_092a: Unknown result type (might be due to invalid IL or missing references)
				//IL_093c: Unknown result type (might be due to invalid IL or missing references)
				//IL_093e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0966: Unknown result type (might be due to invalid IL or missing references)
				//IL_0970: Unknown result type (might be due to invalid IL or missing references)
				//IL_097a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0991: Unknown result type (might be due to invalid IL or missing references)
				//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_09df: Unknown result type (might be due to invalid IL or missing references)
				//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
				//IL_0910: Unknown result type (might be due to invalid IL or missing references)
				//IL_05df: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
				if (m_CollisionFound)
				{
					return;
				}
				if ((m_CollisionMask & CollisionMask.OnGround) != 0)
				{
					if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz))
					{
						return;
					}
				}
				else if (!MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds))
				{
					return;
				}
				Entity val = objectEntity;
				while (m_OwnerData.HasComponent(val) && !m_BuildingData.HasComponent(val))
				{
					val = m_OwnerData[val].m_Owner;
				}
				if (m_TopLevelEntity == val || (m_AttachmentData.HasComponent(val) && m_AttachmentData[val].m_Attached == m_TopLevelEntity))
				{
					return;
				}
				if (m_TopLevelEdges.IsCreated)
				{
					for (int i = 0; i < m_TopLevelEdges.Length; i++)
					{
						if (m_TopLevelEdges[i].m_Edge == val)
						{
							return;
						}
					}
				}
				else if (m_TopLevelNodes.IsCreated)
				{
					for (int j = 0; j < m_TopLevelNodes.Length; j++)
					{
						if (m_TopLevelNodes[j].m_Node == val)
						{
							return;
						}
					}
					if (m_TopLevelEdge.m_Start == val || m_TopLevelEdge.m_End == val)
					{
						return;
					}
				}
				PrefabRef prefabRef = m_PrefabRefData[objectEntity];
				if (!m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				bool overridableCollision = false;
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				if ((objectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == GeometryFlags.Overridable)
				{
					overridableCollision = true;
				}
				Transform transform = m_TransformData[objectEntity];
				bool ignoreMarkers = !m_EditorMode || m_OwnerData.HasComponent(objectEntity);
				Elevation elevation = default(Elevation);
				CollisionMask collisionMask = ((!m_ElevationData.TryGetComponent(objectEntity, ref elevation)) ? ObjectUtils.GetCollisionMask(objectGeometryData, ignoreMarkers) : ObjectUtils.GetCollisionMask(objectGeometryData, elevation, ignoreMarkers));
				if ((m_CollisionMask & collisionMask) == 0)
				{
					return;
				}
				float3 val2 = MathUtils.Center(bounds.m_Bounds);
				float3 pos = default(float3);
				bool flag = false;
				StackData stackData = default(StackData);
				Stack stack = default(Stack);
				if (m_StackData.TryGetComponent(objectEntity, ref stack))
				{
					m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData);
				}
				if (((m_CollisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds)) && !flag)
				{
					quaternion val3 = math.inverse(m_ObjectTransform.m_Rotation);
					quaternion val4 = math.inverse(transform.m_Rotation);
					float3 val5 = math.mul(val3, m_ObjectTransform.m_Position - val2);
					float3 val6 = math.mul(val4, transform.m_Position - val2);
					Bounds3 bounds2 = ObjectUtils.GetBounds(m_ObjectStack, m_PrefabGeometryData, m_ObjectStackData);
					if ((m_PrefabGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
					{
						bounds2.min.y = math.max(bounds2.min.y, 0f);
					}
					if (ObjectUtils.GetStandingLegCount(m_PrefabGeometryData, out var legCount))
					{
						Bounds3 val11 = default(Bounds3);
						Bounds3 val12 = default(Bounds3);
						Bounds3 val14 = default(Bounds3);
						Bounds3 val15 = default(Bounds3);
						Bounds3 val18 = default(Bounds3);
						Bounds3 val19 = default(Bounds3);
						Bounds3 val21 = default(Bounds3);
						Bounds3 val22 = default(Bounds3);
						Bounds3 val23 = default(Bounds3);
						Bounds3 val24 = default(Bounds3);
						Bounds3 val26 = default(Bounds3);
						Bounds3 val27 = default(Bounds3);
						for (int k = 0; k < legCount; k++)
						{
							float3 val7 = val5 + ObjectUtils.GetStandingLegOffset(m_PrefabGeometryData, k);
							if ((m_PrefabGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
							{
								Cylinder3 val8 = new Cylinder3
								{
									circle = new Circle2(m_PrefabGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val7)).xz),
									height = new Bounds1(bounds2.min.y + 0.01f, m_PrefabGeometryData.m_LegSize.y + 0.01f) + val7.y,
									rotation = m_ObjectTransform.m_Rotation
								};
								Bounds3 bounds3 = ObjectUtils.GetBounds(stack, objectGeometryData, stackData);
								if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
								{
									bounds3.min.y = math.max(bounds3.min.y, 0f);
								}
								if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount2))
								{
									for (int l = 0; l < legCount2; l++)
									{
										float3 val9 = val6 + ObjectUtils.GetStandingLegOffset(objectGeometryData, l);
										if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
										{
											if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
											{
												circle = new Circle2(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val9)).xz),
												height = new Bounds1(bounds3.min.y + 0.01f, objectGeometryData.m_LegSize.y + 0.01f) + val9.y,
												rotation = transform.m_Rotation
											}, cylinder1: val8, pos: ref pos))
											{
												AddCollision(overridableCollision, objectEntity);
												return;
											}
										}
										else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
										{
											Box3 val10 = new Box3
											{
												bounds = 
												{
													min = 
													{
														y = bounds3.min.y + 0.01f
													}
												}
											};
											((float3)(ref val10.bounds.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f + 0.01f;
											val10.bounds.max.y = objectGeometryData.m_LegSize.y + 0.01f;
											((float3)(ref val10.bounds.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f - 0.01f;
											ref Bounds3 bounds4 = ref val10.bounds;
											bounds4 += val9;
											val10.rotation = transform.m_Rotation;
											if (MathUtils.Intersect(val8, val10, ref val11, ref val12))
											{
												AddCollision(overridableCollision, objectEntity);
												return;
											}
										}
									}
									bounds3.min.y = objectGeometryData.m_LegSize.y;
								}
								if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
								{
									if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
									{
										circle = new Circle2(objectGeometryData.m_Size.x * 0.5f - 0.01f, ((float3)(ref val6)).xz),
										height = new Bounds1(bounds3.min.y + 0.01f, bounds3.max.y - 0.01f) + val6.y,
										rotation = transform.m_Rotation
									}, cylinder1: val8, pos: ref pos))
									{
										AddCollision(overridableCollision, objectEntity);
										return;
									}
									continue;
								}
								Box3 val13 = default(Box3);
								val13.bounds = bounds3 + val6;
								val13.bounds = MathUtils.Expand(val13.bounds, float3.op_Implicit(-0.01f));
								val13.rotation = transform.m_Rotation;
								if (MathUtils.Intersect(val8, val13, ref val14, ref val15))
								{
									AddCollision(overridableCollision, objectEntity);
									return;
								}
							}
							else
							{
								if ((m_PrefabGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) != GeometryFlags.None)
								{
									continue;
								}
								Box3 val16 = new Box3
								{
									bounds = 
									{
										min = 
										{
											y = bounds2.min.y + 0.01f
										}
									}
								};
								((float3)(ref val16.bounds.min)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * -0.5f + 0.01f;
								val16.bounds.max.y = m_PrefabGeometryData.m_LegSize.y + 0.01f;
								((float3)(ref val16.bounds.max)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * 0.5f - 0.01f;
								ref Bounds3 bounds5 = ref val16.bounds;
								bounds5 += val7;
								val16.rotation = m_ObjectTransform.m_Rotation;
								Bounds3 bounds6 = ObjectUtils.GetBounds(stack, objectGeometryData, stackData);
								if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
								{
									bounds6.min.y = math.max(bounds6.min.y, 0f);
								}
								if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount3))
								{
									for (int m = 0; m < legCount3; m++)
									{
										float3 val17 = val6 + ObjectUtils.GetStandingLegOffset(objectGeometryData, m);
										if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
										{
											if (MathUtils.Intersect(new Cylinder3
											{
												circle = new Circle2(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val17)).xz),
												height = new Bounds1(bounds6.min.y + 0.01f, objectGeometryData.m_LegSize.y + 0.01f) + val17.y,
												rotation = transform.m_Rotation
											}, val16, ref val18, ref val19))
											{
												AddCollision(overridableCollision, objectEntity);
												return;
											}
										}
										else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
										{
											Box3 val20 = new Box3
											{
												bounds = 
												{
													min = 
													{
														y = bounds6.min.y + 0.01f
													}
												}
											};
											((float3)(ref val20.bounds.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f + 0.01f;
											val20.bounds.max.y = objectGeometryData.m_LegSize.y + 0.01f;
											((float3)(ref val20.bounds.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f - 0.01f;
											ref Bounds3 bounds7 = ref val20.bounds;
											bounds7 += val17;
											val20.rotation = transform.m_Rotation;
											if (MathUtils.Intersect(val16, val20, ref val21, ref val22))
											{
												AddCollision(overridableCollision, objectEntity);
												return;
											}
										}
									}
									bounds6.min.y = objectGeometryData.m_LegSize.y;
								}
								if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
								{
									if (MathUtils.Intersect(new Cylinder3
									{
										circle = new Circle2(objectGeometryData.m_Size.x * 0.5f - 0.01f, ((float3)(ref val6)).xz),
										height = new Bounds1(bounds6.min.y + 0.01f, bounds6.max.y - 0.01f) + val6.y,
										rotation = transform.m_Rotation
									}, val16, ref val23, ref val24))
									{
										AddCollision(overridableCollision, objectEntity);
										return;
									}
									continue;
								}
								Box3 val25 = default(Box3);
								val25.bounds = bounds6 + val6;
								val25.bounds = MathUtils.Expand(val25.bounds, float3.op_Implicit(-0.01f));
								val25.rotation = transform.m_Rotation;
								if (MathUtils.Intersect(val16, val25, ref val26, ref val27))
								{
									AddCollision(overridableCollision, objectEntity);
									return;
								}
							}
						}
						bounds2.min.y = m_PrefabGeometryData.m_LegSize.y;
					}
					if ((m_PrefabGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
					{
						Cylinder3 val28 = new Cylinder3
						{
							circle = new Circle2(m_PrefabGeometryData.m_Size.x * 0.5f - 0.01f, ((float3)(ref val5)).xz),
							height = new Bounds1(bounds2.min.y + 0.01f, bounds2.max.y - 0.01f) + val5.y,
							rotation = m_ObjectTransform.m_Rotation
						};
						Bounds3 bounds8 = ObjectUtils.GetBounds(stack, objectGeometryData, stackData);
						if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
						{
							bounds8.min.y = math.max(bounds8.min.y, 0f);
						}
						if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount4))
						{
							Bounds3 val31 = default(Bounds3);
							Bounds3 val32 = default(Bounds3);
							for (int n = 0; n < legCount4; n++)
							{
								float3 val29 = val6 + ObjectUtils.GetStandingLegOffset(objectGeometryData, n);
								if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
								{
									if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
									{
										circle = new Circle2(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val29)).xz),
										height = new Bounds1(bounds8.min.y + 0.01f, objectGeometryData.m_LegSize.y + 0.01f) + val29.y,
										rotation = transform.m_Rotation
									}, cylinder1: val28, pos: ref pos))
									{
										AddCollision(overridableCollision, objectEntity);
										return;
									}
								}
								else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
								{
									Box3 val30 = new Box3
									{
										bounds = 
										{
											min = 
											{
												y = bounds8.min.y + 0.01f
											}
										}
									};
									((float3)(ref val30.bounds.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f + 0.01f;
									val30.bounds.max.y = objectGeometryData.m_LegSize.y + 0.01f;
									((float3)(ref val30.bounds.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f - 0.01f;
									ref Bounds3 bounds9 = ref val30.bounds;
									bounds9 += val29;
									val30.rotation = transform.m_Rotation;
									if (MathUtils.Intersect(val28, val30, ref val31, ref val32))
									{
										AddCollision(overridableCollision, objectEntity);
										return;
									}
								}
							}
							bounds8.min.y = objectGeometryData.m_LegSize.y;
						}
						if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
						{
							if (ValidationHelpers.Intersect(cylinder2: new Cylinder3
							{
								circle = new Circle2(objectGeometryData.m_Size.x * 0.5f - 0.01f, ((float3)(ref val6)).xz),
								height = new Bounds1(bounds8.min.y + 0.01f, bounds8.max.y - 0.01f) + val6.y,
								rotation = transform.m_Rotation
							}, cylinder1: val28, pos: ref pos))
							{
								AddCollision(overridableCollision, objectEntity);
								return;
							}
						}
						else
						{
							Box3 val33 = default(Box3);
							val33.bounds = bounds8 + val6;
							val33.bounds = MathUtils.Expand(val33.bounds, float3.op_Implicit(-0.01f));
							val33.rotation = transform.m_Rotation;
							Bounds3 val34 = default(Bounds3);
							Bounds3 val35 = default(Bounds3);
							if (MathUtils.Intersect(val28, val33, ref val34, ref val35))
							{
								AddCollision(overridableCollision, objectEntity);
								return;
							}
						}
					}
					else
					{
						Box3 val36 = default(Box3);
						val36.bounds = bounds2 + val5;
						val36.bounds = MathUtils.Expand(val36.bounds, float3.op_Implicit(-0.01f));
						val36.rotation = m_ObjectTransform.m_Rotation;
						Bounds3 bounds10 = ObjectUtils.GetBounds(stack, objectGeometryData, stackData);
						if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
						{
							bounds10.min.y = math.max(bounds10.min.y, 0f);
						}
						if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount5))
						{
							Bounds3 val38 = default(Bounds3);
							Bounds3 val39 = default(Bounds3);
							Bounds3 val41 = default(Bounds3);
							Bounds3 val42 = default(Bounds3);
							for (int num = 0; num < legCount5; num++)
							{
								float3 val37 = val6 + ObjectUtils.GetStandingLegOffset(objectGeometryData, num);
								if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
								{
									if (MathUtils.Intersect(new Cylinder3
									{
										circle = new Circle2(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref val37)).xz),
										height = new Bounds1(bounds10.min.y + 0.01f, objectGeometryData.m_LegSize.y + 0.01f) + val37.y,
										rotation = transform.m_Rotation
									}, val36, ref val38, ref val39))
									{
										AddCollision(overridableCollision, objectEntity);
										return;
									}
								}
								else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
								{
									Box3 val40 = new Box3
									{
										bounds = 
										{
											min = 
											{
												y = bounds10.min.y + 0.01f
											}
										}
									};
									((float3)(ref val40.bounds.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f + 0.01f;
									val40.bounds.max.y = objectGeometryData.m_LegSize.y + 0.01f;
									((float3)(ref val40.bounds.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f - 0.01f;
									ref Bounds3 bounds11 = ref val40.bounds;
									bounds11 += val37;
									val40.rotation = transform.m_Rotation;
									if (MathUtils.Intersect(val36, val40, ref val41, ref val42))
									{
										AddCollision(overridableCollision, objectEntity);
										return;
									}
								}
							}
							bounds10.min.y = objectGeometryData.m_LegSize.y;
						}
						if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
						{
							Bounds3 val43 = default(Bounds3);
							Bounds3 val44 = default(Bounds3);
							if (MathUtils.Intersect(new Cylinder3
							{
								circle = new Circle2(objectGeometryData.m_Size.x * 0.5f - 0.01f, ((float3)(ref val6)).xz),
								height = new Bounds1(bounds10.min.y + 0.01f, bounds10.max.y - 0.01f) + val6.y,
								rotation = transform.m_Rotation
							}, val36, ref val43, ref val44))
							{
								AddCollision(overridableCollision, objectEntity);
								return;
							}
						}
						else
						{
							Box3 val45 = default(Box3);
							val45.bounds = bounds10 + val6;
							val45.bounds = MathUtils.Expand(val45.bounds, float3.op_Implicit(-0.01f));
							val45.rotation = transform.m_Rotation;
							Bounds3 val46 = default(Bounds3);
							Bounds3 val47 = default(Bounds3);
							if (MathUtils.Intersect(val36, val45, ref val46, ref val47))
							{
								AddCollision(overridableCollision, objectEntity);
								return;
							}
						}
					}
				}
				if (!CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask))
				{
					return;
				}
				Quad3 val51;
				float3 val53;
				if (ObjectUtils.GetStandingLegCount(m_PrefabGeometryData, out var legCount6))
				{
					Circle2 val48 = default(Circle2);
					Circle2 val49 = default(Circle2);
					Bounds2 val52 = default(Bounds2);
					Circle2 val54 = default(Circle2);
					Bounds2 val55 = default(Bounds2);
					Circle2 val57 = default(Circle2);
					Bounds2 val58 = default(Bounds2);
					Bounds2 val60 = default(Bounds2);
					Circle2 val61 = default(Circle2);
					Bounds2 val62 = default(Bounds2);
					Bounds2 val63 = default(Bounds2);
					for (int num2 = 0; num2 < legCount6; num2++)
					{
						float3 position = ObjectUtils.GetStandingLegPosition(m_PrefabGeometryData, m_ObjectTransform, num2) - val2;
						if ((m_PrefabGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
						{
							((Circle2)(ref val48))._002Ector(m_PrefabGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position)).xz);
							if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount7))
							{
								for (int num3 = 0; num3 < legCount7; num3++)
								{
									float3 position2 = ObjectUtils.GetStandingLegPosition(objectGeometryData, transform, num3) - val2;
									if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
									{
										((Circle2)(ref val49))._002Ector(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position2)).xz);
										if (MathUtils.Intersect(val48, val49))
										{
											AddCollision(overridableCollision, objectEntity);
											return;
										}
									}
									else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
									{
										Bounds3 val50 = default(Bounds3);
										((float3)(ref val50.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f;
										((float3)(ref val50.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f;
										val51 = ObjectUtils.CalculateBaseCorners(position2, transform.m_Rotation, MathUtils.Expand(val50, float3.op_Implicit(-0.01f)));
										if (MathUtils.Intersect(((Quad3)(ref val51)).xz, val48, ref val52))
										{
											AddCollision(overridableCollision, objectEntity);
											return;
										}
									}
								}
							}
							else if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
							{
								float num4 = objectGeometryData.m_Size.x * 0.5f - 0.01f;
								val53 = transform.m_Position - val2;
								((Circle2)(ref val54))._002Ector(num4, ((float3)(ref val53)).xz);
								if (MathUtils.Intersect(val48, val54))
								{
									AddCollision(overridableCollision, objectEntity);
									break;
								}
							}
							else
							{
								val51 = ObjectUtils.CalculateBaseCorners(transform.m_Position - val2, transform.m_Rotation, MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
								if (MathUtils.Intersect(((Quad3)(ref val51)).xz, val48, ref val55))
								{
									AddCollision(overridableCollision, objectEntity);
									break;
								}
							}
						}
						else
						{
							if ((m_PrefabGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) != GeometryFlags.None)
							{
								continue;
							}
							Bounds3 val56 = default(Bounds3);
							((float3)(ref val56.min)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * -0.5f;
							((float3)(ref val56.max)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * 0.5f;
							val51 = ObjectUtils.CalculateBaseCorners(position, m_ObjectTransform.m_Rotation, MathUtils.Expand(val56, float3.op_Implicit(-0.01f)));
							Quad2 xz = ((Quad3)(ref val51)).xz;
							if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount8))
							{
								for (int num5 = 0; num5 < legCount8; num5++)
								{
									float3 position3 = ObjectUtils.GetStandingLegPosition(objectGeometryData, transform, num5) - val2;
									if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
									{
										((Circle2)(ref val57))._002Ector(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position3)).xz);
										if (MathUtils.Intersect(xz, val57, ref val58))
										{
											AddCollision(overridableCollision, objectEntity);
											return;
										}
									}
									else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
									{
										Bounds3 val59 = default(Bounds3);
										((float3)(ref val59.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f;
										((float3)(ref val59.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f;
										val51 = ObjectUtils.CalculateBaseCorners(position3, transform.m_Rotation, MathUtils.Expand(val59, float3.op_Implicit(-0.01f)));
										Quad2 xz2 = ((Quad3)(ref val51)).xz;
										if (MathUtils.Intersect(xz, xz2, ref val60))
										{
											AddCollision(overridableCollision, objectEntity);
											return;
										}
									}
								}
							}
							else if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
							{
								float num6 = objectGeometryData.m_Size.x * 0.5f - 0.01f;
								val53 = transform.m_Position - val2;
								((Circle2)(ref val61))._002Ector(num6, ((float3)(ref val53)).xz);
								if (MathUtils.Intersect(xz, val61, ref val62))
								{
									AddCollision(overridableCollision, objectEntity);
									break;
								}
							}
							else
							{
								val51 = ObjectUtils.CalculateBaseCorners(transform.m_Position - val2, transform.m_Rotation, MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
								Quad2 xz3 = ((Quad3)(ref val51)).xz;
								if (MathUtils.Intersect(xz, xz3, ref val63))
								{
									AddCollision(overridableCollision, objectEntity);
									break;
								}
							}
						}
					}
					return;
				}
				if ((m_PrefabGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					float num7 = m_PrefabGeometryData.m_Size.x * 0.5f - 0.01f;
					val53 = m_ObjectTransform.m_Position - val2;
					Circle2 val64 = default(Circle2);
					((Circle2)(ref val64))._002Ector(num7, ((float3)(ref val53)).xz);
					if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount9))
					{
						Circle2 val65 = default(Circle2);
						Bounds2 val67 = default(Bounds2);
						for (int num8 = 0; num8 < legCount9; num8++)
						{
							float3 position4 = ObjectUtils.GetStandingLegPosition(objectGeometryData, transform, num8) - val2;
							if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
							{
								((Circle2)(ref val65))._002Ector(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position4)).xz);
								if (MathUtils.Intersect(val64, val65))
								{
									AddCollision(overridableCollision, objectEntity);
									break;
								}
							}
							else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
							{
								Bounds3 val66 = default(Bounds3);
								((float3)(ref val66.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f;
								((float3)(ref val66.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f;
								val51 = ObjectUtils.CalculateBaseCorners(position4, transform.m_Rotation, MathUtils.Expand(val66, float3.op_Implicit(-0.01f)));
								if (MathUtils.Intersect(((Quad3)(ref val51)).xz, val64, ref val67))
								{
									AddCollision(overridableCollision, objectEntity);
									break;
								}
							}
						}
					}
					else if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
					{
						float num9 = objectGeometryData.m_Size.x * 0.5f - 0.01f;
						val53 = transform.m_Position - val2;
						Circle2 val68 = default(Circle2);
						((Circle2)(ref val68))._002Ector(num9, ((float3)(ref val53)).xz);
						if (MathUtils.Intersect(val64, val68))
						{
							AddCollision(overridableCollision, objectEntity);
						}
					}
					else
					{
						val51 = ObjectUtils.CalculateBaseCorners(transform.m_Position - val2, transform.m_Rotation, MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
						Bounds2 val69 = default(Bounds2);
						if (MathUtils.Intersect(((Quad3)(ref val51)).xz, val64, ref val69))
						{
							AddCollision(overridableCollision, objectEntity);
						}
					}
					return;
				}
				val51 = ObjectUtils.CalculateBaseCorners(m_ObjectTransform.m_Position - val2, m_ObjectTransform.m_Rotation, MathUtils.Expand(m_PrefabGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
				Quad2 xz4 = ((Quad3)(ref val51)).xz;
				if (ObjectUtils.GetStandingLegCount(objectGeometryData, out var legCount10))
				{
					Circle2 val70 = default(Circle2);
					Bounds2 val71 = default(Bounds2);
					Bounds2 val73 = default(Bounds2);
					for (int num10 = 0; num10 < legCount10; num10++)
					{
						float3 position5 = ObjectUtils.GetStandingLegPosition(objectGeometryData, transform, num10) - val2;
						if ((objectGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
						{
							((Circle2)(ref val70))._002Ector(objectGeometryData.m_LegSize.x * 0.5f - 0.01f, ((float3)(ref position5)).xz);
							if (MathUtils.Intersect(xz4, val70, ref val71))
							{
								AddCollision(overridableCollision, objectEntity);
								break;
							}
						}
						else if ((objectGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
						{
							Bounds3 val72 = default(Bounds3);
							((float3)(ref val72.min)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * -0.5f;
							((float3)(ref val72.max)).xz = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f;
							val51 = ObjectUtils.CalculateBaseCorners(position5, transform.m_Rotation, MathUtils.Expand(val72, float3.op_Implicit(-0.01f)));
							Quad2 xz5 = ((Quad3)(ref val51)).xz;
							if (MathUtils.Intersect(xz4, xz5, ref val73))
							{
								AddCollision(overridableCollision, objectEntity);
								break;
							}
						}
					}
				}
				else if ((objectGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					float num11 = objectGeometryData.m_Size.x * 0.5f - 0.01f;
					val53 = transform.m_Position - val2;
					Circle2 val74 = default(Circle2);
					((Circle2)(ref val74))._002Ector(num11, ((float3)(ref val53)).xz);
					Bounds2 val75 = default(Bounds2);
					if (MathUtils.Intersect(xz4, val74, ref val75))
					{
						AddCollision(overridableCollision, objectEntity);
					}
				}
				else
				{
					val51 = ObjectUtils.CalculateBaseCorners(transform.m_Position - val2, transform.m_Rotation, MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
					Quad2 xz6 = ((Quad3)(ref val51)).xz;
					Bounds2 val76 = default(Bounds2);
					if (MathUtils.Intersect(xz4, xz6, ref val76))
					{
						AddCollision(overridableCollision, objectEntity);
					}
				}
			}

			private void AddCollision(bool overridableCollision, Entity other)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				if (overridableCollision)
				{
					if (!m_OverridableCollisions.IsCreated)
					{
						m_OverridableCollisions = new NativeList<Entity>(10, AllocatorHandle.op_Implicit((Allocator)2));
					}
					m_OverridableCollisions.Add(ref other);
				}
				else
				{
					m_CollisionFound = true;
				}
			}
		}

		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_TopLevelEntity;

			public Entity m_AttachedParent;

			public Entity m_ObjectEntity;

			public Bounds3 m_ObjectBounds;

			public CollisionMask m_CollisionMask;

			public Transform m_ObjectTransform;

			public Stack m_ObjectStack;

			public ObjectGeometryData m_PrefabGeometryData;

			public StackData m_ObjectStackData;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

			public bool m_CollisionFound;

			public bool m_EditorMode;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				if (m_CollisionFound)
				{
					return false;
				}
				if ((m_CollisionMask & CollisionMask.OnGround) != 0)
				{
					return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz);
				}
				return MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity)
			{
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0202: Unknown result type (might be due to invalid IL or missing references)
				//IL_0206: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_023a: Unknown result type (might be due to invalid IL or missing references)
				//IL_023f: Unknown result type (might be due to invalid IL or missing references)
				//IL_024a: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0251: Unknown result type (might be due to invalid IL or missing references)
				//IL_0256: Unknown result type (might be due to invalid IL or missing references)
				//IL_025b: Unknown result type (might be due to invalid IL or missing references)
				//IL_026f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0274: Unknown result type (might be due to invalid IL or missing references)
				//IL_021f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0225: Unknown result type (might be due to invalid IL or missing references)
				//IL_0290: Unknown result type (might be due to invalid IL or missing references)
				//IL_0292: Unknown result type (might be due to invalid IL or missing references)
				//IL_073c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0744: Unknown result type (might be due to invalid IL or missing references)
				//IL_0746: Unknown result type (might be due to invalid IL or missing references)
				//IL_0748: Unknown result type (might be due to invalid IL or missing references)
				//IL_074d: Unknown result type (might be due to invalid IL or missing references)
				//IL_075a: Unknown result type (might be due to invalid IL or missing references)
				//IL_075f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0607: Unknown result type (might be due to invalid IL or missing references)
				//IL_0627: Unknown result type (might be due to invalid IL or missing references)
				//IL_062c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0631: Unknown result type (might be due to invalid IL or missing references)
				//IL_0638: Unknown result type (might be due to invalid IL or missing references)
				//IL_063a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0644: Unknown result type (might be due to invalid IL or missing references)
				//IL_0646: Unknown result type (might be due to invalid IL or missing references)
				//IL_0650: Unknown result type (might be due to invalid IL or missing references)
				//IL_0655: Unknown result type (might be due to invalid IL or missing references)
				//IL_065c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0661: Unknown result type (might be due to invalid IL or missing references)
				//IL_066e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0673: Unknown result type (might be due to invalid IL or missing references)
				//IL_0771: Unknown result type (might be due to invalid IL or missing references)
				//IL_0778: Unknown result type (might be due to invalid IL or missing references)
				//IL_077a: Unknown result type (might be due to invalid IL or missing references)
				//IL_077f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0782: Unknown result type (might be due to invalid IL or missing references)
				//IL_0789: Unknown result type (might be due to invalid IL or missing references)
				//IL_0685: Unknown result type (might be due to invalid IL or missing references)
				//IL_068c: Unknown result type (might be due to invalid IL or missing references)
				//IL_068e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0693: Unknown result type (might be due to invalid IL or missing references)
				//IL_0696: Unknown result type (might be due to invalid IL or missing references)
				//IL_069d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
				//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_06df: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0318: Unknown result type (might be due to invalid IL or missing references)
				//IL_031d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0322: Unknown result type (might be due to invalid IL or missing references)
				//IL_0329: Unknown result type (might be due to invalid IL or missing references)
				//IL_032b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0345: Unknown result type (might be due to invalid IL or missing references)
				//IL_034a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0351: Unknown result type (might be due to invalid IL or missing references)
				//IL_0356: Unknown result type (might be due to invalid IL or missing references)
				//IL_0363: Unknown result type (might be due to invalid IL or missing references)
				//IL_0368: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cb3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
				//IL_085a: Unknown result type (might be due to invalid IL or missing references)
				//IL_085f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0861: Unknown result type (might be due to invalid IL or missing references)
				//IL_0866: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0803: Unknown result type (might be due to invalid IL or missing references)
				//IL_0806: Unknown result type (might be due to invalid IL or missing references)
				//IL_080d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0701: Unknown result type (might be due to invalid IL or missing references)
				//IL_0707: Unknown result type (might be due to invalid IL or missing references)
				//IL_0713: Unknown result type (might be due to invalid IL or missing references)
				//IL_0715: Unknown result type (might be due to invalid IL or missing references)
				//IL_071a: Unknown result type (might be due to invalid IL or missing references)
				//IL_071d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0724: Unknown result type (might be due to invalid IL or missing references)
				//IL_0447: Unknown result type (might be due to invalid IL or missing references)
				//IL_0459: Unknown result type (might be due to invalid IL or missing references)
				//IL_045b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0481: Unknown result type (might be due to invalid IL or missing references)
				//IL_048b: Unknown result type (might be due to invalid IL or missing references)
				//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0502: Unknown result type (might be due to invalid IL or missing references)
				//IL_0507: Unknown result type (might be due to invalid IL or missing references)
				//IL_037a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0381: Unknown result type (might be due to invalid IL or missing references)
				//IL_0383: Unknown result type (might be due to invalid IL or missing references)
				//IL_0388: Unknown result type (might be due to invalid IL or missing references)
				//IL_038b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0392: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cea: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cf4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bb8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
				//IL_089d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0519: Unknown result type (might be due to invalid IL or missing references)
				//IL_0520: Unknown result type (might be due to invalid IL or missing references)
				//IL_0522: Unknown result type (might be due to invalid IL or missing references)
				//IL_0527: Unknown result type (might be due to invalid IL or missing references)
				//IL_052a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0531: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d2c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0d53: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c19: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
				//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_08da: Unknown result type (might be due to invalid IL or missing references)
				//IL_0550: Unknown result type (might be due to invalid IL or missing references)
				//IL_0556: Unknown result type (might be due to invalid IL or missing references)
				//IL_0562: Unknown result type (might be due to invalid IL or missing references)
				//IL_0564: Unknown result type (might be due to invalid IL or missing references)
				//IL_0569: Unknown result type (might be due to invalid IL or missing references)
				//IL_056c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0573: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0408: Unknown result type (might be due to invalid IL or missing references)
				//IL_040a: Unknown result type (might be due to invalid IL or missing references)
				//IL_040f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0412: Unknown result type (might be due to invalid IL or missing references)
				//IL_0419: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
				//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0903: Unknown result type (might be due to invalid IL or missing references)
				//IL_0911: Unknown result type (might be due to invalid IL or missing references)
				//IL_0916: Unknown result type (might be due to invalid IL or missing references)
				//IL_091b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0923: Unknown result type (might be due to invalid IL or missing references)
				//IL_092a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0592: Unknown result type (might be due to invalid IL or missing references)
				//IL_0598: Unknown result type (might be due to invalid IL or missing references)
				//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0950: Unknown result type (might be due to invalid IL or missing references)
				//IL_0956: Unknown result type (might be due to invalid IL or missing references)
				//IL_0964: Unknown result type (might be due to invalid IL or missing references)
				//IL_0969: Unknown result type (might be due to invalid IL or missing references)
				//IL_096e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0976: Unknown result type (might be due to invalid IL or missing references)
				//IL_097d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
				if (m_CollisionFound)
				{
					return;
				}
				if ((m_CollisionMask & CollisionMask.OnGround) != 0)
				{
					if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz))
					{
						return;
					}
				}
				else if (!MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds))
				{
					return;
				}
				if (!m_EdgeGeometryData.HasComponent(edgeEntity))
				{
					return;
				}
				Edge edge = m_EdgeData[edgeEntity];
				Entity val = m_ObjectEntity;
				if (edgeEntity == m_AttachedParent || edge.m_Start == m_AttachedParent || edge.m_End == m_AttachedParent)
				{
					return;
				}
				Owner owner = default(Owner);
				while (m_OwnerData.TryGetComponent(val, ref owner))
				{
					val = owner.m_Owner;
					if (edgeEntity == val || edge.m_Start == val || edge.m_End == val)
					{
						return;
					}
				}
				Entity val2 = edgeEntity;
				bool flag = false;
				Owner owner2 = default(Owner);
				while (m_OwnerData.TryGetComponent(val2, ref owner2) && !m_BuildingData.HasComponent(val2))
				{
					flag = true;
					val2 = owner2.m_Owner;
				}
				if (m_TopLevelEntity == val2)
				{
					return;
				}
				Composition composition = m_CompositionData[edgeEntity];
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[edgeEntity];
				StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[edgeEntity];
				EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[edgeEntity];
				NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
				NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
				NetCompositionData netCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
				CollisionMask collisionMask = NetUtils.GetCollisionMask(netCompositionData, !m_EditorMode || flag);
				CollisionMask collisionMask2 = NetUtils.GetCollisionMask(netCompositionData2, !m_EditorMode || flag);
				CollisionMask collisionMask3 = NetUtils.GetCollisionMask(netCompositionData3, !m_EditorMode || flag);
				CollisionMask collisionMask4 = collisionMask | collisionMask2 | collisionMask3;
				if ((m_CollisionMask & collisionMask4) == 0)
				{
					return;
				}
				DynamicBuffer<NetCompositionArea> areas = default(DynamicBuffer<NetCompositionArea>);
				DynamicBuffer<NetCompositionArea> areas2 = default(DynamicBuffer<NetCompositionArea>);
				DynamicBuffer<NetCompositionArea> areas3 = default(DynamicBuffer<NetCompositionArea>);
				float3 val3 = MathUtils.Center(bounds.m_Bounds);
				Bounds3 intersection = default(Bounds3);
				Bounds2 intersection2 = default(Bounds2);
				if ((m_CollisionMask & CollisionMask.OnGround) == 0 || MathUtils.Intersect(bounds.m_Bounds, m_ObjectBounds))
				{
					float3 val4 = math.mul(math.inverse(m_ObjectTransform.m_Rotation), m_ObjectTransform.m_Position - val3);
					Bounds3 bounds2 = ObjectUtils.GetBounds(m_ObjectStack, m_PrefabGeometryData, m_ObjectStackData);
					if ((m_PrefabGeometryData.m_Flags & GeometryFlags.IgnoreBottomCollision) != GeometryFlags.None)
					{
						bounds2.min.y = math.max(bounds2.min.y, 0f);
					}
					if (ObjectUtils.GetStandingLegCount(m_PrefabGeometryData, out var legCount))
					{
						for (int i = 0; i < legCount; i++)
						{
							float3 val5 = val4 + ObjectUtils.GetStandingLegOffset(m_PrefabGeometryData, i);
							if ((m_PrefabGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
							{
								Cylinder3 cylinder = new Cylinder3
								{
									circle = new Circle2(m_PrefabGeometryData.m_LegSize.x * 0.5f, ((float3)(ref val5)).xz),
									height = new Bounds1(bounds2.min.y, m_PrefabGeometryData.m_LegSize.y) + val5.y,
									rotation = m_ObjectTransform.m_Rotation
								};
								if ((collisionMask & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -val3, cylinder, m_ObjectBounds, netCompositionData, areas, ref intersection))
								{
									m_CollisionFound = true;
									return;
								}
								if ((collisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -val3, cylinder, m_ObjectBounds, netCompositionData2, areas2, ref intersection))
								{
									m_CollisionFound = true;
									return;
								}
								if ((collisionMask3 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -val3, cylinder, m_ObjectBounds, netCompositionData3, areas3, ref intersection))
								{
									m_CollisionFound = true;
									return;
								}
							}
							else if ((m_PrefabGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
							{
								Box3 box = new Box3
								{
									bounds = 
									{
										min = 
										{
											y = bounds2.min.y
										}
									}
								};
								((float3)(ref box.bounds.min)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * -0.5f;
								box.bounds.max.y = m_PrefabGeometryData.m_LegSize.y;
								((float3)(ref box.bounds.max)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * 0.5f;
								ref Bounds3 bounds3 = ref box.bounds;
								bounds3 += val5;
								box.rotation = m_ObjectTransform.m_Rotation;
								if ((collisionMask & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -val3, box, m_ObjectBounds, netCompositionData, areas, ref intersection))
								{
									m_CollisionFound = true;
									return;
								}
								if ((collisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -val3, box, m_ObjectBounds, netCompositionData2, areas2, ref intersection))
								{
									m_CollisionFound = true;
									return;
								}
								if ((collisionMask3 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -val3, box, m_ObjectBounds, netCompositionData3, areas3, ref intersection))
								{
									m_CollisionFound = true;
									return;
								}
							}
						}
						bounds2.min.y = m_PrefabGeometryData.m_LegSize.y;
					}
					if ((m_PrefabGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
					{
						Cylinder3 cylinder2 = new Cylinder3
						{
							circle = new Circle2(m_PrefabGeometryData.m_Size.x * 0.5f, ((float3)(ref val4)).xz),
							height = new Bounds1(bounds2.min.y, bounds2.max.y) + val4.y,
							rotation = m_ObjectTransform.m_Rotation
						};
						if ((collisionMask & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -val3, cylinder2, m_ObjectBounds, netCompositionData, areas, ref intersection))
						{
							m_CollisionFound = true;
							return;
						}
						if ((collisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -val3, cylinder2, m_ObjectBounds, netCompositionData2, areas2, ref intersection))
						{
							m_CollisionFound = true;
							return;
						}
						if ((collisionMask3 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -val3, cylinder2, m_ObjectBounds, netCompositionData3, areas3, ref intersection))
						{
							m_CollisionFound = true;
							return;
						}
					}
					else
					{
						Box3 box2 = new Box3
						{
							bounds = bounds2 + val4,
							rotation = m_ObjectTransform.m_Rotation
						};
						if ((collisionMask & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -val3, box2, m_ObjectBounds, netCompositionData, areas, ref intersection))
						{
							m_CollisionFound = true;
							return;
						}
						if ((collisionMask2 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -val3, box2, m_ObjectBounds, netCompositionData2, areas2, ref intersection))
						{
							m_CollisionFound = true;
							return;
						}
						if ((collisionMask3 & m_CollisionMask) != 0 && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -val3, box2, m_ObjectBounds, netCompositionData3, areas3, ref intersection))
						{
							m_CollisionFound = true;
							return;
						}
					}
				}
				if (!CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask4))
				{
					return;
				}
				Quad3 val6;
				if (ObjectUtils.GetStandingLegCount(m_PrefabGeometryData, out var legCount2))
				{
					Circle2 circle = default(Circle2);
					for (int j = 0; j < legCount2; j++)
					{
						float3 position = ObjectUtils.GetStandingLegPosition(m_PrefabGeometryData, m_ObjectTransform, j) - val3;
						if ((m_PrefabGeometryData.m_Flags & (GeometryFlags.CircularLeg | GeometryFlags.IgnoreLegCollision)) == GeometryFlags.CircularLeg)
						{
							((Circle2)(ref circle))._002Ector(m_PrefabGeometryData.m_LegSize.x * 0.5f, ((float3)(ref position)).xz);
							if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask) && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -((float3)(ref val3)).xz, circle, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData, areas, ref intersection2))
							{
								m_CollisionFound = true;
								break;
							}
							if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask2) && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, circle, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData2, areas2, ref intersection2))
							{
								m_CollisionFound = true;
								break;
							}
							if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask3) && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, circle, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData3, areas3, ref intersection2))
							{
								m_CollisionFound = true;
								break;
							}
						}
						else if ((m_PrefabGeometryData.m_Flags & GeometryFlags.IgnoreLegCollision) == 0)
						{
							Bounds3 bounds4 = default(Bounds3);
							((float3)(ref bounds4.min)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * -0.5f;
							((float3)(ref bounds4.max)).xz = ((float3)(ref m_PrefabGeometryData.m_LegSize)).xz * 0.5f;
							val6 = ObjectUtils.CalculateBaseCorners(position, m_ObjectTransform.m_Rotation, bounds4);
							Quad2 xz = ((Quad3)(ref val6)).xz;
							if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask) && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -((float3)(ref val3)).xz, xz, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData, areas, ref intersection2))
							{
								m_CollisionFound = true;
								break;
							}
							if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask2) && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, xz, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData2, areas2, ref intersection2))
							{
								m_CollisionFound = true;
								break;
							}
							if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask3) && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, xz, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData3, areas3, ref intersection2))
							{
								m_CollisionFound = true;
								break;
							}
						}
					}
				}
				else if ((m_PrefabGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					float num = m_PrefabGeometryData.m_Size.x * 0.5f;
					float3 val7 = m_ObjectTransform.m_Position - val3;
					Circle2 circle2 = default(Circle2);
					((Circle2)(ref circle2))._002Ector(num, ((float3)(ref val7)).xz);
					if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask) && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -((float3)(ref val3)).xz, circle2, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData, areas, ref intersection2))
					{
						m_CollisionFound = true;
					}
					else if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask2) && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, circle2, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData2, areas2, ref intersection2))
					{
						m_CollisionFound = true;
					}
					else if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask3) && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, circle2, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData3, areas3, ref intersection2))
					{
						m_CollisionFound = true;
					}
				}
				else
				{
					val6 = ObjectUtils.CalculateBaseCorners(m_ObjectTransform.m_Position - val3, m_ObjectTransform.m_Rotation, m_PrefabGeometryData.m_Bounds);
					Quad2 xz2 = ((Quad3)(ref val6)).xz;
					if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask) && Game.Net.ValidationHelpers.Intersect(edge, m_ObjectEntity, edgeGeometry, -((float3)(ref val3)).xz, xz2, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData, areas, ref intersection2))
					{
						m_CollisionFound = true;
					}
					else if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask2) && Game.Net.ValidationHelpers.Intersect(edge.m_Start, m_ObjectEntity, startNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, xz2, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData2, areas2, ref intersection2))
					{
						m_CollisionFound = true;
					}
					else if (CommonUtils.ExclusiveGroundCollision(m_CollisionMask, collisionMask3) && Game.Net.ValidationHelpers.Intersect(edge.m_End, m_ObjectEntity, endNodeGeometry.m_Geometry, -((float3)(ref val3)).xz, xz2, ((Bounds3)(ref m_ObjectBounds)).xz, netCompositionData3, areas3, ref intersection2))
					{
						m_CollisionFound = true;
					}
				}
			}
		}

		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Entity m_TopLevelEntity;

			public Entity m_TopLevelArea;

			public Entity m_ObjectEntity;

			public Bounds3 m_ObjectBounds;

			public CollisionMask m_CollisionMask;

			public Transform m_ObjectTransform;

			public ObjectGeometryData m_PrefabGeometryData;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

			public BufferLookup<Game.Areas.Node> m_AreaNodes;

			public BufferLookup<Triangle> m_AreaTriangles;

			public bool m_CollisionFound;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				if (m_CollisionFound)
				{
					return false;
				}
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_017d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				if (m_CollisionFound || !MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_ObjectBounds)).xz))
				{
					return;
				}
				Entity val = areaItem.m_Area;
				if (m_TopLevelArea == val)
				{
					return;
				}
				Owner owner = default(Owner);
				while (m_OwnerData.TryGetComponent(val, ref owner) && !m_BuildingData.HasComponent(val))
				{
					val = owner.m_Owner;
					if (m_TopLevelArea == val)
					{
						return;
					}
				}
				if (m_TopLevelEntity == val)
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[areaItem.m_Area];
				AreaGeometryData areaGeometryData = m_PrefabAreaGeometryData[prefabRef.m_Prefab];
				if ((areaGeometryData.m_Flags & Game.Areas.GeometryFlags.CanOverrideObjects) == 0)
				{
					return;
				}
				CollisionMask collisionMask = AreaUtils.GetCollisionMask(areaGeometryData);
				if ((m_CollisionMask & collisionMask) == 0)
				{
					return;
				}
				Triangle3 triangle = AreaUtils.GetTriangle3(m_AreaNodes[areaItem.m_Area], m_AreaTriangles[areaItem.m_Area][areaItem.m_Triangle]);
				Triangle2 xz = ((Triangle3)(ref triangle)).xz;
				if ((m_PrefabGeometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
				{
					Circle2 val2 = default(Circle2);
					((Circle2)(ref val2))._002Ector(m_PrefabGeometryData.m_Size.x * 0.5f, ((float3)(ref m_ObjectTransform.m_Position)).xz);
					if (MathUtils.Intersect(xz, val2))
					{
						m_CollisionFound = true;
					}
				}
				else
				{
					Quad3 val3 = ObjectUtils.CalculateBaseCorners(m_ObjectTransform.m_Position, m_ObjectTransform.m_Rotation, m_PrefabGeometryData.m_Bounds);
					if (MathUtils.Intersect(((Quad3)(ref val3)).xz, xz))
					{
						m_CollisionFound = true;
					}
				}
			}
		}

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Created> m_CreatedData;

		[ReadOnly]
		public ComponentLookup<Overridden> m_OverriddenData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Stack> m_StackData;

		[ReadOnly]
		public ComponentLookup<Marker> m_MarkerData;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeArray<Entity> m_ObjectArray;

		[ReadOnly]
		public NativeHashSet<Entity> m_ObjectSet;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<TreeAction> m_TreeActions;

		public ParallelWriter<OverridableAction> m_OverridableActions;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_ObjectArray[index];
			PrefabRef prefabRef = m_PrefabRefData[val];
			ObjectGeometryData prefabGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref prefabGeometryData) || (prefabGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) != GeometryFlags.Overridable)
			{
				return;
			}
			NativeList<Entity> overridableCollisions = default(NativeList<Entity>);
			Entity val2 = val;
			bool flag = false;
			Owner owner = default(Owner);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			while (m_OwnerData.TryGetComponent(val2, ref owner))
			{
				val2 = owner.m_Owner;
				PrefabRef prefabRef2 = m_PrefabRefData[owner.m_Owner];
				if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData) || (objectGeometryData.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) != GeometryFlags.Overridable)
				{
					break;
				}
				if (m_ObjectSet.Contains(val2))
				{
					return;
				}
				flag |= m_OverriddenData.HasComponent(val2);
			}
			CheckObject(index, val, prefabRef, prefabGeometryData, flag, delayedResolve: false, ref overridableCollisions);
			if (overridableCollisions.IsCreated)
			{
				overridableCollisions.Dispose();
			}
		}

		private void CheckObject(int jobIndex, Entity entity, PrefabRef prefabRef, ObjectGeometryData prefabGeometryData, bool collision, bool delayedResolve, ref NativeList<Entity> overridableCollisions)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			if (!collision)
			{
				Transform objectTransform = m_TransformData[entity];
				bool ignoreMarkers = !m_EditorMode || m_OwnerData.HasComponent(entity);
				StackData stackData = default(StackData);
				Stack stack = default(Stack);
				Bounds3 objectBounds = ((!m_StackData.TryGetComponent(entity, ref stack) || !m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData)) ? ObjectUtils.CalculateBounds(objectTransform.m_Position, objectTransform.m_Rotation, prefabGeometryData) : ObjectUtils.CalculateBounds(objectTransform.m_Position, objectTransform.m_Rotation, stack, prefabGeometryData, stackData));
				Elevation elevation = default(Elevation);
				CollisionMask collisionMask = ((!m_ElevationData.TryGetComponent(entity, ref elevation)) ? ObjectUtils.GetCollisionMask(prefabGeometryData, ignoreMarkers) : ObjectUtils.GetCollisionMask(prefabGeometryData, elevation, ignoreMarkers));
				Entity val = entity;
				Entity attachedParent = Entity.Null;
				Owner owner = default(Owner);
				Attached attached = default(Attached);
				while (m_OwnerData.TryGetComponent(val, ref owner) && !m_BuildingData.HasComponent(val))
				{
					val = owner.m_Owner;
					if (m_AttachedData.TryGetComponent(owner.m_Owner, ref attached))
					{
						attachedParent = attached.m_Parent;
					}
				}
				Entity val2 = val;
				Owner owner2 = default(Owner);
				while (m_OwnerData.TryGetComponent(val2, ref owner2) && m_AreaNodes.HasBuffer(owner2.m_Owner))
				{
					val2 = owner2.m_Owner;
				}
				if (overridableCollisions.IsCreated)
				{
					overridableCollisions.Clear();
				}
				ObjectIterator objectIterator = new ObjectIterator
				{
					m_TopLevelEntity = val,
					m_ObjectEntity = entity,
					m_ObjectBounds = objectBounds,
					m_CollisionMask = collisionMask,
					m_ObjectTransform = objectTransform,
					m_ObjectStack = stack,
					m_PrefabGeometryData = prefabGeometryData,
					m_ObjectStackData = stackData,
					m_OwnerData = m_OwnerData,
					m_TransformData = m_TransformData,
					m_ElevationData = m_ElevationData,
					m_AttachmentData = m_AttachmentData,
					m_StackData = m_StackData,
					m_BuildingData = m_BuildingData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_PrefabStackData = m_PrefabStackData,
					m_OverridableCollisions = overridableCollisions,
					m_EditorMode = m_EditorMode
				};
				NetIterator netIterator = new NetIterator
				{
					m_TopLevelEntity = val,
					m_AttachedParent = attachedParent,
					m_ObjectEntity = entity,
					m_ObjectBounds = objectBounds,
					m_CollisionMask = collisionMask,
					m_ObjectTransform = objectTransform,
					m_ObjectStack = stack,
					m_PrefabGeometryData = prefabGeometryData,
					m_ObjectStackData = stackData,
					m_OwnerData = m_OwnerData,
					m_BuildingData = m_BuildingData,
					m_EdgeData = m_EdgeData,
					m_EdgeGeometryData = m_EdgeGeometryData,
					m_StartNodeGeometryData = m_StartNodeGeometryData,
					m_EndNodeGeometryData = m_EndNodeGeometryData,
					m_CompositionData = m_CompositionData,
					m_PrefabCompositionData = m_PrefabCompositionData,
					m_EditorMode = m_EditorMode
				};
				AreaIterator areaIterator = new AreaIterator
				{
					m_TopLevelEntity = val,
					m_TopLevelArea = val2,
					m_ObjectEntity = entity,
					m_ObjectBounds = objectBounds,
					m_CollisionMask = collisionMask,
					m_ObjectTransform = objectTransform,
					m_PrefabGeometryData = prefabGeometryData,
					m_OwnerData = m_OwnerData,
					m_BuildingData = m_BuildingData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabAreaGeometryData = m_PrefabAreaGeometryData,
					m_AreaNodes = m_AreaNodes,
					m_AreaTriangles = m_AreaTriangles
				};
				if (m_ConnectedEdges.HasBuffer(val))
				{
					objectIterator.m_TopLevelEdges = m_ConnectedEdges[val];
				}
				else if (m_ConnectedNodes.HasBuffer(val))
				{
					objectIterator.m_TopLevelNodes = m_ConnectedNodes[val];
					objectIterator.m_TopLevelEdge = m_EdgeData[val];
				}
				m_ObjectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
				if (!objectIterator.m_CollisionFound)
				{
					m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
					if (!netIterator.m_CollisionFound)
					{
						m_AreaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
					}
				}
				collision = objectIterator.m_CollisionFound || netIterator.m_CollisionFound || areaIterator.m_CollisionFound;
				overridableCollisions = objectIterator.m_OverridableCollisions;
				if (!collision)
				{
					if (overridableCollisions.IsCreated && overridableCollisions.Length != 0)
					{
						OverridableAction overridableAction = new OverridableAction
						{
							m_Entity = entity,
							m_Mask = GetBoundsMask(entity, prefabGeometryData)
						};
						if (m_CreatedData.HasComponent(entity))
						{
							overridableAction.m_Priority |= 1;
						}
						if (m_OverriddenData.HasComponent(entity))
						{
							overridableAction.m_Priority |= 2;
						}
						for (int i = 0; i < objectIterator.m_OverridableCollisions.Length; i++)
						{
							overridableAction.m_Other = objectIterator.m_OverridableCollisions[i];
							overridableAction.m_OtherOverridden = m_OverriddenData.HasComponent(overridableAction.m_Other);
							m_OverridableActions.Enqueue(overridableAction);
						}
						delayedResolve = true;
					}
					else if (delayedResolve)
					{
						OverridableAction overridableAction2 = new OverridableAction
						{
							m_Entity = entity,
							m_Mask = GetBoundsMask(entity, prefabGeometryData)
						};
						if (m_OverriddenData.HasComponent(entity))
						{
							overridableAction2.m_Priority |= 2;
						}
						m_OverridableActions.Enqueue(overridableAction2);
					}
				}
			}
			if (!collision && delayedResolve)
			{
				DynamicBuffer<SubObject> val3 = default(DynamicBuffer<SubObject>);
				if (!m_SubObjects.TryGetBuffer(entity, ref val3))
				{
					return;
				}
				ObjectGeometryData prefabGeometryData2 = default(ObjectGeometryData);
				for (int j = 0; j < val3.Length; j++)
				{
					Entity subObject = val3[j].m_SubObject;
					PrefabRef prefabRef2 = m_PrefabRefData[subObject];
					if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref prefabGeometryData2) && (prefabGeometryData2.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == GeometryFlags.Overridable)
					{
						CheckObject(jobIndex, subObject, prefabRef, prefabGeometryData2, collision: false, delayedResolve: true, ref overridableCollisions);
					}
				}
				return;
			}
			bool flag = collision != m_OverriddenData.HasComponent(entity);
			if (flag)
			{
				if (collision)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Overridden>(jobIndex, entity, default(Overridden));
					AddTreeAction(entity, prefabGeometryData, isOverridden: true);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Overridden>(jobIndex, entity);
					AddTreeAction(entity, prefabGeometryData, isOverridden: false);
				}
			}
			DynamicBuffer<SubObject> val4 = default(DynamicBuffer<SubObject>);
			if (!m_SubObjects.TryGetBuffer(entity, ref val4))
			{
				return;
			}
			ObjectGeometryData prefabGeometryData3 = default(ObjectGeometryData);
			for (int k = 0; k < val4.Length; k++)
			{
				Entity subObject2 = val4[k].m_SubObject;
				PrefabRef prefabRef3 = m_PrefabRefData[subObject2];
				if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef3.m_Prefab, ref prefabGeometryData3) && (prefabGeometryData3.m_Flags & (GeometryFlags.Overridable | GeometryFlags.DeleteOverridden)) == GeometryFlags.Overridable && (flag || (!collision && m_ObjectSet.Contains(subObject2))))
				{
					CheckObject(jobIndex, subObject2, prefabRef, prefabGeometryData3, collision, delayedResolve, ref overridableCollisions);
				}
			}
		}

		private void AddTreeAction(Entity entity, ObjectGeometryData prefabObjectGeometryData, bool isOverridden)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			TreeAction treeAction = new TreeAction
			{
				m_Entity = entity
			};
			if (!isOverridden)
			{
				treeAction.m_Mask = GetBoundsMask(entity, prefabObjectGeometryData);
			}
			m_TreeActions.Enqueue(treeAction);
		}

		private BoundsMask GetBoundsMask(Entity entity, ObjectGeometryData prefabObjectGeometryData)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			BoundsMask boundsMask = BoundsMask.NotOverridden;
			if (m_EditorMode || !m_MarkerData.HasComponent(entity) || m_OutsideConnectionData.HasComponent(entity))
			{
				MeshLayer layers = prefabObjectGeometryData.m_Layers;
				Owner owner = default(Owner);
				m_OwnerData.TryGetComponent(entity, ref owner);
				boundsMask |= CommonUtils.GetBoundsMask(Game.Net.SearchSystem.GetLayers(owner, default(Game.Net.UtilityLane), layers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
			}
			return boundsMask;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Created> __Game_Common_Created_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Overridden> __Game_Common_Overridden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stack> __Game_Objects_Stack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Marker> __Game_Objects_Marker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

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
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Created_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Created>(true);
			__Game_Common_Overridden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Overridden>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_Stack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stack>(true);
			__Game_Objects_Marker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Marker>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnection>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
		}
	}

	private UpdateCollectSystem m_ObjectUpdateCollectSystem;

	private Game.Net.UpdateCollectSystem m_NetUpdateCollectSystem;

	private Game.Areas.UpdateCollectSystem m_AreaUpdateCollectSystem;

	private SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private ToolSystem m_ToolSystem;

	private ComponentTypeSet m_OverriddenUpdatedSet;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ObjectUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateCollectSystem>();
		m_NetUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.UpdateCollectSystem>();
		m_AreaUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.UpdateCollectSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_OverriddenUpdatedSet = new ComponentTypeSet(ComponentType.ReadWrite<Overridden>(), ComponentType.ReadWrite<Updated>());
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		if (m_ObjectUpdateCollectSystem.isUpdated || m_NetUpdateCollectSystem.netsUpdated || m_AreaUpdateCollectSystem.lotsUpdated)
		{
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeHashSet<Entity> objectSet = default(NativeHashSet<Entity>);
			objectSet._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<TreeAction> actions = default(NativeQueue<TreeAction>);
			actions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<OverridableAction> overridableActions = default(NativeQueue<OverridableAction>);
			overridableActions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, CollectUpdatedObjects(val, objectSet));
			JobHandle dependencies;
			JobHandle dependencies2;
			JobHandle dependencies3;
			CheckObjectOverrideJob checkObjectOverrideJob = new CheckObjectOverrideJob
			{
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedData = InternalCompilerInterface.GetComponentLookup<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OverriddenData = InternalCompilerInterface.GetComponentLookup<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StackData = InternalCompilerInterface.GetComponentLookup<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MarkerData = InternalCompilerInterface.GetComponentLookup<Marker>(ref __TypeHandle.__Game_Objects_Marker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_ObjectArray = val.AsDeferredJobArray(),
				m_ObjectSet = objectSet,
				m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies3)
			};
			EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
			checkObjectOverrideJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkObjectOverrideJob.m_TreeActions = actions.AsParallelWriter();
			checkObjectOverrideJob.m_OverridableActions = overridableActions.AsParallelWriter();
			CheckObjectOverrideJob checkObjectOverrideJob2 = checkObjectOverrideJob;
			JobHandle dependencies4;
			UpdateObjectOverrideJob obj = new UpdateObjectOverrideJob
			{
				m_OverriddenUpdatedSet = m_OverriddenUpdatedSet,
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: false, out dependencies4),
				m_Actions = actions,
				m_OverridableActions = overridableActions,
				m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
			};
			JobHandle val3 = IJobParallelForDeferExtensions.Schedule<CheckObjectOverrideJob, Entity>(checkObjectOverrideJob2, val, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3)));
			JobHandle val4 = IJobExtensions.Schedule<UpdateObjectOverrideJob>(obj, JobHandle.CombineDependencies(val3, dependencies4));
			val.Dispose(val3);
			objectSet.Dispose(val3);
			actions.Dispose(val4);
			overridableActions.Dispose(val4);
			m_ObjectSearchSystem.AddStaticSearchTreeWriter(val4);
			m_NetSearchSystem.AddNetSearchTreeReader(val3);
			m_AreaSearchSystem.AddSearchTreeReader(val3);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val4);
			((SystemBase)this).Dependency = val4;
		}
	}

	private JobHandle CollectUpdatedObjects(NativeList<Entity> updateObjectsList, NativeHashSet<Entity> objectSet)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Entity> queue = default(NativeQueue<Entity>);
		queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Entity> queue2 = default(NativeQueue<Entity>);
		queue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Entity> queue3 = default(NativeQueue<Entity>);
		queue3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> staticSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies);
		JobHandle val = default(JobHandle);
		if (m_ObjectUpdateCollectSystem.isUpdated)
		{
			JobHandle dependencies2;
			NativeList<Bounds2> updatedBounds = m_ObjectUpdateCollectSystem.GetUpdatedBounds(out dependencies2);
			JobHandle val2 = IJobParallelForDeferExtensions.Schedule<FindUpdatedObjectsJob, Bounds2>(new FindUpdatedObjectsJob
			{
				m_Bounds = updatedBounds.AsDeferredJobArray(),
				m_SearchTree = staticSearchTree,
				m_ResultQueue = queue.AsParallelWriter()
			}, updatedBounds, 1, JobHandle.CombineDependencies(dependencies2, dependencies));
			m_ObjectUpdateCollectSystem.AddBoundsReader(val2);
			val = JobHandle.CombineDependencies(val, val2);
		}
		if (m_NetUpdateCollectSystem.netsUpdated)
		{
			JobHandle dependencies3;
			NativeList<Bounds2> updatedNetBounds = m_NetUpdateCollectSystem.GetUpdatedNetBounds(out dependencies3);
			JobHandle val3 = IJobParallelForDeferExtensions.Schedule<FindUpdatedObjectsJob, Bounds2>(new FindUpdatedObjectsJob
			{
				m_Bounds = updatedNetBounds.AsDeferredJobArray(),
				m_SearchTree = staticSearchTree,
				m_ResultQueue = queue2.AsParallelWriter()
			}, updatedNetBounds, 1, JobHandle.CombineDependencies(dependencies3, dependencies));
			m_NetUpdateCollectSystem.AddNetBoundsReader(val3);
			val = JobHandle.CombineDependencies(val, val3);
		}
		if (m_AreaUpdateCollectSystem.lotsUpdated)
		{
			JobHandle dependencies4;
			NativeList<Bounds2> updatedLotBounds = m_AreaUpdateCollectSystem.GetUpdatedLotBounds(out dependencies4);
			JobHandle val4 = IJobParallelForDeferExtensions.Schedule<FindUpdatedObjectsJob, Bounds2>(new FindUpdatedObjectsJob
			{
				m_Bounds = updatedLotBounds.AsDeferredJobArray(),
				m_SearchTree = staticSearchTree,
				m_ResultQueue = queue3.AsParallelWriter()
			}, updatedLotBounds, 1, JobHandle.CombineDependencies(dependencies4, dependencies));
			m_AreaUpdateCollectSystem.AddLotBoundsReader(val4);
			val = JobHandle.CombineDependencies(val, val4);
		}
		JobHandle val5 = IJobExtensions.Schedule<CollectObjectsJob>(new CollectObjectsJob
		{
			m_Queue1 = queue,
			m_Queue2 = queue2,
			m_Queue3 = queue3,
			m_ResultList = updateObjectsList,
			m_ObjectSet = objectSet
		}, val);
		queue.Dispose(val5);
		queue2.Dispose(val5);
		queue3.Dispose(val5);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		return val5;
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
	public OverrideSystem()
	{
	}
}

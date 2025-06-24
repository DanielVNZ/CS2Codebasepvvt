using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ObjectCollisionSystem : GameSystemBase
{
	private struct Collision : IComparable<Collision>
	{
		public quaternion m_Rotation1;

		public quaternion m_Rotation2;

		public float3 m_Position1;

		public float3 m_Position2;

		public float3 m_ImpactLocation;

		public Entity m_Entity1;

		public Entity m_Entity2;

		public float m_Time;

		public int m_CurrentIndex1;

		public int m_CurrentIndex2;

		public int CompareTo(Collision other)
		{
			return (int)math.sign(m_Time - other.m_Time);
		}
	}

	[BurstCompile]
	private struct FindCollisionsJob : IJobChunk
	{
		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Entity m_Ignore;

			public NativeList<Entity> m_ObjectList;

			public ComponentLookup<Unspawned> m_UnspawnedData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<Controller> m_ControllerData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<ObjectGeometryData> m_PrefabGeometryData;

			public BufferLookup<TransformFrame> m_TransformFrames;

			public BufferLookup<ConnectedEdge> m_ConnectedEdges;

			public BufferLookup<Game.Net.SubLane> m_SubLanes;

			public BufferLookup<LaneObject> m_LaneObjects;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(m_Bounds, bounds.m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(m_Bounds, bounds.m_Bounds))
				{
					return;
				}
				if (m_EdgeData.HasComponent(entity))
				{
					Edge edge = m_EdgeData[entity];
					CheckLanes(entity);
					CheckLanes(edge.m_Start);
					CheckLanes(edge.m_End);
				}
				else if (m_ConnectedEdges.HasBuffer(entity))
				{
					DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[entity];
					CheckLanes(entity);
					for (int i = 0; i < val.Length; i++)
					{
						CheckLanes(val[i].m_Edge);
					}
				}
			}

			private void CheckLanes(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				if (!m_SubLanes.HasBuffer(entity))
				{
					return;
				}
				DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[entity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (m_LaneObjects.HasBuffer(subLane))
					{
						DynamicBuffer<LaneObject> val2 = m_LaneObjects[subLane];
						for (int j = 0; j < val2.Length; j++)
						{
							CheckObject(val2[j].m_LaneObject);
						}
					}
				}
			}

			private void CheckObject(Entity entity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				Controller controller = default(Controller);
				if (entity == m_Ignore || (m_ControllerData.TryGetComponent(entity, ref controller) && controller.m_Controller == m_Ignore) || m_UnspawnedData.HasComponent(entity))
				{
					return;
				}
				if (m_TransformFrames.HasBuffer(entity))
				{
					PrefabRef prefabRef = m_PrefabRefData[entity];
					Transform transform = m_TransformData[entity];
					DynamicBuffer<TransformFrame> transformFrames = m_TransformFrames[entity];
					ObjectGeometryData geometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
					int num = CurrentFrameIndex(transform, transformFrames);
					int num2 = math.select(num - 1, 3, num == 0);
					TransformFrame transformFrame = transformFrames[num];
					TransformFrame transformFrame2 = transformFrames[num2];
					Bounds3 val = ObjectUtils.CalculateBounds(transformFrame.m_Position, transformFrame.m_Rotation, geometryData);
					Bounds3 val2 = ObjectUtils.CalculateBounds(transformFrame2.m_Position, transformFrame2.m_Rotation, geometryData);
					if (MathUtils.Intersect(m_Bounds, val | val2))
					{
						CollectionUtils.TryAddUniqueValue<Entity>(m_ObjectList, entity);
					}
				}
				else
				{
					PrefabRef prefabRef2 = m_PrefabRefData[entity];
					Transform transform2 = m_TransformData[entity];
					ObjectGeometryData geometryData2 = m_PrefabGeometryData[prefabRef2.m_Prefab];
					Bounds3 val3 = ObjectUtils.CalculateBounds(transform2.m_Position, transform2.m_Rotation, geometryData2);
					if (MathUtils.Intersect(m_Bounds, val3))
					{
						CollectionUtils.TryAddUniqueValue<Entity>(m_ObjectList, entity);
					}
				}
			}
		}

		[ReadOnly]
		public uint m_PreviousTransformFrameIndex;

		[ReadOnly]
		public uint m_CurrentTransformFrameIndex;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public ParallelWriter<Collision> m_CollisionQueue;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeList<Entity> objectList = default(NativeList<Entity>);
			objectList._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			NetIterator netIterator = new NetIterator
			{
				m_ObjectList = objectList,
				m_UnspawnedData = m_UnspawnedData,
				m_TransformData = m_TransformData,
				m_ControllerData = m_ControllerData,
				m_EdgeData = m_EdgeData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabGeometryData = m_PrefabGeometryData,
				m_TransformFrames = m_TransformFrames,
				m_ConnectedEdges = m_ConnectedEdges,
				m_SubLanes = m_SubLanes,
				m_LaneObjects = m_LaneObjects
			};
			Box3 val4 = default(Box3);
			Box3 val5 = default(Box3);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				DynamicBuffer<TransformFrame> val2 = bufferAccessor[i];
				ObjectGeometryData objectGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				TransformFrame previousFrame = val2[(int)m_PreviousTransformFrameIndex];
				TransformFrame currentFrame = val2[(int)m_CurrentTransformFrameIndex];
				float3 position = previousFrame.m_Position;
				float3 val3 = math.mul(math.inverse(currentFrame.m_Rotation), currentFrame.m_Position - position);
				Bounds3 bounds = objectGeometryData.m_Bounds;
				((Box3)(ref val4))._002Ector(bounds, previousFrame.m_Rotation);
				((Box3)(ref val5))._002Ector(bounds + val3, currentFrame.m_Rotation);
				Bounds3 bounds2 = position + (MathUtils.Bounds(val4) | MathUtils.Bounds(val5));
				netIterator.m_Bounds = bounds2;
				netIterator.m_Ignore = val;
				m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
				for (int j = 0; j < objectList.Length; j++)
				{
					TestCollision(position, previousFrame, currentFrame, val4, val5, val, objectList[j]);
				}
				objectList.Clear();
			}
		}

		private void TestCollision(float3 origin, TransformFrame previousFrame1, TransformFrame currentFrame1, Box3 previousBox1, Box3 currentBox1, Entity entity1, Entity entity2)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[entity2];
			Transform transform = m_TransformData[entity2];
			ObjectGeometryData objectGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
			if (m_TransformFrames.HasBuffer(entity2))
			{
				DynamicBuffer<TransformFrame> transformFrames = m_TransformFrames[entity2];
				int num = CurrentFrameIndex(transform, transformFrames);
				int num2 = math.select(num - 1, 3, num == 0);
				TransformFrame transformFrame = transformFrames[num2];
				TransformFrame transformFrame2 = transformFrames[num];
				quaternion val = math.inverse(transformFrame.m_Rotation);
				quaternion val2 = math.inverse(transformFrame2.m_Rotation);
				float3 val3 = math.mul(val, transformFrame.m_Position - origin);
				float3 val4 = math.mul(val2, transformFrame2.m_Position - origin);
				Box3 val5 = default(Box3);
				((Box3)(ref val5))._002Ector(objectGeometryData.m_Bounds + val3, transformFrame.m_Rotation);
				Box3 val6 = default(Box3);
				((Box3)(ref val6))._002Ector(objectGeometryData.m_Bounds + val4, transformFrame2.m_Rotation);
				Bounds3 val11 = default(Bounds3);
				Bounds3 val12 = default(Bounds3);
				for (int i = 1; i <= 16; i++)
				{
					float num3 = (float)i * 0.0625f;
					Box3 val7 = MathUtils.Lerp(previousBox1, currentBox1, num3);
					Box3 val8 = MathUtils.Lerp(val5, val6, num3);
					Bounds3 val9 = MathUtils.Bounds(val7);
					Bounds3 val10 = MathUtils.Bounds(val8);
					if (MathUtils.Intersect(val9, val10) && MathUtils.Intersect(val7, val8, ref val11, ref val12))
					{
						float3 val13 = math.rotate(val7.rotation, MathUtils.Center(val11)) + origin;
						float3 val14 = math.rotate(val8.rotation, MathUtils.Center(val12)) + origin;
						m_CollisionQueue.Enqueue(new Collision
						{
							m_Rotation1 = val7.rotation,
							m_Rotation2 = val8.rotation,
							m_Position1 = math.lerp(previousFrame1.m_Position, currentFrame1.m_Position, num3),
							m_Position2 = math.lerp(transformFrame.m_Position, transformFrame2.m_Position, num3),
							m_ImpactLocation = math.lerp(val13, val14, 0.5f),
							m_Entity1 = entity1,
							m_Entity2 = entity2,
							m_Time = num3,
							m_CurrentIndex1 = (int)m_CurrentTransformFrameIndex,
							m_CurrentIndex2 = num
						});
						break;
					}
				}
				return;
			}
			float3 val15 = math.mul(math.inverse(transform.m_Rotation), transform.m_Position - origin);
			Box3 val16 = default(Box3);
			((Box3)(ref val16))._002Ector(objectGeometryData.m_Bounds + val15, transform.m_Rotation);
			Bounds3 val17 = MathUtils.Bounds(val16);
			Bounds3 val19 = default(Bounds3);
			Bounds3 val20 = default(Bounds3);
			for (int j = 1; j <= 16; j++)
			{
				float num4 = (float)j * 0.0625f;
				Box3 val18 = MathUtils.Lerp(previousBox1, currentBox1, num4);
				if (MathUtils.Intersect(MathUtils.Bounds(val18), val17) && MathUtils.Intersect(val18, val16, ref val19, ref val20))
				{
					float3 val21 = math.rotate(val18.rotation, MathUtils.Center(val19)) + origin;
					float3 val22 = math.rotate(val16.rotation, MathUtils.Center(val20)) + origin;
					m_CollisionQueue.Enqueue(new Collision
					{
						m_Rotation1 = val18.rotation,
						m_Rotation2 = val16.rotation,
						m_Position1 = math.lerp(previousFrame1.m_Position, currentFrame1.m_Position, num4),
						m_Position2 = transform.m_Position,
						m_ImpactLocation = math.lerp(val21, val22, 0.5f),
						m_Entity1 = entity1,
						m_Entity2 = entity2,
						m_Time = num4,
						m_CurrentIndex1 = (int)m_CurrentTransformFrameIndex,
						m_CurrentIndex2 = -1
					});
					break;
				}
			}
		}

		private static int CurrentFrameIndex(Transform transform, DynamicBuffer<TransformFrame> transformFrames)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			float num = float.MaxValue;
			int result = -1;
			for (int i = 0; i < transformFrames.Length; i++)
			{
				float num2 = math.distancesq(transformFrames[i].m_Position, transform.m_Position);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ResolveCollisionsJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabGeometryData;

		public ComponentLookup<Transform> m_TransformData;

		public ComponentLookup<Moving> m_MovingData;

		public ComponentLookup<Damaged> m_DamagedData;

		public BufferLookup<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public EntityArchetype m_EventImpactArchetype;

		[ReadOnly]
		public EntityArchetype m_DamageEventArchetype;

		[ReadOnly]
		public EntityArchetype m_DestroyEventArchetype;

		[ReadOnly]
		public EventHelpers.StructuralIntegrityData m_StructuralIntegrityData;

		public NativeQueue<Collision> m_CollisionQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_06af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06df: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_0741: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			if (m_CollisionQueue.Count == 0)
			{
				return;
			}
			NativeArray<Collision> val = CollectionUtils.ToArray<Collision>(m_CollisionQueue, (Allocator)2);
			NativeParallelHashSet<Entity> val2 = default(NativeParallelHashSet<Entity>);
			val2._002Ector(val.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
			NativeSortExtension.Sort<Collision>(val);
			Moving moving = default(Moving);
			Moving moving2 = default(Moving);
			InvolvedInAccident involvedInAccident = default(InvolvedInAccident);
			InvolvedInAccident involvedInAccident2 = default(InvolvedInAccident);
			float2 val5 = default(float2);
			for (int i = 0; i < val.Length; i++)
			{
				Collision collision = val[i];
				if (val2.Contains(collision.m_Entity1) || val2.Contains(collision.m_Entity2))
				{
					continue;
				}
				bool flag = m_MovingData.TryGetComponent(collision.m_Entity1, ref moving);
				bool flag2 = m_MovingData.TryGetComponent(collision.m_Entity2, ref moving2);
				PrefabRef prefabRef = m_PrefabRefData[collision.m_Entity1];
				PrefabRef prefabRef2 = m_PrefabRefData[collision.m_Entity2];
				ObjectGeometryData objectGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				ObjectGeometryData objectGeometryData2 = m_PrefabGeometryData[prefabRef2.m_Prefab];
				m_InvolvedInAccidentData.TryGetComponent(collision.m_Entity1, ref involvedInAccident);
				m_InvolvedInAccidentData.TryGetComponent(collision.m_Entity2, ref involvedInAccident2);
				if ((!flag || !flag2) && involvedInAccident.m_Event != Entity.Null && involvedInAccident.m_Event == involvedInAccident2.m_Event)
				{
					continue;
				}
				float3 momentOfInertia = ObjectUtils.CalculateMomentOfInertia(collision.m_Rotation1, objectGeometryData.m_Size);
				float3 momentOfInertia2 = ObjectUtils.CalculateMomentOfInertia(collision.m_Rotation2, objectGeometryData2.m_Size);
				float3 val3 = ObjectUtils.CalculatePointVelocity(collision.m_ImpactLocation - collision.m_Position1, moving);
				float3 val4 = ObjectUtils.CalculatePointVelocity(collision.m_ImpactLocation - collision.m_Position2, moving2);
				if (m_VehicleData.HasComponent(collision.m_Entity2))
				{
					val5.x = objectGeometryData.m_Size.x * objectGeometryData.m_Size.y * objectGeometryData.m_Size.z;
					val5.y = objectGeometryData2.m_Size.x * objectGeometryData2.m_Size.y * objectGeometryData2.m_Size.z;
					val5 = val5 / math.csum(val5) * 1.2f;
					float3 val6 = (val4 - val3) * val5.y;
					float3 val7 = (val3 - val4) * val5.x;
					if (!(math.dot(val6, collision.m_Position1 - collision.m_Position2) < 0f))
					{
						val6 += math.normalizesafe(collision.m_Position1 - collision.m_Position2, default(float3)) * (math.length(val3) * 0.1f);
						val7 += math.normalizesafe(collision.m_Position2 - collision.m_Position1, default(float3)) * (math.length(val4) * 0.1f);
						float3 val8 = CalculatePointAngularVelocityDelta(collision.m_ImpactLocation, collision.m_Position1, val6, momentOfInertia);
						float3 val9 = CalculatePointAngularVelocityDelta(collision.m_ImpactLocation, collision.m_Position2, val7, momentOfInertia2);
						Impact impact = new Impact
						{
							m_Event = ((involvedInAccident2.m_Event != Entity.Null) ? involvedInAccident2.m_Event : involvedInAccident.m_Event),
							m_Target = collision.m_Entity1,
							m_Severity = 10f,
							m_CheckStoppedEvent = true
						};
						Impact impact2 = new Impact
						{
							m_Event = ((involvedInAccident.m_Event != Entity.Null) ? involvedInAccident.m_Event : involvedInAccident2.m_Event),
							m_Target = collision.m_Entity2,
							m_Severity = 10f,
							m_CheckStoppedEvent = true
						};
						if (flag)
						{
							ref float3 velocity = ref moving.m_Velocity;
							velocity += val6;
							ref float3 angularVelocity = ref moving.m_AngularVelocity;
							angularVelocity += val8;
							m_MovingData[collision.m_Entity1] = moving;
							m_TransformData[collision.m_Entity1] = new Transform(collision.m_Position1, collision.m_Rotation1);
							DynamicBuffer<TransformFrame> val10 = m_TransformFrames[collision.m_Entity1];
							val10[collision.m_CurrentIndex1] = new TransformFrame(collision.m_Position1, collision.m_Rotation1, moving.m_Velocity);
						}
						else
						{
							impact.m_VelocityDelta = val6;
							impact.m_AngularVelocityDelta = val8;
						}
						if (flag2)
						{
							ref float3 velocity2 = ref moving2.m_Velocity;
							velocity2 += val7;
							ref float3 angularVelocity2 = ref moving2.m_AngularVelocity;
							angularVelocity2 += val9;
							m_MovingData[collision.m_Entity2] = moving2;
							m_TransformData[collision.m_Entity2] = new Transform(collision.m_Position2, collision.m_Rotation2);
							DynamicBuffer<TransformFrame> val11 = m_TransformFrames[collision.m_Entity2];
							val11[collision.m_CurrentIndex2] = new TransformFrame(collision.m_Position2, collision.m_Rotation2, moving2.m_Velocity);
						}
						else
						{
							impact2.m_VelocityDelta = val7;
							impact2.m_AngularVelocityDelta = val9;
						}
						Entity val12 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_EventImpactArchetype);
						Entity val13 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_EventImpactArchetype);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Impact>(val12, impact);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Impact>(val13, impact2);
						val2.Add(collision.m_Entity1);
						val2.Add(collision.m_Entity2);
						AddDamage(collision.m_Entity1, impact.m_Event, prefabRef, val6);
						AddDamage(collision.m_Entity2, impact2.m_Event, prefabRef2, val7);
					}
					continue;
				}
				float3 val14 = (val3 - val4) * 0.5f;
				if (!(math.dot(val14, collision.m_Position2 - collision.m_Position1) < 0f))
				{
					val14 += math.normalizesafe(collision.m_Position2 - collision.m_Position1, default(float3)) * (math.length(val4) * 0.1f);
					float3 val15 = CalculatePointAngularVelocityDelta(collision.m_ImpactLocation, collision.m_Position2, val14, momentOfInertia2);
					Impact impact3 = new Impact
					{
						m_Event = ((involvedInAccident.m_Event != Entity.Null) ? involvedInAccident.m_Event : involvedInAccident2.m_Event),
						m_Target = collision.m_Entity2,
						m_Severity = 10f
					};
					if (m_MovingData.HasComponent(collision.m_Entity2))
					{
						ref float3 velocity3 = ref moving2.m_Velocity;
						velocity3 += val14;
						ref float3 angularVelocity3 = ref moving2.m_AngularVelocity;
						angularVelocity3 += val15;
						m_MovingData[collision.m_Entity2] = moving2;
						m_TransformData[collision.m_Entity2] = new Transform(collision.m_Position2, collision.m_Rotation2);
						DynamicBuffer<TransformFrame> val16 = m_TransformFrames[collision.m_Entity2];
						val16[collision.m_CurrentIndex2] = new TransformFrame(collision.m_Position2, collision.m_Rotation2, moving2.m_Velocity);
					}
					else
					{
						impact3.m_VelocityDelta = val14;
						impact3.m_AngularVelocityDelta = val15;
					}
					Entity val17 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_EventImpactArchetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Impact>(val17, impact3);
					val2.Add(collision.m_Entity2);
				}
			}
		}

		private float3 CalculatePointAngularVelocityDelta(float3 position, float3 origin, float3 velocityDelta, float3 momentOfInertia)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return math.cross(position - origin, velocityDelta) / momentOfInertia;
		}

		private void AddDamage(Entity entity, Entity _event, PrefabRef prefabRef, float3 velocityDelta)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			float structuralIntegrity = m_StructuralIntegrityData.GetStructuralIntegrity(prefabRef.m_Prefab, isBuilding: false);
			float num = math.min(1f, math.length(velocityDelta) * 100f / structuralIntegrity);
			if (num < 0.01f)
			{
				return;
			}
			if (m_DamagedData.HasComponent(entity))
			{
				Damaged damaged = m_DamagedData[entity];
				damaged.m_Damage.x = math.min(1f, damaged.m_Damage.x + num);
				m_DamagedData[entity] = damaged;
				if (ObjectUtils.GetTotalDamage(damaged) == 1f)
				{
					Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_DestroyEventArchetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Destroy>(val, new Destroy(entity, _event));
				}
			}
			else
			{
				Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_DamageEventArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Damage>(val2, new Damage(entity, new float3(num, 0f, 0f)));
				if (num >= 1f)
				{
					val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_DestroyEventArchetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Destroy>(val2, new Destroy(entity, _event));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

		public ComponentLookup<Moving> __Game_Objects_Moving_RW_ComponentLookup;

		public ComponentLookup<Damaged> __Game_Objects_Damaged_RW_ComponentLookup;

		public BufferLookup<TransformFrame> __Game_Objects_TransformFrame_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_TransformFrame_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Objects_TransformFrame_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
			__Game_Objects_Moving_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(false);
			__Game_Objects_Damaged_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(false);
			__Game_Objects_TransformFrame_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TransformFrame>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_ObjectQuery;

	private EntityQuery m_ConfigQuery;

	private EntityArchetype m_EventImpactArchetype;

	private EntityArchetype m_DamageEventArchetype;

	private EntityArchetype m_DestroyEventArchetype;

	private EventHelpers.StructuralIntegrityData m_StructuralIntegrityData;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Transform>(),
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OutOfControl>() };
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Unspawned>()
		};
		array[0] = val;
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FireConfigurationData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EventImpactArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Impact>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DamageEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Damage>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DestroyEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Destroy>()
		});
		m_StructuralIntegrityData = new EventHelpers.StructuralIntegrityData((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_ObjectQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameIndex = m_SimulationSystem.frameIndex % 16;
		uint num = m_SimulationSystem.frameIndex / 16 % 4;
		NativeQueue<Collision> collisionQueue = default(NativeQueue<Collision>);
		collisionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		FireConfigurationData singleton = ((EntityQuery)(ref m_ConfigQuery)).GetSingleton<FireConfigurationData>();
		m_StructuralIntegrityData.Update((SystemBase)(object)this, singleton);
		JobHandle dependencies;
		FindCollisionsJob findCollisionsJob = new FindCollisionsJob
		{
			m_PreviousTransformFrameIndex = (num - 1) % 4,
			m_CurrentTransformFrameIndex = num,
			m_UpdateFrameIndex = updateFrameIndex,
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_CollisionQueue = collisionQueue.AsParallelWriter(),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		ResolveCollisionsJob obj = new ResolveCollisionsJob
		{
			m_InvolvedInAccidentData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferLookup<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EventImpactArchetype = m_EventImpactArchetype,
			m_DamageEventArchetype = m_DamageEventArchetype,
			m_DestroyEventArchetype = m_DestroyEventArchetype,
			m_StructuralIntegrityData = m_StructuralIntegrityData,
			m_CollisionQueue = collisionQueue,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<FindCollisionsJob>(findCollisionsJob, m_ObjectQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val2 = IJobExtensions.Schedule<ResolveCollisionsJob>(obj, val);
		collisionQueue.Dispose(val2);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public ObjectCollisionSystem()
	{
	}
}

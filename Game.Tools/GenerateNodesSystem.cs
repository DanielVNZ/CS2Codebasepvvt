using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateNodesSystem : GameSystemBase
{
	private struct UpdateData : IEquatable<UpdateData>
	{
		public bool m_OnCourse;

		public bool m_Regenerate;

		public bool m_HasCachedPosition;

		public bool m_AddEdge;

		public bool m_UpdateOnly;

		public bool m_Valid;

		public float3 m_Position;

		public float3 m_CachedPosition;

		public quaternion m_Rotation;

		public Entity m_Prefab;

		public Entity m_Original;

		public Entity m_Owner;

		public Entity m_Lane;

		public OwnerDefinition m_OwnerData;

		public float m_CurvePosition;

		public Bounds1 m_CurveBounds;

		public float2 m_Elevation;

		public CoursePosFlags m_Flags;

		public CreationFlags m_CreationFlags;

		public CompositionFlags m_UpgradeFlags;

		public int m_FixedIndex;

		public int m_RandomSeed;

		public int m_ParentMesh;

		public UpdateData(Node node, Entity original, bool regenerate, bool updateOnly)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			m_OnCourse = false;
			m_Regenerate = regenerate;
			m_HasCachedPosition = false;
			m_AddEdge = false;
			m_UpdateOnly = updateOnly;
			m_Valid = true;
			m_Position = node.m_Position;
			m_CachedPosition = default(float3);
			m_Rotation = node.m_Rotation;
			m_Prefab = Entity.Null;
			m_Original = original;
			m_Owner = Entity.Null;
			m_Lane = Entity.Null;
			m_OwnerData = default(OwnerDefinition);
			m_CurvePosition = 0f;
			m_CurveBounds = new Bounds1(0f, 1f);
			m_Elevation = default(float2);
			m_Flags = (CoursePosFlags)0u;
			m_CreationFlags = (CreationFlags)0u;
			m_UpgradeFlags = default(CompositionFlags);
			m_FixedIndex = 0;
			m_RandomSeed = 0;
			m_ParentMesh = -1;
		}

		public UpdateData(CreationDefinition definitionData, OwnerDefinition ownerData, CoursePos coursePos, Upgraded upgraded, int fixedIndex, float3 cachedPosition, Bounds1 curveBounds, bool hasCachedPosition, bool addEdge)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			m_OnCourse = true;
			m_Regenerate = true;
			m_HasCachedPosition = hasCachedPosition;
			m_AddEdge = addEdge;
			m_UpdateOnly = false;
			m_Valid = true;
			m_Position = coursePos.m_Position;
			m_CachedPosition = cachedPosition;
			m_Rotation = coursePos.m_Rotation;
			m_Prefab = definitionData.m_Prefab;
			m_Original = coursePos.m_Entity;
			m_Owner = definitionData.m_Owner;
			m_Lane = definitionData.m_SubPrefab;
			m_OwnerData = ownerData;
			m_CurvePosition = coursePos.m_SplitPosition;
			m_CurveBounds = curveBounds;
			m_Elevation = coursePos.m_Elevation;
			m_Flags = coursePos.m_Flags;
			m_CreationFlags = definitionData.m_Flags;
			m_UpgradeFlags = upgraded.m_Flags;
			m_FixedIndex = fixedIndex;
			m_RandomSeed = definitionData.m_RandomSeed;
			m_ParentMesh = coursePos.m_ParentMesh;
		}

		public bool Equals(UpdateData other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (m_Original != Entity.Null || other.m_Original != Entity.Null)
			{
				return ((Entity)(ref m_Original)).Equals(other.m_Original);
			}
			return ((float3)(ref m_Position)).Equals(other.m_Position);
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (m_Original != Entity.Null)
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Original)/*cast due to .constrained prefix*/).GetHashCode();
			}
			return ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct NodeKey : IEquatable<NodeKey>
	{
		public Entity m_Original;

		public float3 m_Position;

		public bool m_IsEditor;

		public NodeKey(UpdateData data)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			m_Original = data.m_Original;
			m_Position = data.m_Position;
			m_IsEditor = data.m_Lane != Entity.Null;
		}

		public bool Equals(NodeKey other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (m_Original != Entity.Null || other.m_Original != Entity.Null)
			{
				return ((Entity)(ref m_Original)).Equals(other.m_Original);
			}
			if (((float3)(ref m_Position)).Equals(other.m_Position))
			{
				return m_IsEditor == other.m_IsEditor;
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (m_Original != Entity.Null)
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Original)/*cast due to .constrained prefix*/).GetHashCode();
			}
			return ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct DefinitionData
	{
		public Entity m_Prefab;

		public Entity m_Lane;

		public CreationFlags m_Flags;

		public DefinitionData(Entity prefab, Entity lane, CreationFlags flags)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Prefab = prefab;
			m_Lane = lane;
			m_Flags = flags;
		}
	}

	private struct OldNodeKey : IEquatable<OldNodeKey>
	{
		public Entity m_Prefab;

		public Entity m_SubPrefab;

		public Entity m_Original;

		public Entity m_Owner;

		public bool m_OutsideConnection;

		public bool Equals(OldNodeKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab) && ((Entity)(ref m_SubPrefab)).Equals(other.m_SubPrefab) && ((Entity)(ref m_Original)).Equals(other.m_Original) && ((Entity)(ref m_Owner)).Equals(other.m_Owner))
			{
				return m_OutsideConnection == other.m_OutsideConnection;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SubPrefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Original)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Owner)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct OldNodeValue
	{
		public Entity m_Entity;

		public float3 m_Position;
	}

	[BurstCompile]
	private struct FillOldNodesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainer> m_EditorContainerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		public NativeParallelMultiHashMap<OldNodeKey, OldNodeValue> m_OldNodeMap;

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
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Node> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			NativeArray<EditorContainer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EditorContainer>(ref m_EditorContainerType);
			NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			bool outsideConnection = ((ArchetypeChunk)(ref chunk)).Has<Game.Net.OutsideConnection>(ref m_OutsideConnectionType);
			OldNodeKey oldNodeKey = default(OldNodeKey);
			OldNodeValue oldNodeValue = default(OldNodeValue);
			EditorContainer editorContainer = default(EditorContainer);
			Owner owner = default(Owner);
			Transform transform = default(Transform);
			for (int i = 0; i < nativeArray6.Length; i++)
			{
				oldNodeKey.m_Prefab = nativeArray6[i].m_Prefab;
				oldNodeKey.m_SubPrefab = Entity.Null;
				oldNodeKey.m_Original = nativeArray3[i].m_Original;
				oldNodeKey.m_Owner = Entity.Null;
				oldNodeKey.m_OutsideConnection = outsideConnection;
				oldNodeValue.m_Entity = nativeArray[i];
				oldNodeValue.m_Position = nativeArray4[i].m_Position;
				if (CollectionUtils.TryGet<EditorContainer>(nativeArray5, i, ref editorContainer))
				{
					oldNodeKey.m_SubPrefab = editorContainer.m_Prefab;
				}
				if (CollectionUtils.TryGet<Owner>(nativeArray2, i, ref owner))
				{
					oldNodeKey.m_Owner = owner.m_Owner;
					if (m_TransformData.TryGetComponent(owner.m_Owner, ref transform))
					{
						Transform inverseParentTransform = ObjectUtils.InverseTransform(transform);
						oldNodeValue.m_Position = ObjectUtils.WorldToLocal(inverseParentTransform, oldNodeValue.m_Position);
					}
				}
				m_OldNodeMap.Add(oldNodeKey, oldNodeValue);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillNodeMapJob : IJobChunk
	{
		private struct EdgeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float2 m_Position;

			public float2 m_ConnectRadius;

			public Layer m_AttachLayers;

			public Layer m_ConnectLayers;

			public Layer m_LocalConnectLayers;

			public Entity m_IgnoreEntity;

			public bool m_IsPermanent;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Node> m_NodeData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Deleted> m_DeletedData;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Roundabout> m_RoundaboutData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetData> m_NetData;

			public ComponentLookup<NetGeometryData> m_NetGeometryData;

			public ComponentLookup<RoadData> m_RoadData;

			public BufferLookup<ConnectedEdge> m_Edges;

			public ParallelWriter<UpdateData> m_NodeQueue;

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
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_028a: Unknown result type (might be due to invalid IL or missing references)
				//IL_029d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_02df: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0231: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0271: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || entity == m_IgnoreEntity || !m_CurveData.HasComponent(entity))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[entity];
				NetData netData = m_NetData[prefabRef.m_Prefab];
				if ((m_AttachLayers & netData.m_ConnectLayers) == 0 && ((m_ConnectLayers & netData.m_ConnectLayers) == 0 || (m_LocalConnectLayers & netData.m_LocalConnectLayers) == 0))
				{
					return;
				}
				Edge edge = m_EdgeData[entity];
				if (edge.m_Start == m_IgnoreEntity || edge.m_End == m_IgnoreEntity)
				{
					return;
				}
				Curve curve = m_CurveData[entity];
				NetGeometryData netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
				RoadData roadData = default(RoadData);
				if (m_RoadData.HasComponent(prefabRef.m_Prefab))
				{
					roadData = m_RoadData[prefabRef.m_Prefab];
				}
				float num2 = default(float);
				float num = MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, m_Position, ref num2);
				float num3 = math.select(m_ConnectRadius.x, m_ConnectRadius.y, !m_OwnerData.HasComponent(entity) && (roadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0);
				bool flag = num <= netGeometryData.m_DefaultWidth * 0.5f + num3;
				if (!flag)
				{
					Roundabout roundabout = default(Roundabout);
					Node node = default(Node);
					if (m_RoundaboutData.TryGetComponent(edge.m_Start, ref roundabout) && m_NodeData.TryGetComponent(edge.m_Start, ref node) && math.distance(((float3)(ref node.m_Position)).xz, m_Position) <= roundabout.m_Radius + num3)
					{
						flag = true;
					}
					if (m_RoundaboutData.TryGetComponent(edge.m_End, ref roundabout) && m_NodeData.TryGetComponent(edge.m_End, ref node) && math.distance(((float3)(ref node.m_Position)).xz, m_Position) <= roundabout.m_Radius + num3)
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (m_IsPermanent)
					{
						m_NodeQueue.Enqueue(new UpdateData(default(Node), entity, regenerate: false, updateOnly: true));
						m_NodeQueue.Enqueue(new UpdateData(default(Node), edge.m_Start, regenerate: false, updateOnly: true));
						m_NodeQueue.Enqueue(new UpdateData(default(Node), edge.m_End, regenerate: false, updateOnly: true));
						return;
					}
					Node node2 = m_NodeData[edge.m_Start];
					Node node3 = m_NodeData[edge.m_End];
					m_NodeQueue.Enqueue(new UpdateData(node2, edge.m_Start, regenerate: true, updateOnly: false));
					m_NodeQueue.Enqueue(new UpdateData(node3, edge.m_End, regenerate: true, updateOnly: false));
					AddConnectedEdges(edge.m_Start, edge.m_End);
					AddConnectedEdges(edge.m_End, edge.m_Start);
				}
			}

			private void AddConnectedEdges(Entity node, Entity otherNode)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				DynamicBuffer<ConnectedEdge> val = m_Edges[node];
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					if (!m_DeletedData.HasComponent(edge))
					{
						Edge edge2 = m_EdgeData[edge];
						if (edge2.m_Start != node && edge2.m_Start != otherNode)
						{
							Node node2 = m_NodeData[edge2.m_Start];
							m_NodeQueue.Enqueue(new UpdateData(node2, edge2.m_Start, regenerate: false, updateOnly: false));
						}
						if (edge2.m_End != node && edge2.m_End != otherNode)
						{
							Node node3 = m_NodeData[edge2.m_End];
							m_NodeQueue.Enqueue(new UpdateData(node3, edge2.m_End, regenerate: false, updateOnly: false));
						}
					}
				}
			}
		}

		private struct NodeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Bezier4x3 m_Curve;

			public float2 m_ConnectRadius;

			public Layer m_ConnectLayers;

			public bool m_IsPermanent;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Node> m_NodeData;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<LocalConnect> m_LocalConnectData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<LocalConnectData> m_PrefabLocalConnectData;

			public ComponentLookup<NetGeometryData> m_NetGeometryData;

			public BufferLookup<ConnectedEdge> m_Edges;

			public ParallelWriter<UpdateData> m_NodeQueue;

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
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && m_NodeData.HasComponent(entity))
				{
					CheckNode(entity);
				}
			}

			private void CheckNode(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_0194: Unknown result type (might be due to invalid IL or missing references)
				//IL_0199: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0204: Unknown result type (might be due to invalid IL or missing references)
				//IL_0209: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_0219: Unknown result type (might be due to invalid IL or missing references)
				//IL_022f: Unknown result type (might be due to invalid IL or missing references)
				if (!m_LocalConnectData.HasComponent(entity))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[entity];
				if (!m_PrefabLocalConnectData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				LocalConnectData localConnectData = m_PrefabLocalConnectData[prefabRef.m_Prefab];
				if ((m_ConnectLayers & localConnectData.m_Layers) == 0)
				{
					return;
				}
				NetGeometryData netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
				float num = math.max(0f, netGeometryData.m_DefaultWidth * 0.5f + localConnectData.m_SearchDistance);
				Node node = m_NodeData[entity];
				Bounds3 val = default(Bounds3);
				((Bounds3)(ref val))._002Ector(node.m_Position - num, node.m_Position + num);
				((Bounds3)(ref val)).y = node.m_Position.y + localConnectData.m_HeightRange;
				if (!MathUtils.Intersect(m_Bounds, val))
				{
					return;
				}
				float num3 = default(float);
				float num2 = MathUtils.Distance(((Bezier4x3)(ref m_Curve)).xz, ((float3)(ref node.m_Position)).xz, ref num3);
				float num4 = math.select(m_ConnectRadius.x, m_ConnectRadius.y, m_OwnerData.HasComponent(entity) && localConnectData.m_SearchDistance != 0f && (netGeometryData.m_Flags & Game.Net.GeometryFlags.SubOwner) == 0);
				if (!(num2 <= num + num4))
				{
					return;
				}
				if (m_IsPermanent)
				{
					m_NodeQueue.Enqueue(new UpdateData(default(Node), entity, regenerate: false, updateOnly: true));
					return;
				}
				m_NodeQueue.Enqueue(new UpdateData(node, entity, regenerate: true, updateOnly: false));
				DynamicBuffer<ConnectedEdge> val2 = m_Edges[entity];
				for (int i = 0; i < val2.Length; i++)
				{
					Entity edge = val2[i].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start == entity)
					{
						Node node2 = m_NodeData[edge2.m_End];
						m_NodeQueue.Enqueue(new UpdateData(node2, edge2.m_End, regenerate: false, updateOnly: false));
					}
					else if (edge2.m_End == entity)
					{
						Node node3 = m_NodeData[edge2.m_Start];
						m_NodeQueue.Enqueue(new UpdateData(node3, edge2.m_Start, regenerate: false, updateOnly: false));
					}
				}
			}
		}

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> m_OwnerDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> m_NetCourseType;

		[ReadOnly]
		public ComponentTypeHandle<LocalCurveCache> m_LocalCurveCacheType;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> m_UpgradedType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<LocalConnect> m_LocalConnectData;

		[ReadOnly]
		public ComponentLookup<Roundabout> m_RoundaboutData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_PrefabLocalConnectData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_RoadData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_Nodes;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public ParallelWriter<UpdateData> m_NodeQueue;

		public ParallelWriter<Entity, DefinitionData> m_DefinitionMap;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<NetCourse> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCourse>(ref m_NetCourseType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition creationDefinition = nativeArray[i];
				if (creationDefinition.m_Original != Entity.Null)
				{
					m_DefinitionMap.TryAdd(creationDefinition.m_Original, new DefinitionData(creationDefinition.m_Prefab, creationDefinition.m_SubPrefab, creationDefinition.m_Flags));
				}
			}
			if (nativeArray2.Length == 0)
			{
				return;
			}
			NativeArray<OwnerDefinition> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<OwnerDefinition>(ref m_OwnerDefinitionType);
			NativeArray<LocalCurveCache> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LocalCurveCache>(ref m_LocalCurveCacheType);
			NativeArray<Upgraded> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Upgraded>(ref m_UpgradedType);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				CreationDefinition definitionData = nativeArray[j];
				if (m_DeletedData.HasComponent(definitionData.m_Owner))
				{
					continue;
				}
				OwnerDefinition ownerData = default(OwnerDefinition);
				NetCourse netCourse = nativeArray2[j];
				LocalCurveCache localCurveCache = default(LocalCurveCache);
				Upgraded upgraded = default(Upgraded);
				bool isStandalone = true;
				if (nativeArray3.Length != 0)
				{
					ownerData = nativeArray3[j];
					isStandalone = false;
				}
				if (nativeArray4.Length != 0)
				{
					localCurveCache = nativeArray4[j];
				}
				if (nativeArray5.Length != 0)
				{
					upgraded = nativeArray5[j];
				}
				Random random = new PseudoRandomSeed((ushort)definitionData.m_RandomSeed).GetRandom(PseudoRandomSeed.kEdgeNodes);
				int2 val = ((Random)(ref random)).NextInt2();
				if (((netCourse.m_StartPosition.m_Flags | netCourse.m_EndPosition.m_Flags) & CoursePosFlags.DontCreate) == 0)
				{
					if (((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
					{
						if (netCourse.m_StartPosition.m_Entity != Entity.Null && netCourse.m_EndPosition.m_Entity == Entity.Null)
						{
							definitionData.m_RandomSeed = val.x;
							AddNode(definitionData, ownerData, netCourse.m_StartPosition, upgraded, netCourse.m_FixedIndex, localCurveCache.m_Curve.a, nativeArray4.Length != 0, addEdge: false);
						}
						else
						{
							definitionData.m_RandomSeed = val.y;
							AddNode(definitionData, ownerData, netCourse.m_EndPosition, upgraded, netCourse.m_FixedIndex, localCurveCache.m_Curve.d, nativeArray4.Length != 0, addEdge: false);
						}
					}
					else
					{
						definitionData.m_Flags &= ~(CreationFlags.Select | CreationFlags.Upgrade);
						upgraded.m_Flags = default(CompositionFlags);
						definitionData.m_RandomSeed = val.x;
						AddNode(definitionData, ownerData, netCourse.m_StartPosition, upgraded, netCourse.m_FixedIndex, localCurveCache.m_Curve.a, nativeArray4.Length != 0, (definitionData.m_Flags & CreationFlags.Delete) == 0);
						definitionData.m_RandomSeed = val.y;
						AddNode(definitionData, ownerData, netCourse.m_EndPosition, upgraded, netCourse.m_FixedIndex, localCurveCache.m_Curve.d, nativeArray4.Length != 0, (definitionData.m_Flags & CreationFlags.Delete) == 0);
					}
				}
				bool flag = (definitionData.m_Flags & CreationFlags.Permanent) != 0;
				Entity deleteEdge = Entity.Null;
				if (!flag && (definitionData.m_Flags & CreationFlags.Delete) != 0)
				{
					deleteEdge = definitionData.m_Original;
				}
				if (netCourse.m_StartPosition.m_Entity != Entity.Null)
				{
					AddConnectedNodes(netCourse.m_StartPosition.m_Entity, deleteEdge, flag);
				}
				if (netCourse.m_EndPosition.m_Entity != Entity.Null)
				{
					AddConnectedNodes(netCourse.m_EndPosition.m_Entity, deleteEdge, flag);
				}
				if (definitionData.m_Prefab != Entity.Null)
				{
					AddEdgesForLocalConnectOrAttachment(netCourse.m_StartPosition.m_Flags, netCourse.m_StartPosition.m_Entity, netCourse.m_StartPosition.m_Position, netCourse.m_StartPosition.m_Elevation, definitionData.m_Prefab, flag, isStandalone);
					if (!((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
					{
						AddEdgesForLocalConnectOrAttachment(netCourse.m_EndPosition.m_Flags, netCourse.m_EndPosition.m_Entity, netCourse.m_EndPosition.m_Position, netCourse.m_EndPosition.m_Elevation, definitionData.m_Prefab, flag, isStandalone);
						Bezier4x3 curve = MathUtils.Cut(netCourse.m_Curve, new float2(netCourse.m_StartPosition.m_CourseDelta, netCourse.m_EndPosition.m_CourseDelta));
						AddNodesForLocalConnect(curve, definitionData.m_Prefab, flag, isStandalone);
					}
				}
				else if (m_PrefabRefData.HasComponent(definitionData.m_Original))
				{
					Entity prefab = m_PrefabRefData[definitionData.m_Original].m_Prefab;
					AddEdgesForLocalConnectOrAttachment(netCourse.m_StartPosition.m_Flags, netCourse.m_StartPosition.m_Entity, netCourse.m_StartPosition.m_Position, netCourse.m_StartPosition.m_Elevation, prefab, flag, isStandalone);
					if (!((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
					{
						AddEdgesForLocalConnectOrAttachment(netCourse.m_EndPosition.m_Flags, netCourse.m_EndPosition.m_Entity, netCourse.m_EndPosition.m_Position, netCourse.m_EndPosition.m_Elevation, prefab, flag, isStandalone);
					}
				}
			}
		}

		private void AddConnectedNodes(Entity original, Entity deleteEdge, bool isPermanent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			if (m_EdgeData.HasComponent(original))
			{
				Edge edge = m_EdgeData[original];
				AddNode(edge.m_Start, regenerate: true, isPermanent);
				AddNode(edge.m_End, regenerate: true, isPermanent);
				AddConnectedEdges(edge.m_Start, edge.m_End, isPermanent);
				AddConnectedEdges(edge.m_End, edge.m_Start, isPermanent);
				DynamicBuffer<ConnectedNode> val = m_Nodes[original];
				for (int i = 0; i < val.Length; i++)
				{
					Entity node = val[i].m_Node;
					AddNode(node, regenerate: true, isPermanent);
					AddConnectedEdges(node, isPermanent);
				}
			}
			else
			{
				if (!m_NodeData.HasComponent(original))
				{
					return;
				}
				DynamicBuffer<ConnectedEdge> val2 = m_Edges[original];
				if (deleteEdge != Entity.Null && val2.Length != 3)
				{
					deleteEdge = Entity.Null;
				}
				Owner owner = default(Owner);
				DynamicBuffer<Game.Net.SubNet> val5 = default(DynamicBuffer<Game.Net.SubNet>);
				for (int j = 0; j < val2.Length; j++)
				{
					Entity edge2 = val2[j].m_Edge;
					if (m_DeletedData.HasComponent(edge2))
					{
						continue;
					}
					if (isPermanent)
					{
						m_NodeQueue.Enqueue(new UpdateData(default(Node), edge2, regenerate: false, updateOnly: true));
					}
					Edge edge3 = m_EdgeData[edge2];
					bool flag = edge3.m_Start != original;
					bool flag2 = edge3.m_End != original;
					if (flag && flag2)
					{
						AddNode(edge3.m_Start, regenerate: true, isPermanent);
						AddNode(edge3.m_End, regenerate: true, isPermanent);
						AddConnectedEdges(edge3.m_Start, edge3.m_End, isPermanent);
						AddConnectedEdges(edge3.m_End, edge3.m_Start, isPermanent);
					}
					else
					{
						if (flag)
						{
							if (deleteEdge != Entity.Null)
							{
								AddNode(edge3.m_Start, regenerate: true, isPermanent);
								AddConnectedEdges(edge3.m_Start, original, isPermanent);
							}
							else
							{
								AddNode(edge3.m_Start, regenerate: false, isPermanent);
							}
						}
						if (flag2)
						{
							if (deleteEdge != Entity.Null)
							{
								AddNode(edge3.m_End, regenerate: true, isPermanent);
								AddConnectedEdges(edge3.m_End, original, isPermanent);
							}
							else
							{
								AddNode(edge3.m_End, regenerate: false, isPermanent);
							}
						}
					}
					DynamicBuffer<ConnectedNode> val3 = m_Nodes[edge2];
					for (int k = 0; k < val3.Length; k++)
					{
						Entity node2 = val3[k].m_Node;
						AddNode(node2, regenerate: true, isPermanent);
						AddConnectedEdges(node2, isPermanent);
					}
					if (flag == flag2)
					{
						continue;
					}
					Curve curve = m_CurveData[edge2];
					Entity val4 = edge2;
					if (m_OwnerData.TryGetComponent(edge2, ref owner) && m_EdgeData.TryGetComponent(owner.m_Owner, ref edge3))
					{
						val4 = owner.m_Owner;
						if (isPermanent)
						{
							m_NodeQueue.Enqueue(new UpdateData(default(Node), val4, regenerate: false, updateOnly: true));
						}
						Curve curve2 = m_CurveData[val4];
						bool flag3 = math.dot(((float3)(ref curve.m_Bezier.d)).xz - ((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve2.m_Bezier.d)).xz - ((float3)(ref curve2.m_Bezier.a)).xz) < 0f;
						AddNode(edge3.m_Start, flag3 ? flag : flag2, isPermanent);
						AddNode(edge3.m_End, flag3 ? flag2 : flag, isPermanent);
						if (flag3 ? flag : flag2)
						{
							AddConnectedEdges(edge3.m_Start, edge3.m_End, isPermanent);
						}
						if (flag3 ? flag2 : flag)
						{
							AddConnectedEdges(edge3.m_End, edge3.m_Start, isPermanent);
						}
					}
					if (!m_SubNets.TryGetBuffer(val4, ref val5))
					{
						continue;
					}
					for (int l = 0; l < val5.Length; l++)
					{
						Entity subNet = val5[l].m_SubNet;
						if (!(subNet == edge2) && m_EdgeData.TryGetComponent(subNet, ref edge3))
						{
							if (isPermanent)
							{
								m_NodeQueue.Enqueue(new UpdateData(default(Node), subNet, regenerate: false, updateOnly: true));
							}
							Curve curve3 = m_CurveData[subNet];
							bool flag4 = math.dot(((float3)(ref curve.m_Bezier.d)).xz - ((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve3.m_Bezier.d)).xz - ((float3)(ref curve3.m_Bezier.a)).xz) < 0f;
							AddNode(edge3.m_Start, flag4 ? flag : flag2, isPermanent);
							AddNode(edge3.m_End, flag4 ? flag2 : flag, isPermanent);
							if (flag4 ? flag : flag2)
							{
								AddConnectedEdges(edge3.m_Start, edge3.m_End, isPermanent);
							}
							if (flag4 ? flag2 : flag)
							{
								AddConnectedEdges(edge3.m_End, edge3.m_Start, isPermanent);
							}
						}
					}
				}
			}
		}

		private void AddConnectedEdges(Entity node, Entity otherNode, bool isPermanent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_Edges[node];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				if (!m_DeletedData.HasComponent(edge))
				{
					if (isPermanent)
					{
						m_NodeQueue.Enqueue(new UpdateData(default(Node), edge, regenerate: false, updateOnly: true));
					}
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start != node && edge2.m_Start != otherNode)
					{
						AddNode(edge2.m_Start, regenerate: false, isPermanent);
					}
					if (edge2.m_End != node && edge2.m_End != otherNode)
					{
						AddNode(edge2.m_End, regenerate: false, isPermanent);
					}
				}
			}
		}

		private void AddConnectedEdges(Entity node, bool isPermanent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_Edges[node];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				if (!m_DeletedData.HasComponent(edge))
				{
					if (isPermanent)
					{
						m_NodeQueue.Enqueue(new UpdateData(default(Node), edge, regenerate: false, updateOnly: true));
					}
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_End == node)
					{
						AddNode(edge2.m_Start, regenerate: false, isPermanent);
					}
					if (edge2.m_Start == node)
					{
						AddNode(edge2.m_End, regenerate: false, isPermanent);
					}
				}
			}
		}

		private void AddEdgesForLocalConnectOrAttachment(CoursePosFlags flags, Entity ignoreEntity, float3 position, float2 elevation, Entity prefab, bool isPermanent, bool isStandalone)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(float.MaxValue, float.MinValue);
			float2 val2 = float2.op_Implicit(0f);
			Layer layer = Layer.None;
			Layer layer2 = Layer.None;
			Layer layer3 = Layer.None;
			NetData netData = m_NetData[prefab];
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (m_NetGeometryData.HasComponent(prefab))
			{
				netGeometryData = m_NetGeometryData[prefab];
			}
			if (math.all(elevation >= netGeometryData.m_ElevationLimit * 2f) || (!math.all(elevation < 0f) && (netData.m_RequiredLayers & (Layer.PowerlineLow | Layer.PowerlineHigh)) != Layer.None))
			{
				float num = netGeometryData.m_DefaultWidth * 0.5f;
				float num2 = position.y - math.cmin(elevation);
				val |= new Bounds1(num2 - num, num2 + num);
				val2 = math.max(val2, float2.op_Implicit(num));
				layer |= Layer.Road;
			}
			if (m_PrefabLocalConnectData.HasComponent(prefab))
			{
				LocalConnectData localConnectData = m_PrefabLocalConnectData[prefab];
				if ((localConnectData.m_Flags & LocalConnectFlags.ExplicitNodes) == 0 || (flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0)
				{
					float2 val3 = float2.op_Implicit(netGeometryData.m_DefaultWidth * 0.5f + localConnectData.m_SearchDistance);
					val3.y += math.select(0f, 8f, !isStandalone && localConnectData.m_SearchDistance != 0f && (netGeometryData.m_Flags & Game.Net.GeometryFlags.SubOwner) == 0);
					val |= position.y + localConnectData.m_HeightRange;
					val2 = math.max(val2, val3);
					layer2 |= localConnectData.m_Layers;
					layer3 |= netData.m_ConnectLayers;
				}
			}
			if (layer != Layer.None || layer2 != Layer.None || layer3 != Layer.None)
			{
				float num3 = math.cmax(val2);
				Bounds3 bounds = default(Bounds3);
				((Bounds3)(ref bounds))._002Ector(position - num3, position + num3);
				((Bounds3)(ref bounds)).y = val;
				EdgeIterator edgeIterator = new EdgeIterator
				{
					m_Bounds = bounds,
					m_Position = ((float3)(ref position)).xz,
					m_ConnectRadius = val2,
					m_AttachLayers = layer,
					m_ConnectLayers = layer2,
					m_LocalConnectLayers = layer3,
					m_IgnoreEntity = ignoreEntity,
					m_IsPermanent = isPermanent,
					m_EdgeData = m_EdgeData,
					m_NodeData = m_NodeData,
					m_CurveData = m_CurveData,
					m_DeletedData = m_DeletedData,
					m_OwnerData = m_OwnerData,
					m_RoundaboutData = m_RoundaboutData,
					m_PrefabRefData = m_PrefabRefData,
					m_NetData = m_NetData,
					m_NetGeometryData = m_NetGeometryData,
					m_RoadData = m_RoadData,
					m_Edges = m_Edges,
					m_NodeQueue = m_NodeQueue
				};
				m_NetSearchTree.Iterate<EdgeIterator>(ref edgeIterator, 0);
			}
		}

		private void AddNodesForLocalConnect(Bezier4x3 curve, Entity prefab, bool isPermanent, bool isStandalone)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			NetData netData = m_NetData[prefab];
			NetGeometryData netGeometryData = default(NetGeometryData);
			m_NetGeometryData.TryGetComponent(prefab, ref netGeometryData);
			float2 val = float2.op_Implicit(netGeometryData.m_DefaultWidth * 0.5f);
			if (m_RoadData.HasComponent(prefab))
			{
				val.y += math.select(0f, 8f, isStandalone && (m_RoadData[prefab].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0);
			}
			float num = math.cmax(val) + 4f;
			NodeIterator nodeIterator = new NodeIterator
			{
				m_Bounds = MathUtils.Expand(MathUtils.Bounds(curve), new float3(num, 1000f, num)),
				m_Curve = curve,
				m_ConnectRadius = val,
				m_ConnectLayers = netData.m_ConnectLayers,
				m_IsPermanent = isPermanent,
				m_EdgeData = m_EdgeData,
				m_NodeData = m_NodeData,
				m_OwnerData = m_OwnerData,
				m_LocalConnectData = m_LocalConnectData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabLocalConnectData = m_PrefabLocalConnectData,
				m_NetGeometryData = m_NetGeometryData,
				m_Edges = m_Edges,
				m_NodeQueue = m_NodeQueue
			};
			m_NetSearchTree.Iterate<NodeIterator>(ref nodeIterator, 0);
		}

		private void AddNode(CreationDefinition definitionData, OwnerDefinition ownerData, CoursePos coursePos, Upgraded upgraded, int fixedIndex, float3 cachedPosition, bool hasCachedPosition, bool addEdge)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			if (definitionData.m_Prefab == Entity.Null && coursePos.m_Entity == Entity.Null)
			{
				return;
			}
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(0f, 1f);
			Curve curve = default(Curve);
			if (m_CurveData.TryGetComponent(coursePos.m_Entity, ref curve))
			{
				((Bounds1)(ref val))._002Ector(float2.op_Implicit(coursePos.m_SplitPosition));
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_NetGeometryData.TryGetComponent(definitionData.m_Prefab, ref netGeometryData))
				{
					float num = netGeometryData.m_DefaultWidth * 0.5f;
					float num2 = default(float);
					MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref coursePos.m_Position)).xz, ref num2);
					Bounds1 val2 = default(Bounds1);
					((Bounds1)(ref val2))._002Ector(0f, num2);
					Bounds1 val3 = default(Bounds1);
					((Bounds1)(ref val3))._002Ector(num2, 1f);
					MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val2, num);
					MathUtils.ClampLength(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val3, num);
					val |= new Bounds1(val2.min, val3.max);
				}
			}
			else
			{
				coursePos.m_SplitPosition = 0f;
				Node node = default(Node);
				if ((definitionData.m_Flags & CreationFlags.Permanent) != 0 && m_NodeData.TryGetComponent(coursePos.m_Entity, ref node))
				{
					m_NodeQueue.Enqueue(new UpdateData(node, coursePos.m_Entity, regenerate: false, updateOnly: true));
					return;
				}
			}
			m_NodeQueue.Enqueue(new UpdateData(definitionData, ownerData, coursePos, upgraded, fixedIndex, cachedPosition, val, hasCachedPosition, addEdge));
		}

		private void AddNode(Entity original, bool regenerate, bool isPermanent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Node node = m_NodeData[original];
			m_NodeQueue.Enqueue(new UpdateData(node, original, regenerate && !isPermanent, isPermanent));
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CollectUpdatesJob : IJob
	{
		public NativeQueue<UpdateData> m_UpdateQueue;

		public NativeList<UpdateData> m_UpdateList;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			int count = m_UpdateQueue.Count;
			NativeParallelMultiHashMap<NodeKey, int> val = default(NativeParallelMultiHashMap<NodeKey, int>);
			val._002Ector(count, AllocatorHandle.op_Implicit((Allocator)2));
			int num2 = default(int);
			NativeParallelMultiHashMapIterator<NodeKey> val2 = default(NativeParallelMultiHashMapIterator<NodeKey>);
			for (int i = 0; i < count; i++)
			{
				UpdateData updateData = m_UpdateQueue.Dequeue();
				NodeKey nodeKey = new NodeKey(updateData);
				int num = -1;
				if (val.TryGetFirstValue(nodeKey, ref num2, ref val2))
				{
					do
					{
						UpdateData updateData2 = m_UpdateList[num2];
						if (!updateData2.m_Valid || !MathUtils.Intersect(updateData.m_CurveBounds, updateData2.m_CurveBounds))
						{
							continue;
						}
						if (updateData.m_OnCourse)
						{
							if (updateData2.m_OnCourse)
							{
								updateData.m_AddEdge |= updateData2.m_AddEdge;
								updateData.m_FixedIndex = math.min(updateData.m_FixedIndex, updateData2.m_FixedIndex);
								updateData.m_CreationFlags |= updateData2.m_CreationFlags;
								updateData.m_UpgradeFlags |= updateData2.m_UpgradeFlags;
								updateData.m_RandomSeed ^= updateData2.m_RandomSeed;
							}
							ref Bounds1 reference = ref updateData.m_CurveBounds;
							reference |= updateData2.m_CurveBounds;
						}
						else if (!updateData.m_Regenerate || updateData2.m_OnCourse)
						{
							ref Bounds1 reference2 = ref updateData2.m_CurveBounds;
							reference2 |= updateData.m_CurveBounds;
							updateData = updateData2;
						}
						if (num == -1)
						{
							num = num2;
						}
						else
						{
							m_UpdateList[num2] = default(UpdateData);
						}
					}
					while (val.TryGetNextValue(ref num2, ref val2));
				}
				if (num == -1)
				{
					val.Add(nodeKey, m_UpdateList.Length);
					m_UpdateList.Add(ref updateData);
				}
				else
				{
					m_UpdateList[num] = updateData;
				}
			}
		}
	}

	[BurstCompile]
	private struct CreateNodesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_PrefabLocalConnectData;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_PrefabRoadData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<LocalConnect> m_LocalConnectData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Standalone> m_StandaloneData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<NetCondition> m_ConditionData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public Bounds3 m_TerrainBounds;

		[ReadOnly]
		public NativeList<UpdateData> m_UpdateList;

		[ReadOnly]
		public NativeParallelHashMap<Entity, DefinitionData> m_DefinitionMap;

		[ReadOnly]
		public NativeHashMap<OwnerDefinition, Entity> m_ReusedOwnerMap;

		public NativeParallelMultiHashMap<OldNodeKey, OldNodeValue> m_OldNodeMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			for (int i = 0; i < m_UpdateList.Length; i++)
			{
				Execute(i);
			}
		}

		public void Execute(int index)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0904: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_091f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_098c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0970: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
			UpdateData updateData = m_UpdateList[index];
			if (!updateData.m_Valid)
			{
				return;
			}
			if (updateData.m_UpdateOnly)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(updateData.m_Original, default(Updated));
				return;
			}
			Node node = new Node
			{
				m_Position = updateData.m_Position,
				m_Rotation = updateData.m_Rotation
			};
			Temp temp = new Temp
			{
				m_Original = updateData.m_Original
			};
			if ((updateData.m_CreationFlags & CreationFlags.SubElevation) != 0 && updateData.m_OnCourse)
			{
				temp.m_Flags |= TempFlags.Essential;
			}
			if ((updateData.m_Flags & (CoursePosFlags.IsLast | CoursePosFlags.IsParallel)) == CoursePosFlags.IsLast)
			{
				temp.m_Flags |= TempFlags.IsLast;
			}
			Upgraded upgraded = new Upgraded
			{
				m_Flags = updateData.m_UpgradeFlags
			};
			bool flag = false;
			if (m_NodeData.HasComponent(updateData.m_Original))
			{
				Owner owner = default(Owner);
				if (updateData.m_OnCourse && ((updateData.m_Owner == Entity.Null && updateData.m_OwnerData.m_Prefab == Entity.Null) || (m_OwnerData.TryGetComponent(updateData.m_Original, ref owner) && !TryingToDelete(owner.m_Owner))))
				{
					node = m_NodeData[updateData.m_Original];
				}
				bool alreadyOrphan = false;
				flag = !updateData.m_AddEdge && WillBeOrphan(updateData.m_Original, out alreadyOrphan);
				if (flag && CanDelete(updateData.m_Original) && (!alreadyOrphan || (updateData.m_CreationFlags & CreationFlags.Delete) != 0))
				{
					flag = alreadyOrphan;
					temp.m_Flags |= TempFlags.Delete;
				}
				else if (updateData.m_OnCourse)
				{
					if ((updateData.m_CreationFlags & CreationFlags.Upgrade) != 0)
					{
						temp.m_Flags |= TempFlags.Upgrade;
					}
					else if ((updateData.m_CreationFlags & CreationFlags.Select) != 0)
					{
						temp.m_Flags |= TempFlags.Select;
					}
					else
					{
						temp.m_Flags |= TempFlags.Regenerate;
					}
					if ((updateData.m_CreationFlags & CreationFlags.Parent) != 0)
					{
						temp.m_Flags |= TempFlags.Parent;
					}
				}
				else if (updateData.m_Regenerate)
				{
					temp.m_Flags |= TempFlags.Regenerate;
				}
				if ((updateData.m_CreationFlags & CreationFlags.Upgrade) == 0 && m_UpgradedData.HasComponent(updateData.m_Original))
				{
					upgraded = m_UpgradedData[updateData.m_Original];
				}
			}
			else if (m_EdgeData.HasComponent(updateData.m_Original))
			{
				temp.m_Flags |= TempFlags.Replace;
				temp.m_CurvePosition = updateData.m_CurvePosition;
			}
			else
			{
				flag = !updateData.m_AddEdge;
				temp.m_Flags |= TempFlags.Create;
			}
			if ((updateData.m_CreationFlags & CreationFlags.Hidden) != 0 && ((updateData.m_CreationFlags & CreationFlags.Delete) == 0 || (temp.m_Flags & TempFlags.Delete) != 0))
			{
				temp.m_Flags |= TempFlags.Hidden;
			}
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = updateData.m_Prefab
			};
			bool flag2 = false;
			bool hasNativeEdges = false;
			if (updateData.m_Original != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(updateData.m_Original, default(Hidden));
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(updateData.m_Original, default(BatchesUpdated));
				if (m_StandaloneData.HasComponent(updateData.m_Original))
				{
					flag2 = true;
					Owner owner2 = default(Owner);
					if (!flag && (temp.m_Flags & TempFlags.Delete) == 0 && m_OwnerData.TryGetComponent(updateData.m_Original, ref owner2) && (owner2.m_Owner == Entity.Null || TryingToDelete(owner2.m_Owner)))
					{
						flag2 = false;
					}
				}
				if (!flag2 && updateData.m_Prefab != Entity.Null && (updateData.m_Owner != Entity.Null || updateData.m_OwnerData.m_Prefab != Entity.Null) && (updateData.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) == (CoursePosFlags.IsFirst | CoursePosFlags.IsLast))
				{
					flag2 = true;
					prefabRef.m_Prefab = updateData.m_Prefab;
				}
				else if (flag2)
				{
					prefabRef = m_PrefabRefData[updateData.m_Original];
					updateData.m_Lane = GetEditorLane(updateData.m_Original);
				}
				else
				{
					FindNodePrefab(updateData.m_Original, updateData.m_Prefab, updateData.m_Lane, out prefabRef.m_Prefab, out var lanePrefab, out hasNativeEdges);
					updateData.m_Lane = lanePrefab;
				}
			}
			else if (flag)
			{
				flag2 = true;
			}
			NetData netData = m_NetData[prefabRef.m_Prefab];
			NetGeometryData netGeometryData = default(NetGeometryData);
			bool flag3 = false;
			if (m_NetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
				flag3 = true;
			}
			if (((netGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0 || !flag3) && updateData.m_Prefab != Entity.Null && updateData.m_Original != Entity.Null && (!flag2 || m_StandaloneData.HasComponent(updateData.m_Original)))
			{
				if (m_NodeData.HasComponent(updateData.m_Original))
				{
					Node node2 = m_NodeData[updateData.m_Original];
					node.m_Position = node2.m_Position;
					node.m_Rotation = node2.m_Rotation;
				}
				else if (m_CurveData.HasComponent(updateData.m_Original))
				{
					Curve curve = m_CurveData[updateData.m_Original];
					node.m_Position = MathUtils.Position(curve.m_Bezier, updateData.m_CurvePosition);
					float3 val = MathUtils.Tangent(curve.m_Bezier, updateData.m_CurvePosition);
					float2 xz = ((float3)(ref val)).xz;
					if (MathUtils.TryNormalize(ref xz))
					{
						node.m_Rotation = quaternion.LookRotation(new float3(xz.x, 0f, xz.y), math.up());
					}
				}
			}
			PrefabRef prefabRef2 = default(PrefabRef);
			if (m_PrefabRefData.TryGetComponent(updateData.m_Original, ref prefabRef2) && ((m_NetData[prefabRef2.m_Prefab].m_RequiredLayers ^ netData.m_RequiredLayers) & Layer.Waterway) != Layer.None)
			{
				node.m_Position.y = updateData.m_Position.y;
			}
			bool flag4 = false;
			Owner owner4 = default(Owner);
			if (updateData.m_OwnerData.m_Prefab != Entity.Null)
			{
				if ((temp.m_Flags & TempFlags.Delete) == 0 && m_OwnerData.HasComponent(updateData.m_Original))
				{
					Entity owner3 = m_OwnerData[updateData.m_Original].m_Owner;
					if (owner3 != Entity.Null && TryingToDelete(owner3))
					{
						updateData.m_OwnerData = default(OwnerDefinition);
						updateData.m_Owner = Entity.Null;
					}
				}
			}
			else if (updateData.m_Owner == Entity.Null && m_OwnerData.TryGetComponent(updateData.m_Original, ref owner4) && owner4.m_Owner != Entity.Null && !TryingToDelete(owner4.m_Owner) && !TryingToDelete(updateData.m_Original))
			{
				updateData.m_Owner = owner4.m_Owner;
				Curve curve2 = default(Curve);
				PrefabRef prefabRef3 = default(PrefabRef);
				if (m_CurveData.TryGetComponent(owner4.m_Owner, ref curve2) && m_PrefabRefData.TryGetComponent(owner4.m_Owner, ref prefabRef3))
				{
					flag4 = true;
					updateData.m_OwnerData = new OwnerDefinition
					{
						m_Prefab = prefabRef3.m_Prefab,
						m_Position = curve2.m_Bezier.a,
						m_Rotation = quaternion.op_Implicit(new float4(curve2.m_Bezier.d, 0f))
					};
				}
			}
			float num = 0.1f;
			bool flag5 = !MathUtils.Intersect(MathUtils.Expand(((Bounds3)(ref m_TerrainBounds)).xz, float2.op_Implicit(0f - num)), ((float3)(ref node.m_Position)).xz);
			Entity oldEntity = Entity.Null;
			bool flag6 = (updateData.m_CreationFlags & CreationFlags.Permanent) == 0 && TryGetOldEntity(node, prefabRef.m_Prefab, updateData.m_Lane, temp.m_Original, flag5, ref updateData.m_OwnerData, ref updateData.m_Owner, out oldEntity);
			if (flag6)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(oldEntity);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(oldEntity, default(Updated));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<ConnectedEdge>(oldEntity);
			}
			else
			{
				oldEntity = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(netData.m_NodeArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(oldEntity, prefabRef);
				bool num2;
				if (!(temp.m_Original != Entity.Null))
				{
					if (m_EditorMode)
					{
						goto IL_08ad;
					}
					num2 = (updateData.m_CreationFlags & CreationFlags.SubElevation) == 0;
				}
				else
				{
					num2 = !m_ServiceUpgradeData.HasComponent(temp.m_Original);
				}
				if (num2)
				{
					goto IL_08ad;
				}
			}
			goto IL_08ba;
			IL_08ad:
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Game.Buildings.ServiceUpgrade>(oldEntity);
			goto IL_08ba;
			IL_08ba:
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Node>(oldEntity, node);
			if (flag && flag3)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Orphan>(oldEntity, default(Orphan));
			}
			else if (flag6)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Orphan>(oldEntity);
			}
			if (flag3)
			{
				if (m_PseudoRandomSeedData.HasComponent(updateData.m_Original))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(oldEntity, m_PseudoRandomSeedData[updateData.m_Original]);
				}
				else
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(oldEntity, new PseudoRandomSeed((ushort)updateData.m_RandomSeed));
				}
			}
			if (flag2)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Standalone>(oldEntity, default(Standalone));
			}
			else if (flag6 && m_StandaloneData.HasComponent(oldEntity))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Standalone>(oldEntity);
			}
			if (updateData.m_Lane != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<EditorContainer>(oldEntity, new EditorContainer
				{
					m_Prefab = updateData.m_Lane
				});
			}
			upgraded.m_Flags &= CompositionFlags.nodeMask;
			if (upgraded.m_Flags != default(CompositionFlags))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Upgraded>(oldEntity, upgraded);
			}
			else if (flag6 && m_UpgradedData.HasComponent(oldEntity))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Upgraded>(oldEntity);
			}
			bool flag7 = true;
			bool flag8 = true;
			bool flag9 = true;
			bool num3 = m_NodeData.HasComponent(updateData.m_Original);
			bool flag10 = !num3 && m_EdgeData.HasComponent(updateData.m_Original);
			if (num3 || flag10)
			{
				Game.Net.Elevation elevation = default(Game.Net.Elevation);
				if (m_ElevationData.TryGetComponent(updateData.m_Original, ref elevation))
				{
					if (math.any(elevation.m_Elevation != 0f) || updateData.m_ParentMesh >= 0 || m_OwnerData.HasComponent(updateData.m_Original))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Game.Net.Elevation>(oldEntity, elevation);
						flag7 = false;
					}
				}
				else if (updateData.m_ParentMesh >= 0)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Game.Net.Elevation>(oldEntity, default(Game.Net.Elevation));
					flag7 = false;
				}
			}
			else if (math.any(updateData.m_Elevation != 0f) || updateData.m_ParentMesh >= 0)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Game.Net.Elevation>(oldEntity, new Game.Net.Elevation(updateData.m_Elevation));
				flag7 = false;
			}
			if (num3)
			{
				if (m_NativeData.HasComponent(updateData.m_Original) && (flag2 || hasNativeEdges))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Native>(oldEntity, default(Native));
				}
				else if (flag6 && m_NativeData.HasComponent(oldEntity))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Native>(oldEntity);
				}
				if (m_FixedData.HasComponent(updateData.m_Original))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Fixed>(oldEntity, m_FixedData[updateData.m_Original]);
					flag8 = false;
				}
				if (m_PrefabRoadData.HasComponent(prefabRef.m_Prefab) && m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab))
				{
					NetCondition netCondition = default(NetCondition);
					if (m_ConditionData.TryGetComponent(updateData.m_Original, ref netCondition))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<NetCondition>(oldEntity, netCondition);
					}
					Road road = default(Road);
					if (m_RoadData.TryGetComponent(updateData.m_Original, ref road))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Road>(oldEntity, road);
					}
				}
				if (m_PrefabLocalConnectData.HasComponent(prefabRef.m_Prefab))
				{
					LocalConnectData localConnectData = m_PrefabLocalConnectData[prefabRef.m_Prefab];
					if ((localConnectData.m_Flags & LocalConnectFlags.ExplicitNodes) == 0 || flag)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalConnect>(oldEntity, default(LocalConnect));
						flag9 = false;
					}
					else if (m_LocalConnectData.HasComponent(updateData.m_Original) && ((localConnectData.m_Flags & LocalConnectFlags.KeepOpen) != 0 || HasLocalConnections(updateData.m_Original)))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalConnect>(oldEntity, default(LocalConnect));
						flag9 = false;
					}
				}
			}
			else
			{
				if ((updateData.m_Flags & CoursePosFlags.IsFixed) != 0)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Fixed>(oldEntity, new Fixed
					{
						m_Index = updateData.m_FixedIndex
					});
					flag8 = false;
				}
				if (m_PrefabRoadData.HasComponent(prefabRef.m_Prefab) && m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab))
				{
					NetCondition netCondition2 = default(NetCondition);
					if (m_ConditionData.TryGetComponent(updateData.m_Original, ref netCondition2))
					{
						netCondition2.m_Wear = float2.op_Implicit(math.lerp(netCondition2.m_Wear.x, netCondition2.m_Wear.y, updateData.m_CurvePosition));
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<NetCondition>(oldEntity, netCondition2);
					}
					Road road2 = default(Road);
					if (m_RoadData.TryGetComponent(updateData.m_Original, ref road2))
					{
						road2.m_TrafficFlowDistance0 = (road2.m_TrafficFlowDistance0 + road2.m_TrafficFlowDistance1) * 0.5f;
						road2.m_TrafficFlowDuration0 = (road2.m_TrafficFlowDuration0 + road2.m_TrafficFlowDuration1) * 0.5f;
						road2.m_TrafficFlowDistance1 = road2.m_TrafficFlowDistance0;
						road2.m_TrafficFlowDuration1 = road2.m_TrafficFlowDuration0;
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Road>(oldEntity, road2);
					}
				}
				if (m_PrefabLocalConnectData.HasComponent(prefabRef.m_Prefab) && ((m_PrefabLocalConnectData[prefabRef.m_Prefab].m_Flags & LocalConnectFlags.ExplicitNodes) == 0 || (updateData.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != 0))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalConnect>(oldEntity, default(LocalConnect));
					flag9 = false;
				}
			}
			if (flag6)
			{
				if (flag7 && m_ElevationData.HasComponent(oldEntity))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Game.Net.Elevation>(oldEntity);
				}
				if (flag8 && m_FixedData.HasComponent(oldEntity))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Fixed>(oldEntity);
				}
				if (flag9 && m_LocalConnectData.HasComponent(oldEntity))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<LocalConnect>(oldEntity);
				}
			}
			if (updateData.m_OwnerData.m_Prefab != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(oldEntity, new Owner(flag4 ? updateData.m_Owner : Entity.Null));
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(oldEntity, updateData.m_OwnerData);
			}
			else if (updateData.m_Owner != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(oldEntity, new Owner(updateData.m_Owner));
			}
			if (flag5)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Game.Net.OutsideConnection>(oldEntity, default(Game.Net.OutsideConnection));
			}
			if ((updateData.m_CreationFlags & CreationFlags.Permanent) == 0)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(oldEntity, temp);
			}
			if (updateData.m_HasCachedPosition)
			{
				LocalTransformCache localTransformCache = default(LocalTransformCache);
				localTransformCache.m_Position = updateData.m_CachedPosition;
				localTransformCache.m_Rotation = quaternion.identity;
				localTransformCache.m_ParentMesh = updateData.m_ParentMesh;
				localTransformCache.m_GroupIndex = 0;
				localTransformCache.m_Probability = 100;
				localTransformCache.m_PrefabSubIndex = -1;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalTransformCache>(oldEntity, localTransformCache);
			}
		}

		private bool TryGetOldEntity(Node node, Entity prefab, Entity subPrefab, Entity original, bool isOutsideConnection, ref OwnerDefinition ownerDefinition, ref Entity owner, out Entity oldEntity)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = default(Transform);
			bool flag = false;
			Entity val = default(Entity);
			Transform transform2 = default(Transform);
			if (ownerDefinition.m_Prefab != Entity.Null && m_ReusedOwnerMap.TryGetValue(ownerDefinition, ref val))
			{
				transform.m_Position = ownerDefinition.m_Position;
				transform.m_Rotation = ownerDefinition.m_Rotation;
				owner = val;
				ownerDefinition = default(OwnerDefinition);
				flag = true;
			}
			else if (m_TransformData.TryGetComponent(owner, ref transform2))
			{
				transform = transform2;
				flag = true;
			}
			OldNodeKey oldNodeKey = default(OldNodeKey);
			oldNodeKey.m_Prefab = prefab;
			oldNodeKey.m_SubPrefab = subPrefab;
			oldNodeKey.m_Original = original;
			oldNodeKey.m_Owner = owner;
			oldNodeKey.m_OutsideConnection = isOutsideConnection;
			OldNodeValue oldNodeValue = default(OldNodeValue);
			NativeParallelMultiHashMapIterator<OldNodeKey> val2 = default(NativeParallelMultiHashMapIterator<OldNodeKey>);
			if (m_OldNodeMap.TryGetFirstValue(oldNodeKey, ref oldNodeValue, ref val2))
			{
				float3 val3 = node.m_Position;
				float num = float.MaxValue;
				Entity val4 = oldNodeValue.m_Entity;
				NativeParallelMultiHashMapIterator<OldNodeKey> val5 = val2;
				if (flag)
				{
					val3 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(transform), val3);
				}
				do
				{
					float num2 = math.distancesq(val3, oldNodeValue.m_Position);
					if (num2 < num)
					{
						num = num2;
						val4 = oldNodeValue.m_Entity;
						val5 = val2;
					}
				}
				while (m_OldNodeMap.TryGetNextValue(ref oldNodeValue, ref val2));
				oldEntity = val4;
				m_OldNodeMap.Remove(val5);
				return true;
			}
			oldEntity = Entity.Null;
			return false;
		}

		private bool HasLocalConnections(Entity node)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (m_Edges.HasBuffer(node))
			{
				DynamicBuffer<ConnectedEdge> val = m_Edges[node];
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start != node && edge2.m_End != node)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void FindNodePrefab(Entity original, Entity newPrefab, Entity newLane, out Entity netPrefab, out Entity lanePrefab, out bool hasNativeEdges)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			netPrefab = Entity.Null;
			lanePrefab = Entity.Null;
			float num = float.MinValue;
			hasNativeEdges = false;
			if (m_EdgeData.HasComponent(original))
			{
				hasNativeEdges = m_NativeData.HasComponent(original);
				PrefabRef prefabRef = m_PrefabRefData[original];
				NetData netData = m_NetData[prefabRef.m_Prefab];
				if (netData.m_NodePriority > num)
				{
					netPrefab = prefabRef.m_Prefab;
					lanePrefab = GetEditorLane(original);
					num = netData.m_NodePriority;
				}
			}
			else if (m_Edges.HasBuffer(original))
			{
				DynamicBuffer<ConnectedEdge> val = m_Edges[original];
				DefinitionData definitionData = default(DefinitionData);
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start != original && edge2.m_End != original)
					{
						continue;
					}
					if (m_DefinitionMap.TryGetValue(edge, ref definitionData))
					{
						if ((definitionData.m_Flags & CreationFlags.Delete) != 0)
						{
							continue;
						}
						if (definitionData.m_Prefab != Entity.Null)
						{
							hasNativeEdges |= m_NativeData.HasComponent(edge);
							NetData netData2 = m_NetData[definitionData.m_Prefab];
							if (netData2.m_NodePriority > num)
							{
								netPrefab = definitionData.m_Prefab;
								lanePrefab = definitionData.m_Lane;
								num = netData2.m_NodePriority;
							}
							continue;
						}
					}
					hasNativeEdges |= m_NativeData.HasComponent(edge);
					PrefabRef prefabRef2 = m_PrefabRefData[edge];
					NetData netData3 = m_NetData[prefabRef2.m_Prefab];
					if (netData3.m_NodePriority > num)
					{
						netPrefab = prefabRef2.m_Prefab;
						lanePrefab = GetEditorLane(edge);
						num = netData3.m_NodePriority;
					}
				}
			}
			if (newPrefab != Entity.Null)
			{
				NetData netData4 = m_NetData[newPrefab];
				if (netData4.m_NodePriority > num)
				{
					netPrefab = newPrefab;
					lanePrefab = newLane;
					num = netData4.m_NodePriority;
				}
			}
			if (netPrefab == Entity.Null && m_PrefabRefData.HasComponent(original))
			{
				netPrefab = m_PrefabRefData[original].m_Prefab;
				lanePrefab = GetEditorLane(original);
			}
		}

		private Entity GetEditorLane(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_EditorContainerData.HasComponent(entity))
			{
				return m_EditorContainerData[entity].m_Prefab;
			}
			return Entity.Null;
		}

		private bool TryingToDelete(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			DefinitionData definitionData = default(DefinitionData);
			if (m_DefinitionMap.TryGetValue(entity, ref definitionData))
			{
				return (definitionData.m_Flags & (CreationFlags.Delete | CreationFlags.Relocate)) != 0;
			}
			return false;
		}

		private bool WillBeOrphan(Entity node, out bool alreadyOrphan)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_Edges[node];
			alreadyOrphan = true;
			DefinitionData definitionData = default(DefinitionData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				if (edge2.m_Start == node || edge2.m_End == node)
				{
					alreadyOrphan = false;
					if (!m_DefinitionMap.TryGetValue(edge, ref definitionData))
					{
						return false;
					}
					if ((definitionData.m_Flags & CreationFlags.Delete) == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CanDelete(Entity node)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (TryingToDelete(node))
			{
				return true;
			}
			if (m_OwnerData.HasComponent(node))
			{
				Entity owner = m_OwnerData[node].m_Owner;
				if (owner != Entity.Null && !m_EditorMode && !TryingToDelete(owner))
				{
					DynamicBuffer<ConnectedEdge> val = m_Edges[node];
					for (int i = 0; i < val.Length; i++)
					{
						Entity edge = val[i].m_Edge;
						if (TryingToDelete(edge) && m_OwnerData.HasComponent(edge))
						{
							Edge edge2 = m_EdgeData[edge];
							Entity owner2 = m_OwnerData[edge].m_Owner;
							if ((edge2.m_Start == node || edge2.m_End == node) && owner2 == owner)
							{
								return true;
							}
						}
					}
					return false;
				}
			}
			return true;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> __Game_Tools_OwnerDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> __Game_Tools_NetCourse_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LocalCurveCache> __Game_Tools_LocalCurveCache_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> __Game_Net_Upgraded_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnect> __Game_Net_LocalConnect_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Roundabout> __Game_Net_Roundabout_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> __Game_Prefabs_LocalConnectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Standalone> __Game_Net_Standalone_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCondition> __Game_Net_NetCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

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
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OwnerDefinition>(true);
			__Game_Tools_NetCourse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCourse>(true);
			__Game_Tools_LocalCurveCache_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LocalCurveCache>(true);
			__Game_Net_Upgraded_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Upgraded>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_LocalConnect_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnect>(true);
			__Game_Net_Roundabout_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Roundabout>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_LocalConnectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnectData>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Tools_EditorContainer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EditorContainer>(true);
			__Game_Net_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.OutsideConnection>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Net_Standalone_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Standalone>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_NetCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCondition>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private Game.Net.SearchSystem m_SearchSystem;

	private TerrainSystem m_TerrainSystem;

	private GenerateObjectsSystem m_GenerateObjectsSystem;

	private ModificationBarrier1 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_DeletedQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_GenerateObjectsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GenerateObjectsSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<NetCourse>(),
			ComponentType.ReadOnly<ObjectDefinition>()
		};
		array[0] = val;
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_DeletedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<PrefabRef>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<UpdateData> updateQueue = default(NativeQueue<UpdateData>);
		updateQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<UpdateData> updateList = default(NativeList<UpdateData>);
		updateList._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashMap<Entity, DefinitionData> definitionMap = default(NativeParallelHashMap<Entity, DefinitionData>);
		definitionMap._002Ector(((EntityQuery)(ref m_DefinitionQuery)).CalculateEntityCount(), AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelMultiHashMap<OldNodeKey, OldNodeValue> oldNodeMap = default(NativeParallelMultiHashMap<OldNodeKey, OldNodeValue>);
		oldNodeMap._002Ector(32, AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		NativeHashMap<OwnerDefinition, Entity> reusedOwnerMap = m_GenerateObjectsSystem.GetReusedOwnerMap(out dependencies);
		TerrainHeightData data = m_TerrainSystem.GetHeightData();
		Bounds3 bounds = TerrainUtils.GetBounds(ref data);
		JobHandle dependencies2;
		FillNodeMapJob fillNodeMapJob = new FillNodeMapJob
		{
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<OwnerDefinition>(ref __TypeHandle.__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCourseType = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LocalCurveCacheType = InternalCompilerInterface.GetComponentTypeHandle<LocalCurveCache>(ref __TypeHandle.__Game_Tools_LocalCurveCache_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedType = InternalCompilerInterface.GetComponentTypeHandle<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnect>(ref __TypeHandle.__Game_Net_LocalConnect_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoundaboutData = InternalCompilerInterface.GetComponentLookup<Roundabout>(ref __TypeHandle.__Game_Net_Roundabout_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeQueue = updateQueue.AsParallelWriter(),
			m_DefinitionMap = definitionMap.AsParallelWriter(),
			m_NetSearchTree = m_SearchSystem.GetNetSearchTree(readOnly: true, out dependencies2)
		};
		FillOldNodesJob fillOldNodesJob = new FillOldNodesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OldNodeMap = oldNodeMap
		};
		CollectUpdatesJob collectUpdatesJob = new CollectUpdatesJob
		{
			m_UpdateQueue = updateQueue,
			m_UpdateList = updateList
		};
		CreateNodesJob obj = new CreateNodesJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnect>(ref __TypeHandle.__Game_Net_LocalConnect_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StandaloneData = InternalCompilerInterface.GetComponentLookup<Standalone>(ref __TypeHandle.__Game_Net_Standalone_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConditionData = InternalCompilerInterface.GetComponentLookup<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_TerrainBounds = bounds,
			m_UpdateList = updateList,
			m_DefinitionMap = definitionMap,
			m_ReusedOwnerMap = reusedOwnerMap,
			m_OldNodeMap = oldNodeMap,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<FillNodeMapJob>(fillNodeMapJob, m_DefinitionQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2));
		JobHandle val2 = JobChunkExtensions.Schedule<FillOldNodesJob>(fillOldNodesJob, m_DeletedQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<CollectUpdatesJob>(collectUpdatesJob, val);
		JobHandle val4 = IJobExtensions.Schedule<CreateNodesJob>(obj, JobHandle.CombineDependencies(val3, val2, dependencies));
		updateQueue.Dispose(val3);
		updateList.Dispose(val4);
		definitionMap.Dispose(val4);
		oldNodeMap.Dispose(val4);
		m_SearchSystem.AddNetSearchTreeReader(val);
		m_GenerateObjectsSystem.AddOwnerMapReader(val4);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val4);
		((SystemBase)this).Dependency = val4;
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
	public GenerateNodesSystem()
	{
	}
}

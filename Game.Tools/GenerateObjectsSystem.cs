using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Effects;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Vehicles;
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
public class GenerateObjectsSystem : GameSystemBase
{
	private struct CreationData : IComparable<CreationData>
	{
		public CreationDefinition m_CreationDefinition;

		public OwnerDefinition m_OwnerDefinition;

		public ObjectDefinition m_ObjectDefinition;

		public Entity m_OldEntity;

		public bool m_HasDefinition;

		public CreationData(CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, ObjectDefinition objectDefinition, bool hasDefinition)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			m_CreationDefinition = creationDefinition;
			m_OwnerDefinition = ownerDefinition;
			m_ObjectDefinition = objectDefinition;
			m_OldEntity = Entity.Null;
			m_HasDefinition = hasDefinition;
		}

		public int CompareTo(CreationData other)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return m_CreationDefinition.m_Original.Index - other.m_CreationDefinition.m_Original.Index;
		}
	}

	private struct OldObjectKey : IEquatable<OldObjectKey>
	{
		public Entity m_Prefab;

		public Entity m_SubPrefab;

		public Entity m_Original;

		public Entity m_Owner;

		public bool Equals(OldObjectKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab) && ((Entity)(ref m_SubPrefab)).Equals(other.m_SubPrefab) && ((Entity)(ref m_Original)).Equals(other.m_Original))
			{
				return ((Entity)(ref m_Owner)).Equals(other.m_Owner);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SubPrefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Original)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Owner)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct OldObjectValue
	{
		public Entity m_Entity;

		public Transform m_Transform;
	}

	[BurstCompile]
	private struct FillOldObjectsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainer> m_EditorContainerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public NativeParallelMultiHashMap<OldObjectKey, OldObjectValue> m_OldObjectMap;

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
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<EditorContainer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EditorContainer>(ref m_EditorContainerType);
			NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			OldObjectKey oldObjectKey = default(OldObjectKey);
			OldObjectValue oldObjectValue = default(OldObjectValue);
			for (int i = 0; i < nativeArray6.Length; i++)
			{
				oldObjectKey.m_Prefab = nativeArray6[i].m_Prefab;
				oldObjectKey.m_SubPrefab = Entity.Null;
				oldObjectKey.m_Original = nativeArray3[i].m_Original;
				oldObjectKey.m_Owner = Entity.Null;
				if (nativeArray5.Length != 0)
				{
					oldObjectKey.m_SubPrefab = nativeArray5[i].m_Prefab;
				}
				if (nativeArray2.Length != 0)
				{
					oldObjectKey.m_Owner = nativeArray2[i].m_Owner;
				}
				oldObjectValue.m_Entity = nativeArray[i];
				oldObjectValue.m_Transform = nativeArray4[i];
				m_OldObjectMap.Add(oldObjectKey, oldObjectValue);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillCreationListJob : IJobChunk
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

			public FillCreationListJob m_JobData;

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
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_025d: Unknown result type (might be due to invalid IL or missing references)
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_027b: Unknown result type (might be due to invalid IL or missing references)
				//IL_028c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0292: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0205: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_021f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01da: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0234: Unknown result type (might be due to invalid IL or missing references)
				//IL_023a: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || entity == m_IgnoreEntity || !m_JobData.m_CurveData.HasComponent(entity))
				{
					return;
				}
				PrefabRef prefabRef = m_JobData.m_PrefabRefData[entity];
				NetData netData = m_JobData.m_NetData[prefabRef.m_Prefab];
				if ((m_AttachLayers & netData.m_ConnectLayers) == 0 && ((m_ConnectLayers & netData.m_ConnectLayers) == 0 || (m_LocalConnectLayers & netData.m_LocalConnectLayers) == 0))
				{
					return;
				}
				Edge edge = m_JobData.m_EdgeData[entity];
				if (edge.m_Start == m_IgnoreEntity || edge.m_End == m_IgnoreEntity)
				{
					return;
				}
				Curve curve = m_JobData.m_CurveData[entity];
				NetGeometryData netGeometryData = m_JobData.m_NetGeometryData[prefabRef.m_Prefab];
				RoadData roadData = default(RoadData);
				if (m_JobData.m_RoadData.HasComponent(prefabRef.m_Prefab))
				{
					roadData = m_JobData.m_RoadData[prefabRef.m_Prefab];
				}
				float num2 = default(float);
				float num = MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, m_Position, ref num2);
				float num3 = math.select(m_ConnectRadius.x, m_ConnectRadius.y, !m_JobData.m_OwnerData.HasComponent(entity) && (roadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0);
				bool flag = num <= netGeometryData.m_DefaultWidth * 0.5f + num3;
				if (!flag)
				{
					Roundabout roundabout = default(Roundabout);
					Game.Net.Node node = default(Game.Net.Node);
					if (m_JobData.m_RoundaboutData.TryGetComponent(edge.m_Start, ref roundabout) && m_JobData.m_NodeData.TryGetComponent(edge.m_Start, ref node) && math.distance(((float3)(ref node.m_Position)).xz, m_Position) <= roundabout.m_Radius + num3)
					{
						flag = true;
					}
					if (m_JobData.m_RoundaboutData.TryGetComponent(edge.m_End, ref roundabout) && m_JobData.m_NodeData.TryGetComponent(edge.m_End, ref node) && math.distance(((float3)(ref node.m_Position)).xz, m_Position) <= roundabout.m_Radius + num3)
					{
						flag = true;
					}
				}
				if (flag)
				{
					m_JobData.CheckSubObjects(entity);
					m_JobData.CheckSubObjects(edge.m_Start);
					m_JobData.CheckSubObjects(edge.m_End);
					m_JobData.CheckNodeEdges(edge.m_Start, edge.m_End);
					m_JobData.CheckNodeEdges(edge.m_End, edge.m_Start);
				}
			}
		}

		private struct NodeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Bezier4x3 m_Curve;

			public float2 m_ConnectRadius;

			public Layer m_ConnectLayers;

			public FillCreationListJob m_JobData;

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
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && m_JobData.m_NodeData.HasComponent(entity))
				{
					CheckNode(entity);
				}
			}

			private void CheckNode(Entity entity)
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_0184: Unknown result type (might be due to invalid IL or missing references)
				if (!m_JobData.m_LocalConnectData.HasComponent(entity))
				{
					return;
				}
				PrefabRef prefabRef = m_JobData.m_PrefabRefData[entity];
				if (!m_JobData.m_PrefabLocalConnectData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				LocalConnectData localConnectData = m_JobData.m_PrefabLocalConnectData[prefabRef.m_Prefab];
				if ((m_ConnectLayers & localConnectData.m_Layers) == 0)
				{
					return;
				}
				NetGeometryData netGeometryData = m_JobData.m_NetGeometryData[prefabRef.m_Prefab];
				float num = math.max(0f, netGeometryData.m_DefaultWidth * 0.5f + localConnectData.m_SearchDistance);
				Game.Net.Node node = m_JobData.m_NodeData[entity];
				Bounds3 val = default(Bounds3);
				((Bounds3)(ref val))._002Ector(node.m_Position - num, node.m_Position + num);
				((Bounds3)(ref val)).y = node.m_Position.y + localConnectData.m_HeightRange;
				if (MathUtils.Intersect(m_Bounds, val))
				{
					float num3 = default(float);
					float num2 = MathUtils.Distance(((Bezier4x3)(ref m_Curve)).xz, ((float3)(ref node.m_Position)).xz, ref num3);
					float num4 = math.select(m_ConnectRadius.x, m_ConnectRadius.y, m_JobData.m_OwnerData.HasComponent(entity) && localConnectData.m_SearchDistance != 0f && (netGeometryData.m_Flags & Game.Net.GeometryFlags.SubOwner) == 0);
					if (num2 <= num + num4)
					{
						m_JobData.CheckSubObjects(entity);
						m_JobData.CheckNodeEdges(entity);
					}
				}
			}
		}

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> m_OwnerDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<ObjectDefinition> m_ObjectDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> m_NetCourseType;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_ObjectData;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_PrefabLocalConnectData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_RoadData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<LocalConnect> m_LocalConnectData;

		[ReadOnly]
		public ComponentLookup<Roundabout> m_RoundaboutData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		public ParallelWriter<CreationData> m_CreationQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<OwnerDefinition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<OwnerDefinition>(ref m_OwnerDefinitionType);
			NativeArray<ObjectDefinition> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectDefinition>(ref m_ObjectDefinitionType);
			NativeArray<NetCourse> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCourse>(ref m_NetCourseType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				ObjectDefinition objectDefinition = nativeArray3[i];
				CreationDefinition creationDefinition = nativeArray[i];
				if (m_DeletedData.HasComponent(creationDefinition.m_Owner))
				{
					continue;
				}
				OwnerDefinition ownerDefinition = default(OwnerDefinition);
				if (nativeArray2.Length != 0)
				{
					ownerDefinition = nativeArray2[i];
				}
				if (m_ObjectData.HasComponent(creationDefinition.m_Prefab))
				{
					m_CreationQueue.Enqueue(new CreationData(creationDefinition, ownerDefinition, objectDefinition, hasDefinition: true));
				}
				else if (m_PrefabRefData.HasComponent(creationDefinition.m_Original))
				{
					PrefabRef prefabRef = m_PrefabRefData[creationDefinition.m_Original];
					if (m_ObjectData.HasComponent(prefabRef.m_Prefab))
					{
						m_CreationQueue.Enqueue(new CreationData(creationDefinition, ownerDefinition, objectDefinition, hasDefinition: true));
					}
				}
			}
			for (int j = 0; j < nativeArray4.Length; j++)
			{
				CreationDefinition creationDefinition2 = nativeArray[j];
				if (m_DeletedData.HasComponent(creationDefinition2.m_Owner) || (creationDefinition2.m_Flags & CreationFlags.Permanent) != 0)
				{
					continue;
				}
				NetCourse netCourse = nativeArray4[j];
				CheckSubObjects(netCourse.m_StartPosition.m_Entity);
				CheckSubObjects(netCourse.m_EndPosition.m_Entity);
				Entity deleteEdge = Entity.Null;
				if ((creationDefinition2.m_Flags & CreationFlags.Delete) != 0)
				{
					deleteEdge = creationDefinition2.m_Original;
				}
				if (netCourse.m_StartPosition.m_Entity != Entity.Null)
				{
					CheckConnectedEdges(netCourse.m_StartPosition.m_Entity, deleteEdge);
				}
				if (netCourse.m_EndPosition.m_Entity != Entity.Null)
				{
					CheckConnectedEdges(netCourse.m_EndPosition.m_Entity, deleteEdge);
				}
				bool isStandalone = nativeArray2.Length == 0;
				if (creationDefinition2.m_Prefab != Entity.Null)
				{
					CheckEdgesForLocalConnectOrAttachment(netCourse.m_StartPosition.m_Flags, netCourse.m_StartPosition.m_Entity, netCourse.m_StartPosition.m_Position, netCourse.m_StartPosition.m_Elevation, creationDefinition2.m_Prefab, isStandalone);
					if (!((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
					{
						CheckEdgesForLocalConnectOrAttachment(netCourse.m_EndPosition.m_Flags, netCourse.m_EndPosition.m_Entity, netCourse.m_EndPosition.m_Position, netCourse.m_EndPosition.m_Elevation, creationDefinition2.m_Prefab, isStandalone);
						Bezier4x3 curve = MathUtils.Cut(netCourse.m_Curve, new float2(netCourse.m_StartPosition.m_CourseDelta, netCourse.m_EndPosition.m_CourseDelta));
						CheckNodesForLocalConnect(curve, creationDefinition2.m_Prefab, isStandalone);
					}
				}
				else if (m_PrefabRefData.HasComponent(creationDefinition2.m_Original))
				{
					Entity prefab = m_PrefabRefData[creationDefinition2.m_Original].m_Prefab;
					CheckEdgesForLocalConnectOrAttachment(netCourse.m_StartPosition.m_Flags, netCourse.m_StartPosition.m_Entity, netCourse.m_StartPosition.m_Position, netCourse.m_StartPosition.m_Elevation, prefab, isStandalone);
					if (!((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
					{
						CheckEdgesForLocalConnectOrAttachment(netCourse.m_EndPosition.m_Flags, netCourse.m_EndPosition.m_Entity, netCourse.m_EndPosition.m_Position, netCourse.m_EndPosition.m_Elevation, prefab, isStandalone);
					}
				}
			}
		}

		private void CheckConnectedEdges(Entity entity, Entity deleteEdge)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			if (m_EdgeData.HasComponent(entity))
			{
				Edge edge = m_EdgeData[entity];
				CheckSubObjects(edge.m_Start);
				CheckSubObjects(edge.m_End);
				CheckNodeEdges(edge.m_Start, edge.m_End);
				CheckNodeEdges(edge.m_End, edge.m_Start);
				DynamicBuffer<ConnectedNode> val = m_ConnectedNodes[entity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity node = val[i].m_Node;
					CheckSubObjects(node);
					CheckNodeEdges(node);
				}
			}
			else
			{
				if (!m_NodeData.HasComponent(entity))
				{
					return;
				}
				DynamicBuffer<ConnectedEdge> val2 = m_ConnectedEdges[entity];
				if (deleteEdge != Entity.Null && val2.Length != 3)
				{
					deleteEdge = Entity.Null;
				}
				for (int j = 0; j < val2.Length; j++)
				{
					Entity edge2 = val2[j].m_Edge;
					Edge edge3 = m_EdgeData[edge2];
					if (edge3.m_Start == entity)
					{
						if (deleteEdge != Entity.Null)
						{
							CheckNodeEdges(edge3.m_End, entity);
						}
						CheckSubObjects(edge3.m_End);
						CheckSubObjects(edge2);
					}
					else if (edge3.m_End == entity)
					{
						if (deleteEdge != Entity.Null)
						{
							CheckNodeEdges(edge3.m_Start, entity);
						}
						CheckSubObjects(edge3.m_Start);
						CheckSubObjects(edge2);
					}
					else
					{
						CheckNodeEdges(edge3.m_Start, edge3.m_End);
						CheckNodeEdges(edge3.m_End, edge3.m_Start);
						CheckSubObjects(edge3.m_Start);
						CheckSubObjects(edge3.m_End);
						CheckSubObjects(edge2);
					}
					DynamicBuffer<ConnectedNode> val3 = m_ConnectedNodes[edge2];
					for (int k = 0; k < val3.Length; k++)
					{
						Entity node2 = val3[k].m_Node;
						CheckSubObjects(node2);
						CheckNodeEdges(node2);
					}
				}
			}
		}

		private void CheckNodeEdges(Entity node, Entity otherNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				if (edge2.m_Start == node)
				{
					if (edge2.m_End != otherNode)
					{
						CheckSubObjects(edge2.m_End);
						CheckSubObjects(edge);
					}
				}
				else if (edge2.m_End == node)
				{
					if (edge2.m_Start != otherNode)
					{
						CheckSubObjects(edge2.m_Start);
						CheckSubObjects(edge);
					}
				}
				else
				{
					CheckSubObjects(edge2.m_Start);
					CheckSubObjects(edge2.m_End);
					CheckSubObjects(edge);
				}
			}
		}

		private void CheckNodeEdges(Entity node)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				if (edge2.m_Start == node)
				{
					CheckSubObjects(edge2.m_End);
					CheckSubObjects(edge);
				}
				else if (edge2.m_End == node)
				{
					CheckSubObjects(edge2.m_Start);
					CheckSubObjects(edge);
				}
			}
		}

		private void CheckSubObjects(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubObjects.HasBuffer(entity))
			{
				return;
			}
			DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[entity];
			Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (!m_AttachedData.HasComponent(subObject) || !(m_AttachedData[subObject].m_Parent == entity))
				{
					continue;
				}
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Original = subObject
				};
				creationDefinition.m_Flags |= CreationFlags.Attach;
				if (m_OwnerData.HasComponent(subObject))
				{
					continue;
				}
				Transform transform = m_TransformData[subObject];
				ObjectDefinition objectDefinition = new ObjectDefinition
				{
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
				if (m_ElevationData.TryGetComponent(subObject, ref elevation))
				{
					objectDefinition.m_Elevation = elevation.m_Elevation;
					objectDefinition.m_ParentMesh = ObjectUtils.GetSubParentMesh(elevation.m_Flags);
					if ((elevation.m_Flags & ElevationFlags.Lowered) != 0)
					{
						creationDefinition.m_Flags |= CreationFlags.Lowered;
					}
				}
				else
				{
					objectDefinition.m_ParentMesh = -1;
				}
				m_CreationQueue.Enqueue(new CreationData(creationDefinition, default(OwnerDefinition), objectDefinition, hasDefinition: false));
			}
		}

		private void CheckEdgesForLocalConnectOrAttachment(CoursePosFlags flags, Entity ignoreEntity, float3 position, float2 elevation, Entity prefab, bool isStandalone)
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
					m_JobData = this
				};
				m_NetSearchTree.Iterate<EdgeIterator>(ref edgeIterator, 0);
			}
		}

		private void CheckNodesForLocalConnect(Bezier4x3 curve, Entity prefab, bool isStandalone)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			NetData netData = m_NetData[prefab];
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (m_NetGeometryData.HasComponent(prefab))
			{
				netGeometryData = m_NetGeometryData[prefab];
			}
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
				m_JobData = this
			};
			m_NetSearchTree.Iterate<NodeIterator>(ref nodeIterator, 0);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CollectCreationDataJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public NativeQueue<CreationData> m_CreationQueue;

		public NativeList<CreationData> m_CreationList;

		public NativeParallelMultiHashMap<OldObjectKey, OldObjectValue> m_OldObjectMap;

		public NativeHashMap<OwnerDefinition, Entity> m_ReusedOwnerMap;

		public void Execute()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			m_CreationList.ResizeUninitialized(m_CreationQueue.Count);
			for (int i = 0; i < m_CreationList.Length; i++)
			{
				m_CreationList[i] = m_CreationQueue.Dequeue();
			}
			NativeSortExtension.Sort<CreationData>(m_CreationList);
			CreationData creationData = default(CreationData);
			int num = 0;
			int num2 = 0;
			bool flag = false;
			while (num < m_CreationList.Length)
			{
				CreationData creationData2 = m_CreationList[num++];
				if (creationData2.m_CreationDefinition.m_Original != creationData.m_CreationDefinition.m_Original || creationData2.m_CreationDefinition.m_Original == Entity.Null)
				{
					if (flag)
					{
						m_CreationList[num2++] = creationData;
					}
					creationData = creationData2;
					flag = true;
				}
				else if (creationData2.m_HasDefinition)
				{
					creationData = creationData2;
				}
			}
			if (flag)
			{
				m_CreationList[num2++] = creationData;
			}
			if (num2 < m_CreationList.Length)
			{
				m_CreationList.RemoveRange(num2, m_CreationList.Length - num2);
			}
			for (int j = 0; j < m_CreationList.Length; j++)
			{
				CreationData creationData3 = m_CreationList[j];
				if ((creationData3.m_CreationDefinition.m_Prefab == Entity.Null || (creationData3.m_CreationDefinition.m_Flags & CreationFlags.Upgrade) == 0) && m_PrefabRefData.HasComponent(creationData3.m_CreationDefinition.m_Original))
				{
					creationData3.m_CreationDefinition.m_Prefab = m_PrefabRefData[creationData3.m_CreationDefinition.m_Original].m_Prefab;
					m_CreationList[j] = creationData3;
				}
			}
			OldObjectKey oldObjectKey = default(OldObjectKey);
			OldObjectValue oldObjectValue = default(OldObjectValue);
			NativeParallelMultiHashMapIterator<OldObjectKey> val = default(NativeParallelMultiHashMapIterator<OldObjectKey>);
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			for (int k = 0; k < m_CreationList.Length; k++)
			{
				CreationData creationData4 = m_CreationList[k];
				if (!(creationData4.m_OwnerDefinition.m_Prefab == Entity.Null))
				{
					continue;
				}
				oldObjectKey.m_Prefab = creationData4.m_CreationDefinition.m_Prefab;
				oldObjectKey.m_SubPrefab = creationData4.m_CreationDefinition.m_SubPrefab;
				oldObjectKey.m_Original = creationData4.m_CreationDefinition.m_Original;
				oldObjectKey.m_Owner = creationData4.m_CreationDefinition.m_Owner;
				if (!m_OldObjectMap.TryGetFirstValue(oldObjectKey, ref oldObjectValue, ref val))
				{
					continue;
				}
				float num3 = float.MaxValue;
				Entity val2 = oldObjectValue.m_Entity;
				NativeParallelMultiHashMapIterator<OldObjectKey> val3 = val;
				do
				{
					float num4 = math.distancesq(creationData4.m_ObjectDefinition.m_Position, oldObjectValue.m_Transform.m_Position);
					if (num4 < num3)
					{
						num3 = num4;
						val2 = oldObjectValue.m_Entity;
						val3 = val;
					}
				}
				while (m_OldObjectMap.TryGetNextValue(ref oldObjectValue, ref val));
				creationData4.m_OldEntity = val2;
				m_OldObjectMap.Remove(val3);
				m_CreationList[k] = creationData4;
				ownerDefinition.m_Prefab = creationData4.m_CreationDefinition.m_Prefab;
				ownerDefinition.m_Position = creationData4.m_ObjectDefinition.m_Position;
				ownerDefinition.m_Rotation = creationData4.m_ObjectDefinition.m_Rotation;
				m_ReusedOwnerMap.TryAdd(ownerDefinition, val2);
			}
			Entity owner = default(Entity);
			OldObjectKey oldObjectKey2 = default(OldObjectKey);
			OldObjectValue oldObjectValue2 = default(OldObjectValue);
			NativeParallelMultiHashMapIterator<OldObjectKey> val4 = default(NativeParallelMultiHashMapIterator<OldObjectKey>);
			OwnerDefinition ownerDefinition2 = default(OwnerDefinition);
			for (int l = 0; l < m_CreationList.Length; l++)
			{
				CreationData creationData5 = m_CreationList[l];
				if (!(creationData5.m_OwnerDefinition.m_Prefab != Entity.Null) || !m_ReusedOwnerMap.TryGetValue(creationData5.m_OwnerDefinition, ref owner))
				{
					continue;
				}
				oldObjectKey2.m_Prefab = creationData5.m_CreationDefinition.m_Prefab;
				oldObjectKey2.m_SubPrefab = creationData5.m_CreationDefinition.m_SubPrefab;
				oldObjectKey2.m_Original = creationData5.m_CreationDefinition.m_Original;
				oldObjectKey2.m_Owner = owner;
				if (!m_OldObjectMap.TryGetFirstValue(oldObjectKey2, ref oldObjectValue2, ref val4))
				{
					continue;
				}
				float num5 = float.MaxValue;
				Entity val5 = oldObjectValue2.m_Entity;
				NativeParallelMultiHashMapIterator<OldObjectKey> val6 = val4;
				do
				{
					float num6 = math.distancesq(creationData5.m_ObjectDefinition.m_Position, oldObjectValue2.m_Transform.m_Position);
					if (num6 < num5)
					{
						num5 = num6;
						val5 = oldObjectValue2.m_Entity;
						val6 = val4;
					}
				}
				while (m_OldObjectMap.TryGetNextValue(ref oldObjectValue2, ref val4));
				creationData5.m_OldEntity = val5;
				m_OldObjectMap.Remove(val6);
				m_CreationList[l] = creationData5;
				ownerDefinition2.m_Prefab = creationData5.m_CreationDefinition.m_Prefab;
				ownerDefinition2.m_Position = creationData5.m_ObjectDefinition.m_Position;
				ownerDefinition2.m_Rotation = creationData5.m_ObjectDefinition.m_Rotation;
				m_ReusedOwnerMap.TryAdd(ownerDefinition2, val5);
			}
		}
	}

	[BurstCompile]
	private struct CreateObjectsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public ComponentTypeSet m_SubTypes;

		[ReadOnly]
		public ComponentTypeSet m_StoppedUpdateFrameTypes;

		[ReadOnly]
		public NativeArray<CreationData> m_CreationList;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Stopped> m_StoppedData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<Recent> m_RecentData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_DamagedData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Surface> m_SurfaceData;

		[ReadOnly]
		public ComponentLookup<Stack> m_StackData;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructionData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_ObjectData;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> m_MovingObjectData;

		[ReadOnly]
		public ComponentLookup<TreeData> m_PrefabTreeData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<EffectData> m_PrefabEffectData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameterData;

		public void Execute(int index)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			CreationData creationData = m_CreationList[index];
			CreateObject(index, creationData.m_CreationDefinition, creationData.m_OwnerDefinition, creationData.m_ObjectDefinition, creationData.m_OldEntity);
		}

		private void CreateObject(int jobIndex, CreationDefinition definitionData, OwnerDefinition ownerDefinition, ObjectDefinition objectDefinition, Entity oldEntity)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0871: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0921: Unknown result type (might be due to invalid IL or missing references)
			//IL_092b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_0949: Unknown result type (might be due to invalid IL or missing references)
			//IL_095f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0968: Unknown result type (might be due to invalid IL or missing references)
			//IL_096d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0998: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_09be: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_06df: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be0: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_082a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef4: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = definitionData.m_Prefab
			};
			if (definitionData.m_Original != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(jobIndex, definitionData.m_Original, default(Hidden));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, definitionData.m_Original, default(BatchesUpdated));
			}
			Random random = m_RandomSeed.GetRandom(jobIndex);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			bool flag = m_PrefabObjectData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData);
			bool flag2 = (definitionData.m_Flags & CreationFlags.Permanent) != 0 || m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab);
			Tree tree = default(Tree);
			bool flag3 = flag2 && m_PrefabTreeData.HasComponent(prefabRef.m_Prefab);
			if (flag3 && !m_TreeData.TryGetComponent(definitionData.m_Original, ref tree))
			{
				tree = ObjectUtils.InitializeTreeState(objectDefinition.m_Age);
			}
			Temp temp = new Temp
			{
				m_Original = definitionData.m_Original
			};
			temp.m_Flags |= TempFlags.Essential;
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			ServiceUpgradeData serviceUpgradeData = default(ServiceUpgradeData);
			if (m_PlaceableObjectData.TryGetComponent(definitionData.m_Prefab, ref placeableObjectData))
			{
				temp.m_Value = (int)placeableObjectData.m_ConstructionCost;
				if (flag3)
				{
					temp.m_Value = ObjectUtils.GetContructionCost(temp.m_Value, tree, in m_EconomyParameterData);
				}
			}
			else if (m_ServiceUpgradeData.TryGetComponent(definitionData.m_Prefab, ref serviceUpgradeData))
			{
				temp.m_Value = (int)serviceUpgradeData.m_UpgradeCost;
			}
			if ((definitionData.m_Flags & CreationFlags.Delete) != 0)
			{
				temp.m_Flags |= TempFlags.Delete;
				Recent recent = default(Recent);
				if (m_RecentData.TryGetComponent(definitionData.m_Original, ref recent))
				{
					temp.m_Cost = -ObjectUtils.GetRefundAmount(recent, m_SimulationFrame, m_EconomyParameterData);
				}
			}
			else if ((definitionData.m_Flags & CreationFlags.Select) != 0)
			{
				temp.m_Flags |= TempFlags.Select;
				if ((definitionData.m_Flags & CreationFlags.Dragging) != 0)
				{
					temp.m_Flags |= TempFlags.Dragging;
				}
			}
			else if (definitionData.m_Original != Entity.Null)
			{
				if ((definitionData.m_Flags & CreationFlags.Relocate) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
					if (!m_EditorMode && IsMoved(definitionData.m_Original, objectDefinition))
					{
						Recent recent2 = default(Recent);
						if (m_RecentData.TryGetComponent(definitionData.m_Original, ref recent2))
						{
							temp.m_Cost = ObjectUtils.GetRelocationCost(temp.m_Value, recent2, m_SimulationFrame, m_EconomyParameterData);
						}
						else
						{
							temp.m_Cost = ObjectUtils.GetRelocationCost(temp.m_Value, m_EconomyParameterData);
						}
					}
				}
				else
				{
					if ((definitionData.m_Flags & CreationFlags.Upgrade) != 0)
					{
						temp.m_Flags |= TempFlags.Upgrade;
						PrefabRef prefabRef2 = default(PrefabRef);
						if (!m_EditorMode && m_PrefabRefData.TryGetComponent(definitionData.m_Original, ref prefabRef2) && prefabRef2.m_Prefab != definitionData.m_Prefab)
						{
							int num = 0;
							PlaceableObjectData placeableObjectData2 = default(PlaceableObjectData);
							if (m_PlaceableObjectData.TryGetComponent(prefabRef2.m_Prefab, ref placeableObjectData2))
							{
								num = (int)placeableObjectData2.m_ConstructionCost;
								if (flag3)
								{
									num = ObjectUtils.GetContructionCost(num, tree, in m_EconomyParameterData);
								}
							}
							Recent recent3 = default(Recent);
							if (m_RecentData.TryGetComponent(definitionData.m_Original, ref recent3))
							{
								temp.m_Cost = ObjectUtils.GetUpgradeCost(temp.m_Value, num, recent3, m_SimulationFrame, m_EconomyParameterData);
							}
							else
							{
								temp.m_Cost = ObjectUtils.GetUpgradeCost(temp.m_Value, num);
							}
						}
					}
					else if ((definitionData.m_Flags & CreationFlags.Duplicate) != 0)
					{
						temp.m_Flags |= TempFlags.Duplicate;
					}
					if ((definitionData.m_Flags & CreationFlags.Repair) != 0 && !m_EditorMode && m_DestroyedData.HasComponent(definitionData.m_Original))
					{
						Recent recent4 = default(Recent);
						if (m_RecentData.TryGetComponent(definitionData.m_Original, ref recent4))
						{
							temp.m_Cost = ObjectUtils.GetRebuildCost(temp.m_Value, recent4, m_SimulationFrame, m_EconomyParameterData);
						}
						else
						{
							temp.m_Cost = ObjectUtils.GetRebuildCost(temp.m_Value);
						}
					}
					if ((definitionData.m_Flags & CreationFlags.Parent) != 0)
					{
						temp.m_Flags |= TempFlags.Parent;
					}
				}
			}
			else
			{
				temp.m_Flags |= TempFlags.Create;
				if ((definitionData.m_Flags & CreationFlags.Optional) != 0)
				{
					temp.m_Flags |= TempFlags.Optional;
				}
				if (!m_EditorMode)
				{
					temp.m_Cost = temp.m_Value;
				}
			}
			ElevationFlags elevationFlags = (ElevationFlags)0;
			if (math.abs(objectDefinition.m_ParentMesh) >= 1000)
			{
				elevationFlags |= ElevationFlags.Stacked;
			}
			if (objectDefinition.m_ParentMesh < 0)
			{
				elevationFlags |= ElevationFlags.OnGround;
			}
			if ((definitionData.m_Flags & CreationFlags.Lowered) != 0)
			{
				elevationFlags |= ElevationFlags.Lowered;
			}
			if (oldEntity != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, oldEntity);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldEntity, default(Updated));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, oldEntity, new Transform(objectDefinition.m_Position, objectDefinition.m_Rotation));
				if (ownerDefinition.m_Prefab == Entity.Null && definitionData.m_Owner == Entity.Null && (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Physical) != Game.Objects.GeometryFlags.None)
				{
					if (m_SurfaceData.HasComponent(definitionData.m_Original))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Objects.Surface>(jobIndex, oldEntity, m_SurfaceData[definitionData.m_Original]);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Objects.Surface>(jobIndex, oldEntity, default(Game.Objects.Surface));
					}
				}
				Attached attached2 = default(Attached);
				if ((definitionData.m_Flags & CreationFlags.Attach) != 0 || (placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Attached) != Game.Objects.PlacementFlags.None)
				{
					Attached attached = default(Attached);
					m_AttachedData.TryGetComponent(oldEntity, ref attached);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Attached>(jobIndex, oldEntity, CreateAttached(definitionData.m_Attached, attached.m_Parent, objectDefinition.m_Position));
				}
				else if (m_AttachedData.TryGetComponent(oldEntity, ref attached2))
				{
					attached2.m_OldParent = attached2.m_Parent;
					attached2.m_Parent = oldEntity;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Attached>(jobIndex, oldEntity, attached2);
				}
				if ((definitionData.m_Flags & CreationFlags.Permanent) == 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, oldEntity, temp);
				}
				if (objectDefinition.m_Elevation != 0f || objectDefinition.m_ParentMesh != -1 || definitionData.m_SubPrefab != Entity.Null)
				{
					if (m_ElevationData.HasComponent(oldEntity))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Objects.Elevation>(jobIndex, oldEntity, new Game.Objects.Elevation(objectDefinition.m_Elevation, elevationFlags));
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Objects.Elevation>(jobIndex, oldEntity, new Game.Objects.Elevation(objectDefinition.m_Elevation, elevationFlags));
					}
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Objects.Elevation>(jobIndex, oldEntity);
				}
				if (flag3)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, oldEntity, tree);
				}
				if (m_EditorMode && (ownerDefinition.m_Prefab != Entity.Null || definitionData.m_Owner != Entity.Null))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<LocalTransformCache>(jobIndex, oldEntity, new LocalTransformCache
					{
						m_Position = objectDefinition.m_LocalPosition,
						m_Rotation = objectDefinition.m_LocalRotation,
						m_ParentMesh = objectDefinition.m_ParentMesh,
						m_GroupIndex = objectDefinition.m_GroupIndex,
						m_Probability = objectDefinition.m_Probability,
						m_PrefabSubIndex = objectDefinition.m_PrefabSubIndex
					});
				}
				if (flag)
				{
					if (m_PseudoRandomSeedData.HasComponent(definitionData.m_Original))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, m_PseudoRandomSeedData[definitionData.m_Original]);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, new PseudoRandomSeed((ushort)definitionData.m_RandomSeed));
					}
				}
				if (definitionData.m_SubPrefab != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EditorContainer>(jobIndex, oldEntity, new EditorContainer
					{
						m_Prefab = definitionData.m_SubPrefab,
						m_Scale = objectDefinition.m_Scale,
						m_Intensity = objectDefinition.m_Intensity,
						m_GroupIndex = objectDefinition.m_GroupIndex
					});
				}
				if (definitionData.m_Original != Entity.Null)
				{
					UnderConstruction underConstruction = default(UnderConstruction);
					if (m_UnderConstructionData.TryGetComponent(definitionData.m_Original, ref underConstruction))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<UnderConstruction>(jobIndex, oldEntity, underConstruction);
					}
					else if (m_UnderConstructionData.HasComponent(oldEntity))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<UnderConstruction>(jobIndex, oldEntity);
					}
				}
				return;
			}
			Entity val;
			if (m_StoppedData.HasComponent(definitionData.m_Original))
			{
				val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MovingObjectData[prefabRef.m_Prefab].m_StoppedArchetype);
			}
			else if (m_EditorMode && m_MovingObjectData.HasComponent(prefabRef.m_Prefab))
			{
				val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MovingObjectData[prefabRef.m_Prefab].m_StoppedArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val, ref m_StoppedUpdateFrameTypes);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Static>(jobIndex, val, default(Static));
			}
			else
			{
				val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ObjectData[prefabRef.m_Prefab].m_Archetype);
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val, new Transform(objectDefinition.m_Position, objectDefinition.m_Rotation));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, prefabRef);
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, default(Owner));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, val, ownerDefinition);
			}
			else if (definitionData.m_Owner != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val, new Owner(definitionData.m_Owner));
			}
			else if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Physical) != Game.Objects.GeometryFlags.None)
			{
				if (m_SurfaceData.HasComponent(definitionData.m_Original))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Objects.Surface>(jobIndex, val, m_SurfaceData[definitionData.m_Original]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Objects.Surface>(jobIndex, val, default(Game.Objects.Surface));
				}
			}
			if ((definitionData.m_Flags & CreationFlags.Attach) != 0 || (placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Attached) != Game.Objects.PlacementFlags.None)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Attached>(jobIndex, val, CreateAttached(definitionData.m_Attached, Entity.Null, objectDefinition.m_Position));
			}
			if ((definitionData.m_Flags & CreationFlags.Repair) == 0)
			{
				if (m_DamagedData.HasComponent(definitionData.m_Original))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Damaged>(jobIndex, val, m_DamagedData[definitionData.m_Original]);
				}
				if (m_DestroyedData.HasComponent(definitionData.m_Original))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Destroyed>(jobIndex, val, m_DestroyedData[definitionData.m_Original]);
				}
			}
			if ((definitionData.m_Flags & CreationFlags.Permanent) == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val, temp);
				if (flag)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Animation>(jobIndex, val, default(Animation));
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(jobIndex, val, default(InterpolatedTransform));
				}
				BuildingData buildingData = default(BuildingData);
				if (flag2 && m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData) && (buildingData.m_Flags & Game.Prefabs.BuildingFlags.BackAccess) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BackSide>(jobIndex, val, default(BackSide));
				}
			}
			if ((definitionData.m_Flags & CreationFlags.Native) != 0 || (m_NativeData.HasComponent(definitionData.m_Original) && (temp.m_Flags & (TempFlags.Modify | TempFlags.Upgrade)) == 0))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Native>(jobIndex, val, default(Native));
			}
			if (objectDefinition.m_Elevation != 0f || objectDefinition.m_ParentMesh != -1 || definitionData.m_SubPrefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Objects.Elevation>(jobIndex, val, new Game.Objects.Elevation(objectDefinition.m_Elevation, elevationFlags));
			}
			if (flag3)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Tree>(jobIndex, val, tree);
			}
			StackData stackData = default(StackData);
			if (flag2 && m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
			{
				Stack stack = default(Stack);
				if (m_StackData.TryGetComponent(definitionData.m_Original, ref stack))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Stack>(jobIndex, val, stack);
				}
				else
				{
					if (stackData.m_Direction == StackDirection.Up)
					{
						stack.m_Range.min = stackData.m_FirstBounds.min - objectDefinition.m_Elevation;
						stack.m_Range.max = stackData.m_LastBounds.max;
					}
					else
					{
						stack.m_Range.min = stackData.m_FirstBounds.min;
						stack.m_Range.max = stackData.m_FirstBounds.max + MathUtils.Size(stackData.m_MiddleBounds) * 2f + MathUtils.Size(stackData.m_LastBounds);
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Stack>(jobIndex, val, stack);
				}
			}
			if (m_EditorMode)
			{
				bool flag4 = true;
				if (ownerDefinition.m_Prefab != Entity.Null || definitionData.m_Owner != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalTransformCache>(jobIndex, val, new LocalTransformCache
					{
						m_Position = objectDefinition.m_LocalPosition,
						m_Rotation = objectDefinition.m_LocalRotation,
						m_ParentMesh = objectDefinition.m_ParentMesh,
						m_GroupIndex = objectDefinition.m_GroupIndex,
						m_Probability = objectDefinition.m_Probability,
						m_PrefabSubIndex = objectDefinition.m_PrefabSubIndex
					});
					flag4 = m_ServiceUpgradeData.HasComponent(definitionData.m_Prefab);
				}
				if (flag4)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<EnabledEffect>(jobIndex, val);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val, ref m_SubTypes);
				}
			}
			if (flag)
			{
				if (m_PseudoRandomSeedData.HasComponent(definitionData.m_Original))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, m_PseudoRandomSeedData[definitionData.m_Original]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, new PseudoRandomSeed((ushort)definitionData.m_RandomSeed));
				}
			}
			if (definitionData.m_SubPrefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EditorContainer>(jobIndex, val, new EditorContainer
				{
					m_Prefab = definitionData.m_SubPrefab,
					m_Scale = objectDefinition.m_Scale,
					m_Intensity = objectDefinition.m_Intensity,
					m_GroupIndex = objectDefinition.m_GroupIndex
				});
				if (m_PrefabEffectData.HasComponent(definitionData.m_SubPrefab))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<EnabledEffect>(jobIndex, val);
				}
			}
			if (definitionData.m_Original != Entity.Null)
			{
				UnderConstruction underConstruction2 = default(UnderConstruction);
				if (m_UnderConstructionData.TryGetComponent(definitionData.m_Original, ref underConstruction2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<UnderConstruction>(jobIndex, val, underConstruction2);
				}
				Relative relative = default(Relative);
				if (m_RelativeData.TryGetComponent(definitionData.m_Original, ref relative))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Relative>(jobIndex, val, relative);
				}
			}
			else if ((definitionData.m_Flags & CreationFlags.Construction) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<UnderConstruction>(jobIndex, val, new UnderConstruction
				{
					m_Speed = (byte)((Random)(ref random)).NextInt(39, 89)
				});
			}
		}

		private bool IsMoved(Entity original, ObjectDefinition objectDefinition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = default(Transform);
			if (m_TransformData.TryGetComponent(original, ref transform))
			{
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				bool flag = objectDefinition.m_ParentMesh >= 0 || (m_ElevationData.TryGetComponent(original, ref elevation) && (elevation.m_Flags & ElevationFlags.OnGround) == 0);
				float3 val = transform.m_Position - objectDefinition.m_Position;
				val.y = math.select(0f, val.y, flag);
				if (math.length(val) > 0.1f)
				{
					return true;
				}
				if (MathUtils.RotationAngle(transform.m_Rotation, objectDefinition.m_Rotation) > 0.1f)
				{
					return true;
				}
			}
			return false;
		}

		private Attached CreateAttached(Entity parent, Entity oldParent, float3 position)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			Attached result = new Attached(parent, oldParent, 0f);
			if (m_CurveData.HasComponent(parent))
			{
				MathUtils.Distance(m_CurveData[parent].m_Bezier, position, ref result.m_CurvePosition);
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> __Game_Tools_OwnerDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ObjectDefinition> __Game_Tools_ObjectDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> __Game_Tools_NetCourse_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> __Game_Prefabs_LocalConnectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnect> __Game_Net_LocalConnect_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Roundabout> __Game_Net_Roundabout_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stopped> __Game_Objects_Stopped_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Relative> __Game_Objects_Relative_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Recent> __Game_Tools_Recent_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Surface> __Game_Objects_Surface_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stack> __Game_Objects_Stack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> __Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> __Game_Prefabs_MovingObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EffectData> __Game_Prefabs_EffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

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
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OwnerDefinition>(true);
			__Game_Tools_ObjectDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectDefinition>(true);
			__Game_Tools_NetCourse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCourse>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_LocalConnectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnectData>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_LocalConnect_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnect>(true);
			__Game_Net_Roundabout_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Roundabout>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Tools_EditorContainer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EditorContainer>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Objects_Stopped_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stopped>(true);
			__Game_Objects_Relative_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Relative>(true);
			__Game_Tools_Recent_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Recent>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Objects_Surface_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Surface>(true);
			__Game_Objects_Stack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stack>(true);
			__Game_Objects_UnderConstruction_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UnderConstruction>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUpgradeData>(true);
			__Game_Prefabs_MovingObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingObjectData>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_EffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private SimulationSystem m_SimulationSystem;

	private ModificationBarrier1 m_ModificationBarrier;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_DeletedQuery;

	private EntityQuery m_EconomyParameterQuery;

	private ComponentTypeSet m_SubTypes;

	private ComponentTypeSet m_StoppedUpdateFrameTypes;

	private NativeHashMap<OwnerDefinition, Entity> m_ReusedOwnerMap;

	private JobHandle m_OwnerMapReadDeps;

	private JobHandle m_OwnerMapWriteDeps;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ReusedOwnerMap = new NativeHashMap<OwnerDefinition, Entity>(32, AllocatorHandle.op_Implicit((Allocator)4));
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ObjectDefinition>(),
			ComponentType.ReadOnly<NetCourse>()
		};
		array[0] = val;
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_DeletedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Objects.Object>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<PrefabRef>()
		});
		m_SubTypes = new ComponentTypeSet(ComponentType.ReadWrite<Game.Objects.SubObject>(), ComponentType.ReadWrite<Game.Net.SubNet>(), ComponentType.ReadWrite<Game.Areas.SubArea>());
		m_StoppedUpdateFrameTypes = new ComponentTypeSet(ComponentType.ReadWrite<Stopped>(), ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<ParkedTrain>(), ComponentType.ReadWrite<UpdateFrame>());
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_OwnerMapReadDeps)).Complete();
		((JobHandle)(ref m_OwnerMapWriteDeps)).Complete();
		m_ReusedOwnerMap.Dispose();
		base.OnDestroy();
	}

	public NativeHashMap<OwnerDefinition, Entity> GetReusedOwnerMap(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_OwnerMapWriteDeps;
		return m_ReusedOwnerMap;
	}

	public void AddOwnerMapReader(JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_OwnerMapReadDeps = JobHandle.CombineDependencies(m_OwnerMapReadDeps, dependencies);
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((JobHandle)(ref m_OwnerMapReadDeps)).Complete();
		((JobHandle)(ref m_OwnerMapWriteDeps)).Complete();
		m_ReusedOwnerMap.Clear();
		((COSystemBase)this).OnStopRunning();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<CreationData> creationQueue = default(NativeQueue<CreationData>);
		creationQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<CreationData> val = default(NativeList<CreationData>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelMultiHashMap<OldObjectKey, OldObjectValue> oldObjectMap = default(NativeParallelMultiHashMap<OldObjectKey, OldObjectValue>);
		oldObjectMap._002Ector(32, AllocatorHandle.op_Implicit((Allocator)3));
		((JobHandle)(ref m_OwnerMapReadDeps)).Complete();
		((JobHandle)(ref m_OwnerMapWriteDeps)).Complete();
		m_ReusedOwnerMap.Clear();
		JobHandle dependencies;
		FillCreationListJob fillCreationListJob = new FillCreationListJob
		{
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<OwnerDefinition>(ref __TypeHandle.__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<ObjectDefinition>(ref __TypeHandle.__Game_Tools_ObjectDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCourseType = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnect>(ref __TypeHandle.__Game_Net_LocalConnect_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoundaboutData = InternalCompilerInterface.GetComponentLookup<Roundabout>(ref __TypeHandle.__Game_Net_Roundabout_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_CreationQueue = creationQueue.AsParallelWriter()
		};
		FillOldObjectsJob fillOldObjectsJob = new FillOldObjectsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OldObjectMap = oldObjectMap
		};
		CollectCreationDataJob collectCreationDataJob = new CollectCreationDataJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreationQueue = creationQueue,
			m_CreationList = val,
			m_OldObjectMap = oldObjectMap,
			m_ReusedOwnerMap = m_ReusedOwnerMap
		};
		CreateObjectsJob createObjectsJob = new CreateObjectsJob
		{
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_SubTypes = m_SubTypes,
			m_StoppedUpdateFrameTypes = m_StoppedUpdateFrameTypes,
			m_CreationList = val.AsDeferredJobArray()
		};
		EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
		createObjectsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		createObjectsJob.m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_StoppedData = InternalCompilerInterface.GetComponentLookup<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_RecentData = InternalCompilerInterface.GetComponentLookup<Recent>(ref __TypeHandle.__Game_Tools_Recent_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_SurfaceData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Surface>(ref __TypeHandle.__Game_Objects_Surface_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_StackData = InternalCompilerInterface.GetComponentLookup<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_UnderConstructionData = InternalCompilerInterface.GetComponentLookup<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_ObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_MovingObjectData = InternalCompilerInterface.GetComponentLookup<MovingObjectData>(ref __TypeHandle.__Game_Prefabs_MovingObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PrefabEffectData = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		createObjectsJob.m_EconomyParameterData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
		CreateObjectsJob createObjectsJob2 = createObjectsJob;
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<FillCreationListJob>(fillCreationListJob, m_DefinitionQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val4 = JobChunkExtensions.Schedule<FillOldObjectsJob>(fillOldObjectsJob, m_DeletedQuery, ((SystemBase)this).Dependency);
		JobHandle val5 = IJobExtensions.Schedule<CollectCreationDataJob>(collectCreationDataJob, JobHandle.CombineDependencies(val3, val4));
		JobHandle val6 = IJobParallelForDeferExtensions.Schedule<CreateObjectsJob, CreationData>(createObjectsJob2, val, 1, val5);
		creationQueue.Dispose(val5);
		val.Dispose(val6);
		oldObjectMap.Dispose(val5);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val6);
		m_NetSearchSystem.AddNetSearchTreeReader(val3);
		m_OwnerMapWriteDeps = val5;
		((SystemBase)this).Dependency = val6;
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
	public GenerateObjectsSystem()
	{
	}
}

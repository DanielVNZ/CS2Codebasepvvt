using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
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
public class GenerateEdgesSystem : GameSystemBase
{
	private struct NodeMapKey : IEquatable<NodeMapKey>
	{
		public Entity m_OriginalEntity;

		public float3 m_Position;

		public bool m_IsPermanent;

		public bool m_IsEditor;

		public NodeMapKey(Entity originalEntity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			m_OriginalEntity = originalEntity;
			m_Position = default(float3);
			m_IsPermanent = false;
			m_IsEditor = false;
		}

		public NodeMapKey(Entity originalEntity, float3 position, bool isPermanent, bool isEditor)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_OriginalEntity = originalEntity;
			m_Position = position;
			m_IsPermanent = isPermanent;
			m_IsEditor = isEditor;
		}

		public NodeMapKey(CoursePos coursePos, bool isPermanent, bool isEditor)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			m_OriginalEntity = coursePos.m_Entity;
			m_Position = coursePos.m_Position;
			m_IsPermanent = isPermanent;
			m_IsEditor = isEditor;
		}

		public bool Equals(NodeMapKey other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (m_OriginalEntity != Entity.Null || other.m_OriginalEntity != Entity.Null)
			{
				return ((Entity)(ref m_OriginalEntity)).Equals(other.m_OriginalEntity);
			}
			if (((float3)(ref m_Position)).Equals(other.m_Position) && m_IsPermanent == other.m_IsPermanent)
			{
				return m_IsEditor == other.m_IsEditor;
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (m_OriginalEntity != Entity.Null)
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_OriginalEntity)/*cast due to .constrained prefix*/).GetHashCode();
			}
			return ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct LocalConnectItem
	{
		public Layer m_ConnectLayers;

		public Layer m_LocalConnectLayers;

		public float3 m_Position;

		public float m_Radius;

		public Bounds1 m_HeightRange;

		public Entity m_Node;

		public TempFlags m_TempFlags;

		public bool m_IsPermanent;

		public bool m_IsStandalone;

		public LocalConnectItem(Layer connectLayers, Layer localConnectLayers, float3 position, float radius, Bounds1 heightRange, Entity node, TempFlags tempFlags, bool isPermanent, bool isStandalone)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			m_ConnectLayers = connectLayers;
			m_LocalConnectLayers = localConnectLayers;
			m_Position = position;
			m_Radius = radius;
			m_HeightRange = heightRange;
			m_Node = node;
			m_TempFlags = tempFlags;
			m_IsPermanent = isPermanent;
			m_IsStandalone = isStandalone;
		}
	}

	private struct OldEdgeKey : IEquatable<OldEdgeKey>
	{
		public Entity m_Prefab;

		public Entity m_SubPrefab;

		public Entity m_Original;

		public Entity m_Owner;

		public Entity m_StartNode;

		public Entity m_EndNode;

		public bool Equals(OldEdgeKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab) && ((Entity)(ref m_SubPrefab)).Equals(other.m_SubPrefab) && ((Entity)(ref m_Original)).Equals(other.m_Original) && ((Entity)(ref m_Owner)).Equals(other.m_Owner) && ((Entity)(ref m_StartNode)).Equals(other.m_StartNode))
			{
				return ((Entity)(ref m_EndNode)).Equals(other.m_EndNode);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SubPrefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Original)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Owner)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_StartNode)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_EndNode)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	[BurstCompile]
	private struct CheckNodesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainer> m_EditorContainerType;

		[ReadOnly]
		public ComponentTypeHandle<LocalConnect> m_LocalConnectType;

		[ReadOnly]
		public ComponentTypeHandle<Elevation> m_ElevationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_LocalConnectData;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public ParallelWriter<NodeMapKey, Entity> m_NodeMap;

		public ParallelWriter<LocalConnectItem> m_LocalConnectQueue;

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
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Node> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			bool isEditor = ((ArchetypeChunk)(ref chunk)).Has<EditorContainer>(ref m_EditorContainerType);
			((ArchetypeChunk)(ref chunk)).Has<Elevation>(ref m_ElevationType);
			bool flag = !((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				Node node = nativeArray3[i];
				_ = nativeArray4[i];
				if (nativeArray2.Length != 0)
				{
					m_NodeMap.Add(new NodeMapKey(nativeArray2[i].m_Original, node.m_Position, isPermanent: false, isEditor), nativeArray[i]);
				}
				else
				{
					m_NodeMap.Add(new NodeMapKey(Entity.Null, node.m_Position, isPermanent: true, isEditor), nativeArray[i]);
				}
			}
			if (!((ArchetypeChunk)(ref chunk)).Has<LocalConnect>(ref m_LocalConnectType))
			{
				return;
			}
			for (int j = 0; j < nativeArray4.Length; j++)
			{
				Entity node2 = nativeArray[j];
				Node node3 = nativeArray3[j];
				PrefabRef prefabRef = nativeArray4[j];
				LocalConnectData localConnectData = m_LocalConnectData[prefabRef.m_Prefab];
				NetGeometryData netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
				NetData netData = m_NetData[prefabRef.m_Prefab];
				float radius = math.max(0f, netGeometryData.m_DefaultWidth * 0.5f + localConnectData.m_SearchDistance);
				if (nativeArray2.Length != 0)
				{
					Temp temp = nativeArray2[j];
					m_LocalConnectQueue.Enqueue(new LocalConnectItem(localConnectData.m_Layers, netData.m_ConnectLayers, node3.m_Position, radius, localConnectData.m_HeightRange, node2, temp.m_Flags, isPermanent: false, flag || localConnectData.m_SearchDistance == 0f || (netGeometryData.m_Flags & GeometryFlags.SubOwner) != 0));
				}
				else
				{
					m_LocalConnectQueue.Enqueue(new LocalConnectItem(localConnectData.m_Layers, netData.m_ConnectLayers, node3.m_Position, radius, localConnectData.m_HeightRange, node2, (TempFlags)0u, isPermanent: true, flag || localConnectData.m_SearchDistance == 0f || (netGeometryData.m_Flags & GeometryFlags.SubOwner) != 0));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillOldEdgesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainer> m_EditorContainerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public NativeHashMap<OldEdgeKey, Entity> m_OldEdgeMap;

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
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Edge> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<EditorContainer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EditorContainer>(ref m_EditorContainerType);
			NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			OldEdgeKey oldEdgeKey = default(OldEdgeKey);
			EditorContainer editorContainer = default(EditorContainer);
			Owner owner = default(Owner);
			for (int i = 0; i < nativeArray6.Length; i++)
			{
				Edge edge = nativeArray4[i];
				oldEdgeKey.m_Prefab = nativeArray6[i].m_Prefab;
				oldEdgeKey.m_SubPrefab = Entity.Null;
				oldEdgeKey.m_Original = nativeArray3[i].m_Original;
				oldEdgeKey.m_Owner = Entity.Null;
				oldEdgeKey.m_StartNode = edge.m_Start;
				oldEdgeKey.m_EndNode = edge.m_End;
				if (CollectionUtils.TryGet<EditorContainer>(nativeArray5, i, ref editorContainer))
				{
					oldEdgeKey.m_SubPrefab = editorContainer.m_Prefab;
				}
				if (CollectionUtils.TryGet<Owner>(nativeArray2, i, ref owner))
				{
					oldEdgeKey.m_Owner = owner.m_Owner;
				}
				m_OldEdgeMap.TryAdd(oldEdgeKey, nativeArray[i]);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CheckDefinitionsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		public ParallelWriter<NodeMapKey, Entity> m_NodeMap;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition creationDefinition = nativeArray[i];
				if (!m_DeletedData.HasComponent(creationDefinition.m_Owner) && m_EdgeData.HasComponent(creationDefinition.m_Original))
				{
					m_NodeMap.Add(new NodeMapKey(creationDefinition.m_Original), Entity.Null);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CollectLocalConnectItemsJob : IJob
	{
		public NativeQueue<LocalConnectItem> m_LocalConnectQueue;

		public NativeList<LocalConnectItem> m_LocalConnectList;

		public void Execute()
		{
			int count = m_LocalConnectQueue.Count;
			m_LocalConnectList.ResizeUninitialized(count);
			for (int i = 0; i < count; i++)
			{
				m_LocalConnectList[i] = m_LocalConnectQueue.Dequeue();
			}
		}
	}

	[BurstCompile]
	private struct GenerateEdgesJob : IJobChunk
	{
		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> m_ChunkBaseEntityIndices;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

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
		public BufferTypeHandle<SubReplacement> m_SubReplacementType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Game.Net.BuildOrder> m_BuildOrderData;

		[ReadOnly]
		public ComponentLookup<TramTrack> m_TramTrackData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<NetCondition> m_ConditionData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<Aggregated> m_AggregatedData;

		[ReadOnly]
		public ComponentLookup<Roundabout> m_RoundaboutData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ElectricityConnection> m_ElectricityConnectionData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_LocalConnectData;

		[ReadOnly]
		public ComponentLookup<TrackData> m_PrefabTrackData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_PrefabRoadData;

		[ReadOnly]
		public ComponentLookup<ElectricityConnectionData> m_PrefabElectricityConnectionData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<SubReplacement> m_SubReplacements;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public uint m_BuildOrder;

		[ReadOnly]
		public NativeParallelMultiHashMap<NodeMapKey, Entity> m_NodeMap;

		[ReadOnly]
		public NativeHashMap<OwnerDefinition, Entity> m_ReusedOwnerMap;

		[ReadOnly]
		public NativeHashMap<OldEdgeKey, Entity> m_OldEdgeMap;

		[ReadOnly]
		public NativeArray<LocalConnectItem> m_LocalConnectList;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			int num = m_ChunkBaseEntityIndices[unfilteredChunkIndex];
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			if (nativeArray.Length != 0)
			{
				NativeArray<OwnerDefinition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<OwnerDefinition>(ref m_OwnerDefinitionType);
				NativeArray<NetCourse> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCourse>(ref m_NetCourseType);
				NativeArray<LocalCurveCache> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LocalCurveCache>(ref m_LocalCurveCacheType);
				NativeArray<Upgraded> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Upgraded>(ref m_UpgradedType);
				BufferAccessor<SubReplacement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubReplacement>(ref m_SubReplacementType);
				OwnerDefinition ownerData = default(OwnerDefinition);
				Upgraded upgraded = default(Upgraded);
				LocalCurveCache cachedCurve = default(LocalCurveCache);
				DynamicBuffer<SubReplacement> subReplacements = default(DynamicBuffer<SubReplacement>);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					CreationDefinition definitionData = nativeArray[i];
					if (!m_DeletedData.HasComponent(definitionData.m_Owner))
					{
						NetCourse course = nativeArray3[i];
						CollectionUtils.TryGet<OwnerDefinition>(nativeArray2, i, ref ownerData);
						CollectionUtils.TryGet<Upgraded>(nativeArray5, i, ref upgraded);
						CollectionUtils.TryGet<LocalCurveCache>(nativeArray4, i, ref cachedCurve);
						CollectionUtils.TryGet<SubReplacement>(bufferAccessor, i, ref subReplacements);
						GenerateEdge(unfilteredChunkIndex, num + i, definitionData, ownerData, course, upgraded, cachedCurve, nativeArray4.Length != 0, subReplacements);
					}
				}
				return;
			}
			NativeArray<Entity> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Edge> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			if (nativeArray8.Length != 0)
			{
				if (nativeArray7.Length == 0)
				{
					for (int j = 0; j < nativeArray8.Length; j++)
					{
						UpdateNodeConnections(unfilteredChunkIndex, nativeArray6[j], nativeArray8[j]);
					}
				}
				return;
			}
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			for (int k = 0; k < nativeArray7.Length; k++)
			{
				Temp temp = nativeArray7[k];
				if (m_ConnectedEdges.TryGetBuffer(temp.m_Original, ref val))
				{
					for (int l = 0; l < val.Length; l++)
					{
						Entity edge = val[l].m_Edge;
						if (ShouldDuplicate(edge, temp.m_Original, temp.m_Flags))
						{
							DuplicateEdge(unfilteredChunkIndex, edge);
						}
					}
				}
				else if (m_EdgeData.HasComponent(temp.m_Original))
				{
					SplitEdge(unfilteredChunkIndex, temp.m_Original, nativeArray6[k]);
				}
			}
		}

		private void UpdateNodeConnections(int jobIndex, Entity edge, Edge edgeData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedNode> val = m_ConnectedNodes[edge];
			DynamicBuffer<ConnectedNode> nodes = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ConnectedNode>(jobIndex, edge);
			Curve curveData = m_CurveData[edge];
			PrefabRef prefabRef = m_PrefabRefData[edge];
			NetData netData = m_NetData[prefabRef.m_Prefab];
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (m_NetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
			}
			bool isStandalone = true;
			if (m_OwnerData.HasComponent(edge))
			{
				isStandalone = false;
			}
			bool isZoneable = false;
			if (m_PrefabRoadData.HasComponent(prefabRef.m_Prefab))
			{
				isZoneable = (m_PrefabRoadData[prefabRef.m_Prefab].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0;
			}
			FindNodeConnections(nodes, edgeData, curveData, default(Temp), netData, netGeometryData, isPermanent: true, isStandalone, isZoneable);
			for (int i = 0; i < val.Length; i++)
			{
				ConnectedNode connectedNode = val[i];
				float3 position = m_NodeData[connectedNode.m_Node].m_Position;
				if (!m_NodeMap.ContainsKey(new NodeMapKey(Entity.Null, position, isPermanent: true, m_EditorContainerData.HasComponent(connectedNode.m_Node))))
				{
					nodes.Add(connectedNode);
				}
			}
		}

		private bool ShouldDuplicate(Entity edge, Entity fromNode, TempFlags startTempFlags)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			if (m_DeletedData.HasComponent(edge))
			{
				return false;
			}
			if (m_NodeMap.ContainsKey(new NodeMapKey(edge)))
			{
				return false;
			}
			Edge edge2 = m_EdgeData[edge];
			if (edge2.m_Start != fromNode)
			{
				return false;
			}
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<NodeMapKey> val2 = default(NativeParallelMultiHashMapIterator<NodeMapKey>);
			if (!m_NodeMap.TryGetFirstValue(new NodeMapKey(edge2.m_End), ref val, ref val2))
			{
				return false;
			}
			if ((startTempFlags & (TempFlags.Select | TempFlags.Modify | TempFlags.Regenerate | TempFlags.Upgrade)) != 0)
			{
				return true;
			}
			if (m_TempData.HasComponent(val) && (m_TempData[val].m_Flags & (TempFlags.Select | TempFlags.Modify | TempFlags.Regenerate | TempFlags.Upgrade)) != 0)
			{
				return true;
			}
			DynamicBuffer<ConnectedNode> val3 = m_ConnectedNodes[edge];
			Entity val4 = default(Entity);
			for (int i = 0; i < val3.Length; i++)
			{
				if (m_NodeMap.TryGetFirstValue(new NodeMapKey(val3[i].m_Node), ref val4, ref val2) && m_TempData.HasComponent(val4) && (m_TempData[val4].m_Flags & (TempFlags.Select | TempFlags.Modify | TempFlags.Regenerate | TempFlags.Upgrade)) != 0)
				{
					return true;
				}
			}
			return false;
		}

		private void DuplicateEdge(int jobIndex, Entity edge)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			Edge edge2 = m_EdgeData[edge];
			Curve curve = m_CurveData[edge];
			NativeParallelMultiHashMapIterator<NodeMapKey> val = default(NativeParallelMultiHashMapIterator<NodeMapKey>);
			if (!m_NodeMap.TryGetFirstValue(new NodeMapKey(edge2.m_Start), ref edge2.m_Start, ref val) || !m_NodeMap.TryGetFirstValue(new NodeMapKey(edge2.m_End), ref edge2.m_End, ref val) || edge2.m_Start == edge2.m_End)
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[edge];
			NetGeometryData netGeometryData = default(NetGeometryData);
			bool flag = false;
			bool flag2 = m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab);
			if (m_NetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
				flag = true;
			}
			Temp temp = new Temp
			{
				m_Original = edge
			};
			Temp temp2 = default(Temp);
			if (m_TempData.TryGetComponent(edge2.m_Start, ref temp2) && ((temp2.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent) || (temp2.m_Flags & TempFlags.Select) != 0))
			{
				temp.m_Flags |= TempFlags.SubDetail;
			}
			Temp temp3 = default(Temp);
			if (m_TempData.TryGetComponent(edge2.m_End, ref temp3) && ((temp3.m_Flags & (TempFlags.Upgrade | TempFlags.Parent)) == (TempFlags.Upgrade | TempFlags.Parent) || (temp3.m_Flags & TempFlags.Select) != 0))
			{
				temp.m_Flags |= TempFlags.SubDetail;
			}
			Composition composition = new Composition
			{
				m_Edge = prefabRef.m_Prefab,
				m_StartNode = prefabRef.m_Prefab,
				m_EndNode = prefabRef.m_Prefab
			};
			NetData netData = m_NetData[prefabRef.m_Prefab];
			EditorContainer editorContainer = default(EditorContainer);
			m_EditorContainerData.TryGetComponent(edge, ref editorContainer);
			Owner owner = default(Owner);
			m_OwnerData.TryGetComponent(edge, ref owner);
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			Entity oldEntity;
			bool flag3 = TryGetOldEntity(edge2, prefabRef.m_Prefab, editorContainer.m_Prefab, edge, ref ownerDefinition, ref owner.m_Owner, out oldEntity);
			if (flag3)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, oldEntity);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldEntity, default(Updated));
			}
			else
			{
				oldEntity = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netData.m_EdgeArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, oldEntity, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Edge>(jobIndex, oldEntity, edge2);
				if (!m_ServiceUpgradeData.HasComponent(edge))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Buildings.ServiceUpgrade>(jobIndex, oldEntity);
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, oldEntity, curve);
			if (flag)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Composition>(jobIndex, oldEntity, composition);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Net.BuildOrder>(jobIndex, oldEntity, m_BuildOrderData[edge]);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, m_PseudoRandomSeedData[edge]);
			}
			Elevation elevation = default(Elevation);
			if (m_ElevationData.TryGetComponent(edge, ref elevation))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, oldEntity, elevation);
			}
			Upgraded upgraded = default(Upgraded);
			if (m_UpgradedData.TryGetComponent(edge, ref upgraded))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Upgraded>(jobIndex, oldEntity, upgraded);
			}
			else if (flag3 && m_UpgradedData.HasComponent(oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Upgraded>(jobIndex, oldEntity);
			}
			DynamicBuffer<SubReplacement> val2 = default(DynamicBuffer<SubReplacement>);
			if (m_SubReplacements.TryGetBuffer(edge, ref val2))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<SubReplacement>(jobIndex, oldEntity).CopyFrom(val2);
			}
			else if (flag3 && m_SubReplacements.HasBuffer(oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<SubReplacement>(jobIndex, oldEntity);
			}
			bool isStandalone = true;
			if (owner.m_Owner != Entity.Null)
			{
				isStandalone = false;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, oldEntity, owner);
				Curve curve2 = default(Curve);
				PrefabRef prefabRef2 = default(PrefabRef);
				if (m_CurveData.TryGetComponent(owner.m_Owner, ref curve2) && m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, oldEntity, new OwnerDefinition
					{
						m_Prefab = prefabRef2.m_Prefab,
						m_Position = curve2.m_Bezier.a,
						m_Rotation = quaternion.op_Implicit(new float4(curve2.m_Bezier.d, 0f))
					});
				}
			}
			if (m_FixedData.HasComponent(edge))
			{
				Fixed obj = m_FixedData[edge];
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Fixed>(jobIndex, oldEntity, obj);
			}
			Aggregated aggregated = default(Aggregated);
			if (netGeometryData.m_AggregateType != Entity.Null && m_AggregatedData.TryGetComponent(edge, ref aggregated))
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Aggregated>(jobIndex, oldEntity, aggregated);
			}
			if (flag2 && m_ConditionData.HasComponent(edge))
			{
				NetCondition netCondition = m_ConditionData[edge];
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NetCondition>(jobIndex, oldEntity, netCondition);
			}
			if (!m_PrefabTrackData.HasComponent(prefabRef.m_Prefab))
			{
				if (m_TramTrackData.HasComponent(edge))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TramTrack>(jobIndex, oldEntity, default(TramTrack));
				}
				else if (flag3 && m_TramTrackData.HasComponent(oldEntity))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TramTrack>(jobIndex, oldEntity);
				}
			}
			bool isZoneable = false;
			if (m_PrefabRoadData.HasComponent(prefabRef.m_Prefab))
			{
				isZoneable = (m_PrefabRoadData[prefabRef.m_Prefab].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0;
				if (flag2 && m_RoadData.HasComponent(edge))
				{
					Road road = m_RoadData[edge];
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Road>(jobIndex, oldEntity, road);
				}
			}
			if (editorContainer.m_Prefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EditorContainer>(jobIndex, oldEntity, editorContainer);
				PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
				if (!flag && m_PseudoRandomSeedData.TryGetComponent(edge, ref pseudoRandomSeed))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, pseudoRandomSeed);
				}
			}
			if (m_NativeData.HasComponent(edge))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Native>(jobIndex, oldEntity, default(Native));
			}
			if (m_ElectricityConnectionData.HasComponent(edge))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.ElectricityConnection>(jobIndex, oldEntity, default(Game.Net.ElectricityConnection));
			}
			DynamicBuffer<ConnectedNode> val3 = m_ConnectedNodes[edge];
			DynamicBuffer<ConnectedNode> nodes = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ConnectedNode>(jobIndex, oldEntity);
			FindNodeConnections(nodes, edge2, curve, temp, netData, netGeometryData, isPermanent: false, isStandalone, isZoneable);
			Entity val4 = default(Entity);
			for (int i = 0; i < val3.Length; i++)
			{
				ConnectedNode connectedNode = val3[i];
				if (!m_NodeMap.TryGetFirstValue(new NodeMapKey(connectedNode.m_Node), ref val4, ref val))
				{
					nodes.Add(connectedNode);
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, oldEntity, temp);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(jobIndex, edge, default(Hidden));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, edge, default(BatchesUpdated));
		}

		private bool TryGetNodes(int jobIndex, Entity edge, Entity middleNode, Edge edgeData, out Entity start, out Entity end, out float3 curveRange)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			start = (end = Entity.Null);
			float2 val = default(float2);
			((float2)(ref val))._002Ector(float.MinValue, float.MaxValue);
			Temp temp = m_TempData[middleNode];
			curveRange = new float3(0f, temp.m_CurvePosition, 1f);
			Entity val2 = default(Entity);
			NativeParallelMultiHashMapIterator<NodeMapKey> val3 = default(NativeParallelMultiHashMapIterator<NodeMapKey>);
			if (m_NodeMap.TryGetFirstValue(new NodeMapKey(edge), ref val2, ref val3))
			{
				Temp temp2 = default(Temp);
				do
				{
					if (!(val2 != middleNode) || !m_TempData.TryGetComponent(val2, ref temp2))
					{
						continue;
					}
					float num = temp2.m_CurvePosition - temp.m_CurvePosition;
					if (num < 0f)
					{
						if (num > val.x)
						{
							start = val2;
							curveRange.x = temp2.m_CurvePosition;
							val.x = num;
						}
					}
					else if (num > 0f && num < val.y)
					{
						end = val2;
						curveRange.z = temp2.m_CurvePosition;
						val.y = num;
					}
				}
				while (m_NodeMap.TryGetNextValue(ref val2, ref val3));
			}
			NativeParallelMultiHashMapIterator<NodeMapKey> val4 = default(NativeParallelMultiHashMapIterator<NodeMapKey>);
			if (start == Entity.Null)
			{
				if (!m_NodeMap.TryGetFirstValue(new NodeMapKey(edgeData.m_Start), ref start, ref val4))
				{
					return false;
				}
			}
			else
			{
				start = Entity.Null;
				temp.m_Original = Entity.Null;
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, middleNode, temp);
			}
			if (end == Entity.Null && !m_NodeMap.TryGetFirstValue(new NodeMapKey(edgeData.m_End), ref end, ref val4))
			{
				return false;
			}
			return true;
		}

		private void SplitEdge(int jobIndex, Entity edge, Entity middleNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_066c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0849: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0859: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_076e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_0871: Unknown result type (might be due to invalid IL or missing references)
			//IL_087f: Unknown result type (might be due to invalid IL or missing references)
			//IL_088d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0834: Unknown result type (might be due to invalid IL or missing references)
			//IL_0802: Unknown result type (might be due to invalid IL or missing references)
			//IL_080e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_091b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0920: Unknown result type (might be due to invalid IL or missing references)
			//IL_0927: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_094d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0956: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08de: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0906: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_098c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0993: Unknown result type (might be due to invalid IL or missing references)
			//IL_0998: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			Edge edgeData = m_EdgeData[edge];
			Curve curve = m_CurveData[edge];
			Edge edge2 = default(Edge);
			Edge edge3 = default(Edge);
			edge2.m_End = middleNode;
			edge3.m_Start = middleNode;
			if (!TryGetNodes(jobIndex, edge, middleNode, edgeData, out edge2.m_Start, out edge3.m_End, out var curveRange) || edge2.m_Start == edge2.m_End || edge3.m_Start == edge3.m_End)
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[edge];
			NetData netData = m_NetData[prefabRef.m_Prefab];
			NetGeometryData netGeometryData = default(NetGeometryData);
			bool flag = m_NetGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData);
			Curve curveData = default(Curve);
			Curve curveData2 = default(Curve);
			Game.Net.BuildOrder buildOrderData = default(Game.Net.BuildOrder);
			Game.Net.BuildOrder buildOrderData2 = default(Game.Net.BuildOrder);
			if (flag)
			{
				Game.Net.BuildOrder buildOrder = m_BuildOrderData[edge];
				if (curveRange.x > 0f)
				{
					buildOrderData.m_Start = buildOrder.m_Start + (uint)((float)(buildOrder.m_End - buildOrder.m_Start) * curveRange.x) + 1;
				}
				else
				{
					buildOrderData.m_Start = buildOrder.m_Start;
				}
				buildOrderData.m_End = buildOrder.m_Start + (uint)((float)(buildOrder.m_End - buildOrder.m_Start) * curveRange.y);
				buildOrderData2.m_Start = buildOrderData.m_End + 1;
				if (curveRange.z < 1f)
				{
					buildOrderData2.m_End = buildOrder.m_Start + (uint)((float)(buildOrder.m_End - buildOrder.m_Start) * curveRange.z);
				}
				else
				{
					buildOrderData2.m_End = buildOrder.m_End;
				}
				if (buildOrderData.m_Start > buildOrderData.m_End)
				{
					buildOrderData.m_Start = buildOrderData.m_End;
				}
				if (buildOrderData2.m_Start > buildOrderData2.m_End)
				{
					buildOrderData2.m_Start = buildOrderData2.m_End;
				}
			}
			PrefabRef prefabRef2 = m_PrefabRefData[middleNode];
			NetData netData2 = m_NetData[prefabRef2.m_Prefab];
			float3 val = float3.op_Implicit(0f);
			bool3 val2 = bool3.op_Implicit(false);
			val2.y = ((netData.m_RequiredLayers ^ netData2.m_RequiredLayers) & Layer.Waterway) == 0;
			if (val2.y)
			{
				val.y = m_NodeData[middleNode].m_Position.y;
			}
			if (edge2.m_Start != Entity.Null && curveRange.x > 0f)
			{
				prefabRef2 = m_PrefabRefData[edge2.m_Start];
				netData2 = m_NetData[prefabRef2.m_Prefab];
				val2.x = ((netData.m_RequiredLayers ^ netData2.m_RequiredLayers) & Layer.Waterway) == 0;
				if (val2.x)
				{
					val.x = m_NodeData[edge2.m_Start].m_Position.y;
				}
			}
			if (edge3.m_End != Entity.Null && curveRange.z < 1f)
			{
				prefabRef2 = m_PrefabRefData[edge3.m_End];
				netData2 = m_NetData[prefabRef2.m_Prefab];
				val2.z = ((netData.m_RequiredLayers ^ netData2.m_RequiredLayers) & Layer.Waterway) == 0;
				if (val2.z)
				{
					val.z = m_NodeData[edge3.m_End].m_Position.y;
				}
			}
			Fixed fixedData = default(Fixed);
			if (!m_FixedData.TryGetComponent(edge, ref fixedData))
			{
				fixedData = new Fixed
				{
					m_Index = -1
				};
			}
			if ((netGeometryData.m_Flags & GeometryFlags.StraightEdges) != 0 && fixedData.m_Index < 0)
			{
				float3 val3 = MathUtils.Position(curve.m_Bezier, curveRange.y);
				val3.y = math.select(val3.y, val.y, val2.y);
				if (edge2.m_Start != Entity.Null)
				{
					if (curveRange.x > 0f)
					{
						float3 val4 = MathUtils.Position(curve.m_Bezier, curveRange.x);
						val4.y = math.select(val4.y, val.x, val2.x);
						curveData.m_Bezier = NetUtils.StraightCurve(val4, val3, netGeometryData.m_Hanging);
					}
					else
					{
						curveData.m_Bezier = NetUtils.StraightCurve(curve.m_Bezier.a, val3, netGeometryData.m_Hanging);
					}
				}
				if (edge3.m_End != Entity.Null)
				{
					if (curveRange.z < 1f)
					{
						float3 val5 = MathUtils.Position(curve.m_Bezier, curveRange.z);
						val5.y = math.select(val5.y, val.z, val2.z);
						curveData2.m_Bezier = NetUtils.StraightCurve(val3, val5, netGeometryData.m_Hanging);
					}
					else
					{
						curveData2.m_Bezier = NetUtils.StraightCurve(val3, curve.m_Bezier.d, netGeometryData.m_Hanging);
					}
				}
			}
			else
			{
				MathUtils.Divide(curve.m_Bezier, ref curveData.m_Bezier, ref curveData2.m_Bezier, curveRange.y);
				if (edge2.m_Start != Entity.Null && curveRange.x > 0f)
				{
					curveData.m_Bezier = MathUtils.Cut(curve.m_Bezier, ((float3)(ref curveRange)).xy);
					curveData.m_Bezier.a.y = math.select(curveData.m_Bezier.a.y, val.x, val2.x);
				}
				if (edge3.m_End != Entity.Null && curveRange.z < 1f)
				{
					curveData2.m_Bezier = MathUtils.Cut(curve.m_Bezier, ((float3)(ref curveRange)).yz);
					curveData2.m_Bezier.d.y = math.select(curveData2.m_Bezier.d.y, val.z, val2.z);
				}
				curveData.m_Bezier.d.y = math.select(curveData.m_Bezier.d.y, val.y, val2.y);
				curveData2.m_Bezier.a.y = math.select(curveData2.m_Bezier.a.y, val.y, val2.y);
			}
			curveData.m_Length = MathUtils.Length(curveData.m_Bezier);
			curveData2.m_Length = MathUtils.Length(curveData2.m_Bezier);
			DynamicBuffer<ConnectedNode> oldNodes = m_ConnectedNodes[edge];
			Elevation elevation = default(Elevation);
			m_ElevationData.TryGetComponent(edge, ref elevation);
			Upgraded upgraded = default(Upgraded);
			m_UpgradedData.TryGetComponent(edge, ref upgraded);
			DynamicBuffer<SubReplacement> subReplacements = default(DynamicBuffer<SubReplacement>);
			m_SubReplacements.TryGetBuffer(edge, ref subReplacements);
			Owner owner = default(Owner);
			m_OwnerData.TryGetComponent(edge, ref owner);
			Aggregated aggregated = default(Aggregated);
			if (netGeometryData.m_AggregateType != Entity.Null)
			{
				m_AggregatedData.TryGetComponent(edge, ref aggregated);
			}
			NetCondition condition = default(NetCondition);
			NetCondition condition2 = default(NetCondition);
			NetCondition netCondition = default(NetCondition);
			if (m_ConditionData.TryGetComponent(edge, ref netCondition))
			{
				if (curveRange.x > 0f)
				{
					condition.m_Wear.x = math.lerp(netCondition.m_Wear.x, netCondition.m_Wear.y, curveRange.x);
				}
				else
				{
					condition.m_Wear.x = netCondition.m_Wear.x;
				}
				condition.m_Wear.y = math.lerp(condition.m_Wear.x, condition.m_Wear.y, curveRange.y);
				condition2.m_Wear.x = condition.m_Wear.y;
				if (curveRange.z < 1f)
				{
					condition2.m_Wear.y = math.lerp(netCondition.m_Wear.x, netCondition.m_Wear.y, curveRange.z);
				}
				else
				{
					condition2.m_Wear.y = netCondition.m_Wear.y;
				}
			}
			bool addTramTrack = m_TramTrackData.HasComponent(edge) && !m_PrefabTrackData.HasComponent(prefabRef.m_Prefab);
			bool addNative = m_NativeData.HasComponent(edge);
			bool addElectricityConnection = m_ElectricityConnectionData.HasComponent(edge);
			bool serviceUpgrade = m_ServiceUpgradeData.HasComponent(edge);
			Road road = default(Road);
			Road road2 = default(Road);
			Road road3 = default(Road);
			if (m_RoadData.TryGetComponent(edge, ref road3))
			{
				road = road3;
				road2 = road3;
				if (curveRange.x > 0f)
				{
					road.m_TrafficFlowDistance0 = math.lerp(road3.m_TrafficFlowDistance0, road3.m_TrafficFlowDistance1, curveRange.x);
					road.m_TrafficFlowDuration0 = math.lerp(road3.m_TrafficFlowDuration0, road3.m_TrafficFlowDuration1, curveRange.x);
				}
				road.m_TrafficFlowDistance1 = math.lerp(road3.m_TrafficFlowDistance0, road3.m_TrafficFlowDistance1, curveRange.y);
				road.m_TrafficFlowDuration1 = math.lerp(road3.m_TrafficFlowDuration0, road3.m_TrafficFlowDuration1, curveRange.y);
				road2.m_TrafficFlowDistance0 = road.m_TrafficFlowDistance1;
				road2.m_TrafficFlowDuration0 = road.m_TrafficFlowDuration1;
				if (curveRange.z < 1f)
				{
					road2.m_TrafficFlowDistance1 = math.lerp(road3.m_TrafficFlowDistance0, road3.m_TrafficFlowDistance1, curveRange.z);
					road2.m_TrafficFlowDuration1 = math.lerp(road3.m_TrafficFlowDuration0, road3.m_TrafficFlowDuration1, curveRange.z);
				}
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			PseudoRandomSeed pseudoRandomSeed2 = default(PseudoRandomSeed);
			if (m_PseudoRandomSeedData.HasComponent(edge))
			{
				Random random = m_PseudoRandomSeedData[edge].GetRandom(PseudoRandomSeed.kSplitEdge);
				pseudoRandomSeed = new PseudoRandomSeed(ref random);
				pseudoRandomSeed2 = new PseudoRandomSeed(ref random);
			}
			EditorContainer editorContainer = default(EditorContainer);
			if (m_EditorContainerData.HasComponent(edge))
			{
				editorContainer = m_EditorContainerData[edge];
			}
			if ((netGeometryData.m_Flags & GeometryFlags.SnapCellSize) != 0)
			{
				if (curveRange.x > 0f)
				{
					Bezier4x3 val6 = default(Bezier4x3);
					Bezier4x3 val7 = default(Bezier4x3);
					MathUtils.Divide(curve.m_Bezier, ref val6, ref val7, curveRange.x);
					if (((int)math.round(MathUtils.Length(val6) / 4f) & 1) != 0 != ((road3.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0))
					{
						road.m_Flags |= Game.Net.RoadFlags.StartHalfAligned;
					}
					else
					{
						road.m_Flags &= ~Game.Net.RoadFlags.StartHalfAligned;
					}
				}
				if (((int)math.round(curveData.m_Length / 4f) & 1) != 0 != ((road.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0))
				{
					road.m_Flags |= Game.Net.RoadFlags.EndHalfAligned;
					road2.m_Flags |= Game.Net.RoadFlags.StartHalfAligned;
				}
				else
				{
					road.m_Flags &= ~Game.Net.RoadFlags.EndHalfAligned;
					road2.m_Flags &= ~Game.Net.RoadFlags.StartHalfAligned;
				}
				if (curveRange.z < 1f)
				{
					if (((int)math.round(curveData2.m_Length / 4f) & 1) != 0 != ((road2.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0))
					{
						road2.m_Flags |= Game.Net.RoadFlags.EndHalfAligned;
					}
					else
					{
						road2.m_Flags &= ~Game.Net.RoadFlags.EndHalfAligned;
					}
				}
			}
			if (edge2.m_Start != Entity.Null)
			{
				CreateTempEdge(jobIndex, edge2, curveData, elevation, upgraded, subReplacements, owner, fixedData, aggregated, condition, addTramTrack, addNative, addElectricityConnection, flag, serviceUpgrade, road, pseudoRandomSeed, prefabRef, netGeometryData, buildOrderData, editorContainer, oldNodes);
			}
			if (edge3.m_End != Entity.Null)
			{
				CreateTempEdge(jobIndex, edge3, curveData2, elevation, upgraded, subReplacements, owner, fixedData, aggregated, condition2, addTramTrack, addNative, addElectricityConnection, flag, serviceUpgrade, road2, pseudoRandomSeed2, prefabRef, netGeometryData, buildOrderData2, editorContainer, oldNodes);
			}
		}

		private void CreateTempEdge(int jobIndex, Edge edge, Curve curveData, Elevation elevation, Upgraded upgraded, DynamicBuffer<SubReplacement> subReplacements, Owner owner, Fixed fixedData, Aggregated aggregated, NetCondition condition, bool addTramTrack, bool addNative, bool addElectricityConnection, bool hasGeometry, bool serviceUpgrade, Road road, PseudoRandomSeed pseudoRandomSeed, PrefabRef prefabRef, NetGeometryData netGeometryData, Game.Net.BuildOrder buildOrderData, EditorContainer editorContainer, DynamicBuffer<ConnectedNode> oldNodes)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			bool flag = m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab);
			Composition composition = new Composition
			{
				m_Edge = prefabRef.m_Prefab,
				m_StartNode = prefabRef.m_Prefab,
				m_EndNode = prefabRef.m_Prefab
			};
			Temp temp = default(Temp);
			temp.m_Flags |= TempFlags.Essential;
			NetData netData = m_NetData[prefabRef.m_Prefab];
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			if (TryGetOldEntity(edge, prefabRef.m_Prefab, editorContainer.m_Prefab, Entity.Null, ref ownerDefinition, ref owner.m_Owner, out var oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, oldEntity);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldEntity, default(Updated));
			}
			else
			{
				oldEntity = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netData.m_EdgeArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, oldEntity, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Edge>(jobIndex, oldEntity, edge);
				if (!serviceUpgrade)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Buildings.ServiceUpgrade>(jobIndex, oldEntity);
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, oldEntity, curveData);
			if (hasGeometry)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Composition>(jobIndex, oldEntity, composition);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Net.BuildOrder>(jobIndex, oldEntity, buildOrderData);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, pseudoRandomSeed);
			}
			if (math.any(elevation.m_Elevation != 0f))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, oldEntity, elevation);
			}
			if (upgraded.m_Flags != default(CompositionFlags))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Upgraded>(jobIndex, oldEntity, upgraded);
			}
			if (subReplacements.IsCreated)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<SubReplacement>(jobIndex, oldEntity).CopyFrom(subReplacements);
			}
			bool flag2 = true;
			if (owner.m_Owner != Entity.Null)
			{
				flag2 = false;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, oldEntity, owner);
			}
			if (fixedData.m_Index >= 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Fixed>(jobIndex, oldEntity, fixedData);
			}
			if (aggregated.m_Aggregate != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Aggregated>(jobIndex, oldEntity, aggregated);
			}
			if (flag && math.any(condition.m_Wear != 0f))
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NetCondition>(jobIndex, oldEntity, condition);
			}
			if (addTramTrack)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TramTrack>(jobIndex, oldEntity, default(TramTrack));
			}
			if (addNative)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Native>(jobIndex, oldEntity, default(Native));
			}
			if (addElectricityConnection)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.ElectricityConnection>(jobIndex, oldEntity, default(Game.Net.ElectricityConnection));
			}
			if (editorContainer.m_Prefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EditorContainer>(jobIndex, oldEntity, editorContainer);
				if (!hasGeometry)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, pseudoRandomSeed);
				}
			}
			bool flag3 = false;
			if (m_PrefabRoadData.HasComponent(prefabRef.m_Prefab))
			{
				if (flag)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Road>(jobIndex, oldEntity, road);
				}
				flag3 = (m_PrefabRoadData[prefabRef.m_Prefab].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0;
			}
			DynamicBuffer<ConnectedNode> nodes = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ConnectedNode>(jobIndex, oldEntity);
			FindNodeConnections(nodes, edge, curveData, temp, netData, netGeometryData, isPermanent: false, flag2, flag3);
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<NodeMapKey> val2 = default(NativeParallelMultiHashMapIterator<NodeMapKey>);
			float num3 = default(float);
			for (int i = 0; i < oldNodes.Length; i++)
			{
				ConnectedNode connectedNode = oldNodes[i];
				if (m_NodeMap.TryGetFirstValue(new NodeMapKey(connectedNode.m_Node), ref val, ref val2))
				{
					continue;
				}
				Node node = m_NodeData[val];
				PrefabRef prefabRef2 = m_PrefabRefData[val];
				LocalConnectData localConnectData = m_LocalConnectData[prefabRef2.m_Prefab];
				NetGeometryData netGeometryData2 = m_NetGeometryData[prefabRef2.m_Prefab];
				float num = math.max(0f, netGeometryData2.m_DefaultWidth * 0.5f + localConnectData.m_SearchDistance);
				float num2 = MathUtils.Distance(((Bezier4x3)(ref curveData.m_Bezier)).xz, ((float3)(ref node.m_Position)).xz, ref num3);
				if (m_OwnerData.HasComponent(val) && localConnectData.m_SearchDistance != 0f && (netGeometryData2.m_Flags & GeometryFlags.SubOwner) == 0 && flag2 && flag3)
				{
					num2 -= 8f;
				}
				if (num2 <= netGeometryData.m_DefaultWidth * 0.5f + num)
				{
					float num4 = MathUtils.Position(curveData.m_Bezier, num3).y - node.m_Position.y;
					if (MathUtils.Intersect(localConnectData.m_HeightRange, num4))
					{
						nodes.Add(new ConnectedNode(val, num3));
					}
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, oldEntity, temp);
		}

		private bool TryGetNode(CoursePos coursePos, bool isPermanent, bool isEditor, out Entity node)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			if (isPermanent && m_NodeData.HasComponent(coursePos.m_Entity))
			{
				node = coursePos.m_Entity;
				return true;
			}
			float num = float.MaxValue;
			node = Entity.Null;
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<NodeMapKey> val2 = default(NativeParallelMultiHashMapIterator<NodeMapKey>);
			if (m_NodeMap.TryGetFirstValue(new NodeMapKey(coursePos, isPermanent, isEditor), ref val, ref val2))
			{
				Temp temp = default(Temp);
				do
				{
					if (m_TempData.TryGetComponent(val, ref temp))
					{
						float num2 = math.abs(temp.m_CurvePosition - coursePos.m_SplitPosition);
						if (num2 < num)
						{
							num = num2;
							node = val;
						}
					}
					else if (val != Entity.Null)
					{
						node = val;
						return true;
					}
				}
				while (m_NodeMap.TryGetNextValue(ref val, ref val2));
			}
			return node != Entity.Null;
		}

		private void GenerateEdge(int jobIndex, int entityIndex, CreationDefinition definitionData, OwnerDefinition ownerData, NetCourse course, Upgraded upgraded, LocalCurveCache cachedCurve, bool hasCachedCurve, DynamicBuffer<SubReplacement> subReplacements)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0904: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0988: Unknown result type (might be due to invalid IL or missing references)
			//IL_0934: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0998: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (definitionData.m_Flags & CreationFlags.Permanent) != 0;
			bool isEditor = definitionData.m_SubPrefab != Entity.Null;
			Edge edge = default(Edge);
			if (((course.m_StartPosition.m_Flags | course.m_EndPosition.m_Flags) & CoursePosFlags.DontCreate) != 0 || !TryGetNode(course.m_StartPosition, flag, isEditor, out edge.m_Start) || !TryGetNode(course.m_EndPosition, flag, isEditor, out edge.m_End) || edge.m_Start == edge.m_End)
			{
				return;
			}
			Entity val = default(Entity);
			NativeParallelMultiHashMapIterator<NodeMapKey> val2 = default(NativeParallelMultiHashMapIterator<NodeMapKey>);
			if (definitionData.m_Original != Entity.Null && m_NodeMap.TryGetFirstValue(new NodeMapKey(definitionData.m_Original), ref val, ref val2))
			{
				do
				{
					if (val != Entity.Null)
					{
						definitionData.m_Original = Entity.Null;
						break;
					}
				}
				while (m_NodeMap.TryGetNextValue(ref val, ref val2));
			}
			if (course.m_StartPosition.m_Entity != Entity.Null && course.m_EndPosition.m_Entity != Entity.Null && definitionData.m_Original == Entity.Null && ConnectionExists(course.m_StartPosition.m_Entity, course.m_EndPosition.m_Entity))
			{
				return;
			}
			PrefabRef prefabRef = default(PrefabRef);
			PrefabRef prefabRef2 = default(PrefabRef);
			if (definitionData.m_Original != Entity.Null)
			{
				prefabRef2 = m_PrefabRefData[definitionData.m_Original];
			}
			if (definitionData.m_Prefab == Entity.Null)
			{
				if (!(definitionData.m_Original != Entity.Null))
				{
					return;
				}
				prefabRef = prefabRef2;
			}
			else
			{
				prefabRef.m_Prefab = definitionData.m_Prefab;
			}
			bool flag2 = m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab);
			Composition composition = new Composition
			{
				m_Edge = prefabRef.m_Prefab,
				m_StartNode = prefabRef.m_Prefab,
				m_EndNode = prefabRef.m_Prefab
			};
			NetData netData = m_NetData[prefabRef.m_Prefab];
			NetGeometryData netGeometryData = default(NetGeometryData);
			bool flag3 = false;
			if (m_NetGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
				flag3 = true;
			}
			float3 val3;
			float3 val4;
			if ((netGeometryData.m_Flags & GeometryFlags.StrictNodes) != 0 || !flag3)
			{
				val3 = m_NodeData[edge.m_Start].m_Position;
				val4 = m_NodeData[edge.m_End].m_Position;
			}
			else
			{
				val3 = MathUtils.Position(course.m_Curve, course.m_StartPosition.m_CourseDelta);
				val4 = MathUtils.Position(course.m_Curve, course.m_EndPosition.m_CourseDelta);
				PrefabRef prefabRef3 = m_PrefabRefData[edge.m_Start];
				PrefabRef prefabRef4 = m_PrefabRefData[edge.m_End];
				NetData netData2 = m_NetData[prefabRef3.m_Prefab];
				NetData netData3 = m_NetData[prefabRef4.m_Prefab];
				if (((netData.m_RequiredLayers ^ netData2.m_RequiredLayers) & Layer.Waterway) == 0)
				{
					val3.y = m_NodeData[edge.m_Start].m_Position.y;
				}
				if (((netData.m_RequiredLayers ^ netData3.m_RequiredLayers) & Layer.Waterway) == 0)
				{
					val4.y = m_NodeData[edge.m_End].m_Position.y;
				}
			}
			Curve curve = default(Curve);
			if ((netGeometryData.m_Flags & GeometryFlags.StraightEdges) != 0 && course.m_FixedIndex < 0)
			{
				curve.m_Bezier = NetUtils.StraightCurve(val3, val4, netGeometryData.m_Hanging);
			}
			else
			{
				curve.m_Bezier = MathUtils.Cut(course.m_Curve, new float2(course.m_StartPosition.m_CourseDelta, course.m_EndPosition.m_CourseDelta));
				curve.m_Bezier.a = val3;
				curve.m_Bezier.d = val4;
			}
			curve.m_Length = MathUtils.Length(curve.m_Bezier);
			Temp temp = default(Temp);
			bool flag4 = false;
			if (definitionData.m_Original != Entity.Null)
			{
				temp.m_Original = definitionData.m_Original;
				if ((definitionData.m_Flags & CreationFlags.Delete) != 0)
				{
					temp.m_Flags |= TempFlags.Delete;
				}
				else if ((definitionData.m_Flags & CreationFlags.Select) != 0)
				{
					temp.m_Flags |= TempFlags.Select;
				}
				else if ((definitionData.m_Flags & CreationFlags.Upgrade) != 0)
				{
					temp.m_Flags |= TempFlags.Upgrade;
				}
				else if (prefabRef.m_Prefab != prefabRef2.m_Prefab || (definitionData.m_Flags & CreationFlags.Invert) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
				if ((definitionData.m_Flags & CreationFlags.Parent) != 0)
				{
					temp.m_Flags |= TempFlags.Parent;
				}
				if ((definitionData.m_Flags & CreationFlags.Upgrade) == 0)
				{
					Upgraded upgraded2 = default(Upgraded);
					if (m_UpgradedData.TryGetComponent(definitionData.m_Original, ref upgraded2))
					{
						upgraded = upgraded2;
						if ((definitionData.m_Flags & CreationFlags.Invert) != 0)
						{
							upgraded.m_Flags = NetCompositionHelpers.InvertCompositionFlags(upgraded.m_Flags);
						}
					}
					DynamicBuffer<SubReplacement> val5 = default(DynamicBuffer<SubReplacement>);
					if (m_SubReplacements.TryGetBuffer(definitionData.m_Original, ref val5))
					{
						subReplacements = val5;
						flag4 = (definitionData.m_Flags & CreationFlags.Invert) != 0;
					}
				}
			}
			else
			{
				temp.m_Flags |= TempFlags.Create;
			}
			if ((definitionData.m_Flags & CreationFlags.Hidden) != 0)
			{
				temp.m_Flags |= TempFlags.Hidden;
			}
			if (definitionData.m_Original != Entity.Null)
			{
				course.m_Elevation = float2.op_Implicit(0f);
				course.m_FixedIndex = -1;
				if (m_ElevationData.HasComponent(definitionData.m_Original))
				{
					course.m_Elevation = m_ElevationData[definitionData.m_Original].m_Elevation;
				}
				if (m_FixedData.HasComponent(definitionData.m_Original))
				{
					course.m_FixedIndex = m_FixedData[definitionData.m_Original].m_Index;
				}
				if ((definitionData.m_Flags & CreationFlags.Invert) != 0)
				{
					course.m_Elevation = ((float2)(ref course.m_Elevation)).yx;
				}
			}
			int num;
			if (!math.any(course.m_Elevation != 0f))
			{
				if (course.m_StartPosition.m_ParentMesh >= 0)
				{
					num = ((course.m_EndPosition.m_ParentMesh >= 0) ? 1 : 0);
					if (num != 0)
					{
						goto IL_0745;
					}
				}
				else
				{
					num = 0;
				}
				if ((netGeometryData.m_Flags & GeometryFlags.FlattenTerrain) == 0 || ownerData.m_Prefab != Entity.Null || definitionData.m_Owner != Entity.Null)
				{
					bool flag5 = m_ElevationData.HasComponent(edge.m_Start);
					bool flag6 = m_ElevationData.HasComponent(edge.m_End);
					Curve curve2 = (((netGeometryData.m_Flags & GeometryFlags.OnWater) == 0) ? NetUtils.AdjustPosition(curve, flag5, flag5 || flag6, flag6, ref m_TerrainHeightData) : NetUtils.AdjustPosition(curve, flag5, flag5 || flag6, flag6, ref m_TerrainHeightData, ref m_WaterSurfaceData));
					Bezier4x1 y = ((Bezier4x3)(ref curve2.m_Bezier)).y;
					float4 abcd = ((Bezier4x1)(ref y)).abcd;
					y = ((Bezier4x3)(ref curve.m_Bezier)).y;
					if (math.any(math.abs(abcd - ((Bezier4x1)(ref y)).abcd) >= 0.01f))
					{
						curve = curve2;
					}
				}
			}
			else
			{
				num = 1;
			}
			goto IL_0745;
			IL_082c:
			Entity oldEntity;
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, oldEntity, curve);
			if (flag3)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Composition>(jobIndex, oldEntity, composition);
				PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
				if (m_PseudoRandomSeedData.TryGetComponent(definitionData.m_Original, ref pseudoRandomSeed))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, pseudoRandomSeed);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, new PseudoRandomSeed((ushort)definitionData.m_RandomSeed));
				}
			}
			bool flag7;
			if (num != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, oldEntity, new Elevation(course.m_Elevation));
			}
			else if (flag7 && m_ElevationData.HasComponent(oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Elevation>(jobIndex, oldEntity);
			}
			if (upgraded.m_Flags != default(CompositionFlags))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Upgraded>(jobIndex, oldEntity, upgraded);
			}
			else if (flag7 && m_UpgradedData.HasComponent(oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Upgraded>(jobIndex, oldEntity);
			}
			if (subReplacements.IsCreated && subReplacements.Length != 0)
			{
				DynamicBuffer<SubReplacement> val6 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<SubReplacement>(jobIndex, oldEntity);
				for (int i = 0; i < subReplacements.Length; i++)
				{
					SubReplacement subReplacement = subReplacements[i];
					if (flag4)
					{
						subReplacement.m_Side = (SubReplacementSide)(0 - subReplacement.m_Side);
					}
					val6.Add(subReplacement);
				}
			}
			else if (flag7 && m_SubReplacements.HasBuffer(oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<SubReplacement>(jobIndex, oldEntity);
			}
			if (m_PrefabRoadData.HasComponent(prefabRef.m_Prefab))
			{
				if (((upgraded.m_Flags.m_Left | upgraded.m_Flags.m_Right) & CompositionFlags.Side.PrimaryTrack) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TramTrack>(jobIndex, oldEntity, default(TramTrack));
				}
				else if (flag7 && m_TramTrackData.HasComponent(oldEntity))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TramTrack>(jobIndex, oldEntity);
				}
			}
			if (m_NativeData.HasComponent(definitionData.m_Original) && (temp.m_Flags & (TempFlags.Modify | TempFlags.Upgrade)) == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Native>(jobIndex, oldEntity, default(Native));
			}
			else if (flag7 && m_NativeData.HasComponent(oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Native>(jobIndex, oldEntity);
			}
			bool flag8 = true;
			if (ownerData.m_Prefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, oldEntity, default(Owner));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, oldEntity, ownerData);
				flag8 = false;
			}
			else if (definitionData.m_Owner != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, oldEntity, new Owner(definitionData.m_Owner));
				flag8 = false;
			}
			flag8 |= (netGeometryData.m_Flags & GeometryFlags.SubOwner) != 0;
			if ((definitionData.m_Flags & CreationFlags.SubElevation) != 0)
			{
				temp.m_Flags |= TempFlags.Essential;
			}
			if (course.m_FixedIndex >= 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Fixed>(jobIndex, oldEntity, new Fixed
				{
					m_Index = course.m_FixedIndex
				});
			}
			else if (flag7 && m_FixedData.HasComponent(oldEntity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Fixed>(jobIndex, oldEntity);
			}
			if (definitionData.m_Original != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(jobIndex, definitionData.m_Original, default(Hidden));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, definitionData.m_Original, default(BatchesUpdated));
				if (flag3)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Net.BuildOrder>(jobIndex, oldEntity, m_BuildOrderData[definitionData.m_Original]);
				}
			}
			else if (flag3)
			{
				Game.Net.BuildOrder buildOrder = default(Game.Net.BuildOrder);
				buildOrder.m_Start = m_BuildOrder + (uint)(entityIndex * 16);
				buildOrder.m_End = buildOrder.m_Start + 15;
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Net.BuildOrder>(jobIndex, oldEntity, buildOrder);
			}
			bool flag9 = false;
			if (m_PrefabRoadData.HasComponent(prefabRef.m_Prefab))
			{
				flag9 = (m_PrefabRoadData[prefabRef.m_Prefab].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0;
				if (flag2)
				{
					Road road = default(Road);
					if (m_RoadData.HasComponent(definitionData.m_Original))
					{
						road = m_RoadData[definitionData.m_Original];
						CheckRoadAlignment(definitionData, prefabRef, prefabRef2, netGeometryData, ref road);
					}
					SetRoadAlignment(course, ref road);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Road>(jobIndex, oldEntity, road);
				}
			}
			DynamicBuffer<ConnectedNode> nodes = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<ConnectedNode>(jobIndex, oldEntity);
			FindNodeConnections(nodes, edge, curve, temp, netData, netGeometryData, isPermanent: false, flag8, flag9);
			if (m_PrefabElectricityConnectionData.HasComponent(prefabRef.m_Prefab))
			{
				bool flag10 = NetCompositionHelpers.TestEdgeFlags(m_PrefabElectricityConnectionData[prefabRef.m_Prefab], upgraded.m_Flags);
				if (flag10)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.ElectricityConnection>(jobIndex, oldEntity, default(Game.Net.ElectricityConnection));
				}
				else if (flag7 && m_ElectricityConnectionData.HasComponent(oldEntity))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Net.ElectricityConnection>(jobIndex, oldEntity);
				}
				if (definitionData.m_Original != Entity.Null)
				{
					bool flag11 = m_ElectricityConnectionData.HasComponent(definitionData.m_Original);
					if (flag10 != flag11)
					{
						temp.m_Flags &= ~TempFlags.Upgrade;
						temp.m_Flags |= TempFlags.Replace;
					}
				}
			}
			if (definitionData.m_Original != Entity.Null && prefabRef2.m_Prefab != prefabRef.m_Prefab)
			{
				bool flag12 = false;
				if (m_PrefabRoadData.HasComponent(prefabRef2.m_Prefab))
				{
					flag12 = (m_PrefabRoadData[prefabRef2.m_Prefab].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0;
				}
				if (flag9 != flag12)
				{
					temp.m_Flags &= ~TempFlags.Modify;
					temp.m_Flags |= TempFlags.Replace;
				}
			}
			Aggregated aggregated = default(Aggregated);
			if (netGeometryData.m_AggregateType != Entity.Null && m_AggregatedData.TryGetComponent(definitionData.m_Original, ref aggregated))
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Aggregated>(jobIndex, oldEntity, aggregated);
			}
			NetCondition netCondition = default(NetCondition);
			if (flag2 && m_ConditionData.TryGetComponent(definitionData.m_Original, ref netCondition))
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NetCondition>(jobIndex, oldEntity, netCondition);
			}
			if (hasCachedCurve)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(jobIndex, oldEntity, cachedCurve);
			}
			if (definitionData.m_SubPrefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EditorContainer>(jobIndex, oldEntity, new EditorContainer
				{
					m_Prefab = definitionData.m_SubPrefab
				});
				if (!flag3)
				{
					PseudoRandomSeed pseudoRandomSeed2 = default(PseudoRandomSeed);
					if (m_PseudoRandomSeedData.TryGetComponent(definitionData.m_Original, ref pseudoRandomSeed2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, pseudoRandomSeed2);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, oldEntity, new PseudoRandomSeed((ushort)definitionData.m_RandomSeed));
					}
				}
			}
			if (!flag)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, oldEntity, temp);
			}
			return;
			IL_081e:
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Buildings.ServiceUpgrade>(jobIndex, oldEntity);
			goto IL_082c;
			IL_0745:
			oldEntity = Entity.Null;
			flag7 = !flag && TryGetOldEntity(edge, prefabRef.m_Prefab, definitionData.m_SubPrefab, definitionData.m_Original, ref ownerData, ref definitionData.m_Owner, out oldEntity);
			if (flag7)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, oldEntity);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, oldEntity, default(Updated));
			}
			else
			{
				oldEntity = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netData.m_EdgeArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, oldEntity, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Edge>(jobIndex, oldEntity, edge);
				bool num2;
				if (!(definitionData.m_Original != Entity.Null))
				{
					if (m_EditorMode)
					{
						goto IL_081e;
					}
					num2 = (definitionData.m_Flags & CreationFlags.SubElevation) == 0;
				}
				else
				{
					num2 = !m_ServiceUpgradeData.HasComponent(definitionData.m_Original);
				}
				if (num2)
				{
					goto IL_081e;
				}
			}
			goto IL_082c;
		}

		private bool TryGetOldEntity(Edge edge, Entity prefab, Entity subPrefab, Entity original, ref OwnerDefinition ownerDefinition, ref Entity owner, out Entity oldEntity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			if (ownerDefinition.m_Prefab != Entity.Null && m_ReusedOwnerMap.TryGetValue(ownerDefinition, ref val))
			{
				owner = val;
				ownerDefinition = default(OwnerDefinition);
			}
			OldEdgeKey oldEdgeKey = default(OldEdgeKey);
			oldEdgeKey.m_Prefab = prefab;
			oldEdgeKey.m_SubPrefab = subPrefab;
			oldEdgeKey.m_Original = original;
			oldEdgeKey.m_Owner = owner;
			oldEdgeKey.m_StartNode = edge.m_Start;
			oldEdgeKey.m_EndNode = edge.m_End;
			if (m_OldEdgeMap.TryGetValue(oldEdgeKey, ref oldEntity))
			{
				return true;
			}
			oldEntity = Entity.Null;
			return false;
		}

		private void CheckRoadAlignment(CreationDefinition definitionData, PrefabRef prefabRefData, PrefabRef originalPrefabRef, NetGeometryData netGeometryData, ref Road road)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			bool2 val = default(bool2);
			((bool2)(ref val))._002Ector((road.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0, (road.m_Flags & Game.Net.RoadFlags.EndHalfAligned) != 0);
			road.m_Flags &= ~(Game.Net.RoadFlags.StartHalfAligned | Game.Net.RoadFlags.EndHalfAligned);
			if ((definitionData.m_Flags & CreationFlags.Align) != 0)
			{
				val = (((definitionData.m_Flags & CreationFlags.Invert) != 0) ? ((bool2)(ref val)).yx : val);
				if (prefabRefData.m_Prefab != originalPrefabRef.m_Prefab)
				{
					NetGeometryData netGeometryData2 = m_NetGeometryData[originalPrefabRef.m_Prefab];
					int cellWidth = ZoneUtils.GetCellWidth(netGeometryData.m_DefaultWidth);
					int cellWidth2 = ZoneUtils.GetCellWidth(netGeometryData2.m_DefaultWidth);
					val ^= ((cellWidth ^ cellWidth2) & 1) != 0;
				}
				if (val.x)
				{
					road.m_Flags |= Game.Net.RoadFlags.StartHalfAligned;
				}
				if (val.y)
				{
					road.m_Flags |= Game.Net.RoadFlags.EndHalfAligned;
				}
			}
		}

		private void SetRoadAlignment(NetCourse course, ref Road road)
		{
			if ((course.m_StartPosition.m_Flags & CoursePosFlags.HalfAlign) != 0)
			{
				road.m_Flags ^= Game.Net.RoadFlags.StartHalfAligned;
			}
			if ((course.m_EndPosition.m_Flags & CoursePosFlags.HalfAlign) != 0)
			{
				road.m_Flags ^= Game.Net.RoadFlags.EndHalfAligned;
			}
		}

		private bool ConnectionExists(Entity node1, Entity node2)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (m_ConnectedEdges.TryGetBuffer(node1, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					if (!m_DeletedData.HasComponent(edge))
					{
						Edge edge2 = m_EdgeData[edge];
						if ((edge2.m_Start == node2 && edge2.m_End == node1) || (edge2.m_End == node2 && edge2.m_Start == node1))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void FindNodeConnections(DynamicBuffer<ConnectedNode> nodes, Edge edgeData, Curve curveData, Temp tempData, NetData netData, NetGeometryData netGeometryData, bool isPermanent, bool isStandalone, bool isZoneable)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			float num = netGeometryData.m_DefaultWidth * 0.5f;
			num += math.select(0f, 8f, isStandalone && isZoneable);
			Bounds3 val = MathUtils.Expand(MathUtils.Bounds(curveData.m_Bezier), float3.op_Implicit(num));
			float3 val2 = default(float3);
			Roundabout roundabout = default(Roundabout);
			if (m_RoundaboutData.TryGetComponent(edgeData.m_Start, ref roundabout))
			{
				val2 = m_NodeData[edgeData.m_Start].m_Position;
				val |= new Bounds3(val2 - roundabout.m_Radius, val2 + roundabout.m_Radius);
			}
			float3 val3 = default(float3);
			Roundabout roundabout2 = default(Roundabout);
			if (m_RoundaboutData.TryGetComponent(edgeData.m_End, ref roundabout2))
			{
				val3 = m_NodeData[edgeData.m_End].m_Position;
				val |= new Bounds3(val3 - roundabout2.m_Radius, val3 + roundabout2.m_Radius);
			}
			Bounds3 val4 = default(Bounds3);
			float num5 = default(float);
			for (int i = 0; i < m_LocalConnectList.Length; i++)
			{
				LocalConnectItem localConnectItem = m_LocalConnectList[i];
				if ((localConnectItem.m_ConnectLayers & netData.m_ConnectLayers) == 0 || (localConnectItem.m_LocalConnectLayers & netData.m_LocalConnectLayers) == 0 || localConnectItem.m_Node == edgeData.m_Start || localConnectItem.m_Node == edgeData.m_End || ((tempData.m_Flags ^ localConnectItem.m_TempFlags) & TempFlags.Delete) != 0 || localConnectItem.m_IsPermanent != isPermanent)
				{
					continue;
				}
				((Bounds3)(ref val4))._002Ector(localConnectItem.m_Position - localConnectItem.m_Radius, localConnectItem.m_Position + localConnectItem.m_Radius);
				((Bounds3)(ref val4)).y = localConnectItem.m_Position.y + localConnectItem.m_HeightRange;
				if (!MathUtils.Intersect(val, val4))
				{
					continue;
				}
				float num4;
				if ((netGeometryData.m_Flags & GeometryFlags.NoEdgeConnection) != 0)
				{
					float num2 = math.distance(((float3)(ref curveData.m_Bezier.a)).xz, ((float3)(ref localConnectItem.m_Position)).xz);
					float num3 = math.distance(((float3)(ref curveData.m_Bezier.d)).xz, ((float3)(ref localConnectItem.m_Position)).xz);
					num4 = math.select(num2, num3, num3 < num2);
					num5 = math.select(0f, 1f, num3 < num2);
				}
				else
				{
					num4 = MathUtils.Distance(((Bezier4x3)(ref curveData.m_Bezier)).xz, ((float3)(ref localConnectItem.m_Position)).xz, ref num5);
				}
				num4 -= netGeometryData.m_DefaultWidth * 0.5f;
				if (roundabout.m_Radius != 0f)
				{
					float num6 = math.distance(((float3)(ref val2)).xz, ((float3)(ref localConnectItem.m_Position)).xz) - roundabout.m_Radius;
					if (num6 < num4)
					{
						num4 = num6;
						num5 = 0f;
					}
				}
				if (roundabout2.m_Radius != 0f)
				{
					float num7 = math.distance(((float3)(ref val3)).xz, ((float3)(ref localConnectItem.m_Position)).xz) - roundabout2.m_Radius;
					if (num7 < num4)
					{
						num4 = num7;
						num5 = 1f;
					}
				}
				if (!localConnectItem.m_IsStandalone && isStandalone && isZoneable)
				{
					num4 -= 8f;
				}
				if (num4 <= localConnectItem.m_Radius)
				{
					float num8 = MathUtils.Position(curveData.m_Bezier, num5).y - localConnectItem.m_Position.y;
					if (MathUtils.Intersect(localConnectItem.m_HeightRange, num8))
					{
						nodes.Add(new ConnectedNode(localConnectItem.m_Node, num5));
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateBuildOrderJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.BuildOrder> m_BuildOrderType;

		public NativeValue<uint> m_BuildOrder;

		public void Execute()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			uint num = m_BuildOrder.value;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Game.Net.BuildOrder> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.BuildOrder>(ref m_BuildOrderType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Game.Net.BuildOrder buildOrder = nativeArray[j];
					num = math.max(num, math.max(buildOrder.m_Start, buildOrder.m_End) + 1);
				}
			}
			m_BuildOrder.value = num;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LocalConnect> __Game_Net_LocalConnect_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Elevation> __Game_Net_Elevation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Node> __Game_Net_Node_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> __Game_Prefabs_LocalConnectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

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
		public BufferTypeHandle<SubReplacement> __Game_Net_SubReplacement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.BuildOrder> __Game_Net_BuildOrder_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TramTrack> __Game_Net_TramTrack_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCondition> __Game_Net_NetCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aggregated> __Game_Net_Aggregated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Roundabout> __Game_Net_Roundabout_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ElectricityConnection> __Game_Net_ElectricityConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackData> __Game_Prefabs_TrackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConnectionData> __Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubReplacement> __Game_Net_SubReplacement_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.BuildOrder> __Game_Net_BuildOrder_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Tools_EditorContainer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EditorContainer>(true);
			__Game_Net_LocalConnect_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LocalConnect>(true);
			__Game_Net_Elevation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Elevation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_Node_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(false);
			__Game_Prefabs_LocalConnectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnectData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Tools_CreationDefinition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(false);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OwnerDefinition>(true);
			__Game_Tools_NetCourse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCourse>(true);
			__Game_Tools_LocalCurveCache_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LocalCurveCache>(true);
			__Game_Net_Upgraded_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Upgraded>(true);
			__Game_Net_SubReplacement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubReplacement>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_BuildOrder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.BuildOrder>(true);
			__Game_Net_TramTrack_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TramTrack>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Net_NetCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCondition>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Net_Aggregated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aggregated>(true);
			__Game_Net_Roundabout_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Roundabout>(true);
			__Game_Net_ElectricityConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ElectricityConnection>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TrackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackData>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConnectionData>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubReplacement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubReplacement>(true);
			__Game_Net_BuildOrder_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.BuildOrder>(true);
		}
	}

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private GenerateObjectsSystem m_GenerateObjectsSystem;

	private ToolSystem m_ToolSystem;

	private ModificationBarrier2 m_TempEdgesBarrier;

	private NativeValue<uint> m_BuildOrder;

	private EntityQuery m_CreatedEdgesQuery;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_DeletedQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BuildOrder = new NativeValue<uint>((Allocator)4);
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_GenerateObjectsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GenerateObjectsSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_TempEdgesBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2>();
		m_CreatedEdgesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Game.Net.BuildOrder>(),
			ComponentType.Exclude<Temp>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<NetCourse>(),
			ComponentType.ReadOnly<Updated>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Edge>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[1] = val;
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_DeletedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<PrefabRef>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_CreatedEdgesQuery, m_DefinitionQuery });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_BuildOrder.Dispose();
		base.OnDestroy();
	}

	public NativeValue<uint> GetBuildOrder()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_BuildOrder;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_0912: Unknown result type (might be due to invalid IL or missing references)
		//IL_091f: Unknown result type (might be due to invalid IL or missing references)
		//IL_092c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_099d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = ((SystemBase)this).Dependency;
		if (!((EntityQuery)(ref m_DefinitionQuery)).IsEmptyIgnoreFilter)
		{
			NativeQueue<LocalConnectItem> localConnectQueue = default(NativeQueue<LocalConnectItem>);
			localConnectQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<LocalConnectItem> localConnectList = default(NativeList<LocalConnectItem>);
			localConnectList._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeParallelMultiHashMap<NodeMapKey, Entity> nodeMap = default(NativeParallelMultiHashMap<NodeMapKey, Entity>);
			nodeMap._002Ector(((EntityQuery)(ref m_DefinitionQuery)).CalculateEntityCount(), AllocatorHandle.op_Implicit((Allocator)3));
			NativeHashMap<OldEdgeKey, Entity> oldEdgeMap = default(NativeHashMap<OldEdgeKey, Entity>);
			oldEdgeMap._002Ector(32, AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle dependencies;
			NativeHashMap<OwnerDefinition, Entity> reusedOwnerMap = m_GenerateObjectsSystem.GetReusedOwnerMap(out dependencies);
			TerrainHeightData heightData = m_TerrainSystem.GetHeightData();
			JobHandle deps;
			WaterSurfaceData surfaceData = m_WaterSystem.GetSurfaceData(out deps);
			CheckNodesJob checkNodesJob = new CheckNodesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LocalConnectType = InternalCompilerInterface.GetComponentTypeHandle<LocalConnect>(ref __TypeHandle.__Game_Net_LocalConnect_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationType = InternalCompilerInterface.GetComponentTypeHandle<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainHeightData = heightData,
				m_NodeMap = nodeMap.AsParallelWriter(),
				m_LocalConnectQueue = localConnectQueue.AsParallelWriter()
			};
			FillOldEdgesJob fillOldEdgesJob = new FillOldEdgesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OldEdgeMap = oldEdgeMap
			};
			CheckDefinitionsJob checkDefinitionsJob = new CheckDefinitionsJob
			{
				m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeMap = nodeMap.AsParallelWriter()
			};
			CollectLocalConnectItemsJob obj = new CollectLocalConnectItemsJob
			{
				m_LocalConnectQueue = localConnectQueue,
				m_LocalConnectList = localConnectList
			};
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<CheckNodesJob>(checkNodesJob, m_DefinitionQuery, ((SystemBase)this).Dependency);
			JobHandle val3 = JobChunkExtensions.Schedule<FillOldEdgesJob>(fillOldEdgesJob, m_DeletedQuery, ((SystemBase)this).Dependency);
			JobHandle val4 = JobChunkExtensions.ScheduleParallel<CheckDefinitionsJob>(checkDefinitionsJob, m_DefinitionQuery, val2);
			JobHandle val5 = IJobExtensions.Schedule<CollectLocalConnectItemsJob>(obj, val2);
			JobHandle val6 = default(JobHandle);
			NativeArray<int> chunkBaseEntityIndices = ((EntityQuery)(ref m_DefinitionQuery)).CalculateBaseEntityIndexArrayAsync(AllocatorHandle.op_Implicit((Allocator)3), JobHandle.CombineDependencies(val4, val5, deps), ref val6);
			GenerateEdgesJob generateEdgesJob = new GenerateEdgesJob
			{
				m_ChunkBaseEntityIndices = chunkBaseEntityIndices,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<OwnerDefinition>(ref __TypeHandle.__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NetCourseType = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LocalCurveCacheType = InternalCompilerInterface.GetComponentTypeHandle<LocalCurveCache>(ref __TypeHandle.__Game_Tools_LocalCurveCache_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpgradedType = InternalCompilerInterface.GetComponentTypeHandle<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubReplacementType = InternalCompilerInterface.GetBufferTypeHandle<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildOrderData = InternalCompilerInterface.GetComponentLookup<Game.Net.BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TramTrackData = InternalCompilerInterface.GetComponentLookup<TramTrack>(ref __TypeHandle.__Game_Net_TramTrack_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConditionData = InternalCompilerInterface.GetComponentLookup<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AggregatedData = InternalCompilerInterface.GetComponentLookup<Aggregated>(ref __TypeHandle.__Game_Net_Aggregated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RoundaboutData = InternalCompilerInterface.GetComponentLookup<Roundabout>(ref __TypeHandle.__Game_Net_Roundabout_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.ElectricityConnection>(ref __TypeHandle.__Game_Net_ElectricityConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTrackData = InternalCompilerInterface.GetComponentLookup<TrackData>(ref __TypeHandle.__Game_Prefabs_TrackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabElectricityConnectionData = InternalCompilerInterface.GetComponentLookup<ElectricityConnectionData>(ref __TypeHandle.__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubReplacements = InternalCompilerInterface.GetBufferLookup<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_BuildOrder = m_BuildOrder.value,
				m_NodeMap = nodeMap,
				m_ReusedOwnerMap = reusedOwnerMap,
				m_OldEdgeMap = oldEdgeMap,
				m_LocalConnectList = localConnectList.AsDeferredJobArray(),
				m_TerrainHeightData = heightData,
				m_WaterSurfaceData = surfaceData
			};
			EntityCommandBuffer val7 = m_TempEdgesBarrier.CreateCommandBuffer();
			generateEdgesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val7)).AsParallelWriter();
			JobHandle val8 = JobChunkExtensions.ScheduleParallel<GenerateEdgesJob>(generateEdgesJob, m_DefinitionQuery, JobHandle.CombineDependencies(val6, val3, dependencies));
			localConnectQueue.Dispose(val5);
			localConnectList.Dispose(val8);
			nodeMap.Dispose(val8);
			oldEdgeMap.Dispose(val8);
			m_TerrainSystem.AddCPUHeightReader(val8);
			m_WaterSystem.AddSurfaceReader(val8);
			m_GenerateObjectsSystem.AddOwnerMapReader(val8);
			((EntityCommandBufferSystem)m_TempEdgesBarrier).AddJobHandleForProducer(val8);
			val = val8;
		}
		if (!((EntityQuery)(ref m_CreatedEdgesQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val9 = default(JobHandle);
			NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_CreatedEdgesQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val9);
			JobHandle val10 = IJobExtensions.Schedule<UpdateBuildOrderJob>(new UpdateBuildOrderJob
			{
				m_Chunks = chunks,
				m_BuildOrderType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildOrder = m_BuildOrder
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val9));
			chunks.Dispose(val10);
			val = JobHandle.CombineDependencies(val, val10);
		}
		((SystemBase)this).Dependency = val;
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
	public GenerateEdgesSystem()
	{
	}
}

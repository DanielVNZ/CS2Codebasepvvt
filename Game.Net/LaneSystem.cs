using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class LaneSystem : GameSystemBase
{
	private struct LaneKey : IEquatable<LaneKey>
	{
		private Lane m_Lane;

		private Entity m_Prefab;

		private LaneFlags m_Flags;

		public LaneKey(Lane lane, Entity prefab, LaneFlags flags)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
			m_Prefab = prefab;
			m_Flags = flags & (LaneFlags.Slave | LaneFlags.Master);
		}

		public void ReplaceOwner(Entity oldOwner, Entity newOwner)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			m_Lane.m_StartNode.ReplaceOwner(oldOwner, newOwner);
			m_Lane.m_MiddleNode.ReplaceOwner(oldOwner, newOwner);
			m_Lane.m_EndNode.ReplaceOwner(oldOwner, newOwner);
		}

		public bool Equals(LaneKey other)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_Lane.Equals(other.m_Lane) && ((Entity)(ref m_Prefab)).Equals(other.m_Prefab))
			{
				return m_Flags == other.m_Flags;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_Lane.GetHashCode();
		}
	}

	private struct ConnectionKey : IEquatable<ConnectionKey>
	{
		private int4 m_Data;

		public ConnectionKey(ConnectPosition sourcePosition, ConnectPosition targetPosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			m_Data.x = sourcePosition.m_Owner.Index;
			m_Data.y = sourcePosition.m_LaneData.m_Index;
			m_Data.z = targetPosition.m_Owner.Index;
			m_Data.w = targetPosition.m_LaneData.m_Index;
		}

		public bool Equals(ConnectionKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ((int4)(ref m_Data)).Equals(other.m_Data);
		}

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<int4, int4>(ref m_Data)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct ConnectPosition
	{
		public NetCompositionLane m_LaneData;

		public Entity m_Owner;

		public Entity m_NodeComposition;

		public Entity m_EdgeComposition;

		public float3 m_Position;

		public float3 m_Tangent;

		public float m_Order;

		public CompositionData m_CompositionData;

		public float m_CurvePosition;

		public float m_BaseHeight;

		public float m_Elevation;

		public ushort m_GroupIndex;

		public byte m_SegmentIndex;

		public byte m_UnsafeCount;

		public byte m_ForbiddenCount;

		public byte m_SkippedCount;

		public RoadTypes m_RoadTypes;

		public TrackTypes m_TrackTypes;

		public UtilityTypes m_UtilityTypes;

		public bool m_IsEnd;

		public bool m_IsSideConnection;
	}

	private struct EdgeTarget
	{
		public Entity m_Edge;

		public Entity m_StartNode;

		public Entity m_EndNode;

		public float3 m_StartPos;

		public float3 m_StartTangent;

		public float3 m_EndPos;

		public float3 m_EndTangent;
	}

	private struct MiddleConnection
	{
		public ConnectPosition m_ConnectPosition;

		public Entity m_SourceEdge;

		public Entity m_SourceNode;

		public Curve m_TargetCurve;

		public float m_TargetCurvePos;

		public float m_Distance;

		public CompositionData m_TargetComposition;

		public Entity m_TargetLane;

		public Entity m_TargetOwner;

		public LaneFlags m_TargetFlags;

		public uint m_TargetGroup;

		public int m_SortIndex;

		public PathNode m_TargetNode;

		public ushort m_TargetCarriageway;

		public bool m_IsSource;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct SourcePositionComparer : IComparer<ConnectPosition>
	{
		public int Compare(ConnectPosition x, ConnectPosition y)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			int num = x.m_Owner.Index - y.m_Owner.Index;
			return math.select((int)math.sign(x.m_Order - y.m_Order), num, num != 0);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TargetPositionComparer : IComparer<ConnectPosition>
	{
		public int Compare(ConnectPosition x, ConnectPosition y)
		{
			return (int)math.sign(x.m_Order - y.m_Order);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct MiddleConnectionComparer : IComparer<MiddleConnection>
	{
		public int Compare(MiddleConnection x, MiddleConnection y)
		{
			return math.select(x.m_SortIndex - y.m_SortIndex, (int)(x.m_ConnectPosition.m_UtilityTypes - y.m_ConnectPosition.m_UtilityTypes), x.m_ConnectPosition.m_UtilityTypes != y.m_ConnectPosition.m_UtilityTypes);
		}
	}

	private struct LaneAnchor : IComparable<LaneAnchor>
	{
		public Entity m_Prefab;

		public float m_Order;

		public float3 m_Position;

		public PathNode m_PathNode;

		public int CompareTo(LaneAnchor other)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			return math.select(math.select(0, math.select(1, -1, m_Order < other.m_Order), m_Order != other.m_Order), m_Prefab.Index - other.m_Prefab.Index, m_Prefab.Index != other.m_Prefab.Index);
		}
	}

	private struct LaneBuffer
	{
		public NativeHashMap<LaneKey, Entity> m_OldLanes;

		public NativeHashMap<LaneKey, Entity> m_OriginalLanes;

		public NativeHashMap<Entity, Random> m_SelectedSpawnables;

		public NativeList<Entity> m_Updates;

		public LaneBuffer(Allocator allocator)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			m_OldLanes = new NativeHashMap<LaneKey, Entity>(32, AllocatorHandle.op_Implicit(allocator));
			m_OriginalLanes = new NativeHashMap<LaneKey, Entity>(32, AllocatorHandle.op_Implicit(allocator));
			m_SelectedSpawnables = new NativeHashMap<Entity, Random>(10, AllocatorHandle.op_Implicit(allocator));
			m_Updates = new NativeList<Entity>(32, AllocatorHandle.op_Implicit(allocator));
		}

		public void Clear()
		{
			m_OldLanes.Clear();
			m_OriginalLanes.Clear();
			m_SelectedSpawnables.Clear();
		}

		public void Dispose()
		{
			m_OldLanes.Dispose();
			m_OriginalLanes.Dispose();
			m_SelectedSpawnables.Dispose();
			m_Updates.Dispose();
		}
	}

	private struct CompositionData
	{
		public float m_SpeedLimit;

		public float m_Priority;

		public TaxiwayFlags m_TaxiwayFlags;

		public Game.Prefabs.RoadFlags m_RoadFlags;
	}

	[BurstCompile]
	private struct UpdateLanesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> m_NodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> m_OrphanType;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.EditorContainer> m_EditorContainerType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> m_ElevationType;

		[ReadOnly]
		public ComponentTypeHandle<UnderConstruction> m_UnderConstructionType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedNode> m_ConnectedNodeType;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[ReadOnly]
		public ComponentLookup<BuildOrder> m_BuildOrderData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Overridden> m_OverriddenData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Clear> m_AreaClearData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<RoadComposition> m_RoadData;

		[ReadOnly]
		public ComponentLookup<TrackComposition> m_TrackData;

		[ReadOnly]
		public ComponentLookup<WaterwayComposition> m_WaterwayData;

		[ReadOnly]
		public ComponentLookup<PathwayComposition> m_PathwayData;

		[ReadOnly]
		public ComponentLookup<TaxiwayComposition> m_TaxiwayData;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> m_PrefabLaneArchetypeData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_UtilityLaneData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<NetObjectData> m_PrefabNetObjectData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_Nodes;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<CutRange> m_CutRanges;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> m_PrefabCompositionLanes;

		[ReadOnly]
		public BufferLookup<NetCompositionCrosswalk> m_PrefabCompositionCrosswalks;

		[ReadOnly]
		public BufferLookup<DefaultNetLane> m_DefaultNetLanes;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> m_PrefabSubLanes;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PlaceholderObjects;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_ObjectRequirements;

		[ReadOnly]
		public BufferLookup<AuxiliaryNetLane> m_PrefabAuxiliaryLanes;

		[ReadOnly]
		public BufferLookup<NetCompositionPiece> m_PrefabCompositionPieces;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public Entity m_DefaultTheme;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		[ReadOnly]
		public ComponentTypeSet m_DeletedTempTypes;

		[ReadOnly]
		public ComponentTypeSet m_TempOwnerTypes;

		[ReadOnly]
		public ComponentTypeSet m_HideLaneTypes;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		public ParallelWriter<Lane> m_SkipLaneQueue;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				DeleteLanes(chunk, unfilteredChunkIndex);
			}
			else
			{
				UpdateLanes(chunk, unfilteredChunkIndex);
			}
		}

		private void DeleteLanes(ArchetypeChunk chunk, int chunkIndex)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<SubLane> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Entity subLane = val[j].m_SubLane;
					if (!m_SecondaryLaneData.HasComponent(subLane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, subLane, default(Deleted));
					}
				}
			}
		}

		private void UpdateLanes(ArchetypeChunk chunk, int chunkIndex)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_1598: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_152c: Unknown result type (might be due to invalid IL or missing references)
			//IL_152e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10df: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_10bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_12eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_131f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_13bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_140f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1411: Unknown result type (might be due to invalid IL or missing references)
			//IL_115a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1161: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_125e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1261: Unknown result type (might be due to invalid IL or missing references)
			//IL_133b: Unknown result type (might be due to invalid IL or missing references)
			//IL_133d: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1357: Unknown result type (might be due to invalid IL or missing references)
			//IL_135a: Unknown result type (might be due to invalid IL or missing references)
			//IL_135c: Unknown result type (might be due to invalid IL or missing references)
			//IL_135e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1360: Unknown result type (might be due to invalid IL or missing references)
			//IL_1432: Unknown result type (might be due to invalid IL or missing references)
			//IL_1434: Unknown result type (might be due to invalid IL or missing references)
			//IL_143e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1440: Unknown result type (might be due to invalid IL or missing references)
			//IL_14fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1507: Unknown result type (might be due to invalid IL or missing references)
			//IL_1512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_088d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0932: Unknown result type (might be due to invalid IL or missing references)
			//IL_0943: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0951: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_095c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0961: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0971: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_145f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1462: Unknown result type (might be due to invalid IL or missing references)
			//IL_1464: Unknown result type (might be due to invalid IL or missing references)
			//IL_1466: Unknown result type (might be due to invalid IL or missing references)
			//IL_1468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f19: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07da: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa7: Unknown result type (might be due to invalid IL or missing references)
			//IL_101e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1020: Unknown result type (might be due to invalid IL or missing references)
			//IL_103b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1048: Unknown result type (might be due to invalid IL or missing references)
			//IL_104a: Unknown result type (might be due to invalid IL or missing references)
			//IL_104c: Unknown result type (might be due to invalid IL or missing references)
			//IL_104e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1052: Unknown result type (might be due to invalid IL or missing references)
			//IL_102e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1030: Unknown result type (might be due to invalid IL or missing references)
			LaneBuffer laneBuffer = new LaneBuffer((Allocator)2);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<PseudoRandomSeed> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
			NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Temp> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
			if (nativeArray2.Length != 0)
			{
				NativeList<ConnectPosition> val = default(NativeList<ConnectPosition>);
				val._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> val2 = default(NativeList<ConnectPosition>);
				val2._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> tempBuffer = default(NativeList<ConnectPosition>);
				tempBuffer._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> tempBuffer2 = default(NativeList<ConnectPosition>);
				tempBuffer2._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<EdgeTarget> edgeTargets = default(NativeList<EdgeTarget>);
				edgeTargets._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
				NativeArray<Curve> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<EdgeGeometry> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
				NativeArray<Composition> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
				NativeArray<Game.Tools.EditorContainer> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Tools.EditorContainer>(ref m_EditorContainerType);
				NativeArray<PrefabRef> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<ConnectedNode> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedNode>(ref m_ConnectedNodeType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val3 = nativeArray[i];
					DynamicBuffer<SubLane> lanes = bufferAccessor[i];
					Temp ownerTemp = default(Temp);
					if (nativeArray5.Length != 0)
					{
						ownerTemp = nativeArray5[i];
						if (m_SubLanes.HasBuffer(ownerTemp.m_Original))
						{
							DynamicBuffer<SubLane> lanes2 = m_SubLanes[ownerTemp.m_Original];
							FillOldLaneBuffer(isEdge: true, isNode: false, val3, lanes2, laneBuffer.m_OriginalLanes);
						}
					}
					FillOldLaneBuffer(isEdge: true, isNode: false, val3, lanes, laneBuffer.m_OldLanes);
					if (nativeArray7.Length != 0)
					{
						Edge edge = nativeArray2[i];
						EdgeGeometry edgeGeometry = nativeArray7[i];
						Curve curve = nativeArray6[i];
						Composition composition = nativeArray8[i];
						PrefabRef prefabRef = nativeArray10[i];
						DynamicBuffer<ConnectedNode> val4 = bufferAccessor2[i];
						NetGeometryData prefabGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
						int edgeLaneIndex = 65535;
						int connectionIndex = 0;
						int groupIndex = 0;
						Random random = nativeArray3[i].GetRandom(PseudoRandomSeed.kSubLane);
						Segment segment;
						bool isSingleCurve = NetUtils.TryGetCombinedSegmentForLanes(edgeGeometry, prefabGeometryData, out segment);
						for (int j = 0; j < val4.Length; j++)
						{
							ConnectedNode connectedNode = val4[j];
							GetMiddleConnectionCurves(connectedNode.m_Node, edgeTargets);
							GetNodeConnectPositions(connectedNode.m_Node, connectedNode.m_CurvePosition, val, val2, includeAnchored: true, ref groupIndex, out var _, out var _);
							FilterNodeConnectPositions(val3, ownerTemp.m_Original, val, edgeTargets);
							FilterNodeConnectPositions(val3, ownerTemp.m_Original, val2, edgeTargets);
							CreateEdgeConnectionLanes(chunkIndex, ref edgeLaneIndex, ref connectionIndex, ref random, val3, laneBuffer, val, val2, tempBuffer, tempBuffer2, composition.m_Edge, edgeGeometry, curve, isSingleCurve, nativeArray5.Length != 0, ownerTemp);
							val.Clear();
							val2.Clear();
						}
						CreateEdgeLanes(chunkIndex, ref random, val3, laneBuffer, composition, edge, edgeGeometry, segment, isSingleCurve, nativeArray5.Length != 0, ownerTemp);
					}
					else if (nativeArray9.Length != 0)
					{
						Game.Tools.EditorContainer editorContainer = nativeArray9[i];
						Curve curve2 = nativeArray6[i];
						Segment segment2 = new Segment
						{
							m_Left = curve2.m_Bezier,
							m_Right = curve2.m_Bezier,
							m_Length = float2.op_Implicit(curve2.m_Length)
						};
						NetLaneData netLaneData = m_NetLaneData[editorContainer.m_Prefab];
						NetCompositionLane prefabCompositionLaneData = new NetCompositionLane
						{
							m_Flags = netLaneData.m_Flags,
							m_Lane = editorContainer.m_Prefab
						};
						Random random2 = ((nativeArray3.Length == 0) ? m_RandomSeed.GetRandom(val3.Index) : nativeArray3[i].GetRandom(PseudoRandomSeed.kSubLane));
						CreateEdgeLane(chunkIndex, ref random2, val3, laneBuffer, segment2, default(NetCompositionData), default(CompositionData), default(DynamicBuffer<NetCompositionLane>), prefabCompositionLaneData, new int2(0, 4), new float2(0f, 1f), default(NativeList<LaneAnchor>), default(NativeList<LaneAnchor>), bool2.op_Implicit(false), nativeArray5.Length != 0, ownerTemp);
					}
					RemoveUnusedOldLanes(chunkIndex, val3, lanes, laneBuffer.m_OldLanes);
					laneBuffer.Clear();
				}
				val.Dispose();
				val2.Dispose();
				tempBuffer.Dispose();
				tempBuffer2.Dispose();
				edgeTargets.Dispose();
			}
			else if (nativeArray4.Length != 0)
			{
				NativeArray<PrefabRef> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				bool flag = m_EditorMode && !((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType);
				bool flag2 = !((ArchetypeChunk)(ref chunk)).Has<Game.Objects.Elevation>(ref m_ElevationType);
				bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<UnderConstruction>(ref m_UnderConstructionType);
				bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<Destroyed>(ref m_DestroyedType);
				NativeList<ClearAreaData> clearAreas = default(NativeList<ClearAreaData>);
				DynamicBuffer<InstalledUpgrade> installedUpgrades = default(DynamicBuffer<InstalledUpgrade>);
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				DynamicBuffer<SubLane> val7 = default(DynamicBuffer<SubLane>);
				DynamicBuffer<Game.Prefabs.SubLane> val8 = default(DynamicBuffer<Game.Prefabs.SubLane>);
				BuildingData buildingData = default(BuildingData);
				NetLaneData netLaneData2 = default(NetLaneData);
				float3 val9 = default(float3);
				float3 val10 = default(float3);
				Game.Prefabs.SubLane prefabSubLane2 = default(Game.Prefabs.SubLane);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val5 = nativeArray[k];
					Transform transform = nativeArray4[k];
					PrefabRef prefabRef2 = nativeArray11[k];
					DynamicBuffer<SubLane> lanes3 = bufferAccessor[k];
					Temp ownerTemp2 = default(Temp);
					if (nativeArray5.Length != 0)
					{
						ownerTemp2 = nativeArray5[k];
						if (m_SubLanes.HasBuffer(ownerTemp2.m_Original))
						{
							DynamicBuffer<SubLane> lanes4 = m_SubLanes[ownerTemp2.m_Original];
							FillOldLaneBuffer(isEdge: false, isNode: false, val5, lanes4, laneBuffer.m_OriginalLanes);
						}
					}
					FillOldLaneBuffer(isEdge: false, isNode: false, val5, lanes3, laneBuffer.m_OldLanes);
					if (!flag)
					{
						Entity val6 = val5;
						if ((ownerTemp2.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) != 0 || ownerTemp2.m_Original != Entity.Null)
						{
							val6 = ownerTemp2.m_Original;
						}
						bool flag5 = (ownerTemp2.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) != 0;
						if (m_InstalledUpgrades.TryGetBuffer(val6, ref installedUpgrades) && installedUpgrades.Length != 0)
						{
							ClearAreaHelpers.FillClearAreas(installedUpgrades, Entity.Null, m_TransformData, m_AreaClearData, m_PrefabRefData, m_PrefabObjectGeometryData, m_SubAreas, m_AreaNodes, m_AreaTriangles, ref clearAreas);
							ClearAreaHelpers.InitClearAreas(clearAreas, transform);
						}
						else if (m_EditorMode && ownerTemp2.m_Original != Entity.Null)
						{
							flag5 = true;
						}
						bool flag6 = flag2;
						if (flag6)
						{
							flag6 = m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData) && ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.DeleteOverridden) != Game.Objects.GeometryFlags.None || (objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.Marker)) == 0);
						}
						Random random3 = nativeArray3[k].GetRandom(PseudoRandomSeed.kSubLane);
						if (flag5)
						{
							if (m_SubLanes.TryGetBuffer(ownerTemp2.m_Original, ref val7))
							{
								for (int l = 0; l < val7.Length; l++)
								{
									Entity subLane = val7[l].m_SubLane;
									CreateObjectLane(chunkIndex, ref random3, val5, subLane, laneBuffer, transform, default(Game.Prefabs.SubLane), l, flag6, cutForTraffic: false, nativeArray5.Length != 0, ownerTemp2, clearAreas);
								}
							}
						}
						else
						{
							int num = 0;
							int num2 = 0;
							if (m_PrefabSubLanes.TryGetBuffer(prefabRef2.m_Prefab, ref val8))
							{
								for (int m = 0; m < val8.Length; m++)
								{
									Game.Prefabs.SubLane prefabSubLane = val8[m];
									if (prefabSubLane.m_NodeIndex.y != prefabSubLane.m_NodeIndex.x)
									{
										num = m;
										num2 = math.max(num2, math.cmax(prefabSubLane.m_NodeIndex));
										if ((!flag3 || (m_NetLaneData[prefabSubLane.m_Prefab].m_Flags & (LaneFlags.Road | LaneFlags.Pedestrian | LaneFlags.Parking | LaneFlags.Track)) != 0) && (!flag4 || !math.any(prefabSubLane.m_ParentMesh >= 0)))
										{
											CreateObjectLane(chunkIndex, ref random3, val5, Entity.Null, laneBuffer, transform, prefabSubLane, m, flag6, cutForTraffic: false, nativeArray5.Length != 0, ownerTemp2, clearAreas);
										}
									}
								}
							}
							if (flag3 && m_PrefabBuildingData.TryGetComponent(prefabRef2.m_Prefab, ref buildingData) && m_NetLaneData.TryGetComponent(m_BuildingConfigurationData.m_ConstructionBorder, ref netLaneData2))
							{
								((float3)(ref val9))._002Ector((float)buildingData.m_LotSize.x * 4f - netLaneData2.m_Width * 0.5f, 0f, 0f);
								((float3)(ref val10))._002Ector(0f, 0f, (float)buildingData.m_LotSize.y * 4f - netLaneData2.m_Width * 0.5f);
								prefabSubLane2.m_Prefab = m_BuildingConfigurationData.m_ConstructionBorder;
								prefabSubLane2.m_ParentMesh = int2.op_Implicit(-1);
								prefabSubLane2.m_NodeIndex = new int2(num2, num2 + 1);
								prefabSubLane2.m_Curve = NetUtils.StraightCurve(-val9 - val10, val9 - val10);
								CreateObjectLane(chunkIndex, ref random3, val5, Entity.Null, laneBuffer, transform, prefabSubLane2, num, flag6, (buildingData.m_Flags & Game.Prefabs.BuildingFlags.BackAccess) != 0, nativeArray5.Length != 0, ownerTemp2, clearAreas);
								prefabSubLane2.m_NodeIndex = new int2(num2 + 1, num2 + 2);
								prefabSubLane2.m_Curve = NetUtils.StraightCurve(val9 - val10, val9 + val10);
								CreateObjectLane(chunkIndex, ref random3, val5, Entity.Null, laneBuffer, transform, prefabSubLane2, num + 1, flag6, (buildingData.m_Flags & Game.Prefabs.BuildingFlags.LeftAccess) != 0, nativeArray5.Length != 0, ownerTemp2, clearAreas);
								prefabSubLane2.m_NodeIndex = new int2(num2 + 2, num2 + 3);
								prefabSubLane2.m_Curve = NetUtils.StraightCurve(val9 + val10, -val9 + val10);
								CreateObjectLane(chunkIndex, ref random3, val5, Entity.Null, laneBuffer, transform, prefabSubLane2, num + 2, flag6, cutForTraffic: true, nativeArray5.Length != 0, ownerTemp2, clearAreas);
								prefabSubLane2.m_NodeIndex = new int2(num2 + 3, num2);
								prefabSubLane2.m_Curve = NetUtils.StraightCurve(-val9 + val10, -val9 - val10);
								CreateObjectLane(chunkIndex, ref random3, val5, Entity.Null, laneBuffer, transform, prefabSubLane2, num + 3, flag6, (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RightAccess) != 0, nativeArray5.Length != 0, ownerTemp2, clearAreas);
								num += 4;
								num2 += 4;
							}
						}
					}
					RemoveUnusedOldLanes(chunkIndex, val5, lanes3, laneBuffer.m_OldLanes);
					laneBuffer.Clear();
					if (clearAreas.IsCreated)
					{
						clearAreas.Clear();
					}
				}
				if (clearAreas.IsCreated)
				{
					clearAreas.Dispose();
				}
			}
			else
			{
				NativeParallelHashSet<ConnectionKey> createdConnections = default(NativeParallelHashSet<ConnectionKey>);
				createdConnections._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> val11 = default(NativeList<ConnectPosition>);
				val11._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> val12 = default(NativeList<ConnectPosition>);
				val12._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> val13 = default(NativeList<ConnectPosition>);
				val13._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> val14 = default(NativeList<ConnectPosition>);
				val14._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> val15 = default(NativeList<ConnectPosition>);
				val15._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<ConnectPosition> val16 = default(NativeList<ConnectPosition>);
				val16._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<MiddleConnection> middleConnections = default(NativeList<MiddleConnection>);
				middleConnections._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<EdgeTarget> tempEdgeTargets = default(NativeList<EdgeTarget>);
				tempEdgeTargets._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
				NativeArray<Orphan> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Orphan>(ref m_OrphanType);
				NativeArray<NodeGeometry> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeGeometry>(ref m_NodeGeometryType);
				NativeArray<PrefabRef> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int n = 0; n < nativeArray.Length; n++)
				{
					Entity val17 = nativeArray[n];
					DynamicBuffer<SubLane> lanes5 = bufferAccessor[n];
					Temp ownerTemp3 = default(Temp);
					if (nativeArray5.Length != 0)
					{
						ownerTemp3 = nativeArray5[n];
						if (m_SubLanes.HasBuffer(ownerTemp3.m_Original))
						{
							DynamicBuffer<SubLane> lanes6 = m_SubLanes[ownerTemp3.m_Original];
							FillOldLaneBuffer(isEdge: false, isNode: true, val17, lanes6, laneBuffer.m_OriginalLanes);
						}
					}
					FillOldLaneBuffer(isEdge: false, isNode: true, val17, lanes5, laneBuffer.m_OldLanes);
					if (nativeArray13.Length != 0)
					{
						float3 position = m_NodeData[val17].m_Position;
						position.y = nativeArray13[n].m_Position;
						int groupIndex2 = 1;
						GetNodeConnectPositions(val17, 0f, val11, val12, includeAnchored: false, ref groupIndex2, out var middleRadius2, out var roundaboutSize2);
						bool flag7 = groupIndex2 <= 2;
						GetMiddleConnections(val17, ownerTemp3.m_Original, middleConnections, tempEdgeTargets, val15, val16, ref groupIndex2);
						FilterMainCarConnectPositions(val11, val13);
						FilterMainCarConnectPositions(val12, val14);
						int prevLaneIndex = 0;
						Random random4 = nativeArray3[n].GetRandom(PseudoRandomSeed.kSubLane);
						RoadTypes roadTypes = RoadTypes.None;
						if (middleRadius2 > 0f)
						{
							roadTypes = GetRoundaboutRoadPassThrough(val17);
						}
						if (middleRadius2 > 0f && (roadTypes == RoadTypes.None || flag7))
						{
							if (val13.Length != 0 || val14.Length != 0)
							{
								ConnectPosition roundaboutLane = default(ConnectPosition);
								int laneCount = 0;
								uint laneGroup = 0u;
								float laneWidth = float.MaxValue;
								float spaceForLanes = float.MaxValue;
								bool isPublicOnly = true;
								int nextLaneIndex = prevLaneIndex;
								int num3 = 0;
								int num4 = 0;
								for (int num5 = 0; num5 < val13.Length; num5++)
								{
									if ((val13[num5].m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0)
									{
										num3++;
									}
									else
									{
										num4++;
									}
								}
								for (int num6 = 0; num6 < val14.Length; num6++)
								{
									ConnectPosition connectPosition = val14[num6];
									if ((connectPosition.m_CompositionData.m_RoadFlags & (Game.Prefabs.RoadFlags.DefaultIsForward | Game.Prefabs.RoadFlags.DefaultIsBackward)) != 0)
									{
										if ((connectPosition.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0)
										{
											num3++;
										}
										else
										{
											num4++;
										}
									}
								}
								bool preferHighway = num3 > num4 || num3 >= 2;
								ConnectPosition connectPosition2;
								bool flag8;
								if (val13.Length != 0)
								{
									int num7 = 0;
									for (int num8 = 0; num8 < val13.Length; num8++)
									{
										ConnectPosition main = val13[num8];
										FilterActualCarConnectPositions(main, val11, val15);
										bool roundaboutLane2 = GetRoundaboutLane(val15, roundaboutSize2, ref roundaboutLane, ref laneCount, ref laneWidth, ref isPublicOnly, ref spaceForLanes, isSource: true, preferHighway);
										num7 = math.select(num7, num8, roundaboutLane2);
										val15.Clear();
									}
									if (roundaboutLane.m_LaneData.m_Lane == Entity.Null)
									{
										int laneCount2 = 0;
										float spaceForLanes2 = float.MaxValue;
										for (int num9 = 0; num9 < val14.Length; num9++)
										{
											ConnectPosition main2 = val14[num9];
											FilterActualCarConnectPositions(main2, val12, val16);
											GetRoundaboutLane(val16, roundaboutSize2, ref roundaboutLane, ref laneCount2, ref laneWidth, ref isPublicOnly, ref spaceForLanes2, isSource: false, preferHighway);
											val16.Clear();
										}
										spaceForLanes = spaceForLanes2 * (float)laneCount;
									}
									connectPosition2 = val13[num7];
									flag8 = true;
								}
								else
								{
									int num10 = 0;
									for (int num11 = 0; num11 < val14.Length; num11++)
									{
										ConnectPosition main3 = val14[num11];
										FilterActualCarConnectPositions(main3, val12, val16);
										bool roundaboutLane3 = GetRoundaboutLane(val16, roundaboutSize2, ref roundaboutLane, ref laneCount, ref laneWidth, ref isPublicOnly, ref spaceForLanes, isSource: false, preferHighway);
										num10 = math.select(num10, num11, roundaboutLane3);
										val16.Clear();
									}
									connectPosition2 = val14[num10];
									flag8 = false;
								}
								ExtractNextConnectPosition(connectPosition2, position, val13, val14, out var nextPosition, out var nextIsSource);
								if (flag8)
								{
									FilterActualCarConnectPositions(connectPosition2, val11, val15);
								}
								if (!nextIsSource)
								{
									FilterActualCarConnectPositions(nextPosition, val12, val16);
								}
								int laneCount3 = GetRoundaboutLaneCount(connectPosition2, nextPosition, val15, val16, val12, position);
								connectPosition2 = nextPosition;
								flag8 = nextIsSource;
								val15.Clear();
								val16.Clear();
								int length = val13.Length;
								while (val13.Length != 0 || val14.Length != 0)
								{
									ExtractNextConnectPosition(connectPosition2, position, val13, val14, out var nextPosition2, out var nextIsSource2);
									if (flag8)
									{
										FilterActualCarConnectPositions(connectPosition2, val11, val15);
									}
									if (!nextIsSource2)
									{
										FilterActualCarConnectPositions(nextPosition2, val12, val16);
									}
									CreateRoundaboutCarLanes(chunkIndex, ref random4, val17, laneBuffer, ref prevLaneIndex, -1, ref laneGroup, connectPosition2, nextPosition2, middleConnections, val15, val16, val12, roundaboutLane, position, middleRadius2, ref laneCount3, laneCount, length, spaceForLanes, roadTypes, flag7, nativeArray5.Length != 0, ownerTemp3);
									connectPosition2 = nextPosition2;
									flag8 = nextIsSource2;
									val15.Clear();
									val16.Clear();
								}
								if (flag8)
								{
									FilterActualCarConnectPositions(connectPosition2, val11, val15);
								}
								if (!nextIsSource)
								{
									FilterActualCarConnectPositions(nextPosition, val12, val16);
								}
								CreateRoundaboutCarLanes(chunkIndex, ref random4, val17, laneBuffer, ref prevLaneIndex, nextLaneIndex, ref laneGroup, connectPosition2, nextPosition, middleConnections, val15, val16, val12, roundaboutLane, position, middleRadius2, ref laneCount3, laneCount, length, spaceForLanes, roadTypes, flag7, nativeArray5.Length != 0, ownerTemp3);
								val15.Clear();
								val16.Clear();
							}
						}
						else
						{
							RoadTypes roadTypes2 = RoadTypes.None;
							for (int num12 = 0; num12 < val13.Length; num12++)
							{
								ConnectPosition connectPosition3 = val13[num12];
								int nodeLaneIndex = prevLaneIndex;
								if (roadTypes2 != connectPosition3.m_RoadTypes)
								{
									val16.Clear();
									FilterActualCarConnectPositions(connectPosition3.m_RoadTypes, val12, val16);
									roadTypes2 = connectPosition3.m_RoadTypes;
								}
								int yield = CalculateYieldOffset(connectPosition3, val13, val14);
								FilterActualCarConnectPositions(connectPosition3, val11, val15);
								ProcessCarConnectPositions(chunkIndex, ref nodeLaneIndex, ref random4, val17, laneBuffer, middleConnections, createdConnections, val15, val16, val11, roadTypes, nativeArray5.Length != 0, ownerTemp3, yield);
								val15.Clear();
								for (int num13 = 0; num13 < val14.Length; num13++)
								{
									ConnectPosition targetPosition = val14[num13];
									if ((targetPosition.m_RoadTypes & connectPosition3.m_RoadTypes) == 0)
									{
										continue;
									}
									bool isUnsafe = false;
									bool isForbidden = false;
									bool isSkipped = false;
									for (int num14 = 0; num14 < val16.Length; num14++)
									{
										ConnectPosition connectPosition4 = val16[num14];
										if (connectPosition4.m_Owner == targetPosition.m_Owner && connectPosition4.m_LaneData.m_Group == targetPosition.m_LaneData.m_Group)
										{
											if (connectPosition4.m_SkippedCount != 0)
											{
												isSkipped = true;
												connectPosition4.m_SkippedCount = 0;
												val16[num14] = connectPosition4;
											}
											else if (connectPosition4.m_ForbiddenCount != 0)
											{
												isUnsafe = true;
												isForbidden = true;
												connectPosition4.m_UnsafeCount = 0;
												connectPosition4.m_ForbiddenCount = 0;
												val16[num14] = connectPosition4;
											}
											else if (connectPosition4.m_UnsafeCount != 0)
											{
												isUnsafe = true;
												connectPosition4.m_UnsafeCount = 0;
												val16[num14] = connectPosition4;
											}
										}
									}
									if (((connectPosition3.m_LaneData.m_Flags | targetPosition.m_LaneData.m_Flags) & LaneFlags.Master) != 0)
									{
										uint num15 = (uint)(connectPosition3.m_GroupIndex | (targetPosition.m_GroupIndex << 16));
										bool right;
										bool gentle;
										bool uturn;
										bool isTurn = IsTurn(connectPosition3, targetPosition, out right, out gentle, out uturn);
										float curviness = -1f;
										if (CreateNodeLane(chunkIndex, ref nodeLaneIndex, ref random4, ref curviness, ref isSkipped, val17, laneBuffer, middleConnections, connectPosition3, targetPosition, num15, 0, isUnsafe, isForbidden, nativeArray5.Length != 0, trackOnly: false, 0, ownerTemp3, isTurn, right, gentle, uturn, isRoundabout: false, isLeftLimit: false, isRightLimit: false, isMergeLeft: false, isMergeRight: false, fixedTangents: false, RoadTypes.None))
										{
											createdConnections.Add(new ConnectionKey(connectPosition3, targetPosition));
										}
									}
								}
								prevLaneIndex += 256;
							}
							val16.Clear();
						}
						val13.Clear();
						val14.Clear();
						TrackTypes trackTypes = FilterTrackConnectPositions(val11, val13) & FilterTrackConnectPositions(val12, val14);
						TrackTypes trackTypes2 = TrackTypes.Train;
						while (trackTypes != TrackTypes.None)
						{
							if ((trackTypes & trackTypes2) != TrackTypes.None)
							{
								trackTypes = (TrackTypes)((uint)trackTypes & (uint)(byte)(~(int)trackTypes2));
								FilterTrackConnectPositions(trackTypes2, val14, val16);
								if (val16.Length != 0)
								{
									int index = 0;
									while (index < val13.Length)
									{
										FilterTrackConnectPositions(ref index, trackTypes2, val13, val15);
										if (val15.Length != 0)
										{
											int nodeLaneIndex2 = prevLaneIndex;
											ProcessTrackConnectPositions(chunkIndex, ref nodeLaneIndex2, ref random4, val17, laneBuffer, middleConnections, createdConnections, val15, val16, nativeArray5.Length != 0, ownerTemp3);
											val15.Clear();
											prevLaneIndex += 256;
										}
									}
									val16.Clear();
								}
							}
							trackTypes2 = (TrackTypes)((uint)trackTypes2 << 1);
						}
						val13.Clear();
						val14.Clear();
						for (int num16 = 0; num16 < 2; num16++)
						{
							FilterPedestrianConnectPositions(val12, val13, middleConnections, num16 == 1);
							CreateNodePedestrianLanes(chunkIndex, ref prevLaneIndex, ref random4, val17, laneBuffer, val13, val14, val16, nativeArray5.Length != 0, ownerTemp3, position, middleRadius2, roundaboutSize2);
							val13.Clear();
							val14.Clear();
							val16.Clear();
						}
						UtilityTypes utilityTypes = FilterUtilityConnectPositions(val12, val14);
						UtilityTypes utilityTypes2 = UtilityTypes.WaterPipe;
						while (utilityTypes != UtilityTypes.None)
						{
							if ((utilityTypes & utilityTypes2) != UtilityTypes.None)
							{
								utilityTypes = (UtilityTypes)((uint)utilityTypes & (uint)(byte)(~(int)utilityTypes2));
								FilterUtilityConnectPositions(utilityTypes2, val14, val16);
								FilterUtilityConnectPositions(utilityTypes2, val13, val15);
								if (val16.Length != 0 || val15.Length != 0)
								{
									CreateNodeUtilityLanes(chunkIndex, ref prevLaneIndex, ref random4, val17, laneBuffer, val16, val15, middleConnections, position, middleRadius2 > 0f, nativeArray5.Length != 0, ownerTemp3);
									val16.Clear();
									val15.Clear();
								}
							}
							utilityTypes2 = (UtilityTypes)((uint)utilityTypes2 << 1);
						}
						CreateNodeConnectionLanes(chunkIndex, ref prevLaneIndex, ref random4, val17, laneBuffer, middleConnections, val16, middleRadius2 > 0f, nativeArray5.Length != 0, ownerTemp3);
						createdConnections.Clear();
						val11.Clear();
						val12.Clear();
						val13.Clear();
						val14.Clear();
						middleConnections.Clear();
						if (nativeArray12.Length != 0)
						{
							CreateOrphanLanes(chunkIndex, ref random4, val17, laneBuffer, nativeArray12[n], position, nativeArray14[n].m_Prefab, ref prevLaneIndex, nativeArray5.Length != 0, ownerTemp3);
						}
					}
					RemoveUnusedOldLanes(chunkIndex, val17, lanes5, laneBuffer.m_OldLanes);
					laneBuffer.Clear();
				}
				createdConnections.Dispose();
				val11.Dispose();
				val12.Dispose();
				val13.Dispose();
				val14.Dispose();
				val15.Dispose();
				val16.Dispose();
				middleConnections.Dispose();
				tempEdgeTargets.Dispose();
			}
			UpdateLanes(chunkIndex, laneBuffer.m_Updates);
			laneBuffer.Dispose();
		}

		private void CreateOrphanLanes(int jobIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, Orphan orphan, float3 middlePosition, Entity prefab, ref int nodeLaneIndex, bool isTemp, Temp ownerTemp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			if (!m_DefaultNetLanes.HasBuffer(prefab))
			{
				return;
			}
			DynamicBuffer<DefaultNetLane> val = m_DefaultNetLanes[prefab];
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < val.Length; i++)
			{
				if ((val[i].m_Flags & LaneFlags.Pedestrian) != 0)
				{
					if (num == -1)
					{
						num = i;
					}
					else
					{
						num2 = i;
					}
				}
			}
			if (num2 > num)
			{
				DefaultNetLane defaultNetLane = val[num];
				DefaultNetLane defaultNetLane2 = val[num2];
				ConnectPosition connectPosition = new ConnectPosition
				{
					m_LaneData = 
					{
						m_Lane = defaultNetLane.m_Lane,
						m_Flags = defaultNetLane.m_Flags
					},
					m_NodeComposition = orphan.m_Composition,
					m_Owner = owner,
					m_BaseHeight = middlePosition.y,
					m_Position = middlePosition
				};
				ref float3 reference = ref connectPosition.m_Position;
				((float3)(ref reference)).xy = ((float3)(ref reference)).xy + ((float3)(ref defaultNetLane.m_Position)).xy;
				connectPosition.m_Tangent = new float3(0f, 0f, 1f);
				ConnectPosition connectPosition2 = new ConnectPosition
				{
					m_LaneData = 
					{
						m_Lane = defaultNetLane2.m_Lane,
						m_Flags = defaultNetLane2.m_Flags
					},
					m_NodeComposition = orphan.m_Composition,
					m_Owner = owner,
					m_BaseHeight = middlePosition.y,
					m_Position = middlePosition
				};
				ref float3 reference2 = ref connectPosition2.m_Position;
				((float3)(ref reference2)).xy = ((float3)(ref reference2)).xy + ((float3)(ref defaultNetLane2.m_Position)).xy;
				connectPosition2.m_Tangent = new float3(0f, 0f, 1f);
				PathNode pathNode = new PathNode(owner, (ushort)nodeLaneIndex++);
				CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition2, connectPosition, pathNode, pathNode, isCrosswalk: false, isSideConnection: true, isTemp, ownerTemp, fixedTangents: false, hasSignals: false, out var curve, out var middleNode, out var endNode);
				connectPosition.m_Tangent = -connectPosition.m_Tangent;
				connectPosition2.m_Tangent = -connectPosition2.m_Tangent;
				CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition, connectPosition2, endNode, pathNode, isCrosswalk: false, isSideConnection: true, isTemp, ownerTemp, fixedTangents: false, hasSignals: false, out curve, out middleNode, out var _);
			}
		}

		private int GetRoundaboutLaneCount(ConnectPosition prevMainPosition, ConnectPosition nextMainPosition, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, NativeList<ConnectPosition> allTargets, float3 middlePosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.normalizesafe(((float3)(ref prevMainPosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
			float2 val2 = math.normalizesafe(((float3)(ref nextMainPosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
			float num = ((!((float3)(ref prevMainPosition.m_Position)).Equals(nextMainPosition.m_Position)) ? (m_LeftHandTraffic ? MathUtils.RotationAngleRight(val, val2) : MathUtils.RotationAngleLeft(val, val2)) : ((float)Math.PI * 2f));
			int num2 = math.max(1, Mathf.CeilToInt(num * (2f / (float)Math.PI) - 0.003141593f));
			int num3 = math.max(1, sourceBuffer.Length);
			if (num2 == 1)
			{
				if (sourceBuffer.Length > 0 && targetBuffer.Length > 0)
				{
					int num4 = math.clamp(sourceBuffer.Length + targetBuffer.Length - GetRoundaboutTargetLaneCount(sourceBuffer[sourceBuffer.Length - 1], allTargets), 0, math.min(targetBuffer.Length, sourceBuffer.Length - 1));
					num4 = math.min(1, num4);
					return num3 - num4;
				}
				return num3;
			}
			int num5 = targetBuffer.Length - math.select(1, 0, targetBuffer.Length <= 1);
			return math.max(1, num3 - num5);
		}

		private int GetRoundaboutTargetLaneCount(ConnectPosition sourcePosition, NativeList<ConnectPosition> allTargets)
		{
			int num = 0;
			for (int i = 0; i < allTargets.Length; i++)
			{
				ConnectPosition targetPosition = allTargets[i];
				if ((targetPosition.m_LaneData.m_Flags & (LaneFlags.Master | LaneFlags.Road)) == LaneFlags.Road)
				{
					IsTurn(sourcePosition, targetPosition, out var _, out var _, out var uturn);
					num += math.select(1, 0, uturn);
				}
			}
			return num;
		}

		private void CreateRoundaboutCarLanes(int jobIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, ref int prevLaneIndex, int nextLaneIndex, ref uint laneGroup, ConnectPosition prevMainPosition, ConnectPosition nextMainPosition, NativeList<MiddleConnection> middleConnections, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, NativeList<ConnectPosition> allTargets, ConnectPosition lane, float3 middlePosition, float middleRadius, ref int laneCount, int totalLaneCount, int totalSourceCount, float spaceForLanes, RoadTypes roadPassThrough, bool isDeadEnd, bool isTemp, Temp ownerTemp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0834: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_17a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_17a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_17b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_17b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_17bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
			//IL_11c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_11c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0def: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_11de: Unknown result type (might be due to invalid IL or missing references)
			//IL_105b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1060: Unknown result type (might be due to invalid IL or missing references)
			//IL_1064: Unknown result type (might be due to invalid IL or missing references)
			//IL_1069: Unknown result type (might be due to invalid IL or missing references)
			//IL_1092: Unknown result type (might be due to invalid IL or missing references)
			//IL_1097: Unknown result type (might be due to invalid IL or missing references)
			//IL_109a: Unknown result type (might be due to invalid IL or missing references)
			//IL_109f: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1101: Unknown result type (might be due to invalid IL or missing references)
			//IL_1128: Unknown result type (might be due to invalid IL or missing references)
			//IL_112b: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_120f: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1214: Unknown result type (might be due to invalid IL or missing references)
			//IL_1218: Unknown result type (might be due to invalid IL or missing references)
			//IL_1220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0add: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1239: Unknown result type (might be due to invalid IL or missing references)
			//IL_123b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1230: Unknown result type (might be due to invalid IL or missing references)
			//IL_1232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f81: Unknown result type (might be due to invalid IL or missing references)
			//IL_1258: Unknown result type (might be due to invalid IL or missing references)
			//IL_125a: Unknown result type (might be due to invalid IL or missing references)
			//IL_124f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1251: Unknown result type (might be due to invalid IL or missing references)
			//IL_12c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_12c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_12da: Unknown result type (might be due to invalid IL or missing references)
			//IL_12df: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_131e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1323: Unknown result type (might be due to invalid IL or missing references)
			//IL_132c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1331: Unknown result type (might be due to invalid IL or missing references)
			//IL_133a: Unknown result type (might be due to invalid IL or missing references)
			//IL_133f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1354: Unknown result type (might be due to invalid IL or missing references)
			//IL_1355: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1409: Unknown result type (might be due to invalid IL or missing references)
			//IL_140e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1412: Unknown result type (might be due to invalid IL or missing references)
			//IL_1417: Unknown result type (might be due to invalid IL or missing references)
			//IL_1421: Unknown result type (might be due to invalid IL or missing references)
			//IL_1423: Unknown result type (might be due to invalid IL or missing references)
			//IL_1429: Unknown result type (might be due to invalid IL or missing references)
			//IL_1433: Unknown result type (might be due to invalid IL or missing references)
			//IL_144c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1451: Unknown result type (might be due to invalid IL or missing references)
			//IL_1455: Unknown result type (might be due to invalid IL or missing references)
			//IL_145a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1464: Unknown result type (might be due to invalid IL or missing references)
			//IL_1466: Unknown result type (might be due to invalid IL or missing references)
			//IL_146a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1473: Unknown result type (might be due to invalid IL or missing references)
			//IL_148f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1494: Unknown result type (might be due to invalid IL or missing references)
			//IL_1498: Unknown result type (might be due to invalid IL or missing references)
			//IL_149d: Unknown result type (might be due to invalid IL or missing references)
			//IL_16b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_16bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_16c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_16ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_16cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_16ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_16fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_16fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1606: Unknown result type (might be due to invalid IL or missing references)
			//IL_1608: Unknown result type (might be due to invalid IL or missing references)
			//IL_160a: Unknown result type (might be due to invalid IL or missing references)
			//IL_163e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1641: Unknown result type (might be due to invalid IL or missing references)
			//IL_1540: Unknown result type (might be due to invalid IL or missing references)
			//IL_1545: Unknown result type (might be due to invalid IL or missing references)
			//IL_154a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1554: Unknown result type (might be due to invalid IL or missing references)
			//IL_1556: Unknown result type (might be due to invalid IL or missing references)
			//IL_1558: Unknown result type (might be due to invalid IL or missing references)
			//IL_1585: Unknown result type (might be due to invalid IL or missing references)
			//IL_1588: Unknown result type (might be due to invalid IL or missing references)
			//IL_173c: Unknown result type (might be due to invalid IL or missing references)
			//IL_173e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1740: Unknown result type (might be due to invalid IL or missing references)
			//IL_176d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1770: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.normalizesafe(((float3)(ref prevMainPosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
			float2 val2 = math.normalizesafe(((float3)(ref nextMainPosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
			float num = ((!((float3)(ref prevMainPosition.m_Position)).Equals(nextMainPosition.m_Position)) ? (m_LeftHandTraffic ? MathUtils.RotationAngleRight(val, val2) : MathUtils.RotationAngleLeft(val, val2)) : ((float)Math.PI * 2f));
			int num2 = math.max(1, Mathf.CeilToInt(num * (2f / (float)Math.PI) - 0.003141593f));
			float2 val3 = val;
			ConnectPosition connectPosition = new ConnectPosition
			{
				m_LaneData = 
				{
					m_Lane = lane.m_LaneData.m_Lane
				},
				m_NodeComposition = lane.m_NodeComposition,
				m_EdgeComposition = lane.m_EdgeComposition,
				m_CompositionData = lane.m_CompositionData,
				m_BaseHeight = middlePosition.y + prevMainPosition.m_BaseHeight - prevMainPosition.m_Position.y
			};
			((float3)(ref connectPosition.m_Tangent)).xz = (m_LeftHandTraffic ? MathUtils.Right(val3) : MathUtils.Left(val3));
			connectPosition.m_SegmentIndex = (byte)(prevLaneIndex >> 8);
			connectPosition.m_Owner = owner;
			connectPosition.m_Position.y = middlePosition.y;
			ConnectPosition connectPosition2 = new ConnectPosition
			{
				m_LaneData = 
				{
					m_Lane = lane.m_LaneData.m_Lane
				},
				m_NodeComposition = lane.m_NodeComposition,
				m_EdgeComposition = lane.m_EdgeComposition,
				m_CompositionData = lane.m_CompositionData,
				m_Owner = owner
			};
			if (sourceBuffer.Length >= 2)
			{
				NativeSortExtension.Sort<ConnectPosition, TargetPositionComparer>(sourceBuffer, default(TargetPositionComparer));
			}
			if (targetBuffer.Length >= 2)
			{
				NativeSortExtension.Sort<ConnectPosition, TargetPositionComparer>(targetBuffer, default(TargetPositionComparer));
			}
			int num3 = 0;
			int num4 = targetBuffer.Length - math.select(1, 0, targetBuffer.Length <= 1);
			if (num2 == 1 && sourceBuffer.Length > 0 && targetBuffer.Length > 0)
			{
				num3 = math.clamp(sourceBuffer.Length + targetBuffer.Length - GetRoundaboutTargetLaneCount(sourceBuffer[sourceBuffer.Length - 1], allTargets), 0, math.min(targetBuffer.Length, sourceBuffer.Length - 1));
				if ((num3 == 0) & (sourceBuffer.Length < totalLaneCount))
				{
					int num5 = math.max(1, laneCount - num4);
					num3 = math.select(0, 1, num5 + sourceBuffer.Length >= totalLaneCount);
				}
				num3 = math.min(1, num3);
			}
			Bezier4x3 val9 = default(Bezier4x3);
			Bezier4x3 val11 = default(Bezier4x3);
			float num40 = default(float);
			for (int i = 1; i <= num2; i++)
			{
				int num6 = math.max(1, math.select(laneCount - num4, laneCount, i != num2));
				int num7 = math.select(math.min(totalLaneCount, num6 + sourceBuffer.Length) - num3, num6, i != 1);
				int nodeLaneIndex = prevLaneIndex + totalLaneCount + 2;
				prevLaneIndex += 256;
				float2 val4;
				if (i == num2)
				{
					val4 = val2;
					connectPosition2.m_NodeComposition = lane.m_NodeComposition;
					connectPosition2.m_EdgeComposition = lane.m_EdgeComposition;
					connectPosition2.m_CompositionData = lane.m_CompositionData;
					connectPosition2.m_BaseHeight = middlePosition.y + nextMainPosition.m_BaseHeight - nextMainPosition.m_Position.y;
					connectPosition2.m_SegmentIndex = (byte)(math.select(prevLaneIndex, nextLaneIndex, nextLaneIndex >= 0) >> 8);
					connectPosition2.m_Position.y = middlePosition.y;
				}
				else
				{
					float num8 = (float)i / (float)num2;
					val4 = (m_LeftHandTraffic ? MathUtils.RotateRight(val3, num * num8) : MathUtils.RotateLeft(val3, num * num8));
					connectPosition2.m_CompositionData.m_SpeedLimit = math.lerp(prevMainPosition.m_CompositionData.m_SpeedLimit, nextMainPosition.m_CompositionData.m_SpeedLimit, num8);
					connectPosition2.m_BaseHeight = middlePosition.y + math.lerp(prevMainPosition.m_BaseHeight, nextMainPosition.m_BaseHeight, num8) - math.lerp(prevMainPosition.m_Position.y, nextMainPosition.m_Position.y, num8);
					connectPosition2.m_SegmentIndex = (byte)(prevLaneIndex >> 8);
					connectPosition2.m_Position.y = middlePosition.y;
				}
				float2 val5 = (m_LeftHandTraffic ? MathUtils.RotateRight(val3, num * ((float)i - 0.5f) / (float)num2) : MathUtils.RotateLeft(val3, num * ((float)i - 0.5f) / (float)num2));
				((float3)(ref connectPosition2.m_Tangent)).xz = (m_LeftHandTraffic ? MathUtils.Left(val4) : MathUtils.Right(val4));
				float3 val6 = default(float3);
				((float3)(ref val6)).xz = (m_LeftHandTraffic ? MathUtils.Right(val5) : MathUtils.Left(val5));
				float3 val7 = new float3
				{
					y = math.lerp(connectPosition.m_Position.y, connectPosition2.m_Position.y, 0.5f)
				};
				bool flag = laneCount >= 2;
				bool flag2 = num7 >= 2;
				bool flag3 = i == 1 && sourceBuffer.Length >= 1;
				bool flag4 = i == num2 && targetBuffer.Length >= 1;
				bool flag5 = num2 == 1 && sourceBuffer.Length >= 1 && targetBuffer.Length >= 1;
				float curviness = -1f;
				bool isUnsafe = roadPassThrough != RoadTypes.None || (isDeadEnd && (nextLaneIndex < 0 || flag3 || flag4));
				bool isSkipped = false;
				for (int j = 0; j < num7; j++)
				{
					int num9 = math.select(j, num7 - j - 1, m_LeftHandTraffic);
					int num10 = math.max(0, j - num7 + num6);
					float num11 = middleRadius + ((float)num10 + 0.5f) * spaceForLanes / (float)totalLaneCount;
					float num12 = middleRadius + ((float)j + 0.5f) * spaceForLanes / (float)totalLaneCount;
					((float3)(ref connectPosition.m_Position)).xz = ((float3)(ref middlePosition)).xz + val * num11;
					connectPosition.m_LaneData.m_Index = (byte)num10;
					connectPosition.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
					if (flag)
					{
						connectPosition.m_LaneData.m_Flags |= LaneFlags.Slave;
					}
					((float3)(ref connectPosition2.m_Position)).xz = ((float3)(ref middlePosition)).xz + val4 * num12;
					connectPosition2.m_LaneData.m_Index = (byte)j;
					connectPosition2.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
					if (flag2)
					{
						connectPosition2.m_LaneData.m_Flags |= LaneFlags.Slave;
					}
					bool a = j == 0;
					bool b = j == num7 - 1 && i > 1 && i < num2;
					if (m_LeftHandTraffic)
					{
						CommonUtils.Swap(ref a, ref b);
					}
					if (num10 != j)
					{
						ConnectPosition sourcePosition = connectPosition;
						ConnectPosition targetPosition = connectPosition2;
						float3 endPos = targetPosition.m_Position;
						((float3)(ref endPos)).xz = ((float3)(ref middlePosition)).xz + val4 * num11;
						Bezier4x3 val8 = NetUtils.FitCurve(sourcePosition.m_Position, sourcePosition.m_Tangent, -targetPosition.m_Tangent, endPos);
						sourcePosition.m_Tangent = val8.b - val8.a;
						targetPosition.m_Tangent = MathUtils.Normalize(targetPosition.m_Tangent, ((float3)(ref targetPosition.m_Tangent)).xz);
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, sourcePosition, targetPosition, laneGroup, (ushort)num9, isUnsafe, isForbidden: false, isTemp, trackOnly: false, 0, ownerTemp, isTurn: false, isRight: false, isGentle: false, isUTurn: false, isRoundabout: true, a, b, isMergeLeft: false, isMergeRight: false, fixedTangents: true, roadPassThrough);
					}
					else
					{
						curviness = -1f;
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, connectPosition, connectPosition2, laneGroup, (ushort)num9, isUnsafe, isForbidden: false, isTemp, trackOnly: false, 0, ownerTemp, isTurn: false, isRight: false, isGentle: false, isUTurn: false, isRoundabout: true, a, b, isMergeLeft: false, isMergeRight: false, fixedTangents: false, roadPassThrough);
					}
				}
				if (!isTemp && (flag || flag2))
				{
					float num13 = middleRadius + (float)laneCount * 0.5f * spaceForLanes / (float)totalLaneCount;
					float num14 = middleRadius + (float)num7 * 0.5f * spaceForLanes / (float)totalLaneCount;
					((float3)(ref connectPosition.m_Position)).xz = ((float3)(ref middlePosition)).xz + val * num13;
					connectPosition.m_LaneData.m_Index = (byte)math.select(0, laneCount, flag);
					connectPosition.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
					connectPosition.m_LaneData.m_Flags |= LaneFlags.Master;
					((float3)(ref connectPosition2.m_Position)).xz = ((float3)(ref middlePosition)).xz + val4 * num14;
					connectPosition2.m_LaneData.m_Index = (byte)math.select(0, num7, flag2);
					connectPosition2.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
					connectPosition2.m_LaneData.m_Flags |= LaneFlags.Master;
					curviness = -1f;
					CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, connectPosition, connectPosition2, laneGroup, 0, isUnsafe, isForbidden: false, isTemp, trackOnly: false, 0, ownerTemp, isTurn: false, isRight: false, isGentle: false, isUTurn: false, isRoundabout: true, isLeftLimit: false, isRightLimit: false, isMergeLeft: false, isMergeRight: false, fixedTangents: false, roadPassThrough);
				}
				laneGroup++;
				float num15 = 0f;
				float3 val10;
				if (flag3)
				{
					bool flag6 = flag2;
					int yield = math.select(0, 1, totalSourceCount >= 2);
					isSkipped = false;
					for (int k = 0; k < num7; k++)
					{
						int num16 = math.select(k, num7 - k - 1, m_LeftHandTraffic);
						int num17 = math.max(0, k + math.min(0, sourceBuffer.Length - num7));
						num17 = math.select(num17, sourceBuffer.Length - num17 - 1, m_LeftHandTraffic);
						float num18 = middleRadius + ((float)(k + totalLaneCount - math.max(sourceBuffer.Length, num7)) + 0.5f) * spaceForLanes / (float)totalLaneCount;
						((float3)(ref val7)).xz = ((float3)(ref middlePosition)).xz + val5 * num18;
						float num19 = middleRadius + ((float)k + 0.5f) * spaceForLanes / (float)totalLaneCount;
						((float3)(ref connectPosition2.m_Position)).xz = ((float3)(ref middlePosition)).xz + val4 * num19;
						connectPosition2.m_LaneData.m_Index = (byte)k;
						connectPosition2.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
						if (flag2)
						{
							connectPosition2.m_LaneData.m_Flags |= LaneFlags.Slave;
						}
						bool a2 = false;
						bool b2 = k == num7 - 1 && i < num2;
						if (m_LeftHandTraffic)
						{
							CommonUtils.Swap(ref a2, ref b2);
						}
						ConnectPosition sourcePosition2 = sourceBuffer[num17];
						ConnectPosition targetPosition2 = connectPosition2;
						flag6 |= (sourcePosition2.m_LaneData.m_Flags & LaneFlags.Slave) != 0;
						PresetCurve(ref sourcePosition2, ref targetPosition2, middlePosition, val7, val6, num18, 0f, num / (float)num2, 2f);
						((Bezier4x3)(ref val9))._002Ector(sourcePosition2.m_Position, sourcePosition2.m_Position + sourcePosition2.m_Tangent, targetPosition2.m_Position + targetPosition2.m_Tangent, targetPosition2.m_Position);
						float num20 = num15;
						val10 = MathUtils.Position(val9, 0.5f);
						num15 = math.max(num20, math.distance(((float3)(ref val10)).xz, ((float3)(ref middlePosition)).xz));
						curviness = NetUtils.CalculateStartCurviness(new Curve
						{
							m_Bezier = val9,
							m_Length = 1f
						}, m_NetLaneData[sourcePosition2.m_LaneData.m_Lane].m_Width);
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, sourcePosition2, targetPosition2, laneGroup, (ushort)num16, isUnsafe: false, isForbidden: false, isTemp, trackOnly: false, yield, ownerTemp, isTurn: false, isRight: false, isGentle: false, isUTurn: false, isRoundabout: true, a2, b2, isMergeLeft: false, isMergeRight: false, fixedTangents: true, roadPassThrough);
					}
					if (flag6)
					{
						float num21 = middleRadius + ((float)(totalLaneCount - math.max(sourceBuffer.Length, num7)) + 0.5f) * spaceForLanes / (float)totalLaneCount;
						float num22 = middleRadius + ((float)(num7 - 1 + totalLaneCount - math.max(sourceBuffer.Length, num7)) + 0.5f) * spaceForLanes / (float)totalLaneCount;
						float num23 = math.lerp(num21, num22, 0.5f);
						((float3)(ref val7)).xz = ((float3)(ref middlePosition)).xz + val5 * num23;
						float num24 = middleRadius + (float)num7 * 0.5f * spaceForLanes / (float)totalLaneCount;
						((float3)(ref connectPosition2.m_Position)).xz = ((float3)(ref middlePosition)).xz + val4 * num24;
						connectPosition2.m_LaneData.m_Index = (byte)math.select(0, num7, flag2);
						connectPosition2.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
						connectPosition2.m_LaneData.m_Flags |= LaneFlags.Master;
						ConnectPosition sourcePosition3 = prevMainPosition;
						ConnectPosition targetPosition3 = connectPosition2;
						PresetCurve(ref sourcePosition3, ref targetPosition3, middlePosition, val7, val6, num23, 0f, num / (float)num2, 2f);
						curviness = -1f;
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, sourcePosition3, targetPosition3, laneGroup, 0, isUnsafe: false, isForbidden: false, isTemp, trackOnly: false, yield, ownerTemp, isTurn: false, isRight: false, isGentle: false, isUTurn: false, isRoundabout: true, isLeftLimit: false, isRightLimit: false, isMergeLeft: false, isMergeRight: false, fixedTangents: true, roadPassThrough);
					}
					laneGroup++;
				}
				if (flag4)
				{
					bool flag7 = flag;
					int yield2 = math.select(0, -1, totalSourceCount >= 2);
					isSkipped = false;
					for (int l = 0; l < targetBuffer.Length; l++)
					{
						int num25 = math.select(l, targetBuffer.Length - l - 1, m_LeftHandTraffic);
						int num26 = math.min(laneCount - 1, l + math.max(0, laneCount - targetBuffer.Length));
						float num27 = middleRadius + ((float)math.min(totalLaneCount - 1, l + math.max(0, totalLaneCount - targetBuffer.Length)) + 0.5f) * spaceForLanes / (float)totalLaneCount;
						((float3)(ref val7)).xz = ((float3)(ref middlePosition)).xz + val5 * num27;
						float num28 = middleRadius + ((float)num26 + 0.5f) * spaceForLanes / (float)totalLaneCount;
						((float3)(ref connectPosition.m_Position)).xz = ((float3)(ref middlePosition)).xz + val * num28;
						connectPosition.m_LaneData.m_Index = (byte)num26;
						connectPosition.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
						if (flag)
						{
							connectPosition.m_LaneData.m_Flags |= LaneFlags.Slave;
						}
						bool a3 = false;
						bool b3 = l == targetBuffer.Length - 1 && i > 1;
						if (m_LeftHandTraffic)
						{
							CommonUtils.Swap(ref a3, ref b3);
						}
						ConnectPosition sourcePosition4 = connectPosition;
						ConnectPosition targetPosition4 = targetBuffer[targetBuffer.Length - 1 - num25];
						flag7 |= (targetPosition4.m_LaneData.m_Flags & LaneFlags.Slave) != 0;
						PresetCurve(ref sourcePosition4, ref targetPosition4, middlePosition, val7, val6, num27, num / (float)num2, 0f, 2f);
						((Bezier4x3)(ref val11))._002Ector(sourcePosition4.m_Position, sourcePosition4.m_Position + sourcePosition4.m_Tangent, targetPosition4.m_Position + targetPosition4.m_Tangent, targetPosition4.m_Position);
						float num29 = num15;
						val10 = MathUtils.Position(val11, 0.5f);
						num15 = math.max(num29, math.distance(((float3)(ref val10)).xz, ((float3)(ref middlePosition)).xz));
						isUnsafe = l >= laneCount;
						curviness = NetUtils.CalculateEndCurviness(new Curve
						{
							m_Bezier = val11,
							m_Length = 1f
						}, m_NetLaneData[targetPosition4.m_LaneData.m_Lane].m_Width);
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, sourcePosition4, targetPosition4, laneGroup, (ushort)num25, isUnsafe, isForbidden: false, isTemp, trackOnly: false, yield2, ownerTemp, isTurn: true, !m_LeftHandTraffic, isGentle: true, isUTurn: false, isRoundabout: true, a3, b3, isUnsafe && m_LeftHandTraffic, isUnsafe && !m_LeftHandTraffic, fixedTangents: true, roadPassThrough);
					}
					if (flag7)
					{
						float num30 = middleRadius + ((float)math.min(totalLaneCount - 1, math.max(0, totalLaneCount - targetBuffer.Length)) + 0.5f) * spaceForLanes / (float)totalLaneCount;
						float num31 = middleRadius + ((float)math.min(totalLaneCount - 1, targetBuffer.Length - 1 + math.max(0, totalLaneCount - targetBuffer.Length)) + 0.5f) * spaceForLanes / (float)totalLaneCount;
						float num32 = math.lerp(num30, num31, 0.5f);
						((float3)(ref val7)).xz = ((float3)(ref middlePosition)).xz + val5 * num32;
						float num33 = middleRadius + (float)laneCount * 0.5f * spaceForLanes / (float)totalLaneCount;
						((float3)(ref connectPosition.m_Position)).xz = ((float3)(ref middlePosition)).xz + val * num33;
						connectPosition.m_LaneData.m_Index = (byte)math.select(0, laneCount, flag);
						connectPosition.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
						connectPosition.m_LaneData.m_Flags |= LaneFlags.Master;
						ConnectPosition sourcePosition5 = connectPosition;
						ConnectPosition targetPosition5 = nextMainPosition;
						PresetCurve(ref sourcePosition5, ref targetPosition5, middlePosition, val7, val6, num32, num / (float)num2, 0f, 2f);
						curviness = -1f;
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, sourcePosition5, targetPosition5, laneGroup, 0, isUnsafe: false, isForbidden: false, isTemp, trackOnly: false, yield2, ownerTemp, isTurn: true, !m_LeftHandTraffic, isGentle: true, isUTurn: false, isRoundabout: true, isLeftLimit: false, isRightLimit: false, isMergeLeft: false, isMergeRight: false, fixedTangents: true, roadPassThrough);
					}
					laneGroup++;
				}
				if (flag5)
				{
					bool flag8 = false;
					bool flag9 = false;
					int yield3 = math.select(0, 1, totalSourceCount >= 2);
					float num34 = middleRadius + ((float)totalLaneCount - 0.5f) * spaceForLanes / (float)totalLaneCount;
					num34 = math.lerp(num34, math.max(num34, num15), 0.5f);
					float2 val12 = (m_LeftHandTraffic ? MathUtils.RotateRight(val3, num * ((float)i - 0.75f) / (float)num2) : MathUtils.RotateLeft(val3, num * ((float)i - 0.75f) / (float)num2));
					float2 val13 = (m_LeftHandTraffic ? MathUtils.RotateRight(val3, num * ((float)i - 0.25f) / (float)num2) : MathUtils.RotateLeft(val3, num * ((float)i - 0.25f) / (float)num2));
					float3 centerTangent = default(float3);
					float3 centerTangent2 = default(float3);
					((float3)(ref centerTangent)).xz = (m_LeftHandTraffic ? MathUtils.Right(val12) : MathUtils.Left(val12));
					((float3)(ref centerTangent2)).xz = (m_LeftHandTraffic ? MathUtils.Right(val13) : MathUtils.Left(val13));
					int num35 = sourceBuffer.Length - 1;
					int num36 = math.select(num35, sourceBuffer.Length - num35 - 1, m_LeftHandTraffic);
					int num37 = 0;
					int num38 = math.select(num37, targetBuffer.Length - num37 - 1, m_LeftHandTraffic);
					ConnectPosition connectPosition3 = sourceBuffer[num36];
					ConnectPosition connectPosition4 = targetBuffer[num38];
					Bezier4x3 val14 = NetUtils.FitCurve(connectPosition3.m_Position, connectPosition3.m_Tangent, -connectPosition4.m_Tangent, connectPosition4.m_Position);
					float num39 = MathUtils.Distance(((Bezier4x3)(ref val14)).xz, ((float3)(ref middlePosition)).xz, ref num40);
					num34 = math.max(num34, num39);
					ConnectPosition connectPosition5 = default(ConnectPosition);
					connectPosition5.m_LaneData.m_Lane = lane.m_LaneData.m_Lane;
					connectPosition5.m_NodeComposition = lane.m_NodeComposition;
					connectPosition5.m_EdgeComposition = lane.m_EdgeComposition;
					connectPosition5.m_CompositionData = lane.m_CompositionData;
					connectPosition5.m_Owner = owner;
					connectPosition5.m_CompositionData.m_SpeedLimit = math.lerp(connectPosition.m_CompositionData.m_SpeedLimit, connectPosition2.m_CompositionData.m_SpeedLimit, 0.5f);
					connectPosition5.m_BaseHeight = math.lerp(connectPosition.m_BaseHeight, connectPosition2.m_BaseHeight, 0.5f);
					connectPosition5.m_SegmentIndex = connectPosition2.m_SegmentIndex;
					connectPosition5.m_Tangent = val6;
					connectPosition5.m_Position.y = val7.y;
					connectPosition5.m_LaneData.m_Index = (byte)(num7 + 1);
					connectPosition5.m_LaneData.m_Flags = lane.m_LaneData.m_Flags & (LaneFlags.Road | LaneFlags.Track | LaneFlags.PublicOnly | LaneFlags.HasAuxiliary);
					((float3)(ref connectPosition5.m_Position)).xz = ((float3)(ref middlePosition)).xz + val5 * num34;
					float3 centerPosition = middlePosition;
					centerPosition.y = math.lerp(connectPosition.m_Position.y, val7.y, 0.5f);
					((float3)(ref centerPosition)).xz = ((float3)(ref centerPosition)).xz + val12 * num34;
					float3 centerPosition2 = middlePosition;
					centerPosition2.y = math.lerp(val7.y, connectPosition2.m_Position.y, 0.5f);
					((float3)(ref centerPosition2)).xz = ((float3)(ref centerPosition2)).xz + val13 * num34;
					isSkipped = false;
					bool isSkipped2 = false;
					for (int m = 0; m < targetBuffer.Length; m++)
					{
						int num41 = math.select(m, targetBuffer.Length - m - 1, m_LeftHandTraffic);
						num37 = targetBuffer.Length - 1 - m;
						num38 = math.select(num37, targetBuffer.Length - num37 - 1, m_LeftHandTraffic);
						if (m == 0)
						{
							bool a4 = false;
							bool b4 = true;
							if (m_LeftHandTraffic)
							{
								CommonUtils.Swap(ref a4, ref b4);
							}
							connectPosition3 = sourceBuffer[num36];
							connectPosition4 = connectPosition5;
							flag8 |= (connectPosition3.m_LaneData.m_Flags & LaneFlags.Slave) != 0;
							connectPosition4.m_Tangent = -connectPosition4.m_Tangent;
							PresetCurve(ref connectPosition3, ref connectPosition4, middlePosition, centerPosition, centerTangent, num34, 0f, num * 0.5f / (float)num2, 2f);
							curviness = -1f;
							CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, connectPosition3, connectPosition4, laneGroup, (ushort)num41, isUnsafe: false, isForbidden: false, isTemp, trackOnly: false, yield3, ownerTemp, isTurn: true, !m_LeftHandTraffic, isGentle: false, isUTurn: false, isRoundabout: true, a4, b4, isMergeLeft: false, isMergeRight: false, fixedTangents: true, roadPassThrough);
						}
						bool a5 = false;
						bool b5 = m == targetBuffer.Length - 1;
						if (m_LeftHandTraffic)
						{
							CommonUtils.Swap(ref a5, ref b5);
						}
						connectPosition3 = connectPosition5;
						connectPosition4 = targetBuffer[num38];
						flag9 |= (connectPosition4.m_LaneData.m_Flags & LaneFlags.Slave) != 0;
						PresetCurve(ref connectPosition3, ref connectPosition4, middlePosition, centerPosition2, centerTangent2, num34, num * 0.5f / (float)num2, 0f, 2f);
						isUnsafe = num37 != 0;
						curviness = -1f;
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped2, owner, laneBuffer, middleConnections, connectPosition3, connectPosition4, laneGroup + 1, (ushort)num41, isUnsafe, isForbidden: false, isTemp, trackOnly: false, 0, ownerTemp, isTurn: true, !m_LeftHandTraffic, isGentle: false, isUTurn: false, isRoundabout: true, a5, b5, isUnsafe && !m_LeftHandTraffic, isUnsafe && m_LeftHandTraffic, fixedTangents: true, roadPassThrough);
					}
					if (flag8)
					{
						connectPosition3 = prevMainPosition;
						connectPosition4 = connectPosition5;
						connectPosition4.m_Tangent = -connectPosition4.m_Tangent;
						PresetCurve(ref connectPosition3, ref connectPosition4, middlePosition, centerPosition, centerTangent, num34, 0f, num * 0.5f / (float)num2, 2f);
						curviness = -1f;
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, connectPosition3, connectPosition4, laneGroup, 0, isUnsafe: false, isForbidden: false, isTemp, trackOnly: false, yield3, ownerTemp, isTurn: true, !m_LeftHandTraffic, isGentle: false, isUTurn: false, isRoundabout: true, isLeftLimit: false, isRightLimit: false, isMergeLeft: false, isMergeRight: false, fixedTangents: true, roadPassThrough);
					}
					if (flag9)
					{
						connectPosition3 = connectPosition5;
						connectPosition4 = nextMainPosition;
						PresetCurve(ref connectPosition3, ref connectPosition4, middlePosition, centerPosition2, centerTangent2, num34, num * 0.5f / (float)num2, 0f, 2f);
						curviness = -1f;
						CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped2, owner, laneBuffer, middleConnections, connectPosition3, connectPosition4, laneGroup + 1, 0, isUnsafe: false, isForbidden: false, isTemp, trackOnly: false, 0, ownerTemp, isTurn: true, !m_LeftHandTraffic, isGentle: false, isUTurn: false, isRoundabout: true, isLeftLimit: false, isRightLimit: false, isMergeLeft: false, isMergeRight: false, fixedTangents: true, roadPassThrough);
					}
					laneGroup += 2u;
				}
				val = val4;
				connectPosition = connectPosition2;
				connectPosition.m_Tangent = -connectPosition.m_Tangent;
				laneCount = num7;
			}
		}

		private void PresetCurve(ref ConnectPosition sourcePosition, ref ConnectPosition targetPosition, float3 middlePosition, float3 centerPosition, float3 centerTangent, float middleOffset, float startAngle, float endAngle, float smoothness)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x3 val = NetUtils.FitCurve(sourcePosition.m_Position, sourcePosition.m_Tangent, centerTangent, centerPosition);
			Bezier4x3 val2 = NetUtils.FitCurve(centerPosition, centerTangent, -targetPosition.m_Tangent, targetPosition.m_Position);
			float2 val3 = smoothness * new float2(0.425f, 0.075f);
			if (startAngle > 0f)
			{
				float num = math.distance(((float3)(ref targetPosition.m_Position)).xz, ((float3)(ref middlePosition)).xz) - middleOffset;
				float num2 = math.max(math.distance(((float3)(ref val.a)).xz, ((float3)(ref val.b)).xz) * 2f, middleOffset * math.tan(startAngle / 2f) - num * val3.y);
				float num3 = math.max(math.distance(((float3)(ref val2.d)).xz, ((float3)(ref val2.c)).xz) * smoothness, num * val3.x);
				sourcePosition.m_Tangent = MathUtils.Normalize(sourcePosition.m_Tangent, ((float3)(ref sourcePosition.m_Tangent)).xz) * num2;
				targetPosition.m_Tangent = MathUtils.Normalize(targetPosition.m_Tangent, ((float3)(ref targetPosition.m_Tangent)).xz) * num3;
			}
			else if (endAngle > 0f)
			{
				float num4 = math.distance(((float3)(ref sourcePosition.m_Position)).xz, ((float3)(ref middlePosition)).xz) - middleOffset;
				float num5 = math.max(math.distance(((float3)(ref val.a)).xz, ((float3)(ref val.b)).xz) * smoothness, num4 * val3.x);
				float num6 = math.max(math.distance(((float3)(ref val2.d)).xz, ((float3)(ref val2.c)).xz) * 2f, middleOffset * math.tan(endAngle / 2f) - num4 * val3.y);
				sourcePosition.m_Tangent = MathUtils.Normalize(sourcePosition.m_Tangent, ((float3)(ref sourcePosition.m_Tangent)).xz) * num5;
				targetPosition.m_Tangent = MathUtils.Normalize(targetPosition.m_Tangent, ((float3)(ref targetPosition.m_Tangent)).xz) * num6;
			}
		}

		private void ExtractNextConnectPosition(ConnectPosition prevPosition, float3 middlePosition, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, out ConnectPosition nextPosition, out bool nextIsSource)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.normalizesafe(((float3)(ref prevPosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
			float num = float.MaxValue;
			nextPosition = default(ConnectPosition);
			nextIsSource = false;
			int num2 = -1;
			int num3 = -1;
			if (sourceBuffer.Length + targetBuffer.Length == 1)
			{
				if (sourceBuffer.Length == 1)
				{
					nextPosition = sourceBuffer[0];
					num2 = 0;
				}
				else
				{
					nextPosition = targetBuffer[0];
					num3 = 0;
				}
			}
			else
			{
				for (int i = 0; i < targetBuffer.Length; i++)
				{
					ConnectPosition connectPosition = targetBuffer[i];
					if (connectPosition.m_GroupIndex != prevPosition.m_GroupIndex)
					{
						float2 val2 = math.normalizesafe(((float3)(ref connectPosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
						float num4 = (m_LeftHandTraffic ? MathUtils.RotationAngleRight(val, val2) : MathUtils.RotationAngleLeft(val, val2));
						if (num4 < num)
						{
							num = num4;
							nextPosition = connectPosition;
							num3 = i;
						}
					}
				}
				for (int j = 0; j < sourceBuffer.Length; j++)
				{
					ConnectPosition connectPosition2 = sourceBuffer[j];
					if (connectPosition2.m_GroupIndex != prevPosition.m_GroupIndex)
					{
						float2 val3 = math.normalizesafe(((float3)(ref connectPosition2.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
						float num5 = (m_LeftHandTraffic ? MathUtils.RotationAngleRight(val, val3) : MathUtils.RotationAngleLeft(val, val3));
						if (num5 < num)
						{
							num = num5;
							nextPosition = connectPosition2;
							num2 = j;
						}
					}
				}
			}
			if (num2 >= 0)
			{
				sourceBuffer.RemoveAtSwapBack(num2);
				nextIsSource = true;
			}
			else if (num3 >= 0)
			{
				targetBuffer.RemoveAtSwapBack(num3);
				nextIsSource = false;
			}
		}

		private bool GetRoundaboutLane(NativeList<ConnectPosition> buffer, float roundaboutSize, ref ConnectPosition roundaboutLane, ref int laneCount, ref float laneWidth, ref bool isPublicOnly, ref float spaceForLanes, bool isSource, bool preferHighway)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			if (buffer.Length > 0)
			{
				ConnectPosition connectPosition = buffer[0];
				NetCompositionData netCompositionData = m_PrefabCompositionData[connectPosition.m_NodeComposition];
				float num = (connectPosition.m_IsEnd ? netCompositionData.m_RoundaboutSize.y : netCompositionData.m_RoundaboutSize.x);
				float num2 = roundaboutSize - num;
				for (int i = 0; i < buffer.Length; i++)
				{
					ConnectPosition connectPosition2 = buffer[i];
					Entity val = connectPosition2.m_LaneData.m_Lane;
					if ((connectPosition2.m_LaneData.m_Flags & LaneFlags.Track) != 0 && m_CarLaneData.HasComponent(val))
					{
						CarLaneData carLaneData = m_CarLaneData[val];
						if (carLaneData.m_NotTrackLanePrefab != Entity.Null)
						{
							val = carLaneData.m_NotTrackLanePrefab;
						}
					}
					NetLaneData netLaneData = m_NetLaneData[val];
					num2 = ((!isSource) ? math.max(num2, netLaneData.m_Width * 1.3333334f) : (num2 + netLaneData.m_Width * 1.3333334f));
					if ((connectPosition2.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0 == preferHighway)
					{
						bool flag = (netLaneData.m_Flags & LaneFlags.PublicOnly) != 0;
						if ((isPublicOnly && !flag) | ((isPublicOnly == flag) & (netLaneData.m_Width < laneWidth)))
						{
							laneWidth = netLaneData.m_Width;
							isPublicOnly = flag;
							roundaboutLane = connectPosition2;
						}
					}
				}
				int num3 = math.select(1, buffer.Length, isSource);
				result = num3 > laneCount;
				laneCount = math.max(laneCount, num3);
				spaceForLanes = math.min(spaceForLanes, num2);
			}
			return result;
		}

		private unsafe void FillOldLaneBuffer(bool isEdge, bool isNode, Entity owner, DynamicBuffer<SubLane> lanes, NativeHashMap<LaneKey, Entity> laneBuffer)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			StackList<PathNode> val = StackList<PathNode>.op_Implicit(new Span<PathNode>((void*)stackalloc PathNode[256], 256));
			StackList<PathNode> val2 = StackList<PathNode>.op_Implicit(new Span<PathNode>((void*)stackalloc PathNode[256], 256));
			StackList<bool> val3 = StackList<bool>.op_Implicit(new Span<bool>((void*)stackalloc byte[256], 256));
			if (isNode)
			{
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_EdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				DynamicBuffer<SubLane> val4 = default(DynamicBuffer<SubLane>);
				EdgeLane edgeLane = default(EdgeLane);
				while (edgeIterator.GetNext(out value))
				{
					if (!m_SubLanes.TryGetBuffer(value.m_Edge, ref val4))
					{
						continue;
					}
					float num = math.select(0f, 1f, value.m_End);
					int num2 = math.select(0, 4, value.m_End);
					for (int i = 0; i < val4.Length; i++)
					{
						Entity subLane = val4[i].m_SubLane;
						if ((val4[i].m_PathMethods & (PathMethod.Pedestrian | PathMethod.Road)) == 0 || m_SecondaryLaneData.HasComponent(subLane) || !m_EdgeLaneData.TryGetComponent(subLane, ref edgeLane))
						{
							continue;
						}
						bool2 val5 = edgeLane.m_EdgeDelta == num;
						if (math.any(val5))
						{
							Lane lane = m_LaneData[subLane];
							PathNode pathNode = (val5.x ? lane.m_StartNode : lane.m_EndNode);
							PathNode middleNode = lane.m_MiddleNode;
							middleNode.SetSegmentIndex((byte)num2);
							if (!middleNode.Equals(pathNode) && val.Length < val.Capacity)
							{
								pathNode.SetOwner(owner);
								val.AddNoResize(pathNode);
								val2.AddNoResize(middleNode);
								val3.AddNoResize(val5.y);
							}
						}
					}
				}
			}
			EdgeLane edgeLane2 = default(EdgeLane);
			for (int j = 0; j < lanes.Length; j++)
			{
				Entity subLane2 = lanes[j].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane2))
				{
					continue;
				}
				LaneFlags laneFlags = (LaneFlags)0;
				if (m_MasterLaneData.HasComponent(subLane2))
				{
					laneFlags |= LaneFlags.Master;
				}
				if (m_SlaveLaneData.HasComponent(subLane2))
				{
					laneFlags |= LaneFlags.Slave;
				}
				Lane lane2 = m_LaneData[subLane2];
				if (isEdge)
				{
					if ((lanes[j].m_PathMethods & (PathMethod.Pedestrian | PathMethod.Road | PathMethod.Track)) != 0 && m_EdgeLaneData.TryGetComponent(subLane2, ref edgeLane2))
					{
						bool4 val6 = ((float2)(ref edgeLane2.m_EdgeDelta)).xyxy == new float4(0f, 0f, 1f, 1f);
						if (val6.x | val6.z)
						{
							lane2.m_StartNode = lane2.m_MiddleNode;
							lane2.m_StartNode.SetSegmentIndex((byte)math.select(0, 4, val6.z));
						}
						if (val6.y | val6.w)
						{
							lane2.m_EndNode = lane2.m_MiddleNode;
							lane2.m_EndNode.SetSegmentIndex((byte)math.select(0, 4, val6.w));
						}
					}
				}
				else if (isNode)
				{
					if ((lanes[j].m_PathMethods & PathMethod.Pedestrian) != 0)
					{
						for (int k = 0; k < val.Length; k++)
						{
							if (val[k].Equals(lane2.m_StartNode))
							{
								lane2.m_StartNode = new PathNode(lane2.m_StartNode, 0.5f);
							}
							else if (val[k].Equals(lane2.m_EndNode))
							{
								lane2.m_EndNode = new PathNode(lane2.m_EndNode, 0.5f);
							}
						}
					}
					else if ((lanes[j].m_PathMethods & PathMethod.Road) != 0)
					{
						for (int l = 0; l < val.Length; l++)
						{
							if (val[l].Equals(lane2.m_StartNode) && val3[l])
							{
								lane2.m_StartNode = val2[l];
							}
							else if (val[l].Equals(lane2.m_EndNode) && !val3[l])
							{
								lane2.m_EndNode = val2[l];
							}
						}
					}
				}
				LaneKey laneKey = new LaneKey(lane2, m_PrefabRefData[subLane2].m_Prefab, laneFlags);
				laneBuffer.TryAdd(laneKey, subLane2);
			}
		}

		private unsafe void RemoveUnusedOldLanes(int jobIndex, Entity owner, DynamicBuffer<SubLane> lanes, NativeHashMap<LaneKey, Entity> laneBuffer)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			int length = lanes.Length;
			StackList<Entity> val = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[length], length));
			Enumerator<LaneKey, Entity> enumerator = laneBuffer.GetEnumerator();
			while (enumerator.MoveNext())
			{
				val.AddNoResize(enumerator.Current.Value);
			}
			enumerator.Dispose();
			laneBuffer.Clear();
			if (val.Length != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val.AsArray(), ref m_AppliedTypes);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, val.AsArray());
			}
		}

		private void UpdateLanes(int jobIndex, NativeList<Entity> laneBuffer)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (laneBuffer.Length != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, laneBuffer.AsArray());
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, laneBuffer.AsArray());
			}
		}

		private void CreateEdgeConnectionLanes(int jobIndex, ref int edgeLaneIndex, ref int connectionIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, NativeList<ConnectPosition> tempBuffer1, NativeList<ConnectPosition> tempBuffer2, Entity composition, EdgeGeometry geometryData, Curve curve, bool isSingleCurve, bool isTemp, Temp ownerTemp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0750: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition];
			CompositionData compositionData = GetCompositionData(composition);
			DynamicBuffer<NetCompositionLane> prefabCompositionLanes = m_PrefabCompositionLanes[composition];
			int num = -1;
			for (int i = 0; i < sourceBuffer.Length; i++)
			{
				ConnectPosition connectPosition = sourceBuffer[i];
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Road) == 0)
				{
					continue;
				}
				float3 val = MathUtils.Position(curve.m_Bezier, connectPosition.m_CurvePosition);
				float3 val2 = MathUtils.Tangent(curve.m_Bezier, connectPosition.m_CurvePosition);
				float2 val3 = MathUtils.Right(((float3)(ref val2)).xz);
				MathUtils.TryNormalize(ref val3);
				float3 val4 = connectPosition.m_Position - val;
				val4.x = math.dot(val3, ((float3)(ref val4)).xz);
				float num2 = connectPosition.m_LaneData.m_Position.y + connectPosition.m_Elevation;
				if (m_ElevationData.HasComponent(owner))
				{
					num2 = ((!(val4.x > 0f)) ? (num2 - m_ElevationData[owner].m_Elevation.x) : (num2 - m_ElevationData[owner].m_Elevation.y));
				}
				int num3 = FindBestConnectionLane(prefabCompositionLanes, ((float3)(ref val4)).xy, num2, LaneFlags.Road, connectPosition.m_LaneData.m_Flags);
				if (num3 != -1)
				{
					if (connectPosition.m_GroupIndex != num)
					{
						num = connectPosition.m_GroupIndex;
					}
					else
					{
						connectionIndex--;
					}
					CreateCarEdgeConnections(jobIndex, ref edgeLaneIndex, ref random, owner, laneBuffer, geometryData, prefabCompositionData, compositionData, connectPosition, ((float3)(ref val4)).xy, connectionIndex++, isSingleCurve, isSource: true, isTemp, ownerTemp, prefabCompositionLanes, num3);
				}
			}
			num = -1;
			for (int j = 0; j < targetBuffer.Length; j++)
			{
				ConnectPosition connectPosition2 = targetBuffer[j];
				float3 val5 = MathUtils.Position(curve.m_Bezier, connectPosition2.m_CurvePosition);
				float3 val6 = MathUtils.Tangent(curve.m_Bezier, connectPosition2.m_CurvePosition);
				float2 val7 = MathUtils.Right(((float3)(ref val6)).xz);
				MathUtils.TryNormalize(ref val7);
				float3 val8 = connectPosition2.m_Position - val5;
				val8.x = math.dot(val7, ((float3)(ref val8)).xz);
				float num4 = connectPosition2.m_LaneData.m_Position.y + connectPosition2.m_Elevation;
				if (m_ElevationData.HasComponent(owner))
				{
					num4 = ((!(val8.x > 0f)) ? (num4 - m_ElevationData[owner].m_Elevation.x) : (num4 - m_ElevationData[owner].m_Elevation.y));
				}
				if ((connectPosition2.m_LaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					int num5 = FindBestConnectionLane(prefabCompositionLanes, ((float3)(ref val8)).xy, num4, LaneFlags.Pedestrian, connectPosition2.m_LaneData.m_Flags);
					if (num5 != -1)
					{
						NetCompositionLane prefabCompositionLaneData = prefabCompositionLanes[num5];
						CreateEdgeConnectionLane(jobIndex, ref edgeLaneIndex, ref random, owner, laneBuffer, geometryData.m_Start, geometryData.m_End, prefabCompositionData, compositionData, prefabCompositionLaneData, connectPosition2, connectionIndex++, isSingleCurve, useGroundPosition: false, isSource: false, isTemp, ownerTemp);
					}
				}
				if ((connectPosition2.m_LaneData.m_Flags & LaneFlags.Road) == 0)
				{
					continue;
				}
				int num6 = FindBestConnectionLane(prefabCompositionLanes, ((float3)(ref val8)).xy, num4, LaneFlags.Road, connectPosition2.m_LaneData.m_Flags);
				if (num6 != -1)
				{
					if (connectPosition2.m_GroupIndex != num)
					{
						num = connectPosition2.m_GroupIndex;
					}
					else
					{
						connectionIndex--;
					}
					CreateCarEdgeConnections(jobIndex, ref edgeLaneIndex, ref random, owner, laneBuffer, geometryData, prefabCompositionData, compositionData, connectPosition2, ((float3)(ref val8)).xy, connectionIndex++, isSingleCurve, isSource: false, isTemp, ownerTemp, prefabCompositionLanes, num6);
				}
			}
			UtilityTypes utilityTypes = FilterUtilityConnectPositions(targetBuffer, tempBuffer1);
			UtilityTypes utilityTypes2 = UtilityTypes.WaterPipe;
			while (utilityTypes != UtilityTypes.None)
			{
				if ((utilityTypes & utilityTypes2) != UtilityTypes.None)
				{
					utilityTypes = (UtilityTypes)((uint)utilityTypes & (uint)(byte)(~(int)utilityTypes2));
					FilterUtilityConnectPositions(utilityTypes2, tempBuffer1, tempBuffer2);
					if (tempBuffer2.Length != 0)
					{
						int4 val9 = int4.op_Implicit(-1);
						int4 val10 = int4.op_Implicit(-1);
						for (int k = 0; k < tempBuffer2.Length; k++)
						{
							ConnectPosition connectPosition3 = tempBuffer2[k];
							if ((m_NetLaneData[connectPosition3.m_LaneData.m_Lane].m_Flags & LaneFlags.Underground) != 0)
							{
								((int4)(ref val10)).xy = math.select(((int4)(ref val10)).xy, int2.op_Implicit(k), new bool2(val10.x == -1, k > val10.y));
								if (val9.x != -1)
								{
									break;
								}
							}
							else
							{
								((int4)(ref val9)).xy = math.select(((int4)(ref val9)).xy, int2.op_Implicit(k), new bool2(val9.x == -1, k > val9.y));
								if (val10.x != -1)
								{
									break;
								}
							}
						}
						for (int l = 0; l < prefabCompositionLanes.Length; l++)
						{
							NetCompositionLane netCompositionLane = prefabCompositionLanes[l];
							if ((netCompositionLane.m_Flags & LaneFlags.Utility) == 0 || (m_UtilityLaneData[netCompositionLane.m_Lane].m_UtilityTypes & utilityTypes2) == 0)
							{
								continue;
							}
							if ((m_NetLaneData[netCompositionLane.m_Lane].m_Flags & LaneFlags.Underground) != 0)
							{
								((int4)(ref val10)).zw = math.select(((int4)(ref val10)).zw, int2.op_Implicit(l), new bool2(val10.z == -1, l > val10.w));
								if (val9.z != -1)
								{
									break;
								}
							}
							else
							{
								((int4)(ref val9)).zw = math.select(((int4)(ref val9)).zw, int2.op_Implicit(l), new bool2(val9.z == -1, l > val9.w));
								if (val10.z != -1)
								{
									break;
								}
							}
						}
						val9 = math.select(val9, val10, (val9 == -1) | (math.any(val9 == -1) & math.all(val10 != -1)));
						if (math.all(((int4)(ref val9)).xz != -1))
						{
							ConnectPosition connectPosition4 = tempBuffer2[val9.x];
							NetLaneData netLaneData = m_NetLaneData[connectPosition4.m_LaneData.m_Lane];
							UtilityLaneData utilityLaneData = m_UtilityLaneData[connectPosition4.m_LaneData.m_Lane];
							NetCompositionLane prefabCompositionLaneData2 = prefabCompositionLanes[val9.z];
							UtilityLaneData utilityLaneData2 = m_UtilityLaneData[prefabCompositionLaneData2.m_Lane];
							NetLaneData netLaneData2 = m_NetLaneData[prefabCompositionLaneData2.m_Lane];
							bool useGroundPosition = false;
							if (((netLaneData.m_Flags ^ netLaneData2.m_Flags) & LaneFlags.Underground) != 0)
							{
								if ((netLaneData.m_Flags & LaneFlags.Underground) != 0)
								{
									useGroundPosition = true;
									connectPosition4.m_LaneData.m_Lane = GetConnectionLanePrefab(connectPosition4.m_LaneData.m_Lane, utilityLaneData, utilityLaneData2.m_VisualCapacity < utilityLaneData.m_VisualCapacity, wantLarger: false);
								}
								else
								{
									GetGroundPosition(connectPosition4.m_Owner, math.select(0f, 1f, connectPosition4.m_IsEnd), ref connectPosition4.m_Position);
									connectPosition4.m_LaneData.m_Lane = GetConnectionLanePrefab(prefabCompositionLaneData2.m_Lane, utilityLaneData2, utilityLaneData.m_VisualCapacity < utilityLaneData2.m_VisualCapacity, utilityLaneData.m_VisualCapacity > utilityLaneData2.m_VisualCapacity);
								}
							}
							else
							{
								val9.y++;
								connectPosition4.m_Position = CalculateUtilityConnectPosition(tempBuffer2, ((int4)(ref val9)).xy);
								if (utilityLaneData.m_VisualCapacity < utilityLaneData2.m_VisualCapacity)
								{
									connectPosition4.m_LaneData.m_Lane = GetConnectionLanePrefab(connectPosition4.m_LaneData.m_Lane, utilityLaneData, wantSmaller: false, wantLarger: false);
								}
								else
								{
									connectPosition4.m_LaneData.m_Lane = GetConnectionLanePrefab(prefabCompositionLaneData2.m_Lane, utilityLaneData2, wantSmaller: false, utilityLaneData.m_VisualCapacity > utilityLaneData2.m_VisualCapacity);
								}
							}
							CreateEdgeConnectionLane(jobIndex, ref edgeLaneIndex, ref random, owner, laneBuffer, geometryData.m_Start, geometryData.m_End, prefabCompositionData, compositionData, prefabCompositionLaneData2, connectPosition4, connectionIndex++, isSingleCurve, useGroundPosition, isSource: false, isTemp, ownerTemp);
						}
						tempBuffer2.Clear();
					}
				}
				utilityTypes2 = (UtilityTypes)((uint)utilityTypes2 << 1);
			}
			tempBuffer1.Clear();
		}

		private void GetGroundPosition(Entity entity, float curvePosition, ref float3 position)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			if (m_NodeData.HasComponent(entity))
			{
				position = m_NodeData[entity].m_Position;
			}
			else if (m_CurveData.HasComponent(entity))
			{
				position = MathUtils.Position(m_CurveData[entity].m_Bezier, curvePosition);
			}
			if (m_ElevationData.HasComponent(entity))
			{
				position.y -= math.csum(m_ElevationData[entity].m_Elevation) * 0.5f;
			}
		}

		private Entity GetConnectionLanePrefab(Entity lanePrefab, UtilityLaneData utilityLaneData, bool wantSmaller, bool wantLarger)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			Entity val = lanePrefab;
			UtilityLaneData utilityLaneData2 = default(UtilityLaneData);
			if (m_UtilityLaneData.TryGetComponent(utilityLaneData.m_LocalConnectionPrefab, ref utilityLaneData2))
			{
				if ((wantSmaller && utilityLaneData2.m_VisualCapacity < utilityLaneData.m_VisualCapacity) || (wantLarger && utilityLaneData2.m_VisualCapacity > utilityLaneData.m_VisualCapacity) || (!wantSmaller && !wantLarger && utilityLaneData2.m_VisualCapacity == utilityLaneData.m_VisualCapacity))
				{
					return utilityLaneData.m_LocalConnectionPrefab;
				}
				if (utilityLaneData2.m_VisualCapacity == utilityLaneData.m_VisualCapacity)
				{
					val = utilityLaneData.m_LocalConnectionPrefab;
				}
			}
			if (m_UtilityLaneData.TryGetComponent(utilityLaneData.m_LocalConnectionPrefab2, ref utilityLaneData2))
			{
				if ((wantSmaller && utilityLaneData2.m_VisualCapacity < utilityLaneData.m_VisualCapacity) || (wantLarger && utilityLaneData2.m_VisualCapacity > utilityLaneData.m_VisualCapacity) || (!wantSmaller && !wantLarger && utilityLaneData2.m_VisualCapacity == utilityLaneData.m_VisualCapacity))
				{
					return utilityLaneData.m_LocalConnectionPrefab2;
				}
				if (val == lanePrefab && utilityLaneData2.m_VisualCapacity == utilityLaneData.m_VisualCapacity)
				{
					val = utilityLaneData.m_LocalConnectionPrefab2;
				}
			}
			return val;
		}

		private void FindAnchors(Entity owner, NativeParallelHashSet<Entity> anchorPrefabs)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TransformData.HasComponent(owner))
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[owner];
			if (m_PrefabSubLanes.HasBuffer(prefabRef.m_Prefab))
			{
				DynamicBuffer<Game.Prefabs.SubLane> val = m_PrefabSubLanes[prefabRef.m_Prefab];
				for (int i = 0; i < val.Length; i++)
				{
					anchorPrefabs.Add(val[i].m_Prefab);
				}
			}
		}

		private void FindAnchors(Entity owner, float order, NativeList<LaneAnchor> anchors)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TransformData.HasComponent(owner))
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[owner];
			Transform transform = m_TransformData[owner];
			if (!m_PrefabSubLanes.HasBuffer(prefabRef.m_Prefab))
			{
				return;
			}
			DynamicBuffer<Game.Prefabs.SubLane> val = m_PrefabSubLanes[prefabRef.m_Prefab];
			for (int i = 0; i < val.Length; i++)
			{
				Game.Prefabs.SubLane subLane = val[i];
				if (subLane.m_NodeIndex.x != subLane.m_NodeIndex.y)
				{
					LaneAnchor laneAnchor = new LaneAnchor
					{
						m_Prefab = subLane.m_Prefab,
						m_Position = ObjectUtils.LocalToWorld(transform, subLane.m_Curve.a),
						m_Order = order + 1f,
						m_PathNode = new PathNode(owner, (ushort)subLane.m_NodeIndex.x)
					};
					anchors.Add(ref laneAnchor);
					laneAnchor = new LaneAnchor
					{
						m_Prefab = subLane.m_Prefab,
						m_Position = ObjectUtils.LocalToWorld(transform, subLane.m_Curve.d),
						m_Order = order + 1f,
						m_PathNode = new PathNode(owner, (ushort)subLane.m_NodeIndex.y)
					};
					anchors.Add(ref laneAnchor);
				}
				else
				{
					LaneAnchor laneAnchor = new LaneAnchor
					{
						m_Prefab = subLane.m_Prefab,
						m_Position = ObjectUtils.LocalToWorld(transform, subLane.m_Curve.a),
						m_Order = order,
						m_PathNode = new PathNode(owner, (ushort)subLane.m_NodeIndex.x)
					};
					anchors.Add(ref laneAnchor);
				}
			}
		}

		private bool IsAnchored(Entity owner, ref NativeParallelHashSet<Entity> anchorPrefabs, Entity prefab)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (!anchorPrefabs.IsCreated)
			{
				anchorPrefabs = new NativeParallelHashSet<Entity>(8, AllocatorHandle.op_Implicit((Allocator)2));
				if (m_SubObjects.HasBuffer(owner))
				{
					DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[owner];
					for (int i = 0; i < val.Length; i++)
					{
						FindAnchors(val[i].m_SubObject, anchorPrefabs);
					}
				}
				if (m_OwnerData.HasComponent(owner))
				{
					FindAnchors(m_OwnerData[owner].m_Owner, anchorPrefabs);
				}
			}
			return anchorPrefabs.Contains(prefab);
		}

		private void FindAnchors(Entity node, NativeList<LaneAnchor> anchors, NativeList<LaneAnchor> tempBuffer)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			if (m_SubObjects.HasBuffer(node))
			{
				DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[node];
				for (int i = 0; i < val.Length; i++)
				{
					FindAnchors(val[i].m_SubObject, 0f, tempBuffer);
				}
			}
			if (m_OwnerData.HasComponent(node))
			{
				FindAnchors(m_OwnerData[node].m_Owner, 2f, tempBuffer);
			}
			if (tempBuffer.Length == 0)
			{
				return;
			}
			if (anchors.Length > 1)
			{
				NativeSortExtension.Sort<LaneAnchor>(anchors);
			}
			if (tempBuffer.Length > 1)
			{
				NativeSortExtension.Sort<LaneAnchor>(tempBuffer);
			}
			int num = 0;
			int num2 = 0;
			while (num < anchors.Length && num2 < tempBuffer.Length)
			{
				LaneAnchor laneAnchor = anchors[num];
				LaneAnchor laneAnchor2 = tempBuffer[num2];
				while (laneAnchor.m_Prefab.Index != laneAnchor2.m_Prefab.Index)
				{
					if (laneAnchor.m_Prefab.Index < laneAnchor2.m_Prefab.Index)
					{
						if (++num >= anchors.Length)
						{
							goto end_IL_03f5;
						}
						laneAnchor = anchors[num];
					}
					else
					{
						if (++num2 >= tempBuffer.Length)
						{
							goto end_IL_03f5;
						}
						laneAnchor2 = tempBuffer[num2];
					}
				}
				float3 val2 = laneAnchor.m_Position;
				float3 val3 = laneAnchor.m_Position;
				int j;
				for (j = num + 1; j < anchors.Length; j++)
				{
					LaneAnchor laneAnchor3 = anchors[j];
					if (laneAnchor3.m_Prefab != laneAnchor.m_Prefab)
					{
						break;
					}
					val2 += laneAnchor3.m_Position;
					val3 = laneAnchor3.m_Position;
				}
				val2 /= (float)(j - num);
				int k;
				for (k = num2 + 1; k < tempBuffer.Length && !(tempBuffer[k].m_Prefab != laneAnchor2.m_Prefab); k++)
				{
				}
				int num3 = j - num;
				int num4 = k - num2;
				if (num4 > num3)
				{
					for (int l = num2; l < k; l++)
					{
						LaneAnchor laneAnchor4 = tempBuffer[l];
						laneAnchor4.m_Order = laneAnchor4.m_Order * 10000f + math.distance(laneAnchor4.m_Position, val2);
						tempBuffer[l] = laneAnchor4;
					}
					NativeSortExtension.Sort<LaneAnchor>(tempBuffer.AsArray().GetSubArray(num2, num4));
					num4 = num3;
				}
				if (num4 > 1)
				{
					float3 val4 = val3 - laneAnchor.m_Position;
					int num5 = num2 + num4;
					for (int m = num2; m < num5; m++)
					{
						LaneAnchor laneAnchor5 = tempBuffer[m];
						laneAnchor5.m_Order = math.dot(laneAnchor5.m_Position - val2, val4);
						tempBuffer[m] = laneAnchor5;
					}
					NativeSortExtension.Sort<LaneAnchor>(tempBuffer.AsArray().GetSubArray(num2, num4));
					float num6 = (tempBuffer[num5 - 1].m_Order - tempBuffer[num2].m_Order) * 0.01f;
					int num7 = num2;
					while (num7 < num5)
					{
						LaneAnchor laneAnchor6 = tempBuffer[num7];
						int n;
						for (n = num7 + 1; n < num5 && !(tempBuffer[n].m_Order - laneAnchor6.m_Order >= num6); n++)
						{
						}
						if (n > num7 + 1)
						{
							val4 = anchors[num + n - num2 - 1].m_Position - anchors[num + num7 - num2].m_Position;
							for (int num8 = num7; num8 < n; num8++)
							{
								LaneAnchor laneAnchor7 = tempBuffer[num8];
								laneAnchor7.m_Order = math.dot(laneAnchor7.m_Position - val2, val4);
								tempBuffer[num8] = laneAnchor7;
							}
							NativeSortExtension.Sort<LaneAnchor>(tempBuffer.AsArray().GetSubArray(num7, n - num7));
						}
						num7 = n;
					}
				}
				for (int num9 = 0; num9 < num4; num9++)
				{
					anchors[num + num9] = tempBuffer[num2 + num9];
				}
				num = j;
				num2 = k;
				continue;
				end_IL_03f5:
				break;
			}
			tempBuffer.Clear();
		}

		private void CreateEdgeLanes(int jobIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, Composition composition, Edge edge, EdgeGeometry geometryData, Segment combinedSegment, bool isSingleCurve, bool isTemp, Temp ownerTemp)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07de: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0903: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0823: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_0859: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0922: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_095f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_0968: Unknown result type (might be due to invalid IL or missing references)
			//IL_096d: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_097d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_098f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_0995: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_099d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ede: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef1: Unknown result type (might be due to invalid IL or missing references)
			//IL_125a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1261: Unknown result type (might be due to invalid IL or missing references)
			//IL_126a: Unknown result type (might be due to invalid IL or missing references)
			//IL_126f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1271: Unknown result type (might be due to invalid IL or missing references)
			//IL_1272: Unknown result type (might be due to invalid IL or missing references)
			//IL_1275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_128b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ead: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1126: Unknown result type (might be due to invalid IL or missing references)
			//IL_112b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1132: Unknown result type (might be due to invalid IL or missing references)
			//IL_1137: Unknown result type (might be due to invalid IL or missing references)
			//IL_113c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1143: Unknown result type (might be due to invalid IL or missing references)
			//IL_1148: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1103: Unknown result type (might be due to invalid IL or missing references)
			//IL_110a: Unknown result type (might be due to invalid IL or missing references)
			//IL_110f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1114: Unknown result type (might be due to invalid IL or missing references)
			//IL_111b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0feb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_114c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1150: Unknown result type (might be due to invalid IL or missing references)
			//IL_1155: Unknown result type (might be due to invalid IL or missing references)
			//IL_115c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1161: Unknown result type (might be due to invalid IL or missing references)
			//IL_1166: Unknown result type (might be due to invalid IL or missing references)
			//IL_116d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1172: Unknown result type (might be due to invalid IL or missing references)
			//IL_1177: Unknown result type (might be due to invalid IL or missing references)
			//IL_1180: Unknown result type (might be due to invalid IL or missing references)
			//IL_1185: Unknown result type (might be due to invalid IL or missing references)
			//IL_118d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1194: Unknown result type (might be due to invalid IL or missing references)
			//IL_119d: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1004: Unknown result type (might be due to invalid IL or missing references)
			//IL_1009: Unknown result type (might be due to invalid IL or missing references)
			//IL_1010: Unknown result type (might be due to invalid IL or missing references)
			//IL_1015: Unknown result type (might be due to invalid IL or missing references)
			//IL_101a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1023: Unknown result type (might be due to invalid IL or missing references)
			//IL_1028: Unknown result type (might be due to invalid IL or missing references)
			//IL_1030: Unknown result type (might be due to invalid IL or missing references)
			//IL_1037: Unknown result type (might be due to invalid IL or missing references)
			//IL_1040: Unknown result type (might be due to invalid IL or missing references)
			//IL_1047: Unknown result type (might be due to invalid IL or missing references)
			//IL_104c: Unknown result type (might be due to invalid IL or missing references)
			//IL_104d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1055: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_11dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1204: Unknown result type (might be due to invalid IL or missing references)
			//IL_120d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1212: Unknown result type (might be due to invalid IL or missing references)
			//IL_121a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1221: Unknown result type (might be due to invalid IL or missing references)
			//IL_122a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1231: Unknown result type (might be due to invalid IL or missing references)
			//IL_1236: Unknown result type (might be due to invalid IL or missing references)
			//IL_1237: Unknown result type (might be due to invalid IL or missing references)
			//IL_123b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1074: Unknown result type (might be due to invalid IL or missing references)
			//IL_1079: Unknown result type (might be due to invalid IL or missing references)
			//IL_1080: Unknown result type (might be due to invalid IL or missing references)
			//IL_1085: Unknown result type (might be due to invalid IL or missing references)
			//IL_108c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1091: Unknown result type (might be due to invalid IL or missing references)
			//IL_1096: Unknown result type (might be due to invalid IL or missing references)
			//IL_109d: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_10da: Unknown result type (might be due to invalid IL or missing references)
			//IL_10de: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
			CompositionData compositionData = GetCompositionData(composition.m_Edge);
			DynamicBuffer<NetCompositionLane> prefabCompositionLanes = m_PrefabCompositionLanes[composition.m_Edge];
			NativeList<LaneAnchor> val = default(NativeList<LaneAnchor>);
			NativeList<LaneAnchor> val2 = default(NativeList<LaneAnchor>);
			NativeList<LaneAnchor> tempBuffer = default(NativeList<LaneAnchor>);
			for (int i = 0; i < prefabCompositionLanes.Length; i++)
			{
				NetCompositionLane netCompositionLane = prefabCompositionLanes[i];
				if ((netCompositionLane.m_Flags & LaneFlags.FindAnchor) != 0)
				{
					if (!val.IsCreated)
					{
						val._002Ector(8, AllocatorHandle.op_Implicit((Allocator)2));
					}
					if (!val2.IsCreated)
					{
						val2._002Ector(8, AllocatorHandle.op_Implicit((Allocator)2));
					}
					if (!tempBuffer.IsCreated)
					{
						tempBuffer._002Ector(8, AllocatorHandle.op_Implicit((Allocator)2));
					}
					LaneAnchor laneAnchor = new LaneAnchor
					{
						m_Prefab = netCompositionLane.m_Lane,
						m_Order = i,
						m_PathNode = new PathNode(owner, netCompositionLane.m_Index, 0)
					};
					LaneAnchor laneAnchor2 = new LaneAnchor
					{
						m_Prefab = netCompositionLane.m_Lane,
						m_Order = i,
						m_PathNode = new PathNode(owner, netCompositionLane.m_Index, 4)
					};
					float num = netCompositionLane.m_Position.x / math.max(1f, prefabCompositionData.m_Width) + 0.5f;
					laneAnchor.m_Position = math.lerp(geometryData.m_Start.m_Left.a, geometryData.m_Start.m_Right.a, num);
					laneAnchor2.m_Position = math.lerp(geometryData.m_End.m_Left.d, geometryData.m_End.m_Right.d, num);
					if (netCompositionLane.m_Position.z > 0.001f)
					{
						float num2 = math.distance(laneAnchor.m_Position, laneAnchor2.m_Position);
						float3 val3 = (laneAnchor2.m_Position - laneAnchor.m_Position) * (netCompositionLane.m_Position.z / math.max(netCompositionLane.m_Position.z * 4f, num2));
						ref float3 reference = ref laneAnchor.m_Position;
						reference += val3;
						ref float3 reference2 = ref laneAnchor2.m_Position;
						reference2 -= val3;
					}
					laneAnchor.m_Position.y += netCompositionLane.m_Position.y;
					laneAnchor2.m_Position.y += netCompositionLane.m_Position.y;
					val.Add(ref laneAnchor);
					val2.Add(ref laneAnchor2);
				}
			}
			if (val.IsCreated)
			{
				FindAnchors(edge.m_Start, val, tempBuffer);
			}
			if (val2.IsCreated)
			{
				FindAnchors(edge.m_End, val2, tempBuffer);
			}
			bool flag = false;
			if (isSingleCurve)
			{
				for (int j = 0; j < prefabCompositionLanes.Length; j++)
				{
					NetCompositionLane prefabCompositionLaneData = prefabCompositionLanes[j];
					flag |= (prefabCompositionLaneData.m_Flags & LaneFlags.HasAuxiliary) != 0;
					CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, combinedSegment, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData, new int2(0, 4), new float2(0f, 1f), val, val2, bool2.op_Implicit(true), isTemp, ownerTemp);
				}
			}
			else
			{
				for (int k = 0; k < prefabCompositionLanes.Length; k++)
				{
					NetCompositionLane prefabCompositionLaneData2 = prefabCompositionLanes[k];
					flag |= (prefabCompositionLaneData2.m_Flags & LaneFlags.HasAuxiliary) != 0;
					CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, geometryData.m_Start, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData2, new int2(0, 2), new float2(0f, 0.5f), val, val2, new bool2(true, false), isTemp, ownerTemp);
					CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, geometryData.m_End, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData2, new int2(2, 4), new float2(0.5f, 1f), val, val2, new bool2(false, true), isTemp, ownerTemp);
				}
			}
			if (flag)
			{
				Bounds1 val18 = default(Bounds1);
				float2 val20 = default(float2);
				Bounds1 val21 = default(Bounds1);
				Bounds1 val23 = default(Bounds1);
				Bounds1 val25 = default(Bounds1);
				float2 val27 = default(float2);
				float2 val28 = default(float2);
				for (int l = 0; l < prefabCompositionLanes.Length; l++)
				{
					NetCompositionLane netCompositionLane2 = prefabCompositionLanes[l];
					if ((netCompositionLane2.m_Flags & LaneFlags.HasAuxiliary) == 0)
					{
						continue;
					}
					DynamicBuffer<AuxiliaryNetLane> val4 = m_PrefabAuxiliaryLanes[netCompositionLane2.m_Lane];
					int num3 = 5;
					for (int m = 0; m < val4.Length; m++)
					{
						AuxiliaryNetLane auxiliaryNetLane = val4[m];
						if (!NetCompositionHelpers.TestLaneFlags(auxiliaryNetLane, prefabCompositionData.m_Flags))
						{
							continue;
						}
						bool flag2 = false;
						bool flag3 = false;
						float3 val5 = float3.op_Implicit(0f);
						if (auxiliaryNetLane.m_Spacing.x > 0.1f)
						{
							for (int n = 0; n < prefabCompositionLanes.Length; n++)
							{
								NetCompositionLane netCompositionLane3 = prefabCompositionLanes[n];
								if ((netCompositionLane3.m_Flags & LaneFlags.HasAuxiliary) == 0)
								{
									continue;
								}
								for (int num4 = 0; num4 < val4.Length; num4++)
								{
									AuxiliaryNetLane lane = val4[num4];
									if (NetCompositionHelpers.TestLaneFlags(lane, prefabCompositionData.m_Flags) && !(lane.m_Prefab != auxiliaryNetLane.m_Prefab) && !(lane.m_Spacing.x <= 0.1f) && (n != l || num4 != m))
									{
										if (n < l || (n == l && num4 < m))
										{
											flag3 = true;
											val5 = netCompositionLane3.m_Position;
											((float3)(ref val5)).xy = ((float3)(ref val5)).xy + ((float3)(ref lane.m_Position)).xy;
										}
										else
										{
											flag2 = true;
										}
									}
								}
							}
						}
						float num5 = geometryData.m_Start.middleLength + geometryData.m_End.middleLength;
						float num6 = 0f;
						int num7 = 0;
						float3 val6 = default(float3);
						float3 val7 = default(float3);
						float3 val8 = default(float3);
						float3 val9 = default(float3);
						float3 val10 = default(float3);
						float3 val11 = default(float3);
						float3 val12 = default(float3);
						float3 val13 = default(float3);
						NetCompositionLane prefabCompositionLaneData3 = netCompositionLane2;
						prefabCompositionLaneData3.m_Lane = auxiliaryNetLane.m_Prefab;
						ref float3 position = ref prefabCompositionLaneData3.m_Position;
						((float3)(ref position)).xy = ((float3)(ref position)).xy + ((float3)(ref auxiliaryNetLane.m_Position)).xy;
						prefabCompositionLaneData3.m_Flags = m_NetLaneData[auxiliaryNetLane.m_Prefab].m_Flags | auxiliaryNetLane.m_Flags;
						if ((auxiliaryNetLane.m_Flags & LaneFlags.EvenSpacing) != 0)
						{
							NetCompositionData compositionData2 = m_PrefabCompositionData[composition.m_StartNode];
							NetCompositionData compositionData3 = m_PrefabCompositionData[composition.m_EndNode];
							EdgeNodeGeometry geometry = m_StartNodeGeometryData[owner].m_Geometry;
							EdgeNodeGeometry geometry2 = m_EndNodeGeometryData[owner].m_Geometry;
							if (!NetCompositionHelpers.TestLaneFlags(auxiliaryNetLane, compositionData2.m_Flags))
							{
								float num8 = (geometry.m_Left.middleLength + geometry.m_Right.middleLength) * 0.5f;
								num8 = math.min(num8, auxiliaryNetLane.m_Spacing.z * (1f / 3f));
								num5 += num8;
								num6 -= num8;
								val6 = geometry.m_Right.m_Right.d - geometryData.m_Start.m_Left.a;
								val7 = geometry.m_Left.m_Left.d - geometryData.m_Start.m_Right.a;
							}
							else if (auxiliaryNetLane.m_Position.z > 0.1f)
							{
								float num9 = prefabCompositionLaneData3.m_Position.x / math.max(1f, prefabCompositionData.m_Width) + 0.5f;
								Bezier4x3 val14 = MathUtils.Lerp(geometryData.m_Start.m_Left, geometryData.m_Start.m_Right, num9);
								float3 val15 = -MathUtils.StartTangent(val14);
								val15 = MathUtils.Normalize(val15, ((float3)(ref val15)).xz);
								val7 = (val6 = CalculateAuxialryZOffset(val14.a, val15, geometry, compositionData2, auxiliaryNetLane));
								if (flag3)
								{
									num9 = val5.x / math.max(1f, prefabCompositionData.m_Width) + 0.5f;
									val14 = MathUtils.Lerp(geometryData.m_Start.m_Left, geometryData.m_Start.m_Right, num9);
									val15 = -MathUtils.StartTangent(val14);
									val15 = MathUtils.Normalize(val15, ((float3)(ref val15)).xz);
									val11 = (val10 = CalculateAuxialryZOffset(val14.a, val15, geometry, compositionData2, auxiliaryNetLane));
								}
							}
							if (!NetCompositionHelpers.TestLaneFlags(auxiliaryNetLane, compositionData3.m_Flags))
							{
								float num10 = (geometry2.m_Left.middleLength + geometry2.m_Right.middleLength) * 0.5f;
								num10 = math.min(num10, auxiliaryNetLane.m_Spacing.z * (1f / 3f));
								num5 += num10;
								val8 = geometry2.m_Left.m_Left.d - geometryData.m_End.m_Left.d;
								val9 = geometry2.m_Right.m_Right.d - geometryData.m_End.m_Right.d;
							}
							else if (auxiliaryNetLane.m_Position.z > 0.1f)
							{
								float num11 = prefabCompositionLaneData3.m_Position.x / math.max(1f, prefabCompositionData.m_Width) + 0.5f;
								Bezier4x3 val16 = MathUtils.Lerp(geometryData.m_End.m_Left, geometryData.m_End.m_Right, num11);
								float3 val17 = MathUtils.EndTangent(val16);
								val17 = MathUtils.Normalize(val17, ((float3)(ref val17)).xz);
								val9 = (val8 = CalculateAuxialryZOffset(val16.d, val17, geometry2, compositionData3, auxiliaryNetLane));
								if (flag3)
								{
									num11 = val5.x / math.max(1f, prefabCompositionData.m_Width) + 0.5f;
									val16 = MathUtils.Lerp(geometryData.m_End.m_Left, geometryData.m_End.m_Right, num11);
									val17 = MathUtils.EndTangent(val16);
									val17 = MathUtils.Normalize(val17, ((float3)(ref val17)).xz);
									val13 = (val12 = CalculateAuxialryZOffset(val16.d, val17, geometry2, compositionData3, auxiliaryNetLane));
								}
							}
						}
						if (auxiliaryNetLane.m_Spacing.z > 0.1f)
						{
							num7 = Mathf.FloorToInt(num5 / auxiliaryNetLane.m_Spacing.z + 0.5f);
							num7 = (((auxiliaryNetLane.m_Flags & LaneFlags.EvenSpacing) == 0) ? math.select(num7, 1, (num7 == 0) & (num5 > auxiliaryNetLane.m_Spacing.z * 0.1f)) : math.max(0, num7 - 1));
						}
						float num12;
						float num13;
						if ((auxiliaryNetLane.m_Flags & LaneFlags.EvenSpacing) != 0)
						{
							num12 = 1f;
							num13 = num5 / (float)(num7 + 1);
						}
						else
						{
							num12 = 0.5f;
							num13 = num5 / (float)num7;
						}
						float num14 = (num12 - 1f) * num13 + num6;
						if (num14 > geometryData.m_Start.middleLength)
						{
							((Bounds1)(ref val18))._002Ector(0f, 1f);
							Bezier4x3 val19 = MathUtils.Lerp(geometryData.m_End.m_Left, geometryData.m_End.m_Right, 0.5f);
							MathUtils.ClampLength(((Bezier4x3)(ref val19)).xz, ref val18, num14 - geometryData.m_Start.middleLength);
							val20.x = 1f + val18.max;
						}
						else
						{
							((Bounds1)(ref val21))._002Ector(0f, 1f);
							Bezier4x3 val22 = MathUtils.Lerp(geometryData.m_Start.m_Left, geometryData.m_Start.m_Right, 0.5f);
							MathUtils.ClampLength(((Bezier4x3)(ref val22)).xz, ref val21, num14);
							val20.x = val21.max;
						}
						for (int num15 = 0; num15 <= num7; num15++)
						{
							num14 = ((float)num15 + num12) * num13 + num6;
							if (num14 > geometryData.m_Start.middleLength)
							{
								((Bounds1)(ref val23))._002Ector(0f, 1f);
								Bezier4x3 val24 = MathUtils.Lerp(geometryData.m_End.m_Left, geometryData.m_End.m_Right, 0.5f);
								MathUtils.ClampLength(((Bezier4x3)(ref val24)).xz, ref val23, num14 - geometryData.m_Start.middleLength);
								val20.y = 1f + val23.max;
							}
							else
							{
								((Bounds1)(ref val25))._002Ector(0f, 1f);
								Bezier4x3 val26 = MathUtils.Lerp(geometryData.m_Start.m_Left, geometryData.m_Start.m_Right, 0.5f);
								MathUtils.ClampLength(((Bezier4x3)(ref val26)).xz, ref val25, num14);
								val20.y = val25.max;
							}
							Segment segment = default(Segment);
							if (val20.x >= 1f)
							{
								segment.m_Left = MathUtils.Cut(geometryData.m_End.m_Left, val20 - 1f);
								segment.m_Right = MathUtils.Cut(geometryData.m_End.m_Right, val20 - 1f);
							}
							else if (val20.y <= 1f)
							{
								segment.m_Left = MathUtils.Cut(geometryData.m_Start.m_Left, val20);
								segment.m_Right = MathUtils.Cut(geometryData.m_Start.m_Right, val20);
							}
							else
							{
								((float2)(ref val27))._002Ector(val20.x, 1f);
								((float2)(ref val28))._002Ector(0f, val20.y - 1f);
								segment.m_Left = MathUtils.Join(MathUtils.Cut(geometryData.m_Start.m_Left, val27), MathUtils.Cut(geometryData.m_End.m_Left, val28));
								segment.m_Right = MathUtils.Join(MathUtils.Cut(geometryData.m_Start.m_Right, val27), MathUtils.Cut(geometryData.m_End.m_Right, val28));
							}
							Segment segment2 = segment;
							if ((auxiliaryNetLane.m_Flags & LaneFlags.EvenSpacing) != 0)
							{
								if (num15 == 0)
								{
									ref float3 a = ref segment.m_Left.a;
									a += val6;
									ref float3 a2 = ref segment.m_Right.a;
									a2 += val7;
									ref float3 a3 = ref segment2.m_Left.a;
									a3 += val10;
									ref float3 a4 = ref segment2.m_Right.a;
									a4 += val11;
								}
								if (num15 == num7)
								{
									ref float3 d = ref segment.m_Left.d;
									d += val8;
									ref float3 d2 = ref segment.m_Right.d;
									d2 += val9;
									ref float3 d3 = ref segment2.m_Left.d;
									d3 += val12;
									ref float3 d4 = ref segment2.m_Right.d;
									d4 += val13;
								}
							}
							float2 edgeDelta = math.select(val20 * 0.5f, new float2(0f, 1f), num15 == new int2(0, num7));
							if (auxiliaryNetLane.m_Spacing.x > 0.1f)
							{
								Segment segment3 = default(Segment);
								float3 val29 = float3.op_Implicit(netCompositionLane2.m_Position.x);
								((float3)(ref val29)).xz = ((float3)(ref val29)).xz + new float2(0f - auxiliaryNetLane.m_Spacing.x, auxiliaryNetLane.m_Spacing.x);
								val29.x = math.select(val29.x, val5.x, flag3);
								val29 = math.saturate(val29 / math.max(1f, prefabCompositionData.m_Width) + 0.5f);
								float3 startPos;
								if (num15 == 0)
								{
									startPos = ((!flag3) ? math.lerp(segment.m_Left.a, segment.m_Right.a, val29.x) : math.lerp(segment2.m_Left.a, segment2.m_Right.a, val29.x));
									segment3.m_Left = NetUtils.StraightCurve(startPos, math.lerp(segment.m_Left.a, segment.m_Right.a, val29.y));
									segment3.m_Right = segment3.m_Left;
									CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, segment3, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData3, new int2(num3, num3 + 2), ((float2)(ref edgeDelta)).xx, val, val2, new bool2(!flag3, false), isTemp, ownerTemp);
									num3 += 2;
									if (!flag2)
									{
										segment3.m_Left = NetUtils.StraightCurve(segment3.m_Left.d, math.lerp(segment.m_Left.a, segment.m_Right.a, val29.z));
										segment3.m_Right = segment3.m_Left;
										CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, segment3, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData3, new int2(num3, num3 + 2), ((float2)(ref edgeDelta)).xx, val, val2, new bool2(false, true), isTemp, ownerTemp);
										num3 += 2;
									}
									num3++;
								}
								startPos = ((!flag3) ? math.lerp(segment.m_Left.d, segment.m_Right.d, val29.x) : math.lerp(segment2.m_Left.d, segment2.m_Right.d, val29.x));
								segment3.m_Left = NetUtils.StraightCurve(startPos, math.lerp(segment.m_Left.d, segment.m_Right.d, val29.y));
								segment3.m_Right = segment3.m_Left;
								CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, segment3, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData3, new int2(num3, num3 + 2), ((float2)(ref edgeDelta)).yy, val, val2, new bool2(!flag3, false), isTemp, ownerTemp);
								num3 += 2;
								if (!flag2)
								{
									segment3.m_Left = NetUtils.StraightCurve(segment3.m_Left.d, math.lerp(segment.m_Left.d, segment.m_Right.d, val29.z));
									segment3.m_Right = segment3.m_Left;
									CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, segment3, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData3, new int2(num3, num3 + 2), ((float2)(ref edgeDelta)).yy, val, val2, new bool2(false, true), isTemp, ownerTemp);
									num3 += 2;
								}
								num3++;
							}
							else
							{
								CreateEdgeLane(jobIndex, ref random, owner, laneBuffer, segment, prefabCompositionData, compositionData, prefabCompositionLanes, prefabCompositionLaneData3, new int2(num3, num3 + 2), edgeDelta, val, val2, bool2.op_Implicit(true), isTemp, ownerTemp);
								num3 += 2;
							}
							val20.x = val20.y;
						}
						if (auxiliaryNetLane.m_Spacing.x <= 0.1f)
						{
							num3++;
						}
					}
				}
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
			if (val2.IsCreated)
			{
				val2.Dispose();
			}
			if (tempBuffer.IsCreated)
			{
				tempBuffer.Dispose();
			}
		}

		private float3 CalculateAuxialryZOffset(float3 position, float3 tangent, EdgeNodeGeometry nodeGeometry, NetCompositionData compositionData, AuxiliaryNetLane auxiliaryLane)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			float num = auxiliaryLane.m_Position.z;
			if ((compositionData.m_Flags.m_General & CompositionFlags.General.DeadEnd) != 0)
			{
				num = math.max(0f, math.min(num, compositionData.m_Width * 0.125f));
			}
			else if (nodeGeometry.m_MiddleRadius > 0f)
			{
				float2 val = default(float2);
				if (MathUtils.Intersect(new Line2(((float3)(ref position)).xz, ((float3)(ref position)).xz + ((float3)(ref tangent)).xz), new Line2(((float3)(ref nodeGeometry.m_Right.m_Left.d)).xz, ((float3)(ref nodeGeometry.m_Right.m_Right.d)).xz), ref val))
				{
					num = math.max(0f, math.min(num, val.x * 0.5f));
				}
			}
			else
			{
				float2 val2 = default(float2);
				if (MathUtils.Intersect(new Line2(((float3)(ref position)).xz, ((float3)(ref position)).xz + ((float3)(ref tangent)).xz), new Line2(((float3)(ref nodeGeometry.m_Left.m_Left.d)).xz, ((float3)(ref nodeGeometry.m_Left.m_Right.d)).xz), ref val2))
				{
					num = math.max(0f, math.min(num, val2.x * 0.5f));
				}
				float2 val3 = default(float2);
				if (MathUtils.Intersect(new Line2(((float3)(ref position)).xz, ((float3)(ref position)).xz + ((float3)(ref tangent)).xz), new Line2(((float3)(ref nodeGeometry.m_Right.m_Left.d)).xz, ((float3)(ref nodeGeometry.m_Right.m_Right.d)).xz), ref val3))
				{
					num = math.max(0f, math.min(num, val3.x * 0.5f));
				}
			}
			return tangent * num;
		}

		private int FindBestConnectionLane(DynamicBuffer<NetCompositionLane> prefabCompositionLanes, float2 offset, float elevationOffset, LaneFlags laneType, LaneFlags laneFlags)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			float num = float.MaxValue;
			int result = -1;
			float2 val = default(float2);
			for (int i = 0; i < prefabCompositionLanes.Length; i++)
			{
				NetCompositionLane netCompositionLane = prefabCompositionLanes[i];
				if ((netCompositionLane.m_Flags & laneType) == 0)
				{
					continue;
				}
				if ((laneFlags & LaneFlags.Master) != 0)
				{
					if ((netCompositionLane.m_Flags & LaneFlags.Slave) != 0)
					{
						continue;
					}
				}
				else if ((netCompositionLane.m_Flags & LaneFlags.Master) != 0)
				{
					continue;
				}
				((float2)(ref val))._002Ector(offset.y, elevationOffset);
				val = math.abs(val - ((float3)(ref netCompositionLane.m_Position)).yy);
				float num2 = math.lengthsq(new float2(netCompositionLane.m_Position.x - offset.x, math.cmin(val)));
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		private void CreateCarEdgeConnections(int jobIndex, ref int edgeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, EdgeGeometry geometryData, NetCompositionData prefabCompositionData, CompositionData compositionData, ConnectPosition connectPosition, float2 offset, int connectionIndex, bool isSingleCurve, bool isSource, bool isTemp, Temp ownerTemp, DynamicBuffer<NetCompositionLane> prefabCompositionLanes, int bestIndex)
		{
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionLane netCompositionLane = prefabCompositionLanes[bestIndex];
			int num = -1;
			int num2 = 0;
			float num3 = float.MaxValue;
			for (int i = 0; i < prefabCompositionLanes.Length; i++)
			{
				NetCompositionLane prefabCompositionLaneData = prefabCompositionLanes[i];
				if ((prefabCompositionLaneData.m_Flags & LaneFlags.Road) == 0 || prefabCompositionLaneData.m_Carriageway != netCompositionLane.m_Carriageway)
				{
					continue;
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					if ((prefabCompositionLaneData.m_Flags & LaneFlags.Slave) != 0)
					{
						continue;
					}
				}
				else if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0 && (prefabCompositionLaneData.m_Flags & LaneFlags.Master) != 0)
				{
					continue;
				}
				if (m_CarLaneData[prefabCompositionLaneData.m_Lane].m_RoadTypes != RoadTypes.Car)
				{
					continue;
				}
				if (num != -1 && (prefabCompositionLaneData.m_Group != num || (prefabCompositionLaneData.m_Flags & LaneFlags.Slave) == 0))
				{
					NetCompositionLane prefabCompositionLaneData2 = prefabCompositionLanes[num2];
					CreateEdgeConnectionLane(jobIndex, ref edgeLaneIndex, ref random, owner, laneBuffer, geometryData.m_Start, geometryData.m_End, prefabCompositionData, compositionData, prefabCompositionLaneData2, connectPosition, connectionIndex, isSingleCurve, useGroundPosition: false, isSource, isTemp, ownerTemp);
					num = -1;
					num3 = float.MaxValue;
				}
				if ((prefabCompositionLaneData.m_Flags & LaneFlags.Slave) != 0)
				{
					num = prefabCompositionLaneData.m_Group;
					float num4 = math.abs(prefabCompositionLaneData.m_Position.x - offset.x);
					if (num4 < num3)
					{
						num3 = num4;
						num2 = i;
					}
				}
				else
				{
					CreateEdgeConnectionLane(jobIndex, ref edgeLaneIndex, ref random, owner, laneBuffer, geometryData.m_Start, geometryData.m_End, prefabCompositionData, compositionData, prefabCompositionLaneData, connectPosition, connectionIndex, isSingleCurve, useGroundPosition: false, isSource, isTemp, ownerTemp);
				}
			}
			if (num != -1)
			{
				NetCompositionLane prefabCompositionLaneData3 = prefabCompositionLanes[num2];
				CreateEdgeConnectionLane(jobIndex, ref edgeLaneIndex, ref random, owner, laneBuffer, geometryData.m_Start, geometryData.m_End, prefabCompositionData, compositionData, prefabCompositionLaneData3, connectPosition, connectionIndex, isSingleCurve, useGroundPosition: false, isSource, isTemp, ownerTemp);
			}
		}

		private CompositionData GetCompositionData(Entity composition)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			CompositionData result = default(CompositionData);
			if (m_RoadData.HasComponent(composition))
			{
				RoadComposition roadComposition = m_RoadData[composition];
				result.m_SpeedLimit = roadComposition.m_SpeedLimit;
				result.m_RoadFlags = roadComposition.m_Flags;
				result.m_Priority = roadComposition.m_Priority;
			}
			else if (m_TrackData.HasComponent(composition))
			{
				result.m_SpeedLimit = m_TrackData[composition].m_SpeedLimit;
			}
			else if (m_WaterwayData.HasComponent(composition))
			{
				result.m_SpeedLimit = m_WaterwayData[composition].m_SpeedLimit;
			}
			else if (m_PathwayData.HasComponent(composition))
			{
				result.m_SpeedLimit = m_PathwayData[composition].m_SpeedLimit;
			}
			else if (m_TaxiwayData.HasComponent(composition))
			{
				TaxiwayComposition taxiwayComposition = m_TaxiwayData[composition];
				result.m_SpeedLimit = taxiwayComposition.m_SpeedLimit;
				result.m_TaxiwayFlags = taxiwayComposition.m_Flags;
			}
			else
			{
				result.m_SpeedLimit = 1f;
			}
			return result;
		}

		private NetCompositionLane FindClosestLane(DynamicBuffer<NetCompositionLane> prefabCompositionLanes, LaneFlags all, LaneFlags none, float3 position, int carriageWay = -1)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			float num = float.MaxValue;
			NetCompositionLane result = default(NetCompositionLane);
			if (!prefabCompositionLanes.IsCreated)
			{
				return result;
			}
			for (int i = 0; i < prefabCompositionLanes.Length; i++)
			{
				NetCompositionLane netCompositionLane = prefabCompositionLanes[i];
				if ((netCompositionLane.m_Flags & (all | none)) == all && (carriageWay == -1 || netCompositionLane.m_Carriageway == carriageWay))
				{
					float num2 = math.lengthsq(netCompositionLane.m_Position - position);
					if (num2 < num)
					{
						num = num2;
						result = netCompositionLane;
					}
				}
			}
			return result;
		}

		private static void Invert(ref Lane laneData, ref Curve curveData, ref EdgeLane edgeLaneData)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			PathNode startNode = laneData.m_StartNode;
			laneData.m_StartNode = laneData.m_EndNode;
			laneData.m_EndNode = startNode;
			curveData.m_Bezier = MathUtils.Invert(curveData.m_Bezier);
			edgeLaneData.m_EdgeDelta = ((float2)(ref edgeLaneData.m_EdgeDelta)).yx;
		}

		private void CreateNodeConnectionLanes(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<MiddleConnection> middleConnections, NativeList<ConnectPosition> tempBuffer, bool isRoundabout, bool isTemp, Temp ownerTemp)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < middleConnections.Length; i++)
			{
				MiddleConnection middleConnection = middleConnections[i];
				if (middleConnection.m_TargetLane != Entity.Null)
				{
					middleConnections[num++] = middleConnection;
				}
			}
			middleConnections.RemoveRange(num, middleConnections.Length - num);
			if (middleConnections.Length >= 2)
			{
				NativeSortExtension.Sort<MiddleConnection, MiddleConnectionComparer>(middleConnections, default(MiddleConnectionComparer));
			}
			for (int j = 0; j < middleConnections.Length; j++)
			{
				MiddleConnection middleConnection2 = middleConnections[j];
				if ((middleConnection2.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
				{
					int4 val = int4.op_Implicit(-1);
					int4 val2 = int4.op_Implicit(-1);
					for (int k = j; k < middleConnections.Length; k++)
					{
						MiddleConnection middleConnection3 = middleConnections[k];
						if ((middleConnection3.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Utility) == 0 || (middleConnection3.m_ConnectPosition.m_UtilityTypes & middleConnection2.m_ConnectPosition.m_UtilityTypes) == 0 || middleConnection3.m_SourceNode != middleConnection2.m_SourceNode)
						{
							break;
						}
						tempBuffer.Add(ref middleConnection3.m_ConnectPosition);
						if ((middleConnection3.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Underground) != 0)
						{
							((int4)(ref val2)).xy = math.select(((int4)(ref val2)).xy, int2.op_Implicit(j), new bool2(val2.x == -1, j > val2.y));
						}
						else
						{
							((int4)(ref val)).xy = math.select(((int4)(ref val)).xy, int2.op_Implicit(j), new bool2(val.x == -1, j > val.y));
						}
						if ((middleConnection3.m_TargetFlags & LaneFlags.Underground) != 0)
						{
							((int4)(ref val2)).zw = math.select(((int4)(ref val2)).zw, int2.op_Implicit(j), new bool2(val2.z == -1, j > val2.w));
						}
						else
						{
							((int4)(ref val)).zw = math.select(((int4)(ref val)).zw, int2.op_Implicit(j), new bool2(val.z == -1, j > val.w));
						}
					}
					j += math.max(0, tempBuffer.Length - 1);
					val = math.select(val, val2, (val == -1) | (math.any(val == -1) & math.all(val2 != -1)));
					if (math.all(((int4)(ref val)).xz != -1))
					{
						middleConnection2 = middleConnections[val.x];
						MiddleConnection middleConnection4 = middleConnections[val.z];
						UtilityLaneData utilityLaneData = m_UtilityLaneData[middleConnection2.m_ConnectPosition.m_LaneData.m_Lane];
						UtilityLaneData utilityLaneData2 = m_UtilityLaneData[middleConnection4.m_TargetLane];
						bool useGroundPosition = false;
						if (((middleConnection2.m_ConnectPosition.m_LaneData.m_Flags ^ middleConnection4.m_TargetFlags) & LaneFlags.Underground) != 0)
						{
							if ((middleConnection2.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Underground) != 0)
							{
								useGroundPosition = true;
								middleConnection2.m_ConnectPosition.m_LaneData.m_Lane = GetConnectionLanePrefab(middleConnection2.m_ConnectPosition.m_LaneData.m_Lane, utilityLaneData, utilityLaneData2.m_VisualCapacity < utilityLaneData.m_VisualCapacity, wantLarger: false);
							}
							else
							{
								GetGroundPosition(middleConnection2.m_ConnectPosition.m_Owner, math.select(0f, 1f, middleConnection2.m_ConnectPosition.m_IsEnd), ref middleConnection2.m_ConnectPosition.m_Position);
								middleConnection2.m_ConnectPosition.m_LaneData.m_Lane = GetConnectionLanePrefab(middleConnection4.m_TargetLane, utilityLaneData2, utilityLaneData.m_VisualCapacity < utilityLaneData2.m_VisualCapacity, utilityLaneData.m_VisualCapacity > utilityLaneData2.m_VisualCapacity);
							}
						}
						else
						{
							val.y++;
							middleConnection2.m_ConnectPosition.m_Position = CalculateUtilityConnectPosition(tempBuffer, new int2(0, val.y - val.x));
							if (utilityLaneData.m_VisualCapacity < utilityLaneData2.m_VisualCapacity)
							{
								middleConnection2.m_ConnectPosition.m_LaneData.m_Lane = GetConnectionLanePrefab(middleConnection2.m_ConnectPosition.m_LaneData.m_Lane, utilityLaneData, wantSmaller: false, wantLarger: false);
							}
							else
							{
								middleConnection2.m_ConnectPosition.m_LaneData.m_Lane = GetConnectionLanePrefab(middleConnection4.m_TargetLane, utilityLaneData2, wantSmaller: false, utilityLaneData.m_VisualCapacity > utilityLaneData2.m_VisualCapacity);
							}
						}
						CreateNodeConnectionLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, middleConnection2, useGroundPosition, isTemp, ownerTemp);
					}
					tempBuffer.Clear();
				}
				else
				{
					if ((middleConnection2.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Road) == 0)
					{
						continue;
					}
					float num2 = float.MaxValue;
					Entity val3 = Entity.Null;
					ushort num3 = 0;
					uint num4 = 0u;
					int num5 = j;
					for (; j < middleConnections.Length; j++)
					{
						MiddleConnection middleConnection5 = middleConnections[j];
						if (middleConnection5.m_ConnectPosition.m_GroupIndex != middleConnection2.m_ConnectPosition.m_GroupIndex || middleConnection5.m_IsSource != middleConnection2.m_IsSource)
						{
							break;
						}
						if ((middleConnection5.m_TargetFlags & LaneFlags.Master) == 0 && middleConnection5.m_Distance < num2)
						{
							num2 = middleConnection5.m_Distance;
							val3 = middleConnection5.m_TargetOwner;
							num3 = middleConnection5.m_TargetCarriageway;
							num4 = middleConnection5.m_TargetGroup;
						}
					}
					j--;
					for (int l = num5; l <= j; l++)
					{
						middleConnection2 = middleConnections[l];
						bool flag = middleConnection2.m_TargetCarriageway == num3;
						if (flag && isRoundabout)
						{
							flag = middleConnection2.m_TargetOwner == val3 && (val3 != owner || middleConnection2.m_TargetGroup == num4);
						}
						if (flag && (middleConnection2.m_TargetFlags & LaneFlags.Master) != 0)
						{
							flag = false;
							for (int m = num5; m <= j; m++)
							{
								MiddleConnection middleConnection6 = middleConnections[m];
								if ((middleConnection6.m_TargetFlags & LaneFlags.Master) == 0 && middleConnection6.m_TargetGroup == middleConnection2.m_TargetGroup && middleConnection6.m_TargetCarriageway == num3 && (!isRoundabout || (middleConnection6.m_TargetOwner == val3 && (val3 != owner || middleConnection6.m_TargetGroup == num4))))
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							CreateNodeConnectionLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, middleConnection2, useGroundPosition: false, isTemp, ownerTemp);
						}
					}
				}
			}
		}

		private void CreateNodeConnectionLane(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, MiddleConnection middleConnection, bool useGroundPosition, bool isTemp, Temp ownerTemp)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
			//IL_093f: Unknown result type (might be due to invalid IL or missing references)
			//IL_091a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0908: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_098e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Temp temp = default(Temp);
			if (isTemp)
			{
				temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
			}
			PrefabRef prefabRef = new PrefabRef(middleConnection.m_ConnectPosition.m_LaneData.m_Lane);
			CheckPrefab(ref prefabRef.m_Prefab, ref random, out var outRandom, laneBuffer);
			NodeLane nodeLane = default(NodeLane);
			NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
			if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
			{
				if (m_NetLaneData.HasComponent(middleConnection.m_TargetLane))
				{
					NetLaneData netLaneData2 = m_NetLaneData[middleConnection.m_TargetLane];
					nodeLane.m_WidthOffset.x = netLaneData2.m_Width - netLaneData.m_Width;
				}
				if (m_NetLaneData.HasComponent(middleConnection.m_ConnectPosition.m_LaneData.m_Lane))
				{
					NetLaneData netLaneData3 = m_NetLaneData[middleConnection.m_ConnectPosition.m_LaneData.m_Lane];
					nodeLane.m_WidthOffset.y = netLaneData3.m_Width - netLaneData.m_Width;
				}
				if (middleConnection.m_IsSource)
				{
					nodeLane.m_WidthOffset = ((float2)(ref nodeLane.m_WidthOffset)).yx;
				}
			}
			Curve curve = default(Curve);
			if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
			{
				float num = math.sqrt(math.distance(middleConnection.m_ConnectPosition.m_Position, MathUtils.Position(middleConnection.m_TargetCurve.m_Bezier, middleConnection.m_TargetCurvePos)));
				if (middleConnection.m_IsSource)
				{
					Bounds1 val = default(Bounds1);
					((Bounds1)(ref val))._002Ector(middleConnection.m_TargetCurvePos, 1f);
					MathUtils.ClampLength(middleConnection.m_TargetCurve.m_Bezier, ref val, ref num);
					middleConnection.m_TargetCurvePos = val.max;
				}
				else
				{
					Bounds1 val2 = default(Bounds1);
					((Bounds1)(ref val2))._002Ector(0f, middleConnection.m_TargetCurvePos);
					MathUtils.ClampLengthInverse(middleConnection.m_TargetCurve.m_Bezier, ref val2, ref num);
					middleConnection.m_TargetCurvePos = val2.min;
				}
				float3 val3 = MathUtils.Position(middleConnection.m_TargetCurve.m_Bezier, middleConnection.m_TargetCurvePos);
				float3 val4 = MathUtils.Tangent(middleConnection.m_TargetCurve.m_Bezier, middleConnection.m_TargetCurvePos);
				val4 = MathUtils.Normalize(val4, ((float3)(ref val4)).xz);
				if (middleConnection.m_IsSource)
				{
					val4 = -val4;
				}
				if (math.distance(middleConnection.m_ConnectPosition.m_Position, val3) >= 0.01f)
				{
					curve.m_Bezier = NetUtils.FitCurve(val3, val4, -middleConnection.m_ConnectPosition.m_Tangent, middleConnection.m_ConnectPosition.m_Position);
				}
				else
				{
					curve.m_Bezier = NetUtils.StraightCurve(val3, middleConnection.m_ConnectPosition.m_Position);
				}
				if (middleConnection.m_IsSource)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				curve.m_Length = MathUtils.Length(curve.m_Bezier);
			}
			else if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
			{
				float3 position = MathUtils.Position(middleConnection.m_TargetCurve.m_Bezier, middleConnection.m_TargetCurvePos);
				if (useGroundPosition)
				{
					GetGroundPosition(owner, middleConnection.m_ConnectPosition.m_CurvePosition, ref position);
				}
				if (math.abs(position.y - middleConnection.m_ConnectPosition.m_Position.y) >= 0.01f)
				{
					curve.m_Bezier.a = position;
					curve.m_Bezier.b = math.lerp(position, middleConnection.m_ConnectPosition.m_Position, new float3(0.25f, 0.5f, 0.25f));
					curve.m_Bezier.c = math.lerp(position, middleConnection.m_ConnectPosition.m_Position, new float3(0.75f, 0.5f, 0.75f));
					curve.m_Bezier.d = middleConnection.m_ConnectPosition.m_Position;
				}
				else
				{
					curve.m_Bezier = NetUtils.StraightCurve(position, middleConnection.m_ConnectPosition.m_Position);
				}
				if (middleConnection.m_IsSource)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				curve.m_Length = MathUtils.Length(curve.m_Bezier);
			}
			else
			{
				float3 val5 = MathUtils.Position(middleConnection.m_TargetCurve.m_Bezier, middleConnection.m_TargetCurvePos);
				curve.m_Bezier = NetUtils.StraightCurve(val5, middleConnection.m_ConnectPosition.m_Position);
				if (middleConnection.m_IsSource)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				curve.m_Length = math.distance(val5, middleConnection.m_ConnectPosition.m_Position);
			}
			Lane lane = default(Lane);
			uint num2 = 0u;
			if (middleConnection.m_IsSource)
			{
				lane.m_StartNode = new PathNode(middleConnection.m_ConnectPosition.m_Owner, middleConnection.m_ConnectPosition.m_LaneData.m_Index, middleConnection.m_ConnectPosition.m_SegmentIndex);
				lane.m_MiddleNode = new PathNode(owner, (ushort)nodeLaneIndex++);
				lane.m_EndNode = middleConnection.m_TargetNode;
				num2 = middleConnection.m_ConnectPosition.m_GroupIndex | (middleConnection.m_TargetGroup & 0xFFFF0000u);
			}
			else
			{
				lane.m_StartNode = middleConnection.m_TargetNode;
				lane.m_MiddleNode = new PathNode(owner, (ushort)nodeLaneIndex++);
				lane.m_EndNode = new PathNode(middleConnection.m_ConnectPosition.m_Owner, middleConnection.m_ConnectPosition.m_LaneData.m_Index, middleConnection.m_ConnectPosition.m_SegmentIndex);
				num2 = (middleConnection.m_TargetGroup & 0xFFFF) | (uint)(middleConnection.m_ConnectPosition.m_GroupIndex << 16);
			}
			CarLane carLane = default(CarLane);
			if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
			{
				carLane.m_DefaultSpeedLimit = middleConnection.m_TargetComposition.m_SpeedLimit;
				carLane.m_Curviness = NetUtils.CalculateCurviness(curve, netLaneData.m_Width);
				carLane.m_CarriagewayGroup = middleConnection.m_TargetCarriageway;
				carLane.m_Flags |= CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;
				if (middleConnection.m_IsSource)
				{
					carLane.m_Flags |= CarLaneFlags.Yield;
				}
				float3 val6 = MathUtils.StartTangent(curve.m_Bezier);
				float2 val7 = MathUtils.Right(((float3)(ref val6)).xz);
				val6 = MathUtils.EndTangent(curve.m_Bezier);
				if (math.dot(val7, ((float3)(ref val6)).xz) >= 0f)
				{
					carLane.m_Flags |= CarLaneFlags.TurnRight;
				}
				else
				{
					carLane.m_Flags |= CarLaneFlags.TurnLeft;
				}
				if ((middleConnection.m_TargetComposition.m_TaxiwayFlags & TaxiwayFlags.Runway) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Runway;
				}
				if ((middleConnection.m_TargetComposition.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Highway;
				}
				middleConnection.m_ConnectPosition.m_LaneData.m_Flags &= ~(LaneFlags.Slave | LaneFlags.Master);
				middleConnection.m_ConnectPosition.m_LaneData.m_Flags |= middleConnection.m_TargetFlags & (LaneFlags.Slave | LaneFlags.Master);
			}
			else
			{
				middleConnection.m_ConnectPosition.m_LaneData.m_Flags &= ~(LaneFlags.Slave | LaneFlags.Master);
			}
			PedestrianLane pedestrianLane = default(PedestrianLane);
			if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Pedestrian) != 0)
			{
				pedestrianLane.m_Flags |= PedestrianLaneFlags.SideConnection;
			}
			UtilityLane utilityLane = default(UtilityLane);
			if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
			{
				utilityLane.m_Flags |= UtilityLaneFlags.VerticalConnection;
				PrefabRef prefabRef2 = default(PrefabRef);
				NetData netData = default(NetData);
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_PrefabRefData.TryGetComponent(middleConnection.m_ConnectPosition.m_Owner, ref prefabRef2) && m_PrefabNetData.TryGetComponent(prefabRef2.m_Prefab, ref netData) && m_PrefabGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref netGeometryData) && (netData.m_RequiredLayers & (Layer.PowerlineLow | Layer.PowerlineHigh | Layer.WaterPipe | Layer.SewagePipe)) != Layer.None && (netGeometryData.m_Flags & GeometryFlags.Marker) == 0)
				{
					utilityLane.m_Flags |= UtilityLaneFlags.PipelineConnection;
				}
			}
			LaneKey laneKey = new LaneKey(lane, prefabRef.m_Prefab, middleConnection.m_ConnectPosition.m_LaneData.m_Flags);
			LaneKey laneKey2 = laneKey;
			if (isTemp)
			{
				ReplaceTempOwner(ref laneKey2, owner);
				ReplaceTempOwner(ref laneKey2, middleConnection.m_ConnectPosition.m_Owner);
				GetOriginalLane(laneBuffer, laneKey2, ref temp);
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0 && !m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed))
			{
				pseudoRandomSeed = new PseudoRandomSeed(ref outRandom);
			}
			Entity val8 = default(Entity);
			if (laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val8))
			{
				laneBuffer.m_OldLanes.Remove(laneKey);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val8, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val8, nodeLane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val8, curve);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					if (!m_PseudoRandomSeedData.HasComponent(val8))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, val8, pseudoRandomSeed);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val8, pseudoRandomSeed);
					}
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val8, carLane);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val8, pedestrianLane);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val8, utilityLane);
				}
				if (isTemp)
				{
					laneBuffer.m_Updates.Add(ref val8);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val8, temp);
				}
				else if (m_TempData.HasComponent(val8))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val8, ref m_DeletedTempTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val8, ref m_AppliedTypes);
				}
				else
				{
					laneBuffer.m_Updates.Add(ref val8);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					MasterLane masterLane = new MasterLane
					{
						m_Group = num2
					};
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val8, masterLane);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0)
				{
					SlaveLane slaveLane = new SlaveLane
					{
						m_Group = num2,
						m_MinIndex = 0,
						m_MaxIndex = 0,
						m_SubIndex = 0
					};
					slaveLane.m_Flags |= SlaveLaneFlags.AllowChange;
					slaveLane.m_Flags |= (SlaveLaneFlags)(middleConnection.m_IsSource ? 4096 : 2048);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val8, slaveLane);
				}
			}
			else
			{
				NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[prefabRef.m_Prefab];
				EntityArchetype val9 = (((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0) ? netLaneArchetypeData.m_NodeSlaveArchetype : (((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Master) == 0) ? netLaneArchetypeData.m_NodeLaneArchetype : netLaneArchetypeData.m_NodeMasterArchetype));
				Entity val10 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, val9);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val10, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val10, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val10, nodeLane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val10, curve);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val10, pseudoRandomSeed);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val10, carLane);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val10, pedestrianLane);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val10, utilityLane);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					MasterLane masterLane2 = new MasterLane
					{
						m_Group = num2
					};
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val10, masterLane2);
				}
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0)
				{
					SlaveLane slaveLane2 = new SlaveLane
					{
						m_Group = num2,
						m_MinIndex = 0,
						m_MaxIndex = 0,
						m_SubIndex = 0
					};
					slaveLane2.m_Flags |= SlaveLaneFlags.AllowChange;
					slaveLane2.m_Flags |= (SlaveLaneFlags)(middleConnection.m_IsSource ? 4096 : 2048);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val10, slaveLane2);
				}
				if (isTemp)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val10, ref m_TempOwnerTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val10, owner2);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val10, temp);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val10, owner2);
				}
			}
		}

		private void CreateEdgeConnectionLane(int jobIndex, ref int edgeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, Segment startSegment, Segment endSegment, NetCompositionData prefabCompositionData, CompositionData compositionData, NetCompositionLane prefabCompositionLaneData, ConnectPosition connectPosition, int connectionIndex, bool isSingleCurve, bool useGroundPosition, bool isSource, bool isTemp, Temp ownerTemp)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_066c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_074f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_095d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
			float num = prefabCompositionLaneData.m_Position.x / math.max(1f, prefabCompositionData.m_Width) + 0.5f;
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Temp temp = default(Temp);
			if (isTemp)
			{
				temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
			}
			PrefabRef prefabRef = new PrefabRef(connectPosition.m_LaneData.m_Lane);
			CheckPrefab(ref prefabRef.m_Prefab, ref random, out var outRandom, laneBuffer);
			bool flag = false;
			Bezier4x3 val = default(Bezier4x3);
			Bezier4x3 val2 = default(Bezier4x3);
			Bezier4x3 val3;
			byte b;
			float num2 = default(float);
			if (isSingleCurve)
			{
				val3 = MathUtils.Lerp(MathUtils.Join(startSegment.m_Left, endSegment.m_Left), MathUtils.Join(startSegment.m_Right, endSegment.m_Right), num);
				val3.a.y += prefabCompositionLaneData.m_Position.y;
				val3.b.y += prefabCompositionLaneData.m_Position.y;
				val3.c.y += prefabCompositionLaneData.m_Position.y;
				val3.d.y += prefabCompositionLaneData.m_Position.y;
				if ((prefabCompositionLaneData.m_Flags & (LaneFlags.Road | LaneFlags.Pedestrian | LaneFlags.Parking | LaneFlags.Track)) != 0 && (prefabCompositionLaneData.m_Flags & LaneFlags.Invert) != 0)
				{
					val3 = MathUtils.Invert(val3);
				}
				b = 2;
				MathUtils.Distance(val3, connectPosition.m_Position, ref num2);
			}
			else
			{
				val = MathUtils.Lerp(startSegment.m_Left, startSegment.m_Right, num);
				val2 = MathUtils.Lerp(endSegment.m_Left, endSegment.m_Right, num);
				val.a.y += prefabCompositionLaneData.m_Position.y;
				val.b.y += prefabCompositionLaneData.m_Position.y;
				val.c.y += prefabCompositionLaneData.m_Position.y;
				val.d.y += prefabCompositionLaneData.m_Position.y;
				val2.a.y += prefabCompositionLaneData.m_Position.y;
				val2.b.y += prefabCompositionLaneData.m_Position.y;
				val2.c.y += prefabCompositionLaneData.m_Position.y;
				val2.d.y += prefabCompositionLaneData.m_Position.y;
				if ((prefabCompositionLaneData.m_Flags & (LaneFlags.Road | LaneFlags.Pedestrian | LaneFlags.Parking | LaneFlags.Track)) != 0 && (prefabCompositionLaneData.m_Flags & LaneFlags.Invert) != 0)
				{
					val = MathUtils.Invert(val);
					val2 = MathUtils.Invert(val2);
					flag = !flag;
				}
				float num4 = default(float);
				float num3 = MathUtils.Distance(val, connectPosition.m_Position, ref num4);
				float num5 = default(float);
				if (MathUtils.Distance(val2, connectPosition.m_Position, ref num5) < num3)
				{
					val3 = val2;
					b = 3;
					num2 = num5;
					flag = !flag;
				}
				else
				{
					val3 = val;
					b = 1;
					num2 = num4;
				}
			}
			NodeLane nodeLane = default(NodeLane);
			NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
			if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
			{
				if (m_NetLaneData.HasComponent(prefabCompositionLaneData.m_Lane))
				{
					NetLaneData netLaneData2 = m_NetLaneData[prefabCompositionLaneData.m_Lane];
					nodeLane.m_WidthOffset.x = netLaneData2.m_Width - netLaneData.m_Width;
				}
				if (m_NetLaneData.HasComponent(connectPosition.m_LaneData.m_Lane))
				{
					NetLaneData netLaneData3 = m_NetLaneData[connectPosition.m_LaneData.m_Lane];
					nodeLane.m_WidthOffset.y = netLaneData3.m_Width - netLaneData.m_Width;
				}
				if (isSource)
				{
					nodeLane.m_WidthOffset = ((float2)(ref nodeLane.m_WidthOffset)).yx;
				}
			}
			Curve curve = default(Curve);
			if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
			{
				float num6 = math.sqrt(math.distance(connectPosition.m_Position, MathUtils.Position(val3, num2)));
				float num7 = num6;
				if (isSource)
				{
					Bounds1 val4 = default(Bounds1);
					((Bounds1)(ref val4))._002Ector(num2, 1f);
					if (!MathUtils.ClampLength(val3, ref val4, ref num7) && !isSingleCurve && !flag)
					{
						num7 = num6 - num7;
						if (b == 1)
						{
							val3 = val2;
							b = 3;
						}
						else
						{
							val3 = val;
							b = 1;
						}
						((Bounds1)(ref val4))._002Ector(0f, 1f);
						MathUtils.ClampLength(val3, ref val4, ref num7);
					}
					num2 = val4.max;
				}
				else
				{
					Bounds1 val5 = default(Bounds1);
					((Bounds1)(ref val5))._002Ector(0f, num2);
					if (!MathUtils.ClampLengthInverse(val3, ref val5, ref num7) && !isSingleCurve && flag)
					{
						num7 = num6 - num7;
						if (b == 1)
						{
							val3 = val2;
							b = 3;
						}
						else
						{
							val3 = val;
							b = 1;
						}
						((Bounds1)(ref val5))._002Ector(0f, 1f);
						MathUtils.ClampLengthInverse(val3, ref val5, ref num7);
					}
					num2 = val5.min;
				}
				float3 val6 = MathUtils.Position(val3, num2);
				float3 val7 = MathUtils.Tangent(val3, num2);
				val7 = MathUtils.Normalize(val7, ((float3)(ref val7)).xz);
				if (isSource)
				{
					val7 = -val7;
				}
				if (math.distance(connectPosition.m_Position, val6) >= 0.01f)
				{
					curve.m_Bezier = NetUtils.FitCurve(val6, val7, -connectPosition.m_Tangent, connectPosition.m_Position);
				}
				else
				{
					curve.m_Bezier = NetUtils.StraightCurve(val6, connectPosition.m_Position);
				}
				if (isSource)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				curve.m_Length = MathUtils.Length(curve.m_Bezier);
			}
			else if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
			{
				float3 position = MathUtils.Position(val3, num2);
				if (useGroundPosition)
				{
					GetGroundPosition(owner, connectPosition.m_CurvePosition, ref position);
				}
				if (math.abs(position.y - connectPosition.m_Position.y) >= 0.01f)
				{
					curve.m_Bezier.a = position;
					curve.m_Bezier.b = math.lerp(position, connectPosition.m_Position, new float3(0.25f, 0.5f, 0.25f));
					curve.m_Bezier.c = math.lerp(position, connectPosition.m_Position, new float3(0.75f, 0.5f, 0.75f));
					curve.m_Bezier.d = connectPosition.m_Position;
				}
				else
				{
					curve.m_Bezier = NetUtils.StraightCurve(position, connectPosition.m_Position);
				}
				if (isSource)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				curve.m_Length = MathUtils.Length(curve.m_Bezier);
			}
			else
			{
				float3 val8 = MathUtils.Position(val3, num2);
				curve.m_Bezier = NetUtils.StraightCurve(val8, connectPosition.m_Position);
				if (isSource)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				curve.m_Length = math.distance(val8, connectPosition.m_Position);
			}
			Lane lane = default(Lane);
			if (isSource)
			{
				lane.m_StartNode = new PathNode(connectPosition.m_Owner, connectPosition.m_LaneData.m_Index, connectPosition.m_SegmentIndex);
				lane.m_MiddleNode = new PathNode(owner, (ushort)edgeLaneIndex--);
				lane.m_EndNode = new PathNode(owner, prefabCompositionLaneData.m_Index, b, num2);
			}
			else
			{
				lane.m_StartNode = new PathNode(owner, prefabCompositionLaneData.m_Index, b, num2);
				lane.m_MiddleNode = new PathNode(owner, (ushort)edgeLaneIndex--);
				lane.m_EndNode = new PathNode(connectPosition.m_Owner, connectPosition.m_LaneData.m_Index, connectPosition.m_SegmentIndex);
			}
			CarLane carLane = default(CarLane);
			if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
			{
				carLane.m_DefaultSpeedLimit = compositionData.m_SpeedLimit;
				carLane.m_Curviness = NetUtils.CalculateCurviness(curve, netLaneData.m_Width);
				carLane.m_CarriagewayGroup = (ushort)((b << 8) | prefabCompositionLaneData.m_Carriageway);
				carLane.m_Flags |= CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;
				if (isSource)
				{
					carLane.m_Flags |= CarLaneFlags.Yield;
				}
				float3 val9 = MathUtils.StartTangent(curve.m_Bezier);
				float2 val10 = MathUtils.Right(((float3)(ref val9)).xz);
				val9 = MathUtils.EndTangent(curve.m_Bezier);
				if (math.dot(val10, ((float3)(ref val9)).xz) >= 0f)
				{
					carLane.m_Flags |= CarLaneFlags.TurnRight;
				}
				else
				{
					carLane.m_Flags |= CarLaneFlags.TurnLeft;
				}
				if ((compositionData.m_TaxiwayFlags & TaxiwayFlags.Runway) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Runway;
				}
				if ((compositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Highway;
				}
				connectPosition.m_LaneData.m_Flags |= prefabCompositionLaneData.m_Flags & (LaneFlags.Slave | LaneFlags.Master);
			}
			else
			{
				connectPosition.m_LaneData.m_Flags &= ~(LaneFlags.Slave | LaneFlags.Master);
			}
			PedestrianLane pedestrianLane = default(PedestrianLane);
			if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Pedestrian) != 0)
			{
				pedestrianLane.m_Flags |= PedestrianLaneFlags.SideConnection;
			}
			UtilityLane utilityLane = default(UtilityLane);
			if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
			{
				utilityLane.m_Flags |= UtilityLaneFlags.VerticalConnection;
				PrefabRef prefabRef2 = default(PrefabRef);
				NetData netData = default(NetData);
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_PrefabRefData.TryGetComponent(connectPosition.m_Owner, ref prefabRef2) && m_PrefabNetData.TryGetComponent(prefabRef2.m_Prefab, ref netData) && m_PrefabGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref netGeometryData) && (netData.m_RequiredLayers & (Layer.PowerlineLow | Layer.PowerlineHigh | Layer.WaterPipe | Layer.SewagePipe)) != Layer.None && (netGeometryData.m_Flags & GeometryFlags.Marker) == 0)
				{
					utilityLane.m_Flags |= UtilityLaneFlags.PipelineConnection;
				}
			}
			uint num8 = (uint)(prefabCompositionLaneData.m_Group | (connectionIndex + 4 << 8));
			LaneKey laneKey = new LaneKey(lane, prefabRef.m_Prefab, connectPosition.m_LaneData.m_Flags);
			LaneKey laneKey2 = laneKey;
			if (isTemp)
			{
				ReplaceTempOwner(ref laneKey2, owner);
				ReplaceTempOwner(ref laneKey2, connectPosition.m_Owner);
				GetOriginalLane(laneBuffer, laneKey2, ref temp);
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0 && !m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed))
			{
				pseudoRandomSeed = new PseudoRandomSeed(ref outRandom);
			}
			Entity val11 = default(Entity);
			if (laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val11))
			{
				laneBuffer.m_OldLanes.Remove(laneKey);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val11, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val11, nodeLane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val11, curve);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					if (!m_PseudoRandomSeedData.HasComponent(val11))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, val11, pseudoRandomSeed);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val11, pseudoRandomSeed);
					}
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val11, carLane);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val11, pedestrianLane);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val11, utilityLane);
				}
				if (isTemp)
				{
					laneBuffer.m_Updates.Add(ref val11);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val11, temp);
				}
				else if (m_TempData.HasComponent(val11))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val11, ref m_DeletedTempTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val11, ref m_AppliedTypes);
				}
				else
				{
					laneBuffer.m_Updates.Add(ref val11);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					MasterLane masterLane = new MasterLane
					{
						m_Group = num8
					};
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val11, masterLane);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0)
				{
					SlaveLane slaveLane = new SlaveLane
					{
						m_Group = num8,
						m_MinIndex = prefabCompositionLaneData.m_Index,
						m_MaxIndex = prefabCompositionLaneData.m_Index,
						m_SubIndex = prefabCompositionLaneData.m_Index
					};
					slaveLane.m_Flags |= SlaveLaneFlags.AllowChange;
					slaveLane.m_Flags |= (SlaveLaneFlags)(isSource ? 4096 : 2048);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val11, slaveLane);
				}
			}
			else
			{
				NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[prefabRef.m_Prefab];
				EntityArchetype val12 = (((connectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0) ? netLaneArchetypeData.m_NodeSlaveArchetype : (((connectPosition.m_LaneData.m_Flags & LaneFlags.Master) == 0) ? netLaneArchetypeData.m_NodeLaneArchetype : netLaneArchetypeData.m_NodeMasterArchetype));
				Entity val13 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, val12);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val13, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val13, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val13, nodeLane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val13, curve);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val13, pseudoRandomSeed);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Road) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val13, carLane);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val13, pedestrianLane);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val13, utilityLane);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					MasterLane masterLane2 = new MasterLane
					{
						m_Group = num8
					};
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val13, masterLane2);
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0)
				{
					SlaveLane slaveLane2 = new SlaveLane
					{
						m_Group = num8,
						m_MinIndex = prefabCompositionLaneData.m_Index,
						m_MaxIndex = prefabCompositionLaneData.m_Index,
						m_SubIndex = prefabCompositionLaneData.m_Index
					};
					slaveLane2.m_Flags |= SlaveLaneFlags.AllowChange;
					slaveLane2.m_Flags |= (SlaveLaneFlags)(isSource ? 4096 : 2048);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val13, slaveLane2);
				}
				if (isTemp)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val13, ref m_TempOwnerTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val13, owner2);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val13, temp);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val13, owner2);
				}
			}
		}

		private bool FindAnchor(ref float3 position, ref PathNode pathNode, Entity prefab, NativeList<LaneAnchor> anchors)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			if (!anchors.IsCreated)
			{
				return false;
			}
			for (int i = 0; i < anchors.Length; i++)
			{
				LaneAnchor laneAnchor = anchors[i];
				if (laneAnchor.m_Prefab == prefab)
				{
					bool num = !laneAnchor.m_PathNode.Equals(pathNode);
					if (num)
					{
						position = laneAnchor.m_Position;
						pathNode = laneAnchor.m_PathNode;
					}
					laneAnchor.m_Prefab = Entity.Null;
					anchors[i] = laneAnchor;
					return num;
				}
			}
			return false;
		}

		private void CreateEdgeLane(int jobIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, Segment segment, NetCompositionData prefabCompositionData, CompositionData compositionData, DynamicBuffer<NetCompositionLane> prefabCompositionLanes, NetCompositionLane prefabCompositionLaneData, int2 segmentIndex, float2 edgeDelta, NativeList<LaneAnchor> startAnchors, NativeList<LaneAnchor> endAnchors, bool2 canAnchor, bool isTemp, Temp ownerTemp)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0829: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_090e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0975: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7b: Unknown result type (might be due to invalid IL or missing references)
			LaneFlags laneFlags = prefabCompositionLaneData.m_Flags;
			float num = prefabCompositionLaneData.m_Position.x / math.max(1f, prefabCompositionData.m_Width) + 0.5f;
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Temp temp = default(Temp);
			if (isTemp)
			{
				temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
			}
			Lane laneData = default(Lane);
			int num2 = math.csum(segmentIndex) / 2;
			laneData.m_StartNode = new PathNode(owner, prefabCompositionLaneData.m_Index, (byte)segmentIndex.x);
			laneData.m_MiddleNode = new PathNode(owner, prefabCompositionLaneData.m_Index, (byte)num2);
			laneData.m_EndNode = new PathNode(owner, prefabCompositionLaneData.m_Index, (byte)segmentIndex.y);
			PrefabRef prefabRef = new PrefabRef(prefabCompositionLaneData.m_Lane);
			if ((laneFlags & (LaneFlags.Master | LaneFlags.Road | LaneFlags.Track)) == (LaneFlags.Master | LaneFlags.Road | LaneFlags.Track))
			{
				laneFlags &= ~LaneFlags.Track;
				prefabRef.m_Prefab = m_CarLaneData[prefabRef.m_Prefab].m_NotTrackLanePrefab;
			}
			CheckPrefab(ref prefabRef.m_Prefab, ref random, out var outRandom, laneBuffer);
			NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
			EdgeLane edgeLaneData = new EdgeLane
			{
				m_EdgeDelta = edgeDelta
			};
			Curve curveData = new Curve
			{
				m_Bezier = MathUtils.Lerp(segment.m_Left, segment.m_Right, num)
			};
			curveData.m_Bezier.a.y += prefabCompositionLaneData.m_Position.y;
			curveData.m_Bezier.b.y += prefabCompositionLaneData.m_Position.y;
			curveData.m_Bezier.c.y += prefabCompositionLaneData.m_Position.y;
			curveData.m_Bezier.d.y += prefabCompositionLaneData.m_Position.y;
			bool2 val = bool2.op_Implicit(false);
			if ((laneFlags & LaneFlags.FindAnchor) != 0)
			{
				val.x = canAnchor.x && !FindAnchor(ref curveData.m_Bezier.a, ref laneData.m_StartNode, prefabCompositionLaneData.m_Lane, startAnchors);
				val.y = canAnchor.y && !FindAnchor(ref curveData.m_Bezier.d, ref laneData.m_EndNode, prefabCompositionLaneData.m_Lane, endAnchors);
			}
			UtilityLane utilityLane = default(UtilityLane);
			HangingLane hangingLane = default(HangingLane);
			bool flag = false;
			if ((laneFlags & LaneFlags.Utility) != 0)
			{
				UtilityLaneData utilityLaneData = m_UtilityLaneData[prefabCompositionLaneData.m_Lane];
				if (utilityLaneData.m_Hanging != 0f)
				{
					curveData.m_Bezier.b = math.lerp(curveData.m_Bezier.a, curveData.m_Bezier.d, 1f / 3f);
					curveData.m_Bezier.c = math.lerp(curveData.m_Bezier.a, curveData.m_Bezier.d, 2f / 3f);
					float num3 = math.distance(((float3)(ref curveData.m_Bezier.a)).xz, ((float3)(ref curveData.m_Bezier.d)).xz) * utilityLaneData.m_Hanging * 1.3333334f;
					curveData.m_Bezier.b.y -= num3;
					curveData.m_Bezier.c.y -= num3;
					hangingLane.m_Distances = float2.op_Implicit(0.1f);
					if ((laneFlags & LaneFlags.FindAnchor) != 0)
					{
						hangingLane.m_Distances = math.select(hangingLane.m_Distances, float2.op_Implicit(0f), canAnchor);
					}
					flag = true;
				}
				if (val.x)
				{
					utilityLane.m_Flags |= UtilityLaneFlags.SecondaryStartAnchor;
				}
				if (val.y)
				{
					utilityLane.m_Flags |= UtilityLaneFlags.SecondaryEndAnchor;
				}
			}
			curveData.m_Length = MathUtils.Length(curveData.m_Bezier);
			if ((laneFlags & (LaneFlags.Road | LaneFlags.Pedestrian | LaneFlags.Parking | LaneFlags.Track)) != 0 && (laneFlags & LaneFlags.Invert) != 0)
			{
				Invert(ref laneData, ref curveData, ref edgeLaneData);
			}
			CarLane carLane = default(CarLane);
			if ((laneFlags & LaneFlags.Road) != 0)
			{
				carLane.m_DefaultSpeedLimit = compositionData.m_SpeedLimit;
				carLane.m_Curviness = NetUtils.CalculateCurviness(curveData, netLaneData.m_Width);
				carLane.m_CarriagewayGroup = (ushort)((segmentIndex.x << 8) | prefabCompositionLaneData.m_Carriageway);
				if ((laneFlags & LaneFlags.Invert) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Invert;
				}
				if ((laneFlags & LaneFlags.Twoway) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Twoway;
				}
				if ((laneFlags & LaneFlags.PublicOnly) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.PublicOnly;
				}
				if ((compositionData.m_TaxiwayFlags & TaxiwayFlags.Runway) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Runway;
				}
				if ((compositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Highway;
				}
				if ((laneFlags & LaneFlags.LeftLimit) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.LeftLimit;
				}
				if ((laneFlags & LaneFlags.RightLimit) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.RightLimit;
				}
				if ((laneFlags & LaneFlags.ParkingLeft) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.ParkingLeft;
				}
				if ((laneFlags & LaneFlags.ParkingRight) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.ParkingRight;
				}
			}
			TrackLane trackLane = default(TrackLane);
			if ((laneFlags & LaneFlags.Track) != 0)
			{
				trackLane.m_SpeedLimit = compositionData.m_SpeedLimit;
				trackLane.m_Curviness = NetUtils.CalculateCurviness(curveData, netLaneData.m_Width);
				trackLane.m_Flags |= TrackLaneFlags.AllowMiddle;
				if ((laneFlags & LaneFlags.Invert) != 0)
				{
					trackLane.m_Flags |= TrackLaneFlags.Invert;
				}
				if ((laneFlags & LaneFlags.Twoway) != 0)
				{
					trackLane.m_Flags |= TrackLaneFlags.Twoway;
				}
				if ((laneFlags & LaneFlags.Road) == 0)
				{
					trackLane.m_Flags |= TrackLaneFlags.Exclusive;
				}
				if (((prefabCompositionData.m_Flags.m_Left | prefabCompositionData.m_Flags.m_Right) & (CompositionFlags.Side.PrimaryStop | CompositionFlags.Side.SecondaryStop)) != 0)
				{
					trackLane.m_Flags |= TrackLaneFlags.Station;
				}
			}
			ParkingLane parkingLane = default(ParkingLane);
			if ((laneFlags & LaneFlags.Parking) != 0)
			{
				NetCompositionLane netCompositionLane = FindClosestLane(prefabCompositionLanes, LaneFlags.Road, LaneFlags.Slave, prefabCompositionLaneData.m_Position);
				NetCompositionLane netCompositionLane2 = FindClosestLane(prefabCompositionLanes, LaneFlags.Pedestrian, (LaneFlags)0, prefabCompositionLaneData.m_Position);
				if (netCompositionLane.m_Lane != Entity.Null)
				{
					laneData.m_StartNode = new PathNode(owner, netCompositionLane.m_Index, (byte)num2, 0.5f);
					if ((laneFlags & LaneFlags.Twoway) != 0)
					{
						NetCompositionLane netCompositionLane3 = FindClosestLane(prefabCompositionLanes, LaneFlags.Road | (~netCompositionLane.m_Flags & LaneFlags.Invert), LaneFlags.Slave | (netCompositionLane.m_Flags & LaneFlags.Invert), netCompositionLane.m_Position, netCompositionLane.m_Carriageway);
						if (netCompositionLane3.m_Lane != Entity.Null)
						{
							parkingLane.m_SecondaryStartNode = new PathNode(owner, netCompositionLane3.m_Index, (byte)num2, 0.5f);
							parkingLane.m_Flags |= ParkingLaneFlags.SecondaryStart;
						}
					}
				}
				if (netCompositionLane2.m_Lane != Entity.Null)
				{
					laneData.m_EndNode = new PathNode(owner, netCompositionLane2.m_Index, (byte)num2, 0.5f);
				}
				if ((laneFlags & LaneFlags.Invert) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.Invert;
				}
				if ((laneFlags & LaneFlags.Virtual) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.VirtualLane;
				}
				if ((laneFlags & LaneFlags.PublicOnly) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.SpecialVehicles;
				}
				if (edgeLaneData.m_EdgeDelta.x == 0f || edgeLaneData.m_EdgeDelta.x == 1f)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.StartingLane;
				}
				if (edgeLaneData.m_EdgeDelta.y == 0f || edgeLaneData.m_EdgeDelta.y == 1f)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.EndingLane;
				}
				if (prefabCompositionLaneData.m_Position.x >= 0f)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.RightSide;
				}
				else
				{
					parkingLane.m_Flags |= ParkingLaneFlags.LeftSide;
				}
				if ((laneFlags & LaneFlags.ParkingLeft) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.ParkingLeft;
				}
				if ((laneFlags & LaneFlags.ParkingRight) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.ParkingRight;
				}
			}
			PedestrianLane pedestrianLane = default(PedestrianLane);
			if ((laneFlags & LaneFlags.Pedestrian) != 0)
			{
				pedestrianLane.m_Flags |= PedestrianLaneFlags.AllowMiddle;
				if ((laneFlags & LaneFlags.OnWater) != 0)
				{
					pedestrianLane.m_Flags |= PedestrianLaneFlags.OnWater;
				}
			}
			uint num4 = (uint)(prefabCompositionLaneData.m_Group | (segmentIndex.x << 8));
			LaneKey laneKey = new LaneKey(laneData, prefabRef.m_Prefab, laneFlags);
			LaneKey laneKey2 = laneKey;
			if (isTemp)
			{
				ReplaceTempOwner(ref laneKey2, owner);
				GetOriginalLane(laneBuffer, laneKey2, ref temp);
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0 && !m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed))
			{
				pseudoRandomSeed = new PseudoRandomSeed(ref outRandom);
			}
			Entity val2 = default(Entity);
			bool flag2 = laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val2);
			if (flag2)
			{
				Curve curve = m_CurveData[val2];
				if (math.dot(curve.m_Bezier.d - curve.m_Bezier.a, curveData.m_Bezier.d - curveData.m_Bezier.a) < 0f)
				{
					flag2 = false;
				}
			}
			bool flag3 = m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab);
			if (flag2)
			{
				laneBuffer.m_OldLanes.Remove(laneKey);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val2, laneData);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EdgeLane>(jobIndex, val2, edgeLaneData);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val2, curveData);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					if (!m_PseudoRandomSeedData.HasComponent(val2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, val2, pseudoRandomSeed);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val2, pseudoRandomSeed);
					}
				}
				if (flag3)
				{
					if ((laneFlags & LaneFlags.Road) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val2, carLane);
					}
					if ((laneFlags & LaneFlags.Track) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrackLane>(jobIndex, val2, trackLane);
					}
					if ((laneFlags & LaneFlags.Parking) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkingLane>(jobIndex, val2, parkingLane);
					}
					if ((laneFlags & LaneFlags.Pedestrian) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val2, pedestrianLane);
					}
					if ((laneFlags & LaneFlags.Utility) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val2, utilityLane);
					}
					if (flag)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<HangingLane>(jobIndex, val2, hangingLane);
					}
				}
				if (isTemp)
				{
					laneBuffer.m_Updates.Add(ref val2);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val2, temp);
				}
				else if (m_TempData.HasComponent(val2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val2, ref m_DeletedTempTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val2, ref m_AppliedTypes);
				}
				else
				{
					laneBuffer.m_Updates.Add(ref val2);
				}
				if ((laneFlags & LaneFlags.Master) != 0)
				{
					MasterLane masterLane = new MasterLane
					{
						m_Group = num4
					};
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val2, masterLane);
				}
				if ((laneFlags & LaneFlags.Slave) != 0)
				{
					SlaveLane slaveLane = new SlaveLane
					{
						m_Group = num4,
						m_MinIndex = prefabCompositionLaneData.m_Index,
						m_MaxIndex = prefabCompositionLaneData.m_Index,
						m_SubIndex = prefabCompositionLaneData.m_Index
					};
					slaveLane.m_Flags |= SlaveLaneFlags.AllowChange;
					if ((laneFlags & LaneFlags.DisconnectedStart) != 0)
					{
						slaveLane.m_Flags |= SlaveLaneFlags.StartingLane;
					}
					if ((laneFlags & LaneFlags.DisconnectedEnd) != 0)
					{
						slaveLane.m_Flags |= SlaveLaneFlags.EndingLane;
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val2, slaveLane);
				}
				return;
			}
			EntityArchetype val3 = default(EntityArchetype);
			NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[prefabRef.m_Prefab];
			val3 = (((laneFlags & LaneFlags.Slave) != 0) ? netLaneArchetypeData.m_EdgeSlaveArchetype : (((laneFlags & LaneFlags.Master) == 0) ? netLaneArchetypeData.m_EdgeLaneArchetype : netLaneArchetypeData.m_EdgeMasterArchetype));
			Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, val3);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val4, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val4, laneData);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<EdgeLane>(jobIndex, val4, edgeLaneData);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val4, curveData);
			if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val4, pseudoRandomSeed);
			}
			if (flag3)
			{
				if ((laneFlags & LaneFlags.Road) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val4, carLane);
				}
				if ((laneFlags & LaneFlags.Track) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrackLane>(jobIndex, val4, trackLane);
				}
				if ((laneFlags & LaneFlags.Parking) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkingLane>(jobIndex, val4, parkingLane);
				}
				if ((laneFlags & LaneFlags.Pedestrian) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val4, pedestrianLane);
				}
				if ((laneFlags & LaneFlags.Utility) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val4, utilityLane);
				}
				if (flag)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HangingLane>(jobIndex, val4, hangingLane);
				}
			}
			if ((laneFlags & LaneFlags.Master) != 0)
			{
				MasterLane masterLane2 = new MasterLane
				{
					m_Group = num4
				};
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val4, masterLane2);
			}
			if ((laneFlags & LaneFlags.Slave) != 0)
			{
				SlaveLane slaveLane2 = new SlaveLane
				{
					m_Group = num4,
					m_MinIndex = prefabCompositionLaneData.m_Index,
					m_MaxIndex = prefabCompositionLaneData.m_Index,
					m_SubIndex = prefabCompositionLaneData.m_Index
				};
				slaveLane2.m_Flags |= SlaveLaneFlags.AllowChange;
				if ((laneFlags & LaneFlags.DisconnectedStart) != 0)
				{
					slaveLane2.m_Flags |= SlaveLaneFlags.StartingLane;
				}
				if ((laneFlags & LaneFlags.DisconnectedEnd) != 0)
				{
					slaveLane2.m_Flags |= SlaveLaneFlags.EndingLane;
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val4, slaveLane2);
			}
			if (isTemp)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val4, ref m_TempOwnerTypes);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val4, owner2);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val4, temp);
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val4, owner2);
			}
		}

		private void CreateObjectLane(int jobIndex, ref Random random, Entity owner, Entity original, LaneBuffer laneBuffer, Transform transform, Game.Prefabs.SubLane prefabSubLane, int laneIndex, bool sampleTerrain, bool cutForTraffic, bool isTemp, Temp ownerTemp, NativeList<ClearAreaData> clearAreas)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_087c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_0903: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_091f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0979: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
			//IL_098e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_082f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0843: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
			if (original != Entity.Null)
			{
				prefabSubLane.m_Prefab = m_PrefabRefData[original].m_Prefab;
			}
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Temp temp = default(Temp);
			if (isTemp)
			{
				temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
			}
			Lane laneData = default(Lane);
			if (original != Entity.Null)
			{
				laneData = m_LaneData[original];
			}
			else
			{
				laneData.m_StartNode = new PathNode(owner, (ushort)prefabSubLane.m_NodeIndex.x);
				laneData.m_MiddleNode = new PathNode(owner, (ushort)(65535 - laneIndex));
				laneData.m_EndNode = new PathNode(owner, (ushort)prefabSubLane.m_NodeIndex.y);
			}
			PrefabRef prefabRef = new PrefabRef(prefabSubLane.m_Prefab);
			Random outRandom = random;
			Curve curveData = default(Curve);
			if (original != Entity.Null)
			{
				curveData = m_CurveData[original];
			}
			else
			{
				CheckPrefab(ref prefabRef.m_Prefab, ref random, out outRandom, laneBuffer);
				curveData.m_Bezier.a = ObjectUtils.LocalToWorld(transform, prefabSubLane.m_Curve.a);
				curveData.m_Bezier.b = ObjectUtils.LocalToWorld(transform, prefabSubLane.m_Curve.b);
				curveData.m_Bezier.c = ObjectUtils.LocalToWorld(transform, prefabSubLane.m_Curve.c);
				curveData.m_Bezier.d = ObjectUtils.LocalToWorld(transform, prefabSubLane.m_Curve.d);
			}
			NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
			UtilityLane utilityLane = default(UtilityLane);
			if ((netLaneData.m_Flags & LaneFlags.Utility) != 0)
			{
				UtilityLaneData utilityLaneData = m_UtilityLaneData[prefabSubLane.m_Prefab];
				if (cutForTraffic)
				{
					utilityLane.m_Flags |= UtilityLaneFlags.CutForTraffic;
				}
				if (utilityLaneData.m_Hanging != 0f)
				{
					curveData.m_Bezier.b = math.lerp(curveData.m_Bezier.a, curveData.m_Bezier.d, 1f / 3f);
					curveData.m_Bezier.c = math.lerp(curveData.m_Bezier.a, curveData.m_Bezier.d, 2f / 3f);
					float num = math.distance(((float3)(ref curveData.m_Bezier.a)).xz, ((float3)(ref curveData.m_Bezier.d)).xz) * utilityLaneData.m_Hanging * 1.3333334f;
					curveData.m_Bezier.b.y -= num;
					curveData.m_Bezier.c.y -= num;
				}
			}
			bool2 val = bool2.op_Implicit(false);
			float2 elevation = float2.op_Implicit(0f);
			if (original != Entity.Null)
			{
				Elevation elevation2 = default(Elevation);
				if (m_ElevationData.TryGetComponent(original, ref elevation2))
				{
					val = elevation2.m_Elevation != float.MinValue;
					elevation = elevation2.m_Elevation;
				}
			}
			else
			{
				val = prefabSubLane.m_ParentMesh >= 0;
				elevation = math.select(float2.op_Implicit(float.MinValue), new float2(prefabSubLane.m_Curve.a.y, prefabSubLane.m_Curve.d.y), val);
			}
			if (ClearAreaHelpers.ShouldClear(clearAreas, curveData.m_Bezier, !math.all(val)))
			{
				return;
			}
			if (sampleTerrain && !math.all(val))
			{
				Curve curve = NetUtils.AdjustPosition(curveData, val.x, math.any(val), val.y, ref m_TerrainHeightData);
				Bezier4x1 y = ((Bezier4x3)(ref curve.m_Bezier)).y;
				float4 abcd = ((Bezier4x1)(ref y)).abcd;
				y = ((Bezier4x3)(ref curveData.m_Bezier)).y;
				if (math.any(math.abs(abcd - ((Bezier4x1)(ref y)).abcd) >= 0.01f))
				{
					curveData = curve;
				}
			}
			curveData.m_Length = MathUtils.Length(curveData.m_Bezier);
			if (original == Entity.Null && (netLaneData.m_Flags & (LaneFlags.Road | LaneFlags.Pedestrian | LaneFlags.Parking | LaneFlags.Track)) != 0 && (netLaneData.m_Flags & LaneFlags.Invert) != 0)
			{
				EdgeLane edgeLaneData = default(EdgeLane);
				Invert(ref laneData, ref curveData, ref edgeLaneData);
			}
			CarLane carLane = default(CarLane);
			if ((netLaneData.m_Flags & LaneFlags.Road) != 0)
			{
				carLane.m_DefaultSpeedLimit = 3f;
				carLane.m_Curviness = NetUtils.CalculateCurviness(curveData, netLaneData.m_Width);
				carLane.m_CarriagewayGroup = (ushort)laneIndex;
				if ((netLaneData.m_Flags & LaneFlags.Invert) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Invert;
				}
				if ((netLaneData.m_Flags & LaneFlags.Twoway) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.Twoway;
				}
				if ((netLaneData.m_Flags & LaneFlags.PublicOnly) != 0)
				{
					carLane.m_Flags |= CarLaneFlags.PublicOnly;
				}
			}
			TrackLane trackLane = default(TrackLane);
			if ((netLaneData.m_Flags & LaneFlags.Track) != 0)
			{
				trackLane.m_SpeedLimit = 3f;
				trackLane.m_Curviness = NetUtils.CalculateCurviness(curveData, netLaneData.m_Width);
				trackLane.m_Flags |= TrackLaneFlags.AllowMiddle | TrackLaneFlags.Station;
				if ((netLaneData.m_Flags & LaneFlags.Invert) != 0)
				{
					trackLane.m_Flags |= TrackLaneFlags.Invert;
				}
				if ((netLaneData.m_Flags & LaneFlags.Twoway) != 0)
				{
					trackLane.m_Flags |= TrackLaneFlags.Twoway;
				}
				if ((netLaneData.m_Flags & LaneFlags.Road) == 0)
				{
					trackLane.m_Flags |= TrackLaneFlags.Exclusive;
				}
			}
			ParkingLane parkingLane = default(ParkingLane);
			if ((netLaneData.m_Flags & LaneFlags.Parking) != 0)
			{
				if ((netLaneData.m_Flags & LaneFlags.Invert) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.Invert;
				}
				if ((netLaneData.m_Flags & LaneFlags.Virtual) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.VirtualLane;
				}
				if ((netLaneData.m_Flags & LaneFlags.PublicOnly) != 0)
				{
					parkingLane.m_Flags |= ParkingLaneFlags.SpecialVehicles;
				}
				parkingLane.m_Flags |= ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane | ParkingLaneFlags.FindConnections;
			}
			PedestrianLane pedestrianLane = default(PedestrianLane);
			if ((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
			{
				pedestrianLane.m_Flags |= PedestrianLaneFlags.AllowMiddle;
				if ((netLaneData.m_Flags & LaneFlags.OnWater) != 0)
				{
					pedestrianLane.m_Flags |= PedestrianLaneFlags.OnWater;
				}
			}
			LaneKey laneKey = new LaneKey(laneData, prefabRef.m_Prefab, netLaneData.m_Flags);
			if (original != Entity.Null)
			{
				temp.m_Original = original;
			}
			else
			{
				LaneKey laneKey2 = laneKey;
				if (isTemp)
				{
					ReplaceTempOwner(ref laneKey2, owner);
					GetOriginalLane(laneBuffer, laneKey2, ref temp);
				}
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0 && !m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed))
			{
				pseudoRandomSeed = new PseudoRandomSeed(ref outRandom);
			}
			Entity val2 = default(Entity);
			if (laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val2))
			{
				laneBuffer.m_OldLanes.Remove(laneKey);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val2, laneData);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val2, curveData);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					if (!m_PseudoRandomSeedData.HasComponent(val2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, val2, pseudoRandomSeed);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val2, pseudoRandomSeed);
					}
				}
				if ((netLaneData.m_Flags & LaneFlags.Road) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val2, carLane);
				}
				if ((netLaneData.m_Flags & LaneFlags.Track) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrackLane>(jobIndex, val2, trackLane);
				}
				if ((netLaneData.m_Flags & LaneFlags.Parking) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkingLane>(jobIndex, val2, parkingLane);
				}
				if ((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val2, pedestrianLane);
				}
				if ((netLaneData.m_Flags & LaneFlags.Utility) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val2, utilityLane);
				}
				if (math.any(val))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, val2, new Elevation(elevation));
				}
				else if (m_ElevationData.HasComponent(val2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Elevation>(jobIndex, val2);
				}
				if (isTemp)
				{
					laneBuffer.m_Updates.Add(ref val2);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val2, temp);
				}
				else if (m_TempData.HasComponent(val2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val2, ref m_DeletedTempTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val2, ref m_AppliedTypes);
				}
				else
				{
					laneBuffer.m_Updates.Add(ref val2);
				}
				return;
			}
			NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[prefabRef.m_Prefab];
			Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netLaneArchetypeData.m_LaneArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val3, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val3, laneData);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val3, curveData);
			if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val3, pseudoRandomSeed);
			}
			if ((netLaneData.m_Flags & LaneFlags.Road) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val3, carLane);
			}
			if ((netLaneData.m_Flags & LaneFlags.Track) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrackLane>(jobIndex, val3, trackLane);
			}
			if ((netLaneData.m_Flags & LaneFlags.Parking) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkingLane>(jobIndex, val3, parkingLane);
			}
			if ((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val3, pedestrianLane);
			}
			if ((netLaneData.m_Flags & LaneFlags.Utility) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val3, utilityLane);
			}
			if ((netLaneData.m_Flags & LaneFlags.Secondary) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<SecondaryLane>(jobIndex, val3);
			}
			if (isTemp)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val3, ref m_TempOwnerTypes);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val3, owner2);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val3, temp);
				if (original != Entity.Null)
				{
					if (m_OverriddenData.HasComponent(original))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Overridden>(jobIndex, val3, default(Overridden));
					}
					DynamicBuffer<CutRange> val4 = default(DynamicBuffer<CutRange>);
					if (m_CutRanges.TryGetBuffer(original, ref val4))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<CutRange>(jobIndex, val3).CopyFrom(val4);
					}
				}
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val3, owner2);
			}
			if (math.any(val))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, val3, new Elevation(elevation));
			}
		}

		private void ReplaceTempOwner(ref LaneKey laneKey, Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (m_TempData.HasComponent(owner))
			{
				Temp temp = m_TempData[owner];
				if (temp.m_Original != Entity.Null && (!m_EdgeData.HasComponent(temp.m_Original) || m_EdgeData.HasComponent(owner)))
				{
					laneKey.ReplaceOwner(owner, temp.m_Original);
				}
			}
		}

		private void GetOriginalLane(LaneBuffer laneBuffer, LaneKey laneKey, ref Temp temp)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Entity original = default(Entity);
			if (laneBuffer.m_OriginalLanes.TryGetValue(laneKey, ref original))
			{
				temp.m_Original = original;
				laneBuffer.m_OriginalLanes.Remove(laneKey);
			}
		}

		private void GetMiddleConnectionCurves(Entity node, NativeList<EdgeTarget> edgeTargets)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			edgeTargets.Clear();
			float3 position = m_NodeData[node].m_Position;
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_Edges, m_EdgeData, m_TempData, m_HiddenData, includeMiddleConnections: true);
			EdgeIteratorValue value;
			EdgeTarget edgeTarget = default(EdgeTarget);
			float num = default(float);
			while (edgeIterator.GetNext(out value))
			{
				if (value.m_Middle)
				{
					Edge edge = m_EdgeData[value.m_Edge];
					EdgeNodeGeometry geometry = m_StartNodeGeometryData[value.m_Edge].m_Geometry;
					EdgeNodeGeometry geometry2 = m_EndNodeGeometryData[value.m_Edge].m_Geometry;
					edgeTarget.m_Edge = value.m_Edge;
					edgeTarget.m_StartNode = ((math.any(geometry.m_Left.m_Length > 0.05f) | math.any(geometry.m_Right.m_Length > 0.05f)) ? edge.m_Start : value.m_Edge);
					edgeTarget.m_EndNode = ((math.any(geometry2.m_Left.m_Length > 0.05f) | math.any(geometry2.m_Right.m_Length > 0.05f)) ? edge.m_End : value.m_Edge);
					Curve curve = m_CurveData[value.m_Edge];
					EdgeGeometry edgeGeometry = m_EdgeGeometryData[value.m_Edge];
					MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref position)).xz, ref num);
					float3 val = MathUtils.Position(curve.m_Bezier, num);
					float3 val2 = MathUtils.Tangent(curve.m_Bezier, num);
					if (math.dot(((float3)(ref position)).xz - ((float3)(ref val)).xz, MathUtils.Right(((float3)(ref val2)).xz)) < 0f)
					{
						edgeTarget.m_StartPos = edgeGeometry.m_Start.m_Left.a;
						edgeTarget.m_EndPos = edgeGeometry.m_End.m_Left.d;
						edgeTarget.m_StartTangent = math.normalizesafe(-MathUtils.StartTangent(edgeGeometry.m_Start.m_Left), default(float3));
						edgeTarget.m_EndTangent = math.normalizesafe(MathUtils.EndTangent(edgeGeometry.m_End.m_Left), default(float3));
					}
					else
					{
						edgeTarget.m_StartPos = edgeGeometry.m_Start.m_Right.a;
						edgeTarget.m_EndPos = edgeGeometry.m_End.m_Right.d;
						edgeTarget.m_StartTangent = math.normalizesafe(-MathUtils.StartTangent(edgeGeometry.m_Start.m_Right), default(float3));
						edgeTarget.m_EndTangent = math.normalizesafe(MathUtils.EndTangent(edgeGeometry.m_End.m_Right), default(float3));
					}
					edgeTargets.Add(ref edgeTarget);
				}
			}
		}

		private void FilterNodeConnectPositions(Entity owner, Entity original, NativeList<ConnectPosition> connectPositions, NativeList<EdgeTarget> edgeTargets)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = -1;
			bool flag = false;
			for (int i = 0; i < connectPositions.Length; i++)
			{
				ConnectPosition connectPosition = connectPositions[i];
				if (connectPosition.m_GroupIndex != num2)
				{
					num2 = connectPosition.m_GroupIndex;
					flag = false;
				}
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					if (!flag)
					{
						for (int j = i + 1; j < connectPositions.Length; j++)
						{
							ConnectPosition connectPosition2 = connectPositions[j];
							if (connectPosition2.m_GroupIndex != num2)
							{
								break;
							}
							if (CheckConnectPosition(owner, original, connectPosition2, edgeTargets))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}
					}
				}
				else
				{
					if (!CheckConnectPosition(owner, original, connectPosition, edgeTargets))
					{
						continue;
					}
					flag = true;
				}
				connectPositions[num++] = connectPosition;
			}
			connectPositions.RemoveRange(num, connectPositions.Length - num);
		}

		private bool CheckConnectPosition(Entity owner, Entity original, ConnectPosition connectPosition, NativeList<EdgeTarget> edgeTargets)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			float num = float.MaxValue;
			for (int i = 0; i < edgeTargets.Length; i++)
			{
				EdgeTarget edgeTarget = edgeTargets[i];
				float2 val2 = ((float3)(ref connectPosition.m_Position)).xz - ((float3)(ref edgeTarget.m_StartPos)).xz;
				float2 val3 = ((float3)(ref connectPosition.m_Position)).xz - ((float3)(ref edgeTarget.m_EndPos)).xz;
				float num2 = math.dot(val2, ((float3)(ref edgeTarget.m_StartTangent)).xz);
				float num3 = math.dot(val3, ((float3)(ref edgeTarget.m_EndTangent)).xz);
				float num4 = math.length(val2);
				float num5 = math.length(val3);
				Entity val4;
				float num6;
				if (num2 > 0f)
				{
					val4 = edgeTarget.m_StartNode;
					num6 = num4 + num2;
				}
				else if (num3 > 0f)
				{
					val4 = edgeTarget.m_EndNode;
					num6 = num5 + num3;
				}
				else
				{
					val4 = edgeTarget.m_Edge;
					num6 = math.select(num4 + num2, num5 + num3, num5 < num4);
				}
				if (num6 < num)
				{
					val = val4;
					num = num6;
				}
			}
			if (!(val == owner))
			{
				return val == original;
			}
			return true;
		}

		private unsafe void GetMiddleConnections(Entity owner, Entity original, NativeList<MiddleConnection> middleConnections, NativeList<EdgeTarget> tempEdgeTargets, NativeList<ConnectPosition> tempBuffer1, NativeList<ConnectPosition> tempBuffer2, ref int groupIndex)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_EdgeData, m_TempData, m_HiddenData);
			int maxCount = edgeIterator.GetMaxCount();
			StackList<EdgeIteratorValueSorted> list = StackList<EdgeIteratorValueSorted>.op_Implicit(new Span<EdgeIteratorValueSorted>((void*)stackalloc EdgeIteratorValueSorted[maxCount], maxCount));
			edgeIterator.AddSorted(ref m_BuildOrderData, ref list);
			for (int i = 0; i < list.Length; i++)
			{
				EdgeIteratorValueSorted edgeIteratorValueSorted = list[i];
				DynamicBuffer<ConnectedNode> val = m_Nodes[edgeIteratorValueSorted.m_Edge];
				for (int j = 0; j < val.Length; j++)
				{
					ConnectedNode connectedNode = val[j];
					GetMiddleConnectionCurves(connectedNode.m_Node, tempEdgeTargets);
					bool flag = false;
					for (int k = 0; k < tempEdgeTargets.Length; k++)
					{
						EdgeTarget edgeTarget = tempEdgeTargets[k];
						if (edgeTarget.m_StartNode == owner || edgeTarget.m_StartNode == original || edgeTarget.m_EndNode == owner || edgeTarget.m_EndNode == original)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
					int groupIndex2 = groupIndex;
					GetNodeConnectPositions(connectedNode.m_Node, connectedNode.m_CurvePosition, tempBuffer1, tempBuffer2, includeAnchored: true, ref groupIndex2, out var _, out var _);
					FilterNodeConnectPositions(owner, original, tempBuffer1, tempEdgeTargets);
					FilterNodeConnectPositions(owner, original, tempBuffer2, tempEdgeTargets);
					Entity val2 = default(Entity);
					if (tempBuffer1.Length != 0)
					{
						val2 = tempBuffer1[0].m_Owner;
					}
					else if (tempBuffer2.Length != 0)
					{
						val2 = tempBuffer2[0].m_Owner;
					}
					if (val2 != Entity.Null)
					{
						flag = false;
						for (int l = 0; l < middleConnections.Length; l++)
						{
							if (middleConnections[l].m_ConnectPosition.m_Owner == val2)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							groupIndex = groupIndex2;
							for (int m = 0; m < tempBuffer1.Length; m++)
							{
								MiddleConnection middleConnection = default(MiddleConnection);
								middleConnection.m_ConnectPosition = tempBuffer1[m];
								middleConnection.m_ConnectPosition.m_IsSideConnection = true;
								middleConnection.m_SourceEdge = edgeIteratorValueSorted.m_Edge;
								middleConnection.m_SourceNode = connectedNode.m_Node;
								middleConnection.m_SortIndex = middleConnections.Length;
								middleConnection.m_Distance = float.MaxValue;
								middleConnection.m_IsSource = true;
								middleConnections.Add(ref middleConnection);
							}
							for (int n = 0; n < tempBuffer2.Length; n++)
							{
								MiddleConnection middleConnection2 = default(MiddleConnection);
								middleConnection2.m_ConnectPosition = tempBuffer2[n];
								middleConnection2.m_ConnectPosition.m_IsSideConnection = true;
								middleConnection2.m_SourceEdge = edgeIteratorValueSorted.m_Edge;
								middleConnection2.m_SourceNode = connectedNode.m_Node;
								middleConnection2.m_SortIndex = middleConnections.Length;
								middleConnection2.m_Distance = float.MaxValue;
								middleConnection2.m_IsSource = false;
								middleConnections.Add(ref middleConnection2);
							}
						}
					}
					tempBuffer1.Clear();
					tempBuffer2.Clear();
				}
			}
		}

		private RoadTypes GetRoundaboutRoadPassThrough(Entity owner)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			RoadTypes roadTypes = RoadTypes.None;
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(owner, ref val))
			{
				PrefabRef prefabRef = default(PrefabRef);
				NetObjectData netObjectData = default(NetObjectData);
				for (int i = 0; i < val.Length; i++)
				{
					if (m_PrefabRefData.TryGetComponent(val[i].m_SubObject, ref prefabRef) && m_PrefabNetObjectData.TryGetComponent(prefabRef.m_Prefab, ref netObjectData) && (netObjectData.m_CompositionFlags.m_General & CompositionFlags.General.Roundabout) != 0)
					{
						roadTypes |= netObjectData.m_RoadPassThrough;
					}
				}
			}
			return roadTypes;
		}

		private unsafe void GetNodeConnectPositions(Entity owner, float curvePosition, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, bool includeAnchored, ref int groupIndex, out float middleRadius, out float roundaboutSize)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			middleRadius = 0f;
			roundaboutSize = 0f;
			bool flag = false;
			NativeParallelHashSet<Entity> anchorPrefabs = default(NativeParallelHashSet<Entity>);
			float2 val = default(float2);
			if (m_ElevationData.HasComponent(owner))
			{
				val = m_ElevationData[owner].m_Elevation;
			}
			PrefabRef prefabRef = m_PrefabRefData[owner];
			NetGeometryData prefabGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, owner, m_Edges, m_EdgeData, m_TempData, m_HiddenData);
			int maxCount = edgeIterator.GetMaxCount();
			StackList<EdgeIteratorValueSorted> list = StackList<EdgeIteratorValueSorted>.op_Implicit(new Span<EdgeIteratorValueSorted>((void*)stackalloc EdgeIteratorValueSorted[maxCount], maxCount));
			edgeIterator.AddSorted(ref m_BuildOrderData, ref list);
			for (int i = 0; i < list.Length; i++)
			{
				EdgeIteratorValueSorted edgeIteratorValueSorted = list[i];
				GetNodeConnectPositions(owner, edgeIteratorValueSorted.m_Edge, edgeIteratorValueSorted.m_End, groupIndex++, curvePosition, val, prefabGeometryData, sourceBuffer, targetBuffer, includeAnchored, ref middleRadius, ref roundaboutSize, ref anchorPrefabs);
				flag = true;
			}
			if (!flag && m_DefaultNetLanes.HasBuffer(prefabRef.m_Prefab))
			{
				Node node = m_NodeData[owner];
				NodeGeometry nodeGeometry = default(NodeGeometry);
				if (m_NodeGeometryData.TryGetComponent(owner, ref nodeGeometry))
				{
					node.m_Position.y = nodeGeometry.m_Position;
				}
				DynamicBuffer<DefaultNetLane> val2 = m_DefaultNetLanes[prefabRef.m_Prefab];
				LaneFlags laneFlags = ((!includeAnchored) ? LaneFlags.FindAnchor : ((LaneFlags)0));
				for (int j = 0; j < val2.Length; j++)
				{
					NetCompositionLane laneData = new NetCompositionLane(val2[j]);
					if ((laneData.m_Flags & LaneFlags.Utility) == 0 || ((laneData.m_Flags & laneFlags) != 0 && IsAnchored(owner, ref anchorPrefabs, laneData.m_Lane)))
					{
						continue;
					}
					bool flag2 = (laneData.m_Flags & LaneFlags.Invert) != 0;
					if (((uint)laneData.m_Flags & (uint)(flag2 ? 512 : 256)) == 0)
					{
						laneData.m_Position.x = 0f - laneData.m_Position.x;
						float num = laneData.m_Position.x / math.max(1f, prefabGeometryData.m_DefaultWidth) + 0.5f;
						float3 val3 = node.m_Position + math.rotate(node.m_Rotation, new float3(prefabGeometryData.m_DefaultWidth * -0.5f, 0f, 0f));
						float3 val4 = math.lerp(node.m_Position + math.rotate(node.m_Rotation, new float3(prefabGeometryData.m_DefaultWidth * 0.5f, 0f, 0f)), val3, num);
						ConnectPosition connectPosition = new ConnectPosition
						{
							m_LaneData = laneData,
							m_Owner = owner,
							m_Position = val4
						};
						connectPosition.m_Position.y += laneData.m_Position.y;
						connectPosition.m_Tangent = math.forward(node.m_Rotation);
						connectPosition.m_Tangent = -MathUtils.Normalize(connectPosition.m_Tangent, ((float3)(ref connectPosition.m_Tangent)).xz);
						connectPosition.m_Tangent.y = math.clamp(connectPosition.m_Tangent.y, -1f, 1f);
						connectPosition.m_GroupIndex = (ushort)(laneData.m_Group | (groupIndex << 8));
						connectPosition.m_CurvePosition = curvePosition;
						connectPosition.m_BaseHeight = val4.y;
						connectPosition.m_Elevation = math.lerp(val.x, val.y, 0.5f);
						connectPosition.m_Order = num;
						if ((laneData.m_Flags & LaneFlags.Road) != 0)
						{
							connectPosition.m_RoadTypes = m_CarLaneData[laneData.m_Lane].m_RoadTypes;
						}
						if ((laneData.m_Flags & LaneFlags.Track) != 0)
						{
							connectPosition.m_TrackTypes = m_TrackLaneData[laneData.m_Lane].m_TrackTypes;
						}
						if ((laneData.m_Flags & LaneFlags.Utility) != 0)
						{
							connectPosition.m_UtilityTypes = m_UtilityLaneData[laneData.m_Lane].m_UtilityTypes;
						}
						if ((laneData.m_Flags & (LaneFlags.Pedestrian | LaneFlags.Utility)) != 0)
						{
							targetBuffer.Add(ref connectPosition);
						}
						else if ((laneData.m_Flags & LaneFlags.Twoway) != 0)
						{
							targetBuffer.Add(ref connectPosition);
							sourceBuffer.Add(ref connectPosition);
						}
						else if (!flag2)
						{
							targetBuffer.Add(ref connectPosition);
						}
						else
						{
							sourceBuffer.Add(ref connectPosition);
						}
					}
				}
				groupIndex++;
			}
			if (anchorPrefabs.IsCreated)
			{
				anchorPrefabs.Dispose();
			}
		}

		private unsafe void GetNodeConnectPositions(Entity node, Entity edge, bool isEnd, int groupIndex, float curvePosition, float2 elevation, NetGeometryData prefabGeometryData, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, bool includeAnchored, ref float middleRadius, ref float roundaboutSize, ref NativeParallelHashSet<Entity> anchorPrefabs)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0842: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Unknown result type (might be due to invalid IL or missing references)
			//IL_087f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_091b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			//IL_097e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0991: Unknown result type (might be due to invalid IL or missing references)
			//IL_0998: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			Composition composition = m_CompositionData[edge];
			PrefabRef prefabRef = m_PrefabRefData[edge];
			CompositionData compositionData = GetCompositionData(composition.m_Edge);
			NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
			NetCompositionData netCompositionData2 = m_PrefabCompositionData[isEnd ? composition.m_EndNode : composition.m_StartNode];
			NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
			DynamicBuffer<NetCompositionLane> val = m_PrefabCompositionLanes[composition.m_Edge];
			EdgeGeometry edgeGeometry = m_EdgeGeometryData[edge];
			EdgeNodeGeometry geometry;
			if (isEnd)
			{
				edgeGeometry.m_Start.m_Left = MathUtils.Invert(edgeGeometry.m_End.m_Right);
				edgeGeometry.m_Start.m_Right = MathUtils.Invert(edgeGeometry.m_End.m_Left);
				geometry = m_EndNodeGeometryData[edge].m_Geometry;
			}
			else
			{
				geometry = m_StartNodeGeometryData[edge].m_Geometry;
			}
			middleRadius = math.max(middleRadius, geometry.m_MiddleRadius);
			roundaboutSize = math.max(roundaboutSize, math.select(netCompositionData2.m_RoundaboutSize.x, netCompositionData2.m_RoundaboutSize.y, isEnd));
			bool isSideConnection = (netGeometryData.m_MergeLayers & prefabGeometryData.m_MergeLayers) == 0 && (prefabGeometryData.m_MergeLayers & Layer.Road) != 0;
			LaneFlags laneFlags = ((!includeAnchored) ? LaneFlags.FindAnchor : ((LaneFlags)0));
			if (!m_UpdatedData.HasComponent(edge) && m_SubLanes.HasBuffer(edge))
			{
				DynamicBuffer<SubLane> val2 = m_SubLanes[edge];
				float num = math.select(0f, 1f, isEnd);
				bool* ptr = stackalloc bool[(int)(uint)val.Length];
				for (int i = 0; i < val.Length; i++)
				{
					ptr[i] = false;
				}
				float2 val4 = default(float2);
				for (int j = 0; j < val2.Length; j++)
				{
					Entity subLane = val2[j].m_SubLane;
					if (!m_EdgeLaneData.HasComponent(subLane) || m_SecondaryLaneData.HasComponent(subLane))
					{
						continue;
					}
					bool2 val3 = m_EdgeLaneData[subLane].m_EdgeDelta == num;
					if (!math.any(val3))
					{
						continue;
					}
					bool y = val3.y;
					Curve curve = m_CurveData[subLane];
					if (y)
					{
						curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
					}
					int num2 = -1;
					float num3 = float.MaxValue;
					PrefabRef prefabRef2 = m_PrefabRefData[subLane];
					NetLaneData netLaneData = m_NetLaneData[prefabRef2.m_Prefab];
					LaneFlags laneFlags2 = (y ? LaneFlags.DisconnectedEnd : LaneFlags.DisconnectedStart);
					LaneFlags laneFlags3 = netLaneData.m_Flags & (LaneFlags.Road | LaneFlags.Pedestrian | LaneFlags.Parking | LaneFlags.Track | LaneFlags.Utility | LaneFlags.Underground | LaneFlags.OnWater);
					LaneFlags laneFlags4 = LaneFlags.Invert | LaneFlags.Slave | LaneFlags.Master | LaneFlags.Road | LaneFlags.Pedestrian | LaneFlags.Parking | LaneFlags.Track | LaneFlags.Utility | LaneFlags.Underground | LaneFlags.OnWater | laneFlags2;
					if (y != isEnd)
					{
						laneFlags3 |= LaneFlags.Invert;
					}
					if (m_SlaveLaneData.HasComponent(subLane))
					{
						laneFlags3 |= LaneFlags.Slave;
					}
					if (m_MasterLaneData.HasComponent(subLane))
					{
						laneFlags3 |= LaneFlags.Master;
						laneFlags3 &= ~LaneFlags.Track;
						laneFlags4 &= ~LaneFlags.Track;
					}
					else if ((netLaneData.m_Flags & laneFlags2) != 0)
					{
						continue;
					}
					TrackLaneData trackLaneData = default(TrackLaneData);
					UtilityLaneData utilityLaneData = default(UtilityLaneData);
					if ((netLaneData.m_Flags & LaneFlags.Track) != 0)
					{
						trackLaneData = m_TrackLaneData[prefabRef2.m_Prefab];
					}
					if ((netLaneData.m_Flags & LaneFlags.Utility) != 0)
					{
						utilityLaneData = m_UtilityLaneData[prefabRef2.m_Prefab];
					}
					for (int k = 0; k < val.Length; k++)
					{
						NetCompositionLane netCompositionLane = val[k];
						if ((netCompositionLane.m_Flags & laneFlags4) != laneFlags3 || ((netCompositionLane.m_Flags & laneFlags) != 0 && IsAnchored(node, ref anchorPrefabs, netCompositionLane.m_Lane)) || ((laneFlags3 & LaneFlags.Track) != 0 && m_TrackLaneData[netCompositionLane.m_Lane].m_TrackTypes != trackLaneData.m_TrackTypes) || ((laneFlags3 & LaneFlags.Utility) != 0 && m_UtilityLaneData[netCompositionLane.m_Lane].m_UtilityTypes != utilityLaneData.m_UtilityTypes))
						{
							continue;
						}
						netCompositionLane.m_Position.x = math.select(0f - netCompositionLane.m_Position.x, netCompositionLane.m_Position.x, isEnd);
						float num4 = netCompositionLane.m_Position.x / math.max(1f, netCompositionData.m_Width) + 0.5f;
						if (MathUtils.Intersect(new Line2(((float3)(ref edgeGeometry.m_Start.m_Right.a)).xz, ((float3)(ref edgeGeometry.m_Start.m_Left.a)).xz), new Line2(((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve.m_Bezier.b)).xz), ref val4))
						{
							float num5 = math.abs(num4 - val4.x);
							if (num5 < num3)
							{
								num2 = k;
								num3 = num5;
							}
						}
					}
					if (num2 != -1 && !ptr[num2])
					{
						ptr[num2] = true;
						NetCompositionLane laneData = val[num2];
						laneData.m_Position.x = math.select(0f - laneData.m_Position.x, laneData.m_Position.x, isEnd);
						float order = laneData.m_Position.x / math.max(1f, netCompositionData.m_Width) + 0.5f;
						laneData.m_Index = (byte)(m_LaneData[subLane].m_MiddleNode.GetLaneIndex() & 0xFF);
						ConnectPosition connectPosition = default(ConnectPosition);
						connectPosition.m_LaneData = laneData;
						connectPosition.m_Owner = edge;
						connectPosition.m_NodeComposition = (isEnd ? composition.m_EndNode : composition.m_StartNode);
						connectPosition.m_EdgeComposition = composition.m_Edge;
						connectPosition.m_Position = curve.m_Bezier.a;
						connectPosition.m_Tangent = MathUtils.StartTangent(curve.m_Bezier);
						connectPosition.m_Tangent = -MathUtils.Normalize(connectPosition.m_Tangent, ((float3)(ref connectPosition.m_Tangent)).xz);
						connectPosition.m_Tangent.y = math.clamp(connectPosition.m_Tangent.y, -1f, 1f);
						connectPosition.m_SegmentIndex = (byte)math.select(0, 4, isEnd);
						connectPosition.m_GroupIndex = (ushort)(laneData.m_Group | (groupIndex << 8));
						connectPosition.m_CompositionData = compositionData;
						connectPosition.m_CurvePosition = curvePosition;
						connectPosition.m_BaseHeight = curve.m_Bezier.a.y;
						connectPosition.m_BaseHeight -= laneData.m_Position.y;
						connectPosition.m_Elevation = math.lerp(elevation.x, elevation.y, 0.5f);
						connectPosition.m_IsEnd = isEnd;
						connectPosition.m_Order = order;
						connectPosition.m_IsSideConnection = isSideConnection;
						if ((laneData.m_Flags & LaneFlags.Road) != 0)
						{
							connectPosition.m_RoadTypes = m_CarLaneData[laneData.m_Lane].m_RoadTypes;
						}
						if ((laneData.m_Flags & LaneFlags.Track) != 0)
						{
							connectPosition.m_TrackTypes = m_TrackLaneData[laneData.m_Lane].m_TrackTypes;
						}
						if ((laneData.m_Flags & LaneFlags.Utility) != 0)
						{
							connectPosition.m_UtilityTypes = m_UtilityLaneData[laneData.m_Lane].m_UtilityTypes;
						}
						if ((laneData.m_Flags & (LaneFlags.Pedestrian | LaneFlags.Utility)) != 0)
						{
							targetBuffer.Add(ref connectPosition);
						}
						else if ((laneData.m_Flags & LaneFlags.Twoway) != 0)
						{
							targetBuffer.Add(ref connectPosition);
							sourceBuffer.Add(ref connectPosition);
						}
						else if (!y)
						{
							targetBuffer.Add(ref connectPosition);
						}
						else
						{
							sourceBuffer.Add(ref connectPosition);
						}
					}
				}
				return;
			}
			for (int l = 0; l < val.Length; l++)
			{
				NetCompositionLane laneData2 = val[l];
				bool flag = isEnd == ((laneData2.m_Flags & LaneFlags.Invert) == 0);
				if (((uint)laneData2.m_Flags & (uint)(flag ? 512 : 256)) == 0 && ((laneData2.m_Flags & laneFlags) == 0 || !IsAnchored(node, ref anchorPrefabs, laneData2.m_Lane)))
				{
					laneData2.m_Position.x = math.select(0f - laneData2.m_Position.x, laneData2.m_Position.x, isEnd);
					float num6 = laneData2.m_Position.x / math.max(1f, netCompositionData.m_Width) + 0.5f;
					Bezier4x3 val5 = MathUtils.Lerp(edgeGeometry.m_Start.m_Right, edgeGeometry.m_Start.m_Left, num6);
					ConnectPosition connectPosition2 = new ConnectPosition
					{
						m_LaneData = laneData2,
						m_Owner = edge,
						m_NodeComposition = (isEnd ? composition.m_EndNode : composition.m_StartNode),
						m_EdgeComposition = composition.m_Edge,
						m_Position = val5.a
					};
					connectPosition2.m_Position.y += laneData2.m_Position.y;
					connectPosition2.m_Tangent = MathUtils.StartTangent(val5);
					connectPosition2.m_Tangent = -MathUtils.Normalize(connectPosition2.m_Tangent, ((float3)(ref connectPosition2.m_Tangent)).xz);
					connectPosition2.m_Tangent.y = math.clamp(connectPosition2.m_Tangent.y, -1f, 1f);
					connectPosition2.m_SegmentIndex = (byte)math.select(0, 4, isEnd);
					connectPosition2.m_GroupIndex = (ushort)(laneData2.m_Group | (groupIndex << 8));
					connectPosition2.m_CompositionData = compositionData;
					connectPosition2.m_CurvePosition = curvePosition;
					connectPosition2.m_BaseHeight = val5.a.y;
					connectPosition2.m_Elevation = math.lerp(elevation.x, elevation.y, 0.5f);
					connectPosition2.m_IsEnd = isEnd;
					connectPosition2.m_Order = num6;
					connectPosition2.m_IsSideConnection = isSideConnection;
					if ((laneData2.m_Flags & LaneFlags.Road) != 0)
					{
						connectPosition2.m_RoadTypes = m_CarLaneData[laneData2.m_Lane].m_RoadTypes;
					}
					if ((laneData2.m_Flags & LaneFlags.Track) != 0)
					{
						connectPosition2.m_TrackTypes = m_TrackLaneData[laneData2.m_Lane].m_TrackTypes;
					}
					if ((laneData2.m_Flags & LaneFlags.Utility) != 0)
					{
						connectPosition2.m_UtilityTypes = m_UtilityLaneData[laneData2.m_Lane].m_UtilityTypes;
					}
					if ((laneData2.m_Flags & (LaneFlags.Pedestrian | LaneFlags.Utility)) != 0)
					{
						targetBuffer.Add(ref connectPosition2);
					}
					else if ((laneData2.m_Flags & LaneFlags.Twoway) != 0)
					{
						targetBuffer.Add(ref connectPosition2);
						sourceBuffer.Add(ref connectPosition2);
					}
					else if (!flag)
					{
						targetBuffer.Add(ref connectPosition2);
					}
					else
					{
						sourceBuffer.Add(ref connectPosition2);
					}
				}
			}
		}

		private void FilterMainCarConnectPositions(NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_LaneData.m_Flags & (LaneFlags.Slave | LaneFlags.Road)) == LaneFlags.Road)
				{
					output.Add(ref connectPosition);
				}
			}
		}

		private void FilterActualCarConnectPositions(RoadTypes roadTypes, NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_LaneData.m_Flags & (LaneFlags.Master | LaneFlags.Road)) == LaneFlags.Road && (connectPosition.m_RoadTypes & roadTypes) != RoadTypes.None)
				{
					output.Add(ref connectPosition);
				}
			}
		}

		private void FilterActualCarConnectPositions(ConnectPosition main, NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_LaneData.m_Flags & (LaneFlags.Master | LaneFlags.Road)) == LaneFlags.Road && connectPosition.m_Owner == main.m_Owner && connectPosition.m_LaneData.m_Group == main.m_LaneData.m_Group)
				{
					output.Add(ref connectPosition);
				}
			}
		}

		private TrackTypes FilterTrackConnectPositions(NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			TrackTypes trackTypes = TrackTypes.None;
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_LaneData.m_Flags & (LaneFlags.Master | LaneFlags.Track)) == LaneFlags.Track)
				{
					output.Add(ref connectPosition);
					trackTypes |= connectPosition.m_TrackTypes;
				}
			}
			return trackTypes;
		}

		private void FilterTrackConnectPositions(ref int index, TrackTypes trackType, NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			ConnectPosition connectPosition = default(ConnectPosition);
			while (index < input.Length)
			{
				connectPosition = input[index++];
				if ((connectPosition.m_TrackTypes & trackType) != TrackTypes.None)
				{
					output.Add(ref connectPosition);
					break;
				}
			}
			while (index < input.Length)
			{
				ConnectPosition connectPosition2 = input[index];
				if (!(connectPosition2.m_Owner != connectPosition.m_Owner))
				{
					if ((connectPosition2.m_TrackTypes & trackType) != TrackTypes.None)
					{
						output.Add(ref connectPosition2);
					}
					index++;
					continue;
				}
				break;
			}
		}

		private void FilterTrackConnectPositions(TrackTypes trackType, NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_TrackTypes & trackType) != TrackTypes.None)
				{
					output.Add(ref connectPosition);
				}
			}
		}

		private void FilterPedestrianConnectPositions(NativeList<ConnectPosition> input, NativeList<ConnectPosition> output, NativeList<MiddleConnection> middleConnections, bool onWater)
		{
			LaneFlags laneFlags = (onWater ? (LaneFlags.Pedestrian | LaneFlags.OnWater) : LaneFlags.Pedestrian);
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_LaneData.m_Flags & (LaneFlags.Pedestrian | LaneFlags.OnWater)) == laneFlags)
				{
					output.Add(ref connectPosition);
				}
			}
			for (int j = 0; j < middleConnections.Length; j++)
			{
				MiddleConnection middleConnection = middleConnections[j];
				if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & (LaneFlags.Pedestrian | LaneFlags.OnWater)) == laneFlags)
				{
					output.Add(ref middleConnection.m_ConnectPosition);
				}
			}
		}

		private UtilityTypes FilterUtilityConnectPositions(NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			UtilityTypes utilityTypes = UtilityTypes.None;
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_LaneData.m_Flags & LaneFlags.Utility) != 0)
				{
					output.Add(ref connectPosition);
					utilityTypes |= connectPosition.m_UtilityTypes;
				}
			}
			return utilityTypes;
		}

		private void FilterUtilityConnectPositions(UtilityTypes utilityTypes, NativeList<ConnectPosition> input, NativeList<ConnectPosition> output)
		{
			for (int i = 0; i < input.Length; i++)
			{
				ConnectPosition connectPosition = input[i];
				if ((connectPosition.m_UtilityTypes & utilityTypes) != UtilityTypes.None)
				{
					output.Add(ref connectPosition);
				}
			}
		}

		private int CalculateYieldOffset(ConnectPosition source, NativeList<ConnectPosition> sources, NativeList<ConnectPosition> targets)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionData netCompositionData = m_PrefabCompositionData[source.m_NodeComposition];
			if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.AllWayStop) != 0)
			{
				return 2;
			}
			if ((netCompositionData.m_Flags.m_General & (CompositionFlags.General.LevelCrossing | CompositionFlags.General.TrafficLights)) != 0)
			{
				return 0;
			}
			Entity val = Entity.Null;
			for (int i = 0; i < sources.Length; i++)
			{
				ConnectPosition connectPosition = sources[i];
				if (connectPosition.m_Owner != source.m_Owner && connectPosition.m_Owner != val && connectPosition.m_CompositionData.m_Priority - source.m_CompositionData.m_Priority > 0.99f)
				{
					if (val != Entity.Null)
					{
						return 1;
					}
					val = connectPosition.m_Owner;
				}
			}
			if (val == Entity.Null)
			{
				if ((source.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0)
				{
					int num = 0;
					for (int j = 0; j < sources.Length; j++)
					{
						ConnectPosition sourcePosition = sources[j];
						bool flag = false;
						for (int k = 0; k < targets.Length; k++)
						{
							ConnectPosition targetPosition = targets[k];
							if (targetPosition.m_Owner != sourcePosition.m_Owner)
							{
								bool right;
								bool gentle;
								bool uturn;
								bool flag2 = IsTurn(sourcePosition, targetPosition, out right, out gentle, out uturn);
								flag = flag || !flag2 || gentle;
							}
						}
						if (flag)
						{
							if (sourcePosition.m_Owner == source.m_Owner)
							{
								return 0;
							}
							num++;
						}
					}
					return math.select(0, 1, num >= 1);
				}
				return 0;
			}
			for (int l = 0; l < targets.Length; l++)
			{
				ConnectPosition connectPosition2 = targets[l];
				if (connectPosition2.m_Owner != source.m_Owner && connectPosition2.m_Owner != val && connectPosition2.m_CompositionData.m_Priority - source.m_CompositionData.m_Priority > 0.99f)
				{
					return 1;
				}
			}
			return 0;
		}

		private void ProcessCarConnectPositions(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<MiddleConnection> middleConnections, NativeParallelHashSet<ConnectionKey> createdConnections, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, NativeList<ConnectPosition> allSources, RoadTypes roadPassThrough, bool isTemp, Temp ownerTemp, int yield)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (sourceBuffer.Length >= 1 && targetBuffer.Length >= 1)
			{
				NativeSortExtension.Sort<ConnectPosition, SourcePositionComparer>(sourceBuffer, default(SourcePositionComparer));
				ConnectPosition sourcePosition = sourceBuffer[0];
				SortTargets(sourcePosition, targetBuffer);
				CreateNodeCarLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, middleConnections, createdConnections, sourceBuffer, targetBuffer, allSources, roadPassThrough, isTemp, ownerTemp, yield);
			}
		}

		private void SortTargets(ConnectPosition sourcePosition, NativeList<ConnectPosition> targetBuffer)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(sourcePosition.m_Tangent.z, 0f - sourcePosition.m_Tangent.x);
			for (int i = 0; i < targetBuffer.Length; i++)
			{
				ConnectPosition connectPosition = targetBuffer[i];
				float2 val2 = ((float3)(ref connectPosition.m_Position)).xz - ((float3)(ref sourcePosition.m_Position)).xz;
				val2 -= ((float3)(ref connectPosition.m_Tangent)).xz;
				MathUtils.TryNormalize(ref val2);
				float order;
				if (math.dot(((float3)(ref sourcePosition.m_Tangent)).xz, val2) > 0f)
				{
					order = math.dot(val, val2) * 0.5f;
				}
				else
				{
					float num = math.dot(val, val2);
					order = math.select(-1f, 1f, num >= 0f) - num * 0.5f;
				}
				connectPosition.m_Order = order;
				targetBuffer[i] = connectPosition;
			}
			NativeSortExtension.Sort<ConnectPosition, TargetPositionComparer>(targetBuffer, default(TargetPositionComparer));
		}

		private int2 CalculateSourcesBetween(ConnectPosition source, ConnectPosition target, NativeList<ConnectPosition> allSources)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			float2 val = MathUtils.Right(((float3)(ref target.m_Position)).xz - ((float3)(ref source.m_Position)).xz);
			int2 val2 = int2.op_Implicit(0);
			for (int i = 0; i < allSources.Length; i++)
			{
				ConnectPosition sourcePosition = allSources[i];
				if (sourcePosition.m_GroupIndex != source.m_GroupIndex && (sourcePosition.m_LaneData.m_Flags & (LaneFlags.Master | LaneFlags.Road)) == LaneFlags.Road && (sourcePosition.m_RoadTypes & source.m_RoadTypes) != RoadTypes.None && !(IsTurn(sourcePosition, target, out var _, out var _, out var uturn) && uturn))
				{
					val2 += math.select(new int2(0, 1), new int2(1, 0), math.dot(val, ((float3)(ref target.m_Position)).xz - ((float3)(ref sourcePosition.m_Position)).xz) > 0f);
				}
			}
			return val2;
		}

		private void CreateNodeCarLanes(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<MiddleConnection> middleConnections, NativeParallelHashSet<ConnectionKey> createdConnections, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, NativeList<ConnectPosition> allSources, RoadTypes roadPassThrough, bool isTemp, Temp ownerTemp, int yield)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f11: Unknown result type (might be due to invalid IL or missing references)
			ConnectPosition sourcePosition = sourceBuffer[0];
			ConnectPosition sourcePosition2 = sourceBuffer[sourceBuffer.Length - 1];
			NetCompositionData netCompositionData = m_PrefabCompositionData[sourcePosition.m_NodeComposition];
			CompositionFlags.Side num = (((netCompositionData.m_Flags.m_General & CompositionFlags.General.Invert) != 0 != ((sourcePosition.m_LaneData.m_Flags & LaneFlags.Invert) != 0)) ? netCompositionData.m_Flags.m_Left : netCompositionData.m_Flags.m_Right);
			bool flag = (num & CompositionFlags.Side.ForbidLeftTurn) != 0;
			bool flag2 = (num & CompositionFlags.Side.ForbidRightTurn) != 0;
			bool flag3 = (num & CompositionFlags.Side.ForbidStraight) != 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			while (num2 < targetBuffer.Length)
			{
				ConnectPosition targetPosition = targetBuffer[num2];
				int i;
				for (i = num2 + 1; i < targetBuffer.Length; i++)
				{
					ConnectPosition connectPosition = targetBuffer[i];
					if (connectPosition.m_GroupIndex != targetPosition.m_GroupIndex)
					{
						break;
					}
					targetPosition = connectPosition;
				}
				if (!IsTurn(sourcePosition, targetPosition, out var right, out var _, out var uturn) || right || !uturn)
				{
					break;
				}
				num2 = i;
				if (targetPosition.m_Owner == sourcePosition.m_Owner && targetPosition.m_LaneData.m_Carriageway == sourcePosition.m_LaneData.m_Carriageway)
				{
					num3 = i;
				}
				if (flag)
				{
					num4 = i;
				}
			}
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			while (num5 < targetBuffer.Length - num2)
			{
				ConnectPosition targetPosition2 = targetBuffer[targetBuffer.Length - num5 - 1];
				int j;
				for (j = num5 + 1; j < targetBuffer.Length - num2; j++)
				{
					ConnectPosition connectPosition2 = targetBuffer[targetBuffer.Length - j - 1];
					if (connectPosition2.m_GroupIndex != targetPosition2.m_GroupIndex)
					{
						break;
					}
					targetPosition2 = connectPosition2;
				}
				if (!IsTurn(sourcePosition2, targetPosition2, out var right2, out var _, out var uturn2) || !right2 || !uturn2)
				{
					break;
				}
				num5 = j;
				if ((targetPosition2.m_Owner == sourcePosition2.m_Owner && targetPosition2.m_LaneData.m_Carriageway == sourcePosition2.m_LaneData.m_Carriageway) || flag2)
				{
					num6 = j;
				}
				if (flag2)
				{
					num7 = j;
				}
			}
			int num8 = 0;
			int num9 = 0;
			while (num2 + num8 < targetBuffer.Length - num5)
			{
				ConnectPosition targetPosition3 = targetBuffer[num2 + num8];
				int k;
				for (k = num8 + 1; num2 + k < targetBuffer.Length - num5; k++)
				{
					ConnectPosition connectPosition3 = targetBuffer[num2 + k];
					if (connectPosition3.m_GroupIndex != targetPosition3.m_GroupIndex)
					{
						break;
					}
					targetPosition3 = connectPosition3;
				}
				if (!IsTurn(sourcePosition, targetPosition3, out var right3, out var _, out var _) || right3)
				{
					break;
				}
				num8 = k;
				if (flag)
				{
					num9 = k;
				}
			}
			int num10 = 0;
			int num11 = 0;
			while (num5 + num10 < targetBuffer.Length - num2 - num8)
			{
				ConnectPosition targetPosition4 = targetBuffer[targetBuffer.Length - num5 - num10 - 1];
				int l;
				for (l = num10 + 1; num5 + l < targetBuffer.Length - num2 - num8; l++)
				{
					ConnectPosition connectPosition4 = targetBuffer[targetBuffer.Length - num5 - l - 1];
					if (connectPosition4.m_GroupIndex != targetPosition4.m_GroupIndex)
					{
						break;
					}
					targetPosition4 = connectPosition4;
				}
				if (!IsTurn(sourcePosition2, targetPosition4, out var right4, out var _, out var _) || !right4)
				{
					break;
				}
				num10 = l;
				if (flag2)
				{
					num11 = l;
				}
			}
			int num12 = num2 + num5;
			int num13 = num8 + num10;
			int num14 = targetBuffer.Length - num12;
			int num15 = num14 - num13;
			int num16 = math.select(0, num15, flag3);
			int num17 = num15 - num16;
			int num18 = math.min(sourceBuffer.Length, num14);
			if (num3 + num6 == targetBuffer.Length)
			{
				num4 = math.max(0, num4 - num3);
				num7 = math.max(0, num7 - num6);
				num3 = 0;
				num6 = 0;
			}
			int num19 = num8 - num9;
			int num20 = num10 - num11;
			int num21 = num19 + num20;
			int num22 = num2 - math.max(num3, num4);
			int num23 = num5 - math.max(num6, num7);
			int num24 = num22 + num23;
			int num25 = sourceBuffer.Length - num18;
			int num26 = math.min(num22, math.max(0, num25 * num22 + num24 - 1) / math.max(1, num24));
			int num27 = math.min(num23, math.max(0, num25 * num23 + num24 - 1) / math.max(1, num24));
			if (num26 + num27 > num25)
			{
				if (m_LeftHandTraffic)
				{
					num27 = num25 - num26;
				}
				else
				{
					num26 = num25 - num27;
				}
			}
			int num28 = math.min(num18, num17);
			if (num28 >= 2 && num14 >= 4)
			{
				int num29 = math.max(num19, num20);
				int num30 = math.max(0, num29 - 1) * sourceBuffer.Length / (num14 - 1);
				num28 = math.clamp(sourceBuffer.Length - num30, 1, num28);
			}
			num25 = num18 - num28;
			int num31 = math.min(num19, math.max(0, num25 * num19 + num21 - 1) / math.max(1, num21));
			int num32 = math.min(num20, math.max(0, num25 * num20 + num21 - 1) / math.max(1, num21));
			if (num31 + num32 > num25)
			{
				if (num20 > num19)
				{
					num31 = num25 - num32;
				}
				else if (num19 > num20)
				{
					num32 = num25 - num31;
				}
				else if (m_LeftHandTraffic)
				{
					num31 = num25 - num32;
				}
				else
				{
					num32 = num25 - num31;
				}
			}
			num25 = sourceBuffer.Length - num28 - num26 - num27 - num31 - num32;
			if (num25 > 0)
			{
				if (num17 > 0)
				{
					num28 += num25;
				}
				else if (num21 > 0)
				{
					int num33 = (num25 * num19 + num21 - 1) / num21;
					int num34 = (num25 * num20 + num21 - 1) / num21;
					if (num33 + num34 > num25)
					{
						if (m_LeftHandTraffic)
						{
							num33 = num25 - num34;
						}
						else
						{
							num34 = num25 - num33;
						}
					}
					num31 += num33;
					num32 += num34;
				}
				else if (num24 > 0)
				{
					int num35 = (num25 * num22 + num24 - 1) / num24;
					int num36 = (num25 * num23 + num24 - 1) / num24;
					if (num35 + num36 > num25)
					{
						if (m_LeftHandTraffic)
						{
							num35 = num25 - num36;
						}
						else
						{
							num36 = num25 - num35;
						}
					}
					num26 += num35;
					num27 += num36;
				}
				else if (num15 > 0)
				{
					num28 += num25;
				}
				else if (num13 > 0)
				{
					int num37 = (num25 * num8 + num13 - 1) / num13;
					int num38 = (num25 * num10 + num13 - 1) / num13;
					if (num37 + num38 > num25)
					{
						if (m_LeftHandTraffic)
						{
							num37 = num25 - num38;
						}
						else
						{
							num38 = num25 - num37;
						}
					}
					num31 += num37;
					num32 += num38;
				}
				else if (num12 > 0)
				{
					int num39 = (num25 * num2 + num12 - 1) / num12;
					int num40 = (num25 * num5 + num12 - 1) / num12;
					if (num39 + num40 > num25)
					{
						if (m_LeftHandTraffic)
						{
							num39 = num25 - num40;
						}
						else
						{
							num40 = num25 - num39;
						}
					}
					num26 += num39;
					num27 += num40;
				}
				else
				{
					num28 += num25;
				}
			}
			int num41 = math.max(num26, math.select(0, 1, num2 != 0));
			int num42 = math.max(num27, math.select(0, 1, num5 != 0));
			int num43 = num31 + math.select(0, 1, (num8 > num31) & (sourceBuffer.Length > num31));
			int num44 = num32 + math.select(0, 1, (num10 > num32) & (sourceBuffer.Length > num32));
			if (num28 == 0 && num43 > num31 && num44 > num32)
			{
				if (num44 > num43)
				{
					num44 = math.max(1, num44 - 1);
				}
				else if (num43 > num44)
				{
					num43 = math.max(1, num43 - 1);
				}
				else if (m_LeftHandTraffic ? (num43 <= 1) : (num44 > 1))
				{
					num44 = math.max(1, num44 - 1);
				}
				else
				{
					num43 = math.max(1, num43 - 1);
				}
			}
			int num45 = 0;
			while (num45 < targetBuffer.Length)
			{
				ConnectPosition connectPosition5 = targetBuffer[num45];
				ConnectPosition targetPosition5 = connectPosition5;
				int m;
				for (m = num45 + 1; m < targetBuffer.Length; m++)
				{
					ConnectPosition connectPosition6 = targetBuffer[m];
					if (connectPosition6.m_GroupIndex != connectPosition5.m_GroupIndex)
					{
						break;
					}
					targetPosition5 = connectPosition6;
				}
				int num46 = m - num45;
				int num47 = targetBuffer.Length - m;
				uint num48 = (uint)(sourcePosition.m_GroupIndex | (connectPosition5.m_GroupIndex << 16));
				bool flag4 = num45 < num2 + num8 || num47 < num5 + num10;
				bool flag5 = num45 < num2 || num47 < num5;
				bool flag6 = num45 < num3 || num47 < num6;
				bool flag7 = num45 < num4 + num9 || num47 < num7 + num11;
				bool flag8 = num47 < num5 + num10;
				bool isGentle = false;
				int num49;
				int num50;
				if (flag4)
				{
					bool right5;
					bool uturn5;
					if (flag5)
					{
						if (flag8)
						{
							num49 = (num47 * num42 + math.select(0, num5 - 1, num42 > num5)) / num5;
							num50 = ((num47 + num46) * num42 + num5 - 1) / num5 - 1;
							num49 = sourceBuffer.Length - num49 - 1;
							num50 = sourceBuffer.Length - num50 - 1;
							CommonUtils.Swap(ref num49, ref num50);
						}
						else
						{
							int num51 = num45;
							num49 = (num51 * num41 + math.select(0, num2 - 1, num41 > num2)) / num2;
							num50 = ((num51 + num46) * num41 + num2 - 1) / num2 - 1;
						}
					}
					else if (flag8)
					{
						int num52 = num47 - num5;
						num49 = (num52 * num44 + math.select(0, num10 - 1, num44 > num10)) / num10;
						num50 = ((num52 + num46) * num44 + num10 - 1) / num10 - 1;
						num49 = sourceBuffer.Length - num27 - num49 - 1;
						num50 = sourceBuffer.Length - num27 - num50 - 1;
						CommonUtils.Swap(ref num49, ref num50);
						IsTurn(sourceBuffer[num49], connectPosition5, out right5, out var gentle5, out uturn5);
						IsTurn(sourceBuffer[num50], targetPosition5, out uturn5, out var gentle6, out right5);
						isGentle = gentle5 && gentle6;
					}
					else
					{
						int num53 = num45 - num2;
						num49 = (num53 * num43 + math.select(0, num8 - 1, num43 > num8)) / num8;
						num50 = ((num53 + num46) * num43 + num8 - 1) / num8 - 1;
						num49 = num26 + num49;
						num50 = num26 + num50;
						IsTurn(sourceBuffer[num49], connectPosition5, out right5, out var gentle7, out uturn5);
						IsTurn(sourceBuffer[num50], targetPosition5, out uturn5, out var gentle8, out right5);
						isGentle = gentle7 && gentle8;
					}
				}
				else
				{
					int num54 = num45 - num2 - num8;
					if (num28 == 0)
					{
						num49 = ((!m_LeftHandTraffic) ? math.min(num26 + num31, sourceBuffer.Length - 1) : math.max(num26 + num31 - 1, 0));
						num50 = num49;
					}
					else
					{
						num49 = (num54 * num28 + math.select(0, num15 - 1, num28 > num15)) / num15;
						num50 = ((num54 + num46) * num28 + num15 - 1) / num15 - 1;
						num49 = num26 + num31 + num49;
						num50 = num26 + num31 + num50;
					}
					if (num16 > 0)
					{
						flag7 = ((!m_LeftHandTraffic) ? (flag7 || num15 - num54 - 1 < num16) : (flag7 || num54 < num16));
					}
				}
				int num55 = num50 - num49 + 1;
				int num56 = math.max(num55, num46);
				int num57 = math.min(num55, num46);
				int num58 = 0;
				int num59 = num56 - num57;
				int num60 = 0;
				int num61 = 0;
				float num62 = float.MaxValue;
				int2 val = int2.op_Implicit(0);
				if (num46 > num55)
				{
					val = CalculateSourcesBetween(sourceBuffer[num49], targetBuffer[num45], allSources);
					if (math.any(val >= 1))
					{
						int num63 = math.csum(val);
						int num64;
						int num65;
						if (num63 > num59)
						{
							num64 = val.x * num59 / num63;
							num65 = val.y * num59 / num63;
							if ((num59 >= 2) & math.all(val >= 1))
							{
								num64 = math.max(num64, 1);
								num65 = math.max(num65, 1);
							}
						}
						else
						{
							num64 = val.x;
							num65 = val.y;
						}
						num58 += num64;
						num59 -= num65;
					}
				}
				for (int n = num58; n <= num59; n++)
				{
					int num66 = math.max(n + num55 - num56, 0);
					int num67 = math.max(n + num46 - num56, 0);
					num66 += num49;
					num67 += num45;
					ConnectPosition connectPosition7 = sourceBuffer[num66];
					ConnectPosition connectPosition8 = sourceBuffer[num66 + num57 - 1];
					ConnectPosition connectPosition9 = targetBuffer[num67];
					ConnectPosition connectPosition10 = targetBuffer[num67 + num57 - 1];
					float num68 = math.max(0f, math.dot(connectPosition7.m_Tangent, connectPosition9.m_Tangent) * -0.5f);
					float num69 = math.max(0f, math.dot(connectPosition8.m_Tangent, connectPosition10.m_Tangent) * -0.5f);
					num68 *= math.distance(((float3)(ref connectPosition7.m_Position)).xz, ((float3)(ref connectPosition9.m_Position)).xz);
					num69 *= math.distance(((float3)(ref connectPosition8.m_Position)).xz, ((float3)(ref connectPosition10.m_Position)).xz);
					ref float3 reference = ref connectPosition7.m_Position;
					((float3)(ref reference)).xz = ((float3)(ref reference)).xz + ((float3)(ref connectPosition7.m_Tangent)).xz * num68;
					ref float3 reference2 = ref connectPosition9.m_Position;
					((float3)(ref reference2)).xz = ((float3)(ref reference2)).xz + ((float3)(ref connectPosition9.m_Tangent)).xz * num68;
					ref float3 reference3 = ref connectPosition8.m_Position;
					((float3)(ref reference3)).xz = ((float3)(ref reference3)).xz + ((float3)(ref connectPosition8.m_Tangent)).xz * num69;
					ref float3 reference4 = ref connectPosition10.m_Position;
					((float3)(ref reference4)).xz = ((float3)(ref reference4)).xz + ((float3)(ref connectPosition10.m_Tangent)).xz * num69;
					float num70 = math.distancesq(((float3)(ref connectPosition7.m_Position)).xz, ((float3)(ref connectPosition9.m_Position)).xz);
					float num71 = math.distancesq(((float3)(ref connectPosition8.m_Position)).xz, ((float3)(ref connectPosition10.m_Position)).xz);
					float num72 = math.max(num70, num71);
					if (num72 < num62)
					{
						num60 = math.min(num56 - num46 - n, 0);
						num61 = math.min(num56 - num55 - n, 0);
						num62 = num72;
					}
				}
				for (int num73 = 0; num73 < num56; num73++)
				{
					int num74 = math.clamp(num73 + num60, 0, num55 - 1);
					int num75 = math.clamp(num73 + num61, 0, num46 - 1);
					bool flag9 = num73 + num60 < 0;
					bool flag10 = num73 + num60 >= num55;
					bool flag11 = num73 + num61 < 0;
					bool flag12 = num73 + num61 >= num46;
					bool flag13 = flag9 || flag11;
					bool flag14 = flag10 || flag12;
					bool isUnsafe = ((!flag4) ? (flag13 || flag14 || flag7) : ((!flag8) ? (flag13 || flag14 || flag6 || flag7 || (flag5 && num26 == 0)) : (flag13 || flag14 || flag6 || flag7 || (flag5 && num27 == 0))));
					num74 += num49;
					num75 += num45;
					bool isLeftLimit = num74 == 0 && num75 == 0;
					bool isRightLimit = (num74 == sourceBuffer.Length - 1) & (num75 == targetBuffer.Length - 1);
					ConnectPosition sourcePosition3 = sourceBuffer[num74];
					ConnectPosition connectPosition11 = targetBuffer[num75];
					if ((sourcePosition3.m_CompositionData.m_RoadFlags & connectPosition11.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) == 0 || !((flag9 & (val.x > 0)) | (flag10 & (val.y > 0))))
					{
						float curviness = -1f;
						bool isSkipped = false;
						if (CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, sourcePosition3, connectPosition11, num48, (ushort)num73, isUnsafe, flag7, isTemp, trackOnly: false, yield, ownerTemp, flag4, flag8, isGentle, flag5, isRoundabout: false, isLeftLimit, isRightLimit, flag13, flag14, fixedTangents: false, roadPassThrough))
						{
							createdConnections.Add(new ConnectionKey(sourcePosition3, connectPosition11));
						}
						if (isSkipped)
						{
							connectPosition11.m_SkippedCount++;
							targetBuffer[num75] = connectPosition11;
						}
						else if (flag7)
						{
							connectPosition11.m_UnsafeCount++;
							connectPosition11.m_ForbiddenCount++;
							targetBuffer[num75] = connectPosition11;
						}
						else if (flag5 && (flag8 ? num27 : num26) == 0)
						{
							connectPosition11.m_UnsafeCount++;
							targetBuffer[num75] = connectPosition11;
						}
					}
				}
				num45 = m;
			}
		}

		private void ProcessTrackConnectPositions(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<MiddleConnection> middleConnections, NativeParallelHashSet<ConnectionKey> createdConnections, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, bool isTemp, Temp ownerTemp)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			NativeSortExtension.Sort<ConnectPosition, SourcePositionComparer>(sourceBuffer, default(SourcePositionComparer));
			ConnectPosition sourcePosition = sourceBuffer[0];
			SortTargets(sourcePosition, targetBuffer);
			CreateNodeTrackLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, middleConnections, createdConnections, sourceBuffer, targetBuffer, isTemp, ownerTemp);
		}

		private void CreateNodeTrackLanes(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<MiddleConnection> middleConnections, NativeParallelHashSet<ConnectionKey> createdConnections, NativeList<ConnectPosition> sourceBuffer, NativeList<ConnectPosition> targetBuffer, bool isTemp, Temp ownerTemp)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			ConnectPosition connectPosition = sourceBuffer[0];
			for (int i = 1; i < sourceBuffer.Length; i++)
			{
				ConnectPosition connectPosition2 = sourceBuffer[i];
				ref float3 reference = ref connectPosition.m_Position;
				reference += connectPosition2.m_Position;
				ref float3 reference2 = ref connectPosition.m_Tangent;
				reference2 += connectPosition2.m_Tangent;
			}
			ref float3 reference3 = ref connectPosition.m_Position;
			reference3 /= (float)sourceBuffer.Length;
			connectPosition.m_Tangent.y = 0f;
			connectPosition.m_Tangent = math.normalizesafe(connectPosition.m_Tangent, default(float3));
			TrackLaneData trackLaneData = m_TrackLaneData[connectPosition.m_LaneData.m_Lane];
			NetCompositionData netCompositionData = m_PrefabCompositionData[connectPosition.m_NodeComposition];
			int num = targetBuffer.Length;
			int num2 = 0;
			ConnectPosition connectPosition3 = targetBuffer[0];
			int num3 = 0;
			for (int j = 1; j < targetBuffer.Length; j++)
			{
				ConnectPosition connectPosition4 = targetBuffer[j];
				if (((Entity)(ref connectPosition4.m_Owner)).Equals(connectPosition3.m_Owner))
				{
					ref float3 reference4 = ref connectPosition3.m_Position;
					reference4 += connectPosition4.m_Position;
					ref float3 reference5 = ref connectPosition3.m_Tangent;
					reference5 += connectPosition4.m_Tangent;
					continue;
				}
				ref float3 reference6 = ref connectPosition3.m_Position;
				reference6 /= (float)(j - num3);
				connectPosition3.m_Tangent.y = 0f;
				connectPosition3.m_Tangent = math.normalizesafe(connectPosition3.m_Tangent, default(float3));
				if (!((Entity)(ref connectPosition3.m_Owner)).Equals(connectPosition.m_Owner))
				{
					float distance = math.max(1f, math.distance(connectPosition.m_Position, connectPosition3.m_Position));
					if (NetUtils.CalculateCurviness(connectPosition.m_Tangent, -connectPosition3.m_Tangent, distance) <= trackLaneData.m_MaxCurviness)
					{
						num = math.min(num, num3);
						num2 = math.max(num2, j - num);
					}
				}
				connectPosition3 = connectPosition4;
				num3 = j;
			}
			ref float3 reference7 = ref connectPosition3.m_Position;
			reference7 /= (float)(targetBuffer.Length - num3);
			connectPosition3.m_Tangent.y = 0f;
			connectPosition3.m_Tangent = math.normalizesafe(connectPosition3.m_Tangent, default(float3));
			if (!((Entity)(ref connectPosition3.m_Owner)).Equals(connectPosition.m_Owner))
			{
				float distance2 = math.max(1f, math.distance(connectPosition.m_Position, connectPosition3.m_Position));
				if (NetUtils.CalculateCurviness(connectPosition.m_Tangent, -connectPosition3.m_Tangent, distance2) <= trackLaneData.m_MaxCurviness)
				{
					num = math.min(num, num3);
					num2 = math.max(num2, targetBuffer.Length - num);
				}
			}
			for (int k = 0; k < sourceBuffer.Length; k++)
			{
				ConnectPosition sourcePosition = sourceBuffer[k];
				for (int l = 0; l < num2; l++)
				{
					ConnectPosition targetPosition = targetBuffer[num + l];
					if (createdConnections.Contains(new ConnectionKey(sourcePosition, targetPosition)))
					{
						continue;
					}
					if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.Intersection) == 0)
					{
						float num4 = math.distance(sourcePosition.m_Position, targetPosition.m_Position);
						if (num4 > 1f)
						{
							float3 val = math.normalizesafe(sourcePosition.m_Tangent, default(float3));
							float3 val2 = -math.normalizesafe(targetPosition.m_Tangent, default(float3));
							if (math.dot(val, val2) > 0.99f)
							{
								float3 val3 = (targetPosition.m_Position - sourcePosition.m_Position) / num4;
								if (math.min(math.dot(val, val3), math.dot(val3, val2)) < 0.01f)
								{
									continue;
								}
							}
						}
					}
					bool isLeftLimit = num + l == 0;
					bool isRightLimit = num + l == targetBuffer.Length - 1;
					bool right;
					bool gentle;
					bool uturn;
					bool isTurn = IsTurn(sourcePosition, targetPosition, out right, out gentle, out uturn);
					bool isSkipped = false;
					float curviness = -1f;
					CreateNodeLane(jobIndex, ref nodeLaneIndex, ref random, ref curviness, ref isSkipped, owner, laneBuffer, middleConnections, sourcePosition, targetPosition, 0u, 0, isUnsafe: false, isForbidden: false, isTemp, trackOnly: true, 0, ownerTemp, isTurn, right, gentle, uturn, isRoundabout: false, isLeftLimit, isRightLimit, isMergeLeft: false, isMergeRight: false, fixedTangents: false, RoadTypes.None);
				}
			}
		}

		private bool IsTurn(ConnectPosition sourcePosition, ConnectPosition targetPosition, out bool right, out bool gentle, out bool uturn)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			return NetUtils.IsTurn(((float3)(ref sourcePosition.m_Position)).xz, ((float3)(ref sourcePosition.m_Tangent)).xz, ((float3)(ref targetPosition.m_Position)).xz, ((float3)(ref targetPosition.m_Tangent)).xz, out right, out gentle, out uturn);
		}

		private void ModifyCurveHeight(ref Bezier4x3 curve, float startBaseHeight, float endBaseHeight, NetCompositionData startCompositionData, NetCompositionData endCompositionData)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			float num = startBaseHeight + startCompositionData.m_SurfaceHeight.min;
			float num2 = endBaseHeight + endCompositionData.m_SurfaceHeight.min;
			float num3 = math.max(curve.a.y, curve.d.y);
			float num4 = math.min(curve.a.y, curve.d.y);
			if ((startCompositionData.m_Flags.m_General & (CompositionFlags.General.Roundabout | CompositionFlags.General.LevelCrossing)) != 0)
			{
				curve.b.y += (math.max(0f, num4 - curve.b.y) - math.max(0f, curve.b.y - num3)) * (2f / 3f);
			}
			if ((endCompositionData.m_Flags.m_General & (CompositionFlags.General.Roundabout | CompositionFlags.General.LevelCrossing)) != 0)
			{
				curve.c.y += (math.max(0f, num4 - curve.c.y) - math.max(0f, curve.c.y - num3)) * (2f / 3f);
			}
			curve.b.y += math.max(0f, num - math.max(curve.a.y, curve.b.y)) * 1.3333334f;
			curve.c.y += math.max(0f, num2 - math.max(curve.d.y, curve.c.y)) * 1.3333334f;
		}

		private bool CanConnectTrack(bool isUTurn, bool isRoundabout, ConnectPosition sourcePosition, ConnectPosition targetPosition, PrefabRef prefabRef)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			if (isUTurn || isRoundabout)
			{
				return false;
			}
			if (((sourcePosition.m_LaneData.m_Flags | targetPosition.m_LaneData.m_Flags) & LaneFlags.Master) != 0)
			{
				return false;
			}
			if ((sourcePosition.m_LaneData.m_Flags & targetPosition.m_LaneData.m_Flags & LaneFlags.Track) == 0)
			{
				return false;
			}
			TrackLaneData trackLaneData = default(TrackLaneData);
			if (m_TrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
			{
				float distance = math.max(1f, math.distance(sourcePosition.m_Position, targetPosition.m_Position));
				sourcePosition.m_Tangent.y = 0f;
				targetPosition.m_Tangent.y = 0f;
				if (NetUtils.CalculateCurviness(sourcePosition.m_Tangent, -targetPosition.m_Tangent, distance) > trackLaneData.m_MaxCurviness)
				{
					return false;
				}
				return sourcePosition.m_TrackTypes == targetPosition.m_TrackTypes;
			}
			return false;
		}

		private bool CheckPrefab(ref Entity prefab, ref Random random, out Random outRandom, LaneBuffer laneBuffer)
		{
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			if (!m_EditorMode)
			{
				DynamicBuffer<PlaceholderObjectElement> val = default(DynamicBuffer<PlaceholderObjectElement>);
				if (m_PlaceholderObjects.TryGetBuffer(prefab, ref val))
				{
					float num = -1f;
					Entity val2 = Entity.Null;
					Entity val3 = Entity.Null;
					Random val4 = default(Random);
					int num2 = 0;
					DynamicBuffer<ObjectRequirementElement> val6 = default(DynamicBuffer<ObjectRequirementElement>);
					Random val9 = default(Random);
					for (int i = 0; i < val.Length; i++)
					{
						Entity val5 = val[i].m_Object;
						float num3 = 0f;
						if (m_ObjectRequirements.TryGetBuffer(val5, ref val6))
						{
							int num4 = -1;
							bool flag = true;
							for (int j = 0; j < val6.Length; j++)
							{
								ObjectRequirementElement objectRequirementElement = val6[j];
								if (objectRequirementElement.m_Group != num4)
								{
									if (!flag)
									{
										break;
									}
									num4 = objectRequirementElement.m_Group;
									flag = false;
								}
								flag |= objectRequirementElement.m_Requirement == m_DefaultTheme;
							}
							if (!flag)
							{
								continue;
							}
						}
						SpawnableObjectData spawnableObjectData = m_PrefabSpawnableObjectData[val5];
						Entity val7 = ((spawnableObjectData.m_RandomizationGroup != Entity.Null) ? spawnableObjectData.m_RandomizationGroup : val5);
						Random val8 = random;
						((Random)(ref random)).NextInt();
						((Random)(ref random)).NextInt();
						if (laneBuffer.m_SelectedSpawnables.TryGetValue(val7, ref val9))
						{
							num3 += 0.5f;
							val8 = val9;
						}
						if (num3 > num)
						{
							num = num3;
							val2 = val5;
							val3 = val7;
							val4 = val8;
							num2 = spawnableObjectData.m_Probability;
						}
						else if (num3 == num)
						{
							num2 += spawnableObjectData.m_Probability;
							if (((Random)(ref random)).NextInt(num2) < spawnableObjectData.m_Probability)
							{
								val2 = val5;
								val3 = val7;
								val4 = val8;
							}
						}
					}
					if (((Random)(ref random)).NextInt(100) < num2)
					{
						laneBuffer.m_SelectedSpawnables.TryAdd(val3, val4);
						prefab = val2;
						outRandom = val4;
						return true;
					}
					outRandom = random;
					((Random)(ref random)).NextInt();
					((Random)(ref random)).NextInt();
					return false;
				}
				Entity val10 = prefab;
				SpawnableObjectData spawnableObjectData2 = default(SpawnableObjectData);
				if (m_PrefabSpawnableObjectData.TryGetComponent(prefab, ref spawnableObjectData2) && spawnableObjectData2.m_RandomizationGroup != Entity.Null)
				{
					val10 = spawnableObjectData2.m_RandomizationGroup;
				}
				outRandom = random;
				((Random)(ref random)).NextInt();
				((Random)(ref random)).NextInt();
				Random val11 = default(Random);
				if (laneBuffer.m_SelectedSpawnables.TryGetValue(val10, ref val11))
				{
					outRandom = val11;
				}
				else
				{
					laneBuffer.m_SelectedSpawnables.TryAdd(val10, outRandom);
				}
				return true;
			}
			outRandom = random;
			((Random)(ref random)).NextInt();
			((Random)(ref random)).NextInt();
			return true;
		}

		private bool CreateNodeLane(int jobIndex, ref int nodeLaneIndex, ref Random random, ref float curviness, ref bool isSkipped, Entity owner, LaneBuffer laneBuffer, NativeList<MiddleConnection> middleConnections, ConnectPosition sourcePosition, ConnectPosition targetPosition, uint group, ushort laneIndex, bool isUnsafe, bool isForbidden, bool isTemp, bool trackOnly, int yield, Temp ownerTemp, bool isTurn, bool isRight, bool isGentle, bool isUTurn, bool isRoundabout, bool isLeftLimit, bool isRightLimit, bool isMergeLeft, bool isMergeRight, bool fixedTangents, RoadTypes roadPassThrough)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0879: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0883: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0843: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0854: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0912: Unknown result type (might be due to invalid IL or missing references)
			//IL_0917: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_092d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0932: Unknown result type (might be due to invalid IL or missing references)
			//IL_0940: Unknown result type (might be due to invalid IL or missing references)
			//IL_0945: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0951: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0960: Unknown result type (might be due to invalid IL or missing references)
			//IL_0971: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_132b: Unknown result type (might be due to invalid IL or missing references)
			//IL_134f: Unknown result type (might be due to invalid IL or missing references)
			//IL_136c: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_1309: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_1129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c90: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_11c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1730: Unknown result type (might be due to invalid IL or missing references)
			//IL_1747: Unknown result type (might be due to invalid IL or missing references)
			//IL_1753: Unknown result type (might be due to invalid IL or missing references)
			//IL_1762: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e68: Unknown result type (might be due to invalid IL or missing references)
			//IL_1796: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_19eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_19f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_17f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a12: Unknown result type (might be due to invalid IL or missing references)
			//IL_1816: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a29: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a23: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_184e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1838: Unknown result type (might be due to invalid IL or missing references)
			//IL_1826: Unknown result type (might be due to invalid IL or missing references)
			//IL_149e: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebf: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a70: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a90: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1865: Unknown result type (might be due to invalid IL or missing references)
			//IL_1519: Unknown result type (might be due to invalid IL or missing references)
			//IL_151e: Unknown result type (might be due to invalid IL or missing references)
			//IL_152a: Unknown result type (might be due to invalid IL or missing references)
			//IL_152f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1534: Unknown result type (might be due to invalid IL or missing references)
			//IL_1542: Unknown result type (might be due to invalid IL or missing references)
			//IL_154e: Unknown result type (might be due to invalid IL or missing references)
			//IL_155a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1566: Unknown result type (might be due to invalid IL or missing references)
			//IL_156d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1572: Unknown result type (might be due to invalid IL or missing references)
			//IL_1577: Unknown result type (might be due to invalid IL or missing references)
			//IL_1589: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
			//IL_1abf: Unknown result type (might be due to invalid IL or missing references)
			//IL_187f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1893: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_15a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_18c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_18b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1618: Unknown result type (might be due to invalid IL or missing references)
			//IL_160f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b06: Unknown result type (might be due to invalid IL or missing references)
			//IL_18d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_18e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_161d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1924: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b41: Unknown result type (might be due to invalid IL or missing references)
			//IL_1672: Unknown result type (might be due to invalid IL or missing references)
			//IL_19ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_199b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1699: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bed: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_19dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_19ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_1988: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ba5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c00: Unknown result type (might be due to invalid IL or missing references)
			NetCompositionData netCompositionData = m_PrefabCompositionData[sourcePosition.m_NodeComposition];
			NetCompositionData netCompositionData2 = m_PrefabCompositionData[targetPosition.m_NodeComposition];
			if (isUTurn && (netCompositionData.m_State & CompositionState.BlockUTurn) != 0)
			{
				return false;
			}
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Temp temp = default(Temp);
			if (isTemp)
			{
				temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
			}
			LaneFlags laneFlags = (LaneFlags)0;
			PrefabRef prefabRef = default(PrefabRef);
			if (trackOnly)
			{
				laneFlags = sourcePosition.m_LaneData.m_Flags & ~(LaneFlags.Slave | LaneFlags.Master);
				prefabRef.m_Prefab = sourcePosition.m_LaneData.m_Lane;
				if ((laneFlags & (LaneFlags.Road | LaneFlags.Track)) == (LaneFlags.Road | LaneFlags.Track))
				{
					laneFlags &= ~LaneFlags.Road;
					prefabRef.m_Prefab = m_TrackLaneData[prefabRef.m_Prefab].m_FallbackPrefab;
				}
			}
			else
			{
				if ((sourcePosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					laneFlags = sourcePosition.m_LaneData.m_Flags;
					prefabRef.m_Prefab = sourcePosition.m_LaneData.m_Lane;
				}
				else if ((targetPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
				{
					laneFlags = targetPosition.m_LaneData.m_Flags;
					prefabRef.m_Prefab = targetPosition.m_LaneData.m_Lane;
				}
				else if ((sourcePosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0)
				{
					laneFlags = sourcePosition.m_LaneData.m_Flags;
					prefabRef.m_Prefab = sourcePosition.m_LaneData.m_Lane;
				}
				else if ((targetPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0)
				{
					laneFlags = targetPosition.m_LaneData.m_Flags;
					prefabRef.m_Prefab = targetPosition.m_LaneData.m_Lane;
				}
				else
				{
					laneFlags = sourcePosition.m_LaneData.m_Flags;
					prefabRef.m_Prefab = sourcePosition.m_LaneData.m_Lane;
				}
				int num = math.select(0, 1, (sourcePosition.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0);
				int num2 = math.select(0, 1, (targetPosition.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0);
				if (m_PrefabCompositionData.HasComponent(sourcePosition.m_EdgeComposition))
				{
					NetCompositionData netCompositionData3 = m_PrefabCompositionData[sourcePosition.m_EdgeComposition];
					num = math.select(num, num + 2, (netCompositionData3.m_Flags.m_General & CompositionFlags.General.Tiles) != 0);
					num = math.select(num, num - 4, (netCompositionData3.m_Flags.m_General & CompositionFlags.General.Gravel) != 0);
				}
				if (m_PrefabCompositionData.HasComponent(targetPosition.m_EdgeComposition))
				{
					NetCompositionData netCompositionData4 = m_PrefabCompositionData[targetPosition.m_EdgeComposition];
					num2 = math.select(num2, num2 + 2, (netCompositionData4.m_Flags.m_General & CompositionFlags.General.Tiles) != 0);
					num2 = math.select(num2, num2 - 4, (netCompositionData4.m_Flags.m_General & CompositionFlags.General.Gravel) != 0);
				}
				if (num > num2 && prefabRef.m_Prefab != sourcePosition.m_LaneData.m_Lane)
				{
					laneFlags = (laneFlags & (LaneFlags.Slave | LaneFlags.Master)) | (sourcePosition.m_LaneData.m_Flags & ~(LaneFlags.Slave | LaneFlags.Master));
					prefabRef.m_Prefab = sourcePosition.m_LaneData.m_Lane;
				}
				if (num2 > num && prefabRef.m_Prefab != targetPosition.m_LaneData.m_Lane)
				{
					laneFlags = (laneFlags & (LaneFlags.Slave | LaneFlags.Master)) | (targetPosition.m_LaneData.m_Flags & ~(LaneFlags.Slave | LaneFlags.Master));
					prefabRef.m_Prefab = targetPosition.m_LaneData.m_Lane;
				}
				if ((laneFlags & (LaneFlags.Road | LaneFlags.PublicOnly)) == (LaneFlags.Road | LaneFlags.PublicOnly) && (sourcePosition.m_LaneData.m_Flags & targetPosition.m_LaneData.m_Flags & LaneFlags.PublicOnly) == 0)
				{
					laneFlags &= ~LaneFlags.PublicOnly;
					prefabRef.m_Prefab = m_CarLaneData[prefabRef.m_Prefab].m_NotBusLanePrefab;
				}
				if ((laneFlags & (LaneFlags.Road | LaneFlags.Track)) == (LaneFlags.Road | LaneFlags.Track) && !CanConnectTrack(isUTurn, isRoundabout, sourcePosition, targetPosition, prefabRef))
				{
					laneFlags &= ~LaneFlags.Track;
					prefabRef.m_Prefab = m_CarLaneData[prefabRef.m_Prefab].m_NotTrackLanePrefab;
				}
			}
			CheckPrefab(ref prefabRef.m_Prefab, ref random, out var outRandom, laneBuffer);
			NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
			DynamicBuffer<AuxiliaryNetLane> val = default(DynamicBuffer<AuxiliaryNetLane>);
			int num3 = 0;
			if ((netLaneData.m_Flags & LaneFlags.HasAuxiliary) != 0)
			{
				val = m_PrefabAuxiliaryLanes[prefabRef.m_Prefab];
				num3 += val.Length;
			}
			float3 position = sourcePosition.m_Position;
			float3 position2 = targetPosition.m_Position;
			float2 val5 = default(float2);
			float2 val7 = default(float2);
			TrackLaneData trackLaneData = default(TrackLaneData);
			float2 val9 = default(float2);
			Entity val10 = default(Entity);
			for (int i = 0; i <= num3; i++)
			{
				if (i != 0)
				{
					AuxiliaryNetLane auxiliaryNetLane = val[i - 1];
					if (!NetCompositionHelpers.TestLaneFlags(auxiliaryNetLane, netCompositionData.m_Flags) || !NetCompositionHelpers.TestLaneFlags(auxiliaryNetLane, netCompositionData2.m_Flags) || auxiliaryNetLane.m_Spacing.x > 0.1f)
					{
						continue;
					}
					sourcePosition.m_Position = position;
					targetPosition.m_Position = position2;
					sourcePosition.m_Position.y += auxiliaryNetLane.m_Position.y;
					targetPosition.m_Position.y += auxiliaryNetLane.m_Position.y;
					prefabRef.m_Prefab = auxiliaryNetLane.m_Prefab;
					netLaneData = m_NetLaneData[prefabRef.m_Prefab];
					laneFlags = netLaneData.m_Flags | auxiliaryNetLane.m_Flags;
					if (auxiliaryNetLane.m_Position.z > 0.1f)
					{
						if (sourcePosition.m_Owner != owner)
						{
							if ((sourcePosition.m_LaneData.m_Flags & LaneFlags.HasAuxiliary) != 0)
							{
								DynamicBuffer<AuxiliaryNetLane> val2 = m_PrefabAuxiliaryLanes[sourcePosition.m_LaneData.m_Lane];
								for (int j = 0; j < val2.Length; j++)
								{
									AuxiliaryNetLane auxiliaryNetLane2 = val2[j];
									if (auxiliaryNetLane2.m_Prefab == auxiliaryNetLane.m_Prefab)
									{
										auxiliaryNetLane = auxiliaryNetLane2;
										break;
									}
								}
							}
							EdgeNodeGeometry nodeGeometry = ((!sourcePosition.m_IsEnd) ? m_StartNodeGeometryData[sourcePosition.m_Owner].m_Geometry : m_EndNodeGeometryData[sourcePosition.m_Owner].m_Geometry);
							ref float3 reference = ref sourcePosition.m_Position;
							reference += CalculateAuxialryZOffset(sourcePosition.m_Position, sourcePosition.m_Tangent, nodeGeometry, netCompositionData, auxiliaryNetLane);
						}
						if (targetPosition.m_Owner != owner)
						{
							if ((targetPosition.m_LaneData.m_Flags & LaneFlags.HasAuxiliary) != 0)
							{
								DynamicBuffer<AuxiliaryNetLane> val3 = m_PrefabAuxiliaryLanes[targetPosition.m_LaneData.m_Lane];
								for (int k = 0; k < val3.Length; k++)
								{
									AuxiliaryNetLane auxiliaryNetLane3 = val3[k];
									if (auxiliaryNetLane3.m_Prefab == auxiliaryNetLane.m_Prefab)
									{
										auxiliaryNetLane = auxiliaryNetLane3;
										break;
									}
								}
							}
							EdgeNodeGeometry nodeGeometry2 = ((!targetPosition.m_IsEnd) ? m_StartNodeGeometryData[targetPosition.m_Owner].m_Geometry : m_EndNodeGeometryData[targetPosition.m_Owner].m_Geometry);
							ref float3 reference2 = ref targetPosition.m_Position;
							reference2 += CalculateAuxialryZOffset(targetPosition.m_Position, targetPosition.m_Tangent, nodeGeometry2, netCompositionData2, auxiliaryNetLane);
						}
					}
				}
				NodeLane nodeLane = default(NodeLane);
				if ((laneFlags & LaneFlags.Road) != 0)
				{
					if (m_NetLaneData.HasComponent(sourcePosition.m_LaneData.m_Lane))
					{
						NetLaneData netLaneData2 = m_NetLaneData[sourcePosition.m_LaneData.m_Lane];
						nodeLane.m_WidthOffset.x = netLaneData2.m_Width - netLaneData.m_Width;
					}
					if (m_NetLaneData.HasComponent(targetPosition.m_LaneData.m_Lane))
					{
						NetLaneData netLaneData3 = m_NetLaneData[targetPosition.m_LaneData.m_Lane];
						nodeLane.m_WidthOffset.y = netLaneData3.m_Width - netLaneData.m_Width;
					}
				}
				Curve curve = default(Curve);
				bool flag = false;
				if (math.distance(sourcePosition.m_Position, targetPosition.m_Position) >= 0.1f)
				{
					if (fixedTangents)
					{
						curve.m_Bezier = new Bezier4x3(sourcePosition.m_Position, sourcePosition.m_Position + sourcePosition.m_Tangent, targetPosition.m_Position + targetPosition.m_Tangent, targetPosition.m_Position);
					}
					else
					{
						curve.m_Bezier = NetUtils.FitCurve(sourcePosition.m_Position, sourcePosition.m_Tangent, -targetPosition.m_Tangent, targetPosition.m_Position);
					}
				}
				else
				{
					curve.m_Bezier = NetUtils.StraightCurve(sourcePosition.m_Position, targetPosition.m_Position);
					flag = true;
				}
				ModifyCurveHeight(ref curve.m_Bezier, sourcePosition.m_BaseHeight, targetPosition.m_BaseHeight, netCompositionData, netCompositionData2);
				UtilityLane utilityLane = default(UtilityLane);
				HangingLane hangingLane = default(HangingLane);
				bool flag2 = false;
				if ((laneFlags & LaneFlags.Utility) != 0)
				{
					UtilityLaneData utilityLaneData = m_UtilityLaneData[prefabRef.m_Prefab];
					if (utilityLaneData.m_Hanging != 0f)
					{
						curve.m_Bezier.b = math.lerp(curve.m_Bezier.a, curve.m_Bezier.d, 1f / 3f);
						curve.m_Bezier.c = math.lerp(curve.m_Bezier.a, curve.m_Bezier.d, 2f / 3f);
						float num4 = math.distance(((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve.m_Bezier.d)).xz) * utilityLaneData.m_Hanging * 1.3333334f;
						curve.m_Bezier.b.y -= num4;
						curve.m_Bezier.c.y -= num4;
						hangingLane.m_Distances = float2.op_Implicit(0.1f);
						flag2 = true;
					}
					if ((laneFlags & LaneFlags.FindAnchor) != 0)
					{
						utilityLane.m_Flags |= UtilityLaneFlags.SecondaryStartAnchor | UtilityLaneFlags.SecondaryEndAnchor;
						hangingLane.m_Distances = float2.op_Implicit(0f);
					}
				}
				if ((laneFlags & LaneFlags.Road) != 0 && isUTurn && sourcePosition.m_Owner == targetPosition.m_Owner && sourcePosition.m_LaneData.m_Carriageway != targetPosition.m_LaneData.m_Carriageway)
				{
					DynamicBuffer<NetCompositionPiece> val4 = m_PrefabCompositionPieces[sourcePosition.m_NodeComposition];
					((float2)(ref val5))._002Ector(math.distance(((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve.m_Bezier.b)).xz), math.distance(((float3)(ref curve.m_Bezier.d)).xz, ((float3)(ref curve.m_Bezier.c)).xz));
					float2 val6 = float2.op_Implicit(float.MinValue);
					for (int l = 0; l < val4.Length; l++)
					{
						NetCompositionPiece netCompositionPiece = val4[l];
						if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.BlockTraffic) != 0)
						{
							((float2)(ref val7))._002Ector(sourcePosition.m_LaneData.m_Position.x, targetPosition.m_LaneData.m_Position.x);
							val7 -= netCompositionPiece.m_Offset.x;
							bool2 val8 = val7 > 0f;
							if (val8.x != val8.y)
							{
								val7 = math.abs(val7) - (netLaneData.m_Width + nodeLane.m_WidthOffset);
								val7 = (netCompositionPiece.m_Size.z + netCompositionPiece.m_Size.x * 0.5f - val7) * 1.3333334f;
								val7 += math.max(float2.op_Implicit(0f), val7 - math.max(float2.op_Implicit(0f), ((float2)(ref val7)).yx));
								val6 = math.max(val6, val7 - val5);
							}
						}
					}
					if (math.any(val6 > 0f))
					{
						val6 = math.max(val6, math.max(math.min(float2.op_Implicit(0f), -((float2)(ref val6)).yx), val5 * -0.5f));
						ref float3 b = ref curve.m_Bezier.b;
						b += sourcePosition.m_Tangent * val6.x;
						ref float3 c = ref curve.m_Bezier.c;
						c += targetPosition.m_Tangent * val6.y;
					}
				}
				curve.m_Length = MathUtils.Length(curve.m_Bezier);
				bool flag3 = false;
				CarLane carLane = default(CarLane);
				if ((laneFlags & LaneFlags.Road) != 0)
				{
					carLane.m_DefaultSpeedLimit = math.lerp(sourcePosition.m_CompositionData.m_SpeedLimit, targetPosition.m_CompositionData.m_SpeedLimit, 0.5f);
					if (curviness < 0f)
					{
						curviness = NetUtils.CalculateCurviness(curve, m_NetLaneData[prefabRef.m_Prefab].m_Width);
					}
					carLane.m_Curviness = curviness;
					bool flag4 = (sourcePosition.m_CompositionData.m_RoadFlags & targetPosition.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0;
					if (isUnsafe)
					{
						carLane.m_Flags |= CarLaneFlags.Unsafe;
					}
					if (isForbidden)
					{
						carLane.m_Flags |= CarLaneFlags.Forbidden;
					}
					if (isRoundabout)
					{
						carLane.m_Flags |= CarLaneFlags.Roundabout;
					}
					if (isLeftLimit)
					{
						carLane.m_Flags |= CarLaneFlags.LeftLimit;
					}
					if (isRightLimit)
					{
						carLane.m_Flags |= CarLaneFlags.RightLimit;
					}
					if (flag4)
					{
						carLane.m_Flags |= CarLaneFlags.Highway;
					}
					if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.Intersection) != 0 || isRoundabout || isUTurn)
					{
						carLane.m_CarriagewayGroup = 0;
						carLane.m_Flags |= CarLaneFlags.ForbidPassing;
						if (isTurn)
						{
							if (isGentle)
							{
								carLane.m_Flags |= (CarLaneFlags)(isRight ? 524288 : 262144);
							}
							else if (isUTurn)
							{
								carLane.m_Flags |= (CarLaneFlags)(isRight ? 131072 : 2);
							}
							else
							{
								carLane.m_Flags |= (CarLaneFlags)(isRight ? 32 : 16);
							}
						}
						else
						{
							carLane.m_Flags |= CarLaneFlags.Forward;
						}
					}
					else
					{
						if (sourcePosition.m_Owner.Index >= targetPosition.m_Owner.Index)
						{
							carLane.m_CarriagewayGroup = targetPosition.m_LaneData.m_Carriageway;
						}
						else
						{
							carLane.m_CarriagewayGroup = sourcePosition.m_LaneData.m_Carriageway;
						}
						if (flag4)
						{
							if (m_PrefabCompositionData.HasComponent(sourcePosition.m_EdgeComposition) && (m_PrefabCompositionData[sourcePosition.m_EdgeComposition].m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes | CompositionState.Multilane)) == (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes | CompositionState.Multilane))
							{
								carLane.m_Flags |= CarLaneFlags.ForbidPassing;
							}
							if (m_PrefabCompositionData.HasComponent(targetPosition.m_EdgeComposition) && (m_PrefabCompositionData[targetPosition.m_EdgeComposition].m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes | CompositionState.Multilane)) == (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes | CompositionState.Multilane))
							{
								carLane.m_Flags |= CarLaneFlags.ForbidPassing;
							}
						}
						if (carLane.m_Curviness > math.select((float)Math.PI / 180f, (float)Math.PI / 360f, flag4))
						{
							carLane.m_Flags |= CarLaneFlags.ForbidPassing;
						}
					}
					if ((sourcePosition.m_LaneData.m_Flags & targetPosition.m_LaneData.m_Flags & LaneFlags.Twoway) != 0)
					{
						if (sourcePosition.m_Owner.Index >= targetPosition.m_Owner.Index)
						{
							return false;
						}
						carLane.m_Flags |= CarLaneFlags.Twoway;
					}
					if ((sourcePosition.m_LaneData.m_Flags & targetPosition.m_LaneData.m_Flags & LaneFlags.PublicOnly) != 0)
					{
						carLane.m_Flags |= CarLaneFlags.PublicOnly;
					}
					if ((sourcePosition.m_CompositionData.m_TaxiwayFlags & targetPosition.m_CompositionData.m_TaxiwayFlags & TaxiwayFlags.Runway) != 0)
					{
						carLane.m_Flags |= CarLaneFlags.Runway;
					}
					switch (yield)
					{
					case 1:
						carLane.m_Flags |= CarLaneFlags.Yield;
						flag = false;
						break;
					case 2:
						carLane.m_Flags |= CarLaneFlags.Stop;
						flag = false;
						break;
					case -1:
						carLane.m_Flags |= CarLaneFlags.RightOfWay;
						flag = false;
						break;
					}
					if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.TrafficLights) != 0)
					{
						carLane.m_Flags |= CarLaneFlags.TrafficLights;
						flag3 = true;
						flag = false;
					}
					if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.LevelCrossing) != 0)
					{
						carLane.m_Flags |= CarLaneFlags.LevelCrossing;
						flag3 = true;
						flag = false;
					}
				}
				TrackLane trackLane = default(TrackLane);
				if ((laneFlags & LaneFlags.Track) != 0)
				{
					trackLane.m_SpeedLimit = math.lerp(sourcePosition.m_CompositionData.m_SpeedLimit, targetPosition.m_CompositionData.m_SpeedLimit, 0.5f);
					if (curviness < 0f)
					{
						curviness = NetUtils.CalculateCurviness(curve, m_NetLaneData[prefabRef.m_Prefab].m_Width);
					}
					trackLane.m_Curviness = curviness;
					if (trackLane.m_Curviness > 1E-06f && m_TrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
					{
						trackLane.m_Curviness = math.min(trackLane.m_Curviness, trackLaneData.m_MaxCurviness);
					}
					if ((sourcePosition.m_LaneData.m_Flags & targetPosition.m_LaneData.m_Flags & LaneFlags.Twoway) != 0)
					{
						bool num5 = sourcePosition.m_IsEnd == ((sourcePosition.m_LaneData.m_Flags & LaneFlags.Invert) == 0);
						bool flag5 = targetPosition.m_IsEnd == ((targetPosition.m_LaneData.m_Flags & LaneFlags.Invert) == 0);
						if (num5 != flag5)
						{
							if (flag5)
							{
								return false;
							}
						}
						else if (sourcePosition.m_Owner.Index >= targetPosition.m_Owner.Index)
						{
							return false;
						}
					}
					if (((sourcePosition.m_LaneData.m_Flags | targetPosition.m_LaneData.m_Flags) & LaneFlags.Twoway) != 0)
					{
						trackLane.m_Flags |= TrackLaneFlags.Twoway;
					}
					if ((laneFlags & LaneFlags.Road) == 0)
					{
						trackLane.m_Flags |= TrackLaneFlags.Exclusive;
					}
					if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.TrafficLights) != 0)
					{
						flag3 = true;
						flag = false;
					}
					if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.LevelCrossing) != 0)
					{
						trackLane.m_Flags |= TrackLaneFlags.LevelCrossing;
						flag3 = true;
						flag = false;
					}
					if (((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right | netCompositionData2.m_Flags.m_Left | netCompositionData2.m_Flags.m_Right) & (CompositionFlags.Side.PrimaryStop | CompositionFlags.Side.SecondaryStop)) != 0)
					{
						trackLane.m_Flags |= TrackLaneFlags.Station;
					}
					if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.Intersection) != 0 && isTurn)
					{
						trackLane.m_Flags |= (TrackLaneFlags)(isRight ? 8192 : 4096);
					}
				}
				Lane lane = default(Lane);
				if (i != 0)
				{
					lane.m_StartNode = new PathNode(owner, (ushort)nodeLaneIndex++);
					lane.m_MiddleNode = new PathNode(owner, (ushort)nodeLaneIndex++);
					lane.m_EndNode = new PathNode(owner, (ushort)nodeLaneIndex++);
					flag = false;
				}
				else
				{
					lane.m_StartNode = new PathNode(sourcePosition.m_Owner, sourcePosition.m_LaneData.m_Index, sourcePosition.m_SegmentIndex);
					lane.m_MiddleNode = new PathNode(owner, (ushort)nodeLaneIndex++);
					lane.m_EndNode = new PathNode(targetPosition.m_Owner, targetPosition.m_LaneData.m_Index, targetPosition.m_SegmentIndex);
				}
				if ((carLane.m_Flags & CarLaneFlags.Unsafe) == 0)
				{
					for (int m = 0; m < middleConnections.Length; m++)
					{
						MiddleConnection middleConnection = middleConnections[m];
						if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Road) == 0 || (isRoundabout && roadPassThrough != RoadTypes.None && ((sourcePosition.m_Owner == owner && !middleConnection.m_IsSource) || (targetPosition.m_Owner == owner && middleConnection.m_IsSource))))
						{
							continue;
						}
						LaneFlags laneFlags2 = laneFlags;
						if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Master) != 0)
						{
							if ((laneFlags & LaneFlags.Slave) != 0)
							{
								continue;
							}
							laneFlags2 |= LaneFlags.Master;
						}
						else if ((middleConnection.m_ConnectPosition.m_LaneData.m_Flags & LaneFlags.Slave) != 0)
						{
							if ((laneFlags & LaneFlags.Master) != 0)
							{
								continue;
							}
							laneFlags2 |= LaneFlags.Slave;
						}
						uint num6;
						uint num7;
						if (isRoundabout)
						{
							num6 = group | (group << 16);
							num7 = uint.MaxValue;
						}
						else if ((laneFlags & LaneFlags.Master) != 0)
						{
							num6 = group;
							num7 = uint.MaxValue;
						}
						else if (middleConnection.m_IsSource)
						{
							num6 = group;
							num7 = 4294901760u;
						}
						else
						{
							num6 = group;
							num7 = 65535u;
						}
						int num8 = m;
						if (middleConnection.m_TargetLane != Entity.Null)
						{
							middleConnection.m_Distance = float.MaxValue;
							num8 = -1;
							for (; m < middleConnections.Length; m++)
							{
								MiddleConnection middleConnection2 = middleConnections[m];
								if (middleConnection2.m_SortIndex != middleConnection.m_SortIndex)
								{
									break;
								}
								if (((middleConnection2.m_TargetGroup ^ num6) & num7) == 0 && ((middleConnection2.m_TargetFlags ^ laneFlags2) & LaneFlags.Master) == 0)
								{
									middleConnection = middleConnection2;
									num8 = m;
								}
							}
							m--;
						}
						float num9 = math.length(MathUtils.Size(MathUtils.Bounds(curve.m_Bezier) | middleConnection.m_ConnectPosition.m_Position));
						float num10 = MathUtils.Distance(curve.m_Bezier, new Segment(middleConnection.m_ConnectPosition.m_Position, middleConnection.m_ConnectPosition.m_Position + middleConnection.m_ConnectPosition.m_Tangent * num9), ref val9);
						num10 += num9 * val9.y;
						if (roadPassThrough != RoadTypes.None)
						{
							if (middleConnection.m_IsSource)
							{
								val9.x = math.lerp(val9.x, 1f, 0.5f);
							}
							else
							{
								val9.x = math.lerp(0f, val9.x, 0.5f);
							}
						}
						if (num10 < middleConnection.m_Distance)
						{
							middleConnection.m_Distance = num10;
							middleConnection.m_TargetLane = prefabRef.m_Prefab;
							middleConnection.m_TargetOwner = (middleConnection.m_IsSource ? sourcePosition.m_Owner : targetPosition.m_Owner);
							middleConnection.m_TargetGroup = num6;
							middleConnection.m_TargetNode = lane.m_MiddleNode;
							middleConnection.m_TargetCarriageway = carLane.m_CarriagewayGroup;
							middleConnection.m_TargetComposition = (middleConnection.m_IsSource ? targetPosition.m_CompositionData : sourcePosition.m_CompositionData);
							middleConnection.m_TargetCurve = curve;
							middleConnection.m_TargetCurvePos = val9.x;
							middleConnection.m_TargetFlags = laneFlags2;
							if (num8 != -1)
							{
								middleConnections[num8] = middleConnection;
							}
							else
							{
								CollectionUtils.Insert<MiddleConnection>(middleConnections, m + 1, middleConnection);
							}
						}
					}
				}
				if ((laneFlags & LaneFlags.Master) != 0)
				{
					flag = isSkipped;
				}
				else if (i == 0)
				{
					isSkipped |= flag;
				}
				if (flag)
				{
					if (isTemp)
					{
						lane.m_StartNode = new PathNode(lane.m_StartNode, secondaryNode: true);
						lane.m_MiddleNode = new PathNode(lane.m_MiddleNode, secondaryNode: true);
						lane.m_EndNode = new PathNode(lane.m_EndNode, secondaryNode: true);
					}
					m_SkipLaneQueue.Enqueue(lane);
					continue;
				}
				LaneKey laneKey = new LaneKey(lane, prefabRef.m_Prefab, laneFlags);
				LaneKey laneKey2 = laneKey;
				if (isTemp)
				{
					ReplaceTempOwner(ref laneKey2, owner);
					ReplaceTempOwner(ref laneKey2, sourcePosition.m_Owner);
					ReplaceTempOwner(ref laneKey2, targetPosition.m_Owner);
					GetOriginalLane(laneBuffer, laneKey2, ref temp);
				}
				PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0 && !m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed))
				{
					pseudoRandomSeed = new PseudoRandomSeed(ref outRandom);
				}
				if (laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val10))
				{
					laneBuffer.m_OldLanes.Remove(laneKey);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val10, lane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val10, nodeLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val10, curve);
					if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
					{
						if (!m_PseudoRandomSeedData.HasComponent(val10))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, val10, pseudoRandomSeed);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val10, pseudoRandomSeed);
						}
					}
					if ((laneFlags & LaneFlags.Road) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val10, carLane);
					}
					if ((laneFlags & LaneFlags.Track) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrackLane>(jobIndex, val10, trackLane);
					}
					if ((laneFlags & LaneFlags.Utility) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val10, utilityLane);
					}
					if (flag2)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<HangingLane>(jobIndex, val10, hangingLane);
					}
					if (isTemp)
					{
						laneBuffer.m_Updates.Add(ref val10);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val10, temp);
					}
					else if (m_TempData.HasComponent(val10))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val10, ref m_DeletedTempTypes);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val10, ref m_AppliedTypes);
					}
					else
					{
						laneBuffer.m_Updates.Add(ref val10);
					}
					if ((laneFlags & LaneFlags.Master) != 0)
					{
						MasterLane masterLane = new MasterLane
						{
							m_Group = group
						};
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val10, masterLane);
					}
					if ((laneFlags & LaneFlags.Slave) != 0)
					{
						SlaveLane slaveLane = new SlaveLane
						{
							m_Group = group,
							m_MinIndex = laneIndex,
							m_MaxIndex = laneIndex,
							m_SubIndex = laneIndex
						};
						if (isMergeLeft)
						{
							slaveLane.m_Flags |= SlaveLaneFlags.MergingLane;
						}
						if (isMergeRight)
						{
							slaveLane.m_Flags |= SlaveLaneFlags.MergingLane;
						}
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val10, slaveLane);
					}
					if (flag3)
					{
						if (!m_LaneSignalData.HasComponent(val10))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LaneSignal>(jobIndex, val10, default(LaneSignal));
						}
					}
					else if (m_LaneSignalData.HasComponent(val10))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<LaneSignal>(jobIndex, val10);
					}
					continue;
				}
				EntityArchetype val11 = default(EntityArchetype);
				NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[prefabRef.m_Prefab];
				val11 = (((laneFlags & LaneFlags.Slave) != 0) ? netLaneArchetypeData.m_NodeSlaveArchetype : (((laneFlags & LaneFlags.Master) == 0) ? netLaneArchetypeData.m_NodeLaneArchetype : netLaneArchetypeData.m_NodeMasterArchetype));
				Entity val12 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, val11);
				if (((netCompositionData.m_State | netCompositionData2.m_State) & CompositionState.Hidden) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val12, ref m_HideLaneTypes);
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val12, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val12, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val12, nodeLane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val12, curve);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val12, pseudoRandomSeed);
				}
				if ((laneFlags & LaneFlags.Road) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarLane>(jobIndex, val12, carLane);
				}
				if ((laneFlags & LaneFlags.Track) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrackLane>(jobIndex, val12, trackLane);
				}
				if ((laneFlags & LaneFlags.Utility) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<UtilityLane>(jobIndex, val12, utilityLane);
				}
				if (flag2)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HangingLane>(jobIndex, val12, hangingLane);
				}
				if ((laneFlags & LaneFlags.Master) != 0)
				{
					MasterLane masterLane2 = new MasterLane
					{
						m_Group = group
					};
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MasterLane>(jobIndex, val12, masterLane2);
				}
				if ((laneFlags & LaneFlags.Slave) != 0)
				{
					SlaveLane slaveLane2 = new SlaveLane
					{
						m_Group = group,
						m_MinIndex = laneIndex,
						m_MaxIndex = laneIndex,
						m_SubIndex = laneIndex
					};
					if (isMergeLeft)
					{
						slaveLane2.m_Flags |= SlaveLaneFlags.MergingLane;
					}
					if (isMergeRight)
					{
						slaveLane2.m_Flags |= SlaveLaneFlags.MergingLane;
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<SlaveLane>(jobIndex, val12, slaveLane2);
				}
				if (isTemp)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val12, ref m_TempOwnerTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val12, owner2);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val12, temp);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val12, owner2);
				}
				if (flag3)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LaneSignal>(jobIndex, val12, default(LaneSignal));
				}
			}
			return true;
		}

		private float3 CalculateUtilityConnectPosition(NativeList<ConnectPosition> buffer, int2 bufferRange)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			NodeGeometry nodeGeometry = default(NodeGeometry);
			Segment val6 = default(Segment);
			Segment val7 = default(Segment);
			float2 val8 = default(float2);
			for (int i = bufferRange.x + 1; i < bufferRange.y; i++)
			{
				ConnectPosition connectPosition = buffer[i];
				float3 val2 = connectPosition.m_Tangent;
				if (!(math.lengthsq(((float3)(ref val2)).xz) > 0.01f))
				{
					continue;
				}
				float3 val3;
				if (!m_NodeData.HasComponent(connectPosition.m_Owner))
				{
					val3 = ((connectPosition.m_SegmentIndex == 0) ? m_StartNodeGeometryData[connectPosition.m_Owner].m_Geometry.m_Middle.d : m_EndNodeGeometryData[connectPosition.m_Owner].m_Geometry.m_Middle.d);
				}
				else
				{
					val3 = m_NodeData[connectPosition.m_Owner].m_Position;
					if (m_NodeGeometryData.TryGetComponent(connectPosition.m_Owner, ref nodeGeometry))
					{
						val3.y = nodeGeometry.m_Position;
					}
				}
				for (int j = bufferRange.x; j < i; j++)
				{
					ConnectPosition connectPosition2 = buffer[j];
					float3 val4 = connectPosition2.m_Tangent;
					if (math.lengthsq(((float3)(ref val4)).xz) > 0.01f)
					{
						float num = math.dot(((float3)(ref val2)).xz, ((float3)(ref val4)).xz);
						float2 val5 = math.distance(((float3)(ref connectPosition.m_Position)).xz, ((float3)(ref connectPosition2.m_Position)).xz) * new float2(math.max(0f, num * 0.5f), 1f - math.abs(num) * 0.5f);
						((Segment)(ref val6))._002Ector(((float3)(ref connectPosition.m_Position)).xz + ((float3)(ref val2)).xz * val5.x, ((float3)(ref connectPosition.m_Position)).xz + ((float3)(ref val2)).xz * val5.y);
						((Segment)(ref val7))._002Ector(((float3)(ref connectPosition2.m_Position)).xz + ((float3)(ref val4)).xz * val5.x, ((float3)(ref connectPosition2.m_Position)).xz + ((float3)(ref val4)).xz * val5.y);
						MathUtils.Distance(val6, val7, ref val8);
						float4 val9 = new float4
						{
							y = val3.y + connectPosition.m_Position.y - connectPosition.m_BaseHeight + val3.y + connectPosition2.m_Position.y - connectPosition2.m_BaseHeight
						};
						((float4)(ref val9)).xz = MathUtils.Position(val6, val8.x) + MathUtils.Position(val7, val8.y);
						val9.w = 2f;
						float num2 = 1.01f - math.abs(num);
						val += val9 * num2;
					}
				}
			}
			if (val.w == 0f)
			{
				return buffer[bufferRange.x].m_Position;
			}
			val /= val.w;
			return ((float4)(ref val)).xyz;
		}

		private void CreateNodeUtilityLanes(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<ConnectPosition> buffer1, NativeList<ConnectPosition> buffer2, NativeList<MiddleConnection> middleConnections, float3 middlePosition, bool isRoundabout, bool isTemp, Temp ownerTemp)
		{
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			if (buffer1.Length >= 2)
			{
				NativeSortExtension.Sort<ConnectPosition, SourcePositionComparer>(buffer1, default(SourcePositionComparer));
				Entity val = Entity.Null;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < buffer1.Length; i++)
				{
					ConnectPosition connectPosition = buffer1[i];
					if (connectPosition.m_Owner != val)
					{
						if (i - num > num2)
						{
							num3 = num;
							num2 = i - num;
						}
						val = connectPosition.m_Owner;
						num = i;
					}
				}
				if (buffer1.Length - num > num2)
				{
					num3 = num;
					num2 = buffer1.Length - num;
				}
				for (int j = 0; j < num2; j++)
				{
					ConnectPosition connectPosition2 = buffer1[num3 + j];
					connectPosition2.m_Order = j;
					buffer1[num3 + j] = buffer1[j];
					buffer1[j] = connectPosition2;
				}
				for (int k = num2; k < buffer1.Length; k++)
				{
					ConnectPosition connectPosition3 = buffer1[k];
					float num4 = float.MaxValue;
					int num5 = 0;
					for (int l = 0; l < num2; l++)
					{
						ConnectPosition connectPosition4 = buffer1[l];
						float num6 = math.distancesq(connectPosition3.m_Position, connectPosition4.m_Position);
						if (num6 < num4)
						{
							num4 = num6;
							num5 = l;
						}
					}
					connectPosition3.m_Order = num5;
					buffer1[k] = connectPosition3;
				}
				NativeSortExtension.Sort<ConnectPosition, TargetPositionComparer>(buffer1, default(TargetPositionComparer));
				float num7 = -1f;
				num = 0;
				for (int m = 0; m < buffer1.Length; m++)
				{
					ConnectPosition connectPosition5 = buffer1[m];
					if (connectPosition5.m_Order != num7)
					{
						if (m - num > 0)
						{
							CreateNodeUtilityLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, buffer1, buffer2, middleConnections, new int2(num, m), middlePosition, isRoundabout, isTemp, ownerTemp);
						}
						num7 = connectPosition5.m_Order;
						num = m;
					}
				}
				if (buffer1.Length > num)
				{
					CreateNodeUtilityLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, buffer1, buffer2, middleConnections, new int2(num, buffer1.Length), middlePosition, isRoundabout, isTemp, ownerTemp);
				}
			}
			else
			{
				CreateNodeUtilityLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, buffer1, buffer2, middleConnections, new int2(0, buffer1.Length), middlePosition, isRoundabout, isTemp, ownerTemp);
			}
			if (buffer1.Length <= 0)
			{
				return;
			}
			ConnectPosition connectPosition6 = buffer1[0];
			for (int n = 0; n < middleConnections.Length; n++)
			{
				MiddleConnection middleConnection = middleConnections[n];
				if ((middleConnection.m_ConnectPosition.m_UtilityTypes & connectPosition6.m_UtilityTypes) != UtilityTypes.None && middleConnection.m_SourceEdge == connectPosition6.m_Owner && middleConnection.m_TargetLane == Entity.Null)
				{
					float num8 = math.distance(connectPosition6.m_Position, middleConnection.m_ConnectPosition.m_Position);
					if (num8 < middleConnection.m_Distance)
					{
						middleConnection.m_Distance = num8;
						middleConnection.m_TargetLane = connectPosition6.m_LaneData.m_Lane;
						middleConnection.m_TargetNode = new PathNode(connectPosition6.m_Owner, connectPosition6.m_LaneData.m_Index, connectPosition6.m_SegmentIndex);
						middleConnection.m_TargetCurve = new Curve
						{
							m_Bezier = new Bezier4x3(connectPosition6.m_Position, connectPosition6.m_Position, connectPosition6.m_Position, connectPosition6.m_Position)
						};
						middleConnection.m_TargetCurvePos = 0f;
						middleConnection.m_TargetFlags = connectPosition6.m_LaneData.m_Flags;
						middleConnections[n] = middleConnection;
					}
				}
			}
		}

		private void CreateNodeUtilityLanes(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<ConnectPosition> buffer1, NativeList<ConnectPosition> buffer2, NativeList<MiddleConnection> middleConnections, int2 bufferRange, float3 middlePosition, bool isRoundabout, bool isTemp, Temp ownerTemp)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			int num = bufferRange.y - bufferRange.x;
			if (num == 2 && buffer2.Length == 0)
			{
				ConnectPosition connectPosition = buffer1[bufferRange.x];
				ConnectPosition connectPosition2 = buffer1[bufferRange.x + 1];
				if (connectPosition.m_LaneData.m_Lane == connectPosition2.m_LaneData.m_Lane)
				{
					CreateNodeUtilityLane(jobIndex, -1, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition, connectPosition2, middleConnections, isTemp, ownerTemp);
					return;
				}
			}
			if (num < 2 && (num <= 0 || buffer2.Length <= 0) && !(num == 1 && isRoundabout))
			{
				return;
			}
			float3 position;
			if (num >= 2)
			{
				position = CalculateUtilityConnectPosition(buffer1, bufferRange);
			}
			else if (isRoundabout)
			{
				float3 val = buffer1[bufferRange.x].m_Position;
				float3 val2 = buffer1[bufferRange.x].m_Tangent;
				position = val + val2 * math.dot(val2, middlePosition - val);
				position.y = middlePosition.y;
				position.y = middlePosition.y + val.y - buffer1[bufferRange.x].m_BaseHeight;
			}
			else
			{
				position = buffer1[bufferRange.x].m_Position;
			}
			int endNodeLaneIndex = nodeLaneIndex++;
			if (num >= 2 || isRoundabout)
			{
				for (int i = bufferRange.x; i < bufferRange.y; i++)
				{
					ConnectPosition connectPosition3 = buffer1[i];
					CreateNodeUtilityLane(jobIndex, endNodeLaneIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition3, new ConnectPosition
					{
						m_Position = position
					}, middleConnections, isTemp, ownerTemp);
				}
			}
			for (int j = 0; j < buffer2.Length; j++)
			{
				ConnectPosition connectPosition4 = buffer2[j];
				CreateNodeUtilityLane(jobIndex, endNodeLaneIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition4, new ConnectPosition
				{
					m_Position = position
				}, middleConnections, isTemp, ownerTemp);
			}
		}

		private void CreateNodeUtilityLane(int jobIndex, int endNodeLaneIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, ConnectPosition connectPosition1, ConnectPosition connectPosition2, NativeList<MiddleConnection> middleConnections, bool isTemp, Temp ownerTemp)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Temp temp = default(Temp);
			if (isTemp)
			{
				temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
			}
			Lane lane = default(Lane);
			if (connectPosition1.m_Owner != Entity.Null)
			{
				lane.m_StartNode = new PathNode(connectPosition1.m_Owner, connectPosition1.m_LaneData.m_Index, connectPosition1.m_SegmentIndex);
			}
			else
			{
				lane.m_StartNode = new PathNode(owner, (ushort)nodeLaneIndex++);
			}
			lane.m_MiddleNode = new PathNode(owner, (ushort)nodeLaneIndex++);
			if (connectPosition2.m_Owner != Entity.Null)
			{
				lane.m_EndNode = new PathNode(connectPosition2.m_Owner, connectPosition2.m_LaneData.m_Index, connectPosition2.m_SegmentIndex);
			}
			else
			{
				lane.m_EndNode = new PathNode(owner, (ushort)endNodeLaneIndex);
				float2 val = ((float3)(ref connectPosition2.m_Position)).xz - ((float3)(ref connectPosition1.m_Position)).xz;
				if (MathUtils.TryNormalize(ref val))
				{
					((float3)(ref connectPosition2.m_Tangent)).xz = math.reflect(((float3)(ref connectPosition1.m_Tangent)).xz, val);
				}
			}
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = connectPosition1.m_LaneData.m_Lane
			};
			CheckPrefab(ref prefabRef.m_Prefab, ref random, out var outRandom, laneBuffer);
			NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
			bool flag = math.distance(connectPosition1.m_Position, connectPosition2.m_Position) < 0.1f;
			Curve curve = default(Curve);
			if (((float3)(ref connectPosition1.m_Tangent)).Equals(default(float3)))
			{
				if (math.abs(connectPosition1.m_Position.y - connectPosition2.m_Position.y) >= 0.01f)
				{
					curve.m_Bezier.a = connectPosition1.m_Position;
					curve.m_Bezier.b = math.lerp(connectPosition1.m_Position, connectPosition2.m_Position, new float3(0.25f, 0.5f, 0.25f));
					curve.m_Bezier.c = math.lerp(connectPosition1.m_Position, connectPosition2.m_Position, new float3(0.75f, 0.5f, 0.75f));
					curve.m_Bezier.d = connectPosition2.m_Position;
				}
				else
				{
					curve.m_Bezier = NetUtils.StraightCurve(connectPosition1.m_Position, connectPosition2.m_Position);
				}
			}
			else
			{
				curve.m_Bezier = NetUtils.FitCurve(connectPosition1.m_Position, connectPosition1.m_Tangent, -connectPosition2.m_Tangent, connectPosition2.m_Position);
			}
			curve.m_Length = MathUtils.Length(curve.m_Bezier);
			float targetCurvePos = default(float);
			for (int i = 0; i < middleConnections.Length; i++)
			{
				MiddleConnection middleConnection = middleConnections[i];
				if ((middleConnection.m_ConnectPosition.m_UtilityTypes & connectPosition1.m_UtilityTypes) != UtilityTypes.None && (middleConnection.m_SourceEdge == connectPosition1.m_Owner || middleConnection.m_SourceEdge == connectPosition2.m_Owner))
				{
					if (middleConnection.m_TargetLane != Entity.Null && ((middleConnection.m_TargetFlags ^ middleConnection.m_ConnectPosition.m_LaneData.m_Flags) & LaneFlags.Underground) != 0 && ((connectPosition1.m_LaneData.m_Flags ^ middleConnection.m_ConnectPosition.m_LaneData.m_Flags) & LaneFlags.Underground) == 0)
					{
						middleConnection.m_Distance = float.MaxValue;
					}
					float num = MathUtils.Distance(curve.m_Bezier, middleConnection.m_ConnectPosition.m_Position, ref targetCurvePos);
					if (num < middleConnection.m_Distance)
					{
						middleConnection.m_Distance = num;
						middleConnection.m_TargetLane = connectPosition1.m_LaneData.m_Lane;
						middleConnection.m_TargetNode = lane.m_MiddleNode;
						middleConnection.m_TargetCurve = curve;
						middleConnection.m_TargetCurvePos = targetCurvePos;
						middleConnection.m_TargetFlags = connectPosition1.m_LaneData.m_Flags;
						middleConnections[i] = middleConnection;
					}
				}
			}
			if (flag)
			{
				return;
			}
			LaneKey laneKey = new LaneKey(lane, prefabRef.m_Prefab, (LaneFlags)0);
			LaneKey laneKey2 = laneKey;
			if (isTemp)
			{
				ReplaceTempOwner(ref laneKey2, owner);
				ReplaceTempOwner(ref laneKey2, connectPosition1.m_Owner);
				ReplaceTempOwner(ref laneKey2, connectPosition2.m_Owner);
				GetOriginalLane(laneBuffer, laneKey2, ref temp);
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0 && !m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed))
			{
				pseudoRandomSeed = new PseudoRandomSeed(ref outRandom);
			}
			Entity val2 = default(Entity);
			if (laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val2))
			{
				laneBuffer.m_OldLanes.Remove(laneKey);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val2, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val2, curve);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					if (!m_PseudoRandomSeedData.HasComponent(val2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, val2, pseudoRandomSeed);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val2, pseudoRandomSeed);
					}
				}
				if (isTemp)
				{
					laneBuffer.m_Updates.Add(ref val2);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val2, temp);
				}
				else if (m_TempData.HasComponent(val2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val2, ref m_DeletedTempTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val2, ref m_AppliedTypes);
				}
				else
				{
					laneBuffer.m_Updates.Add(ref val2);
				}
			}
			else
			{
				EntityArchetype nodeLaneArchetype = m_PrefabLaneArchetypeData[prefabRef.m_Prefab].m_NodeLaneArchetype;
				Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, nodeLaneArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val3, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val3, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val3, curve);
				if ((netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val3, pseudoRandomSeed);
				}
				if (isTemp)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val3, ref m_TempOwnerTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val3, owner2);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val3, temp);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val3, owner2);
				}
			}
		}

		private unsafe void CreateNodePedestrianLanes(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, NativeList<ConnectPosition> buffer, NativeList<ConnectPosition> tempBuffer, NativeList<ConnectPosition> tempBuffer2, bool isTemp, Temp ownerTemp, float3 middlePosition, float middleRadius, float roundaboutSize)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_0767: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0891: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_090d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0984: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			if (buffer.Length >= 2)
			{
				NativeSortExtension.Sort<ConnectPosition, SourcePositionComparer>(buffer, default(SourcePositionComparer));
			}
			int num = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				num += math.select(1, 0, buffer[i].m_IsSideConnection);
			}
			bool flag = true;
			if (num == 0)
			{
				num = buffer.Length;
				flag = false;
			}
			if (num == 0)
			{
				return;
			}
			int num2 = num;
			StackList<int2> val = StackList<int2>.op_Implicit(new Span<int2>((void*)stackalloc int2[num2], num2));
			Bounds1 val2 = default(Bounds1);
			((Bounds1)(ref val2))._002Ector(float.MaxValue, 0f);
			int num3 = -1;
			int num4 = -1;
			int num5 = 0;
			bool flag2 = false;
			bool flag3 = false;
			int num6 = 0;
			DynamicBuffer<NetCompositionCrosswalk> val3 = default(DynamicBuffer<NetCompositionCrosswalk>);
			Segment left = default(Segment);
			while (num6 < buffer.Length)
			{
				ConnectPosition connectPosition = buffer[num6];
				int j;
				for (j = num6 + 1; j < buffer.Length && !(buffer[j].m_Owner != connectPosition.m_Owner); j++)
				{
				}
				if (buffer[j - 1].m_IsSideConnection && flag)
				{
					num6 = j;
					continue;
				}
				if (m_PrefabCompositionCrosswalks.TryGetBuffer(connectPosition.m_NodeComposition, ref val3))
				{
					if (connectPosition.m_SegmentIndex != 0)
					{
						EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[connectPosition.m_Owner];
						if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
						{
							left = endNodeGeometry.m_Geometry.m_Left;
						}
						else
						{
							left.m_Left = endNodeGeometry.m_Geometry.m_Left.m_Left;
							left.m_Right = endNodeGeometry.m_Geometry.m_Right.m_Right;
							left.m_Length = new float2(endNodeGeometry.m_Geometry.m_Left.m_Length.x, endNodeGeometry.m_Geometry.m_Right.m_Length.y);
						}
					}
					else
					{
						StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[connectPosition.m_Owner];
						if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
						{
							left = startNodeGeometry.m_Geometry.m_Left;
						}
						else
						{
							left.m_Left = startNodeGeometry.m_Geometry.m_Left.m_Left;
							left.m_Right = startNodeGeometry.m_Geometry.m_Right.m_Right;
							left.m_Length = new float2(startNodeGeometry.m_Geometry.m_Left.m_Length.x, startNodeGeometry.m_Geometry.m_Right.m_Length.y);
						}
					}
					NetCompositionData netCompositionData = m_PrefabCompositionData[connectPosition.m_NodeComposition];
					if (val3.Length >= 1)
					{
						flag2 |= (netCompositionData.m_Flags.m_General & CompositionFlags.General.Crosswalk) != 0;
						float3 start = val3[0].m_Start;
						float3 end = val3[val3.Length - 1].m_End;
						float num7 = start.x / math.max(1f, netCompositionData.m_Width) + 0.5f;
						float num8 = end.x / math.max(1f, netCompositionData.m_Width) + 0.5f;
						float3 val4 = math.lerp(left.m_Left.a, left.m_Right.a, num7);
						end = math.lerp(left.m_Left.a, left.m_Right.a, num8);
						float num9 = math.distance(val4, end);
						if (num9 < val2.min)
						{
							num3 = num6;
						}
						val2 |= num9;
					}
					flag3 |= math.any(left.m_Length >= ((float2)(ref left.m_Length)).yx * 4f + 0.1f);
					num5++;
				}
				num6 = j;
			}
			num6 = 0;
			DynamicBuffer<NetCompositionCrosswalk> val5 = default(DynamicBuffer<NetCompositionCrosswalk>);
			Segment left2 = default(Segment);
			while (num6 < buffer.Length)
			{
				ConnectPosition targetPosition = buffer[num6];
				int k;
				for (k = num6 + 1; k < buffer.Length && !(buffer[k].m_Owner != targetPosition.m_Owner); k++)
				{
				}
				ConnectPosition connectPosition2 = buffer[k - 1];
				if (connectPosition2.m_IsSideConnection && flag)
				{
					num6 = k;
					continue;
				}
				int num10 = nodeLaneIndex;
				bool flag4 = k == num6 + 1;
				if (FindNextRightLane(connectPosition2, buffer, out var result))
				{
					ConnectPosition targetPosition2 = buffer[result.x];
					bool flag5 = result.y == result.x + 1;
					int num11 = 0;
					while (targetPosition2.m_IsSideConnection && flag)
					{
						if (++num11 > buffer.Length)
						{
							tempBuffer2.Clear();
							return;
						}
						for (int l = result.x; l < result.y; l++)
						{
							ConnectPosition connectPosition3 = buffer[l];
							tempBuffer2.Add(ref connectPosition3);
						}
						int num12 = result.y - 1;
						if (!FindNextRightLane(buffer[num12], buffer, out result))
						{
							tempBuffer2.Clear();
							return;
						}
						targetPosition2 = buffer[result.x];
					}
					if (num > 2)
					{
						CreateNodePedestrianLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition2, targetPosition2, tempBuffer2, isTemp, ownerTemp, middlePosition, middleRadius, roundaboutSize);
					}
					else if (num == 2)
					{
						if (connectPosition2.m_Owner.Index == targetPosition2.m_Owner.Index)
						{
							CreateNodePedestrianLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition2, targetPosition2, tempBuffer2, isTemp, ownerTemp, middlePosition, middleRadius, roundaboutSize);
						}
						else if (connectPosition2.m_Owner.Index < targetPosition2.m_Owner.Index || (flag4 && flag5 && middleRadius > 0f))
						{
							CreateNodePedestrianLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition2, targetPosition2, tempBuffer2, isTemp, ownerTemp, middlePosition, middleRadius, roundaboutSize);
						}
					}
					if (flag4)
					{
						val.AddNoResize(new int2(num6, result.x));
					}
				}
				if (num == 1 && middleRadius > 0f)
				{
					CreateNodePedestrianLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, connectPosition2, targetPosition, tempBuffer2, isTemp, ownerTemp, middlePosition, middleRadius, roundaboutSize);
				}
				tempBuffer2.Clear();
				if (m_PrefabCompositionCrosswalks.TryGetBuffer(targetPosition.m_NodeComposition, ref val5))
				{
					if (targetPosition.m_SegmentIndex != 0)
					{
						EndNodeGeometry endNodeGeometry2 = m_EndNodeGeometryData[targetPosition.m_Owner];
						if (endNodeGeometry2.m_Geometry.m_MiddleRadius > 0f)
						{
							left2 = endNodeGeometry2.m_Geometry.m_Left;
						}
						else
						{
							left2.m_Left = endNodeGeometry2.m_Geometry.m_Left.m_Left;
							left2.m_Right = endNodeGeometry2.m_Geometry.m_Right.m_Right;
							left2.m_Length = new float2(endNodeGeometry2.m_Geometry.m_Left.m_Length.x, endNodeGeometry2.m_Geometry.m_Right.m_Length.y);
						}
					}
					else
					{
						StartNodeGeometry startNodeGeometry2 = m_StartNodeGeometryData[targetPosition.m_Owner];
						if (startNodeGeometry2.m_Geometry.m_MiddleRadius > 0f)
						{
							left2 = startNodeGeometry2.m_Geometry.m_Left;
						}
						else
						{
							left2.m_Left = startNodeGeometry2.m_Geometry.m_Left.m_Left;
							left2.m_Right = startNodeGeometry2.m_Geometry.m_Right.m_Right;
							left2.m_Length = new float2(startNodeGeometry2.m_Geometry.m_Left.m_Length.x, startNodeGeometry2.m_Geometry.m_Right.m_Length.y);
						}
					}
					NetCompositionData netCompositionData2 = m_PrefabCompositionData[targetPosition.m_NodeComposition];
					bool flag6 = false;
					if (num5 == 2)
					{
						if ((netCompositionData2.m_Flags.m_General & (CompositionFlags.General.DeadEnd | CompositionFlags.General.Intersection | CompositionFlags.General.LevelCrossing)) == 0)
						{
							if ((netCompositionData2.m_Flags.m_General & CompositionFlags.General.Crosswalk) != 0 || !flag3)
							{
								flag6 = true;
							}
						}
						else if ((netCompositionData2.m_Flags.m_General & (CompositionFlags.General.DeadEnd | CompositionFlags.General.LevelCrossing)) == 0)
						{
							if (!flag2 && !flag3)
							{
								if (val2.max > val2.min + 0.1f)
								{
									if (num6 != num3)
									{
										num6 = k;
										continue;
									}
								}
								else
								{
									flag6 = true;
								}
							}
							else if (flag2 && (netCompositionData2.m_Flags.m_General & CompositionFlags.General.Crosswalk) == 0)
							{
								num6 = k;
								continue;
							}
						}
					}
					if (flag6 && num4 == -1)
					{
						num4 = num10;
						num6 = k;
						continue;
					}
					if (val5.Length >= 1)
					{
						tempBuffer.ResizeUninitialized(val5.Length + 1);
						for (int m = 0; m < tempBuffer.Length; m++)
						{
							float3 val6 = ((m != 0) ? ((m != tempBuffer.Length - 1) ? math.lerp(val5[m - 1].m_End, val5[m].m_Start, 0.5f) : val5[m - 1].m_End) : val5[m].m_Start);
							float num13 = val6.x / math.max(1f, netCompositionData2.m_Width) + 0.5f;
							ConnectPosition connectPosition4 = new ConnectPosition
							{
								m_Position = math.lerp(left2.m_Left.a, left2.m_Right.a, num13)
							};
							connectPosition4.m_Position.y += val6.y;
							tempBuffer[m] = connectPosition4;
						}
						for (int n = num6; n < k; n++)
						{
							ConnectPosition connectPosition5 = buffer[n];
							float num14 = float.MaxValue;
							int num15 = 0;
							for (int num16 = 0; num16 < tempBuffer.Length; num16++)
							{
								ConnectPosition connectPosition6 = tempBuffer[num16];
								if (connectPosition6.m_Owner == Entity.Null)
								{
									float num17 = math.lengthsq(connectPosition5.m_Position - connectPosition6.m_Position);
									if (num17 < num14)
									{
										num14 = num17;
										num15 = num16;
									}
								}
							}
							tempBuffer[num15] = connectPosition5;
						}
						for (int num18 = 0; num18 < tempBuffer.Length; num18++)
						{
							ConnectPosition connectPosition7 = tempBuffer[num18];
							if (connectPosition7.m_Owner == Entity.Null)
							{
								connectPosition7.m_Owner = owner;
								connectPosition7.m_NodeComposition = targetPosition.m_NodeComposition;
								connectPosition7.m_EdgeComposition = targetPosition.m_EdgeComposition;
								connectPosition7.m_Tangent = targetPosition.m_Tangent;
								tempBuffer[num18] = connectPosition7;
							}
						}
						PathNode pathNode = default(PathNode);
						PathNode endPathNode = default(PathNode);
						for (int num19 = 0; num19 < val5.Length; num19++)
						{
							NetCompositionCrosswalk netCompositionCrosswalk = val5[num19];
							ConnectPosition sourcePosition = tempBuffer[num19];
							ConnectPosition targetPosition3 = tempBuffer[num19 + 1];
							float num20 = netCompositionCrosswalk.m_Start.x / math.max(1f, netCompositionData2.m_Width) + 0.5f;
							float num21 = netCompositionCrosswalk.m_End.x / math.max(1f, netCompositionData2.m_Width) + 0.5f;
							if (flag6)
							{
								sourcePosition.m_Position = math.lerp(left2.m_Left.d, left2.m_Right.d, num20);
								targetPosition3.m_Position = math.lerp(left2.m_Left.d, left2.m_Right.d, num21);
								if (num19 == 0)
								{
									sourcePosition.m_Owner = owner;
									pathNode = new PathNode(owner, (ushort)num4, 0.5f);
									endPathNode = pathNode;
								}
								if (num19 == val5.Length - 1)
								{
									targetPosition3.m_Owner = owner;
									endPathNode = new PathNode(owner, (ushort)num10, 0.5f);
								}
							}
							else
							{
								sourcePosition.m_Position = math.lerp(left2.m_Left.a, left2.m_Right.a, num20);
								targetPosition3.m_Position = math.lerp(left2.m_Left.a, left2.m_Right.a, num21);
								ref float3 reference = ref sourcePosition.m_Position;
								reference += sourcePosition.m_Tangent * netCompositionCrosswalk.m_Start.z;
								ref float3 reference2 = ref targetPosition3.m_Position;
								reference2 += targetPosition3.m_Tangent * netCompositionCrosswalk.m_End.z;
								if (num19 == 0 && sourcePosition.m_Owner == owner)
								{
									pathNode = new PathNode(owner, (ushort)nodeLaneIndex++);
									endPathNode = pathNode;
								}
							}
							sourcePosition.m_Position.y += netCompositionCrosswalk.m_Start.y;
							targetPosition3.m_Position.y += netCompositionCrosswalk.m_End.y;
							sourcePosition.m_LaneData.m_Lane = netCompositionCrosswalk.m_Lane;
							targetPosition3.m_LaneData.m_Lane = netCompositionCrosswalk.m_Lane;
							CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, sourcePosition, targetPosition3, pathNode, endPathNode, isCrosswalk: true, isSideConnection: false, isTemp, ownerTemp, fixedTangents: false, (netCompositionCrosswalk.m_Flags & LaneFlags.CrossRoad) != 0, out var _, out var _, out var endNode);
							pathNode = endNode;
							endPathNode = endNode;
						}
					}
				}
				num6 = k;
			}
			if (flag2 || !(middleRadius <= 0f))
			{
				return;
			}
			for (int num22 = 1; num22 < val.Length; num22++)
			{
				int2 val7 = val[num22];
				for (int num23 = 0; num23 < num22; num23++)
				{
					int2 val8 = val[num23];
					if (math.all(val7 != ((int2)(ref val8)).yx))
					{
						ConnectPosition sourcePosition2 = buffer[val7.x];
						ConnectPosition targetPosition4 = buffer[val8.x];
						CreateNodePedestrianLanes(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, sourcePosition2, targetPosition4, tempBuffer2, isTemp, ownerTemp, middlePosition, middleRadius, roundaboutSize);
					}
				}
			}
		}

		private void CreateNodePedestrianLanes(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, ConnectPosition sourcePosition, ConnectPosition targetPosition, NativeList<ConnectPosition> sideConnections, bool isTemp, Temp ownerTemp, float3 middlePosition, float middleRadius, float roundaboutSize)
		{
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_0741: Unknown result type (might be due to invalid IL or missing references)
			//IL_0746: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			PathNode endNode2;
			float num2 = default(float);
			Bezier4x3 curve2;
			if (middleRadius == 0f)
			{
				ConnectPosition sourcePosition2 = sourcePosition;
				ConnectPosition targetPosition2 = targetPosition;
				PathNode endNode = default(PathNode);
				CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, sourcePosition2, targetPosition2, default(PathNode), endNode, isCrosswalk: false, isSideConnection: false, isTemp, ownerTemp, fixedTangents: false, hasSignals: true, out var curve, out var middleNode, out endNode2);
				float num3 = default(float);
				for (int i = 0; i < sideConnections.Length; i++)
				{
					ConnectPosition targetPosition3 = sideConnections[i];
					float num = MathUtils.Distance(curve, targetPosition3.m_Position, ref num2);
					float3 val = targetPosition3.m_Position + targetPosition3.m_Tangent * (num * 0.5f);
					MathUtils.Distance(curve, val, ref num3);
					ConnectPosition sourcePosition3 = default(ConnectPosition);
					sourcePosition3.m_LaneData.m_Lane = targetPosition3.m_LaneData.m_Lane;
					sourcePosition3.m_LaneData.m_Flags = targetPosition3.m_LaneData.m_Flags;
					sourcePosition3.m_NodeComposition = targetPosition3.m_NodeComposition;
					sourcePosition3.m_EdgeComposition = targetPosition3.m_EdgeComposition;
					sourcePosition3.m_Owner = owner;
					sourcePosition3.m_BaseHeight = math.lerp(sourcePosition.m_BaseHeight, targetPosition.m_BaseHeight, num3);
					sourcePosition3.m_Position = MathUtils.Position(curve, num3);
					sourcePosition3.m_Tangent = val - sourcePosition3.m_Position;
					sourcePosition3.m_Tangent = MathUtils.Normalize(sourcePosition3.m_Tangent, ((float3)(ref sourcePosition3.m_Tangent)).xz);
					PathNode pathNode = new PathNode(middleNode, num3);
					CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, sourcePosition3, targetPosition3, pathNode, pathNode, isCrosswalk: false, isSideConnection: true, isTemp, ownerTemp, fixedTangents: false, hasSignals: true, out curve2, out endNode2, out endNode);
				}
				return;
			}
			float2 val2 = math.normalizesafe(((float3)(ref sourcePosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
			float2 val3 = math.normalizesafe(((float3)(ref targetPosition.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
			NetCompositionData netCompositionData = m_PrefabCompositionData[sourcePosition.m_NodeComposition];
			NetCompositionData netCompositionData2 = m_PrefabCompositionData[targetPosition.m_NodeComposition];
			float num4 = netCompositionData.m_Width * 0.5f - sourcePosition.m_LaneData.m_Position.x;
			float num5 = netCompositionData2.m_Width * 0.5f + targetPosition.m_LaneData.m_Position.x;
			float num6 = middleRadius + roundaboutSize - math.lerp(num4, num5, 0.5f);
			bool flag = ((float3)(ref sourcePosition.m_Position)).Equals(targetPosition.m_Position) && sourcePosition.m_Owner == targetPosition.m_Owner && sourcePosition.m_LaneData.m_Index == targetPosition.m_LaneData.m_Index;
			float num7 = math.select(MathUtils.RotationAngleLeft(val2, val3), (float)Math.PI * 2f, flag);
			int num8 = 1 + math.max(1, Mathf.CeilToInt(num7 * (2f / (float)Math.PI) - 0.003141593f));
			if (num8 == 2)
			{
				Bezier4x3 val4 = NetUtils.FitCurve(sourcePosition.m_Position, sourcePosition.m_Tangent, -targetPosition.m_Tangent, targetPosition.m_Position);
				float num9 = MathUtils.Distance(((Bezier4x3)(ref val4)).xz, ((float3)(ref middlePosition)).xz, ref num2);
				num6 = math.max(num6, num9);
			}
			ConnectPosition connectPosition = sourcePosition;
			float num10 = 0f;
			PathNode pathNode2 = default(PathNode);
			if (num8 >= 2)
			{
				for (int j = 0; j < sideConnections.Length; j++)
				{
					ref ConnectPosition reference = ref sideConnections.ElementAt(j);
					float2 val5 = math.normalizesafe(((float3)(ref reference.m_Position)).xz - ((float3)(ref middlePosition)).xz, default(float2));
					reference.m_Order = MathUtils.RotationAngleLeft(val2, val5);
					if (reference.m_Order > num7)
					{
						float num11 = MathUtils.RotationAngleRight(val2, val5);
						reference.m_Order = math.select(0f, num7, num11 > reference.m_Order - num7);
					}
				}
			}
			float num15 = default(float);
			for (int k = 1; k <= num8; k++)
			{
				float num12 = math.saturate(((float)k - 0.5f) / ((float)num8 - 1f));
				ConnectPosition connectPosition2 = default(ConnectPosition);
				if (k == num8)
				{
					connectPosition2 = targetPosition;
				}
				else
				{
					float2 val6 = MathUtils.RotateLeft(val2, num7 * num12);
					connectPosition2.m_LaneData.m_Lane = sourcePosition.m_LaneData.m_Lane;
					connectPosition2.m_LaneData.m_Flags = sourcePosition.m_LaneData.m_Flags;
					connectPosition2.m_NodeComposition = sourcePosition.m_NodeComposition;
					connectPosition2.m_EdgeComposition = sourcePosition.m_EdgeComposition;
					connectPosition2.m_Owner = owner;
					connectPosition2.m_BaseHeight = math.lerp(sourcePosition.m_BaseHeight, targetPosition.m_BaseHeight, num12);
					connectPosition2.m_Position.y = math.lerp(sourcePosition.m_Position.y, targetPosition.m_Position.y, num12);
					((float3)(ref connectPosition2.m_Position)).xz = ((float3)(ref middlePosition)).xz + val6 * num6;
					((float3)(ref connectPosition2.m_Tangent)).xz = MathUtils.Right(val6);
				}
				ConnectPosition sourcePosition4 = connectPosition;
				ConnectPosition targetPosition4 = connectPosition2;
				Bezier4x3 curve3;
				PathNode middleNode2;
				if (k > 1 && k < num8)
				{
					CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, sourcePosition4, targetPosition4, pathNode2, pathNode2, isCrosswalk: false, isSideConnection: false, isTemp, ownerTemp, fixedTangents: false, hasSignals: true, out curve3, out middleNode2, out var endNode3);
					pathNode2 = endNode3;
				}
				else
				{
					float num13 = math.lerp(num10, num12, 0.5f);
					float2 val7 = MathUtils.RotateLeft(val2, num7 * num13);
					float3 centerPosition = middlePosition;
					centerPosition.y = math.lerp(connectPosition.m_Position.y, connectPosition2.m_Position.y, 0.5f);
					((float3)(ref centerPosition)).xz = ((float3)(ref centerPosition)).xz + val7 * num6;
					float3 centerTangent = default(float3);
					((float3)(ref centerTangent)).xz = MathUtils.Left(val7);
					if (k == 1)
					{
						PresetCurve(ref sourcePosition4, ref targetPosition4, middlePosition, centerPosition, centerTangent, num6, 0f, num7 / (float)num8, 2f);
					}
					else
					{
						PresetCurve(ref sourcePosition4, ref targetPosition4, middlePosition, centerPosition, centerTangent, num6, num7 / (float)num8, 0f, 2f);
					}
					CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, sourcePosition4, targetPosition4, pathNode2, pathNode2, isCrosswalk: false, isSideConnection: false, isTemp, ownerTemp, fixedTangents: true, hasSignals: true, out curve3, out middleNode2, out var endNode4);
					pathNode2 = endNode4;
				}
				for (int l = 0; l < sideConnections.Length; l++)
				{
					ConnectPosition targetPosition5 = sideConnections[l];
					if ((!(targetPosition5.m_Order < num7 * num10) || k == 1) && (!(targetPosition5.m_Order >= num7 * num12) || k == num8))
					{
						float num14 = MathUtils.Distance(curve3, targetPosition5.m_Position, ref num2);
						float3 val8 = targetPosition5.m_Position + targetPosition5.m_Tangent * (num14 * 0.5f);
						MathUtils.Distance(curve3, val8, ref num15);
						ConnectPosition sourcePosition5 = default(ConnectPosition);
						sourcePosition5.m_LaneData.m_Lane = targetPosition5.m_LaneData.m_Lane;
						sourcePosition5.m_LaneData.m_Flags = targetPosition5.m_LaneData.m_Flags;
						sourcePosition5.m_NodeComposition = targetPosition5.m_NodeComposition;
						sourcePosition5.m_EdgeComposition = targetPosition5.m_EdgeComposition;
						sourcePosition5.m_Owner = owner;
						sourcePosition5.m_BaseHeight = math.lerp(sourcePosition.m_BaseHeight, targetPosition.m_BaseHeight, num15);
						sourcePosition5.m_Position = MathUtils.Position(curve3, num15);
						sourcePosition5.m_Tangent = val8 - sourcePosition5.m_Position;
						sourcePosition5.m_Tangent = MathUtils.Normalize(sourcePosition5.m_Tangent, ((float3)(ref sourcePosition5.m_Tangent)).xz);
						PathNode pathNode3 = new PathNode(middleNode2, num15);
						CreateNodePedestrianLane(jobIndex, ref nodeLaneIndex, ref random, owner, laneBuffer, sourcePosition5, targetPosition5, pathNode3, pathNode3, isCrosswalk: false, isSideConnection: true, isTemp, ownerTemp, fixedTangents: false, hasSignals: true, out curve2, out var _, out endNode2);
					}
				}
				connectPosition = connectPosition2;
				connectPosition.m_Tangent = -connectPosition2.m_Tangent;
				num10 = num12;
			}
		}

		private bool FindNextRightLane(ConnectPosition position, NativeList<ConnectPosition> buffer, out int2 result)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(position.m_Tangent.z, 0f - position.m_Tangent.x);
			float num = -1f;
			result = default(int2);
			int num2 = 0;
			while (num2 < buffer.Length)
			{
				ConnectPosition connectPosition = buffer[num2];
				int i;
				for (i = num2 + 1; i < buffer.Length && !(buffer[i].m_Owner != connectPosition.m_Owner); i++)
				{
				}
				if (!((Entity)(ref connectPosition.m_Owner)).Equals(position.m_Owner) || i != num2 + 1)
				{
					float2 val2 = ((float3)(ref connectPosition.m_Position)).xz - ((float3)(ref position.m_Position)).xz;
					val2 -= ((float3)(ref connectPosition.m_Tangent)).xz;
					MathUtils.TryNormalize(ref val2);
					float num3;
					if (math.dot(((float3)(ref position.m_Tangent)).xz, val2) > 0f)
					{
						num3 = math.dot(val, val2) * 0.5f;
					}
					else
					{
						float num4 = math.dot(val, val2);
						num3 = math.select(-1f, 1f, num4 >= 0f) - num4 * 0.5f;
					}
					if (num3 > num)
					{
						num = num3;
						result = new int2(num2, i);
					}
				}
				num2 = i;
			}
			return num != -1f;
		}

		private void CreateNodePedestrianLane(int jobIndex, ref int nodeLaneIndex, ref Random random, Entity owner, LaneBuffer laneBuffer, ConnectPosition sourcePosition, ConnectPosition targetPosition, PathNode startPathNode, PathNode endPathNode, bool isCrosswalk, bool isSideConnection, bool isTemp, Temp ownerTemp, bool fixedTangents, bool hasSignals, out Bezier4x3 curve, out PathNode middleNode, out PathNode endNode)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0802: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0746: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_096c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0988: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0998: Unknown result type (might be due to invalid IL or missing references)
			//IL_099a: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			curve = default(Bezier4x3);
			NetCompositionData startCompositionData = m_PrefabCompositionData[sourcePosition.m_NodeComposition];
			NetCompositionData endCompositionData = m_PrefabCompositionData[targetPosition.m_NodeComposition];
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Temp temp = default(Temp);
			if (isTemp)
			{
				temp.m_Flags = ownerTemp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
				if ((ownerTemp.m_Flags & TempFlags.Replace) != 0)
				{
					temp.m_Flags |= TempFlags.Modify;
				}
			}
			Lane lane = default(Lane);
			if (sourcePosition.m_Owner == owner)
			{
				lane.m_StartNode = startPathNode;
			}
			else
			{
				lane.m_StartNode = new PathNode(sourcePosition.m_Owner, sourcePosition.m_LaneData.m_Index, sourcePosition.m_SegmentIndex);
			}
			lane.m_MiddleNode = new PathNode(owner, (ushort)nodeLaneIndex++);
			if (targetPosition.m_Owner == owner)
			{
				if (!endPathNode.Equals(startPathNode))
				{
					lane.m_EndNode = endPathNode;
				}
				else
				{
					lane.m_EndNode = new PathNode(owner, (ushort)nodeLaneIndex++);
				}
			}
			else
			{
				lane.m_EndNode = new PathNode(targetPosition.m_Owner, targetPosition.m_LaneData.m_Index, targetPosition.m_SegmentIndex);
			}
			middleNode = lane.m_MiddleNode;
			endNode = lane.m_EndNode;
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = sourcePosition.m_LaneData.m_Lane
			};
			NetLaneData netLaneData = m_NetLaneData[sourcePosition.m_LaneData.m_Lane];
			NetLaneData netLaneData2 = m_NetLaneData[targetPosition.m_LaneData.m_Lane];
			if (!isCrosswalk)
			{
				bool num = sourcePosition.m_LaneData.m_Position.x > 0f;
				bool flag = targetPosition.m_LaneData.m_Position.x > 0f;
				CompositionFlags.Side side = (num ? startCompositionData.m_Flags.m_Right : startCompositionData.m_Flags.m_Left);
				CompositionFlags.Side side2 = (flag ? endCompositionData.m_Flags.m_Right : endCompositionData.m_Flags.m_Left);
				int num2 = math.select(0, 1, (sourcePosition.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0);
				int num3 = math.select(0, 1, (targetPosition.m_CompositionData.m_RoadFlags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0);
				num2 = math.select(num2, 1 - num2, (side & CompositionFlags.Side.Sidewalk) != 0);
				num3 = math.select(num3, 1 - num3, (side2 & CompositionFlags.Side.Sidewalk) != 0);
				if (m_PrefabCompositionData.HasComponent(sourcePosition.m_EdgeComposition))
				{
					NetCompositionData netCompositionData = m_PrefabCompositionData[sourcePosition.m_EdgeComposition];
					num2 = math.select(num2, num2 + 2, (netCompositionData.m_Flags.m_General & CompositionFlags.General.Tiles) != 0);
					num2 = math.select(num2, num2 - 4, (netCompositionData.m_Flags.m_General & CompositionFlags.General.Gravel) != 0);
				}
				if (m_PrefabCompositionData.HasComponent(targetPosition.m_EdgeComposition))
				{
					NetCompositionData netCompositionData2 = m_PrefabCompositionData[targetPosition.m_EdgeComposition];
					num3 = math.select(num3, num3 + 2, (netCompositionData2.m_Flags.m_General & CompositionFlags.General.Tiles) != 0);
					num3 = math.select(num3, num3 - 4, (netCompositionData2.m_Flags.m_General & CompositionFlags.General.Gravel) != 0);
				}
				if ((sourcePosition.m_LaneData.m_Flags & targetPosition.m_LaneData.m_Flags & LaneFlags.OnWater) != 0)
				{
					num2 += math.select(0, 1, netLaneData.m_Width < netLaneData2.m_Width);
					num3 += math.select(0, 1, netLaneData2.m_Width < netLaneData.m_Width);
				}
				if (num2 > num3 && prefabRef.m_Prefab != sourcePosition.m_LaneData.m_Lane)
				{
					prefabRef.m_Prefab = sourcePosition.m_LaneData.m_Lane;
				}
				if (num3 > num2 && prefabRef.m_Prefab != targetPosition.m_LaneData.m_Lane)
				{
					prefabRef.m_Prefab = targetPosition.m_LaneData.m_Lane;
				}
			}
			CheckPrefab(ref prefabRef.m_Prefab, ref random, out var outRandom, laneBuffer);
			PedestrianLane pedestrianLane = default(PedestrianLane);
			NodeLane nodeLane = default(NodeLane);
			Curve curve2 = default(Curve);
			float2 val = float2.op_Implicit(0f);
			bool flag2 = false;
			NetLaneData netLaneData3 = m_NetLaneData[prefabRef.m_Prefab];
			nodeLane.m_WidthOffset.x = netLaneData.m_Width - netLaneData3.m_Width;
			nodeLane.m_WidthOffset.y = netLaneData2.m_Width - netLaneData3.m_Width;
			if (isCrosswalk)
			{
				pedestrianLane.m_Flags |= PedestrianLaneFlags.Crosswalk;
				if ((startCompositionData.m_Flags.m_General & CompositionFlags.General.Crosswalk) == 0)
				{
					if ((startCompositionData.m_Flags.m_General & CompositionFlags.General.DeadEnd) != 0)
					{
						return;
					}
					pedestrianLane.m_Flags |= PedestrianLaneFlags.Unsafe;
					hasSignals = false;
				}
				curve2.m_Bezier = NetUtils.StraightCurve(sourcePosition.m_Position, targetPosition.m_Position);
				curve2.m_Length = math.distance(sourcePosition.m_Position, targetPosition.m_Position);
				hasSignals &= (startCompositionData.m_Flags.m_General & CompositionFlags.General.TrafficLights) != 0;
				if (curve2.m_Length >= 0.1f)
				{
					float2 val2 = math.normalizesafe(((float3)(ref targetPosition.m_Position)).xz - ((float3)(ref sourcePosition.m_Position)).xz, default(float2));
					((float2)(ref val))._002Ector(math.dot(((float3)(ref sourcePosition.m_Tangent)).xz, val2), math.dot(((float3)(ref targetPosition.m_Tangent)).xz, val2));
					val = math.tan((float)Math.PI / 2f - math.acos(math.saturate(math.abs(val))));
					val = val * (netLaneData3.m_Width + nodeLane.m_WidthOffset) * 0.5f;
					val = math.select(math.saturate(val / curve2.m_Length), float2.op_Implicit(0f), val < 0.01f);
				}
			}
			else
			{
				if (sourcePosition.m_Owner == targetPosition.m_Owner && ((startCompositionData.m_Flags.m_General & (CompositionFlags.General.DeadEnd | CompositionFlags.General.Roundabout)) == 0 || (startCompositionData.m_Flags.m_Right & CompositionFlags.Side.AbruptEnd) != 0))
				{
					return;
				}
				if (math.distance(sourcePosition.m_Position, targetPosition.m_Position) >= 0.1f)
				{
					if (fixedTangents)
					{
						curve2.m_Bezier = new Bezier4x3(sourcePosition.m_Position, sourcePosition.m_Position + sourcePosition.m_Tangent, targetPosition.m_Position + targetPosition.m_Tangent, targetPosition.m_Position);
					}
					else
					{
						curve2.m_Bezier = NetUtils.FitCurve(sourcePosition.m_Position, sourcePosition.m_Tangent, -targetPosition.m_Tangent, targetPosition.m_Position);
					}
				}
				else
				{
					curve2.m_Bezier = NetUtils.StraightCurve(sourcePosition.m_Position, targetPosition.m_Position);
					flag2 = true;
				}
				if (isSideConnection)
				{
					pedestrianLane.m_Flags |= PedestrianLaneFlags.SideConnection;
				}
				else
				{
					pedestrianLane.m_Flags |= PedestrianLaneFlags.AllowMiddle;
					ModifyCurveHeight(ref curve2.m_Bezier, sourcePosition.m_BaseHeight, targetPosition.m_BaseHeight, startCompositionData, endCompositionData);
				}
				curve2.m_Length = MathUtils.Length(curve2.m_Bezier);
				hasSignals &= (startCompositionData.m_Flags.m_General & CompositionFlags.General.LevelCrossing) != 0;
			}
			curve = curve2.m_Bezier;
			if ((sourcePosition.m_LaneData.m_Flags & targetPosition.m_LaneData.m_Flags & LaneFlags.OnWater) != 0)
			{
				pedestrianLane.m_Flags |= PedestrianLaneFlags.OnWater;
			}
			if (flag2)
			{
				if (isTemp)
				{
					lane.m_StartNode = new PathNode(lane.m_StartNode, secondaryNode: true);
					lane.m_MiddleNode = new PathNode(lane.m_MiddleNode, secondaryNode: true);
					lane.m_EndNode = new PathNode(lane.m_EndNode, secondaryNode: true);
				}
				m_SkipLaneQueue.Enqueue(lane);
				return;
			}
			LaneKey laneKey = new LaneKey(lane, prefabRef.m_Prefab, (LaneFlags)0);
			LaneKey laneKey2 = laneKey;
			if (isTemp)
			{
				ReplaceTempOwner(ref laneKey2, owner);
				ReplaceTempOwner(ref laneKey2, sourcePosition.m_Owner);
				ReplaceTempOwner(ref laneKey2, targetPosition.m_Owner);
				GetOriginalLane(laneBuffer, laneKey2, ref temp);
			}
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if ((netLaneData3.m_Flags & LaneFlags.PseudoRandom) != 0 && !m_PseudoRandomSeedData.TryGetComponent(temp.m_Original, ref pseudoRandomSeed))
			{
				pseudoRandomSeed = new PseudoRandomSeed(ref outRandom);
			}
			Entity val3 = default(Entity);
			if (laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val3))
			{
				laneBuffer.m_OldLanes.Remove(laneKey);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val3, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val3, nodeLane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val3, curve2);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val3, pedestrianLane);
				if ((netLaneData3.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					if (!m_PseudoRandomSeedData.HasComponent(val3))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PseudoRandomSeed>(jobIndex, val3, pseudoRandomSeed);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val3, pseudoRandomSeed);
					}
				}
				if (hasSignals)
				{
					if (!m_LaneSignalData.HasComponent(val3))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LaneSignal>(jobIndex, val3, default(LaneSignal));
					}
				}
				else if (m_LaneSignalData.HasComponent(val3))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<LaneSignal>(jobIndex, val3);
				}
				if (math.any(val != 0f))
				{
					DynamicBuffer<CutRange> val4 = ((!m_CutRanges.HasBuffer(val3)) ? ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<CutRange>(jobIndex, val3) : ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<CutRange>(jobIndex, val3));
					if (val.x != 0f)
					{
						val4.Add(new CutRange
						{
							m_CurveDelta = new Bounds1(0f, val.x)
						});
					}
					if (val.y != 0f)
					{
						val4.Add(new CutRange
						{
							m_CurveDelta = new Bounds1(1f - val.y, 1f)
						});
					}
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CutRange>(jobIndex, val3);
				}
				if (isTemp)
				{
					laneBuffer.m_Updates.Add(ref val3);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val3, temp);
				}
				else if (m_TempData.HasComponent(val3))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val3, ref m_DeletedTempTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val3, ref m_AppliedTypes);
				}
				else
				{
					laneBuffer.m_Updates.Add(ref val3);
				}
				return;
			}
			EntityArchetype nodeLaneArchetype = m_PrefabLaneArchetypeData[prefabRef.m_Prefab].m_NodeLaneArchetype;
			Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, nodeLaneArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val5, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val5, lane);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NodeLane>(jobIndex, val5, nodeLane);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val5, curve2);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PedestrianLane>(jobIndex, val5, pedestrianLane);
			if ((netLaneData3.m_Flags & LaneFlags.PseudoRandom) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val5, pseudoRandomSeed);
			}
			if (isTemp)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val5, ref m_TempOwnerTypes);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val5, owner2);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val5, temp);
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val5, owner2);
			}
			if (hasSignals)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LaneSignal>(jobIndex, val5, default(LaneSignal));
			}
			if (math.any(val != 0f))
			{
				DynamicBuffer<CutRange> val6 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<CutRange>(jobIndex, val5);
				if (val.x != 0f)
				{
					val6.Add(new CutRange
					{
						m_CurveDelta = new Bounds1(0f, val.x)
					});
				}
				if (val.y != 0f)
				{
					val6.Add(new CutRange
					{
						m_CurveDelta = new Bounds1(1f - val.y, 1f)
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> __Game_Net_Orphan_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildOrder> __Game_Net_BuildOrder_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Overridden> __Game_Common_Overridden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Clear> __Game_Areas_Clear_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadComposition> __Game_Prefabs_RoadComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackComposition> __Game_Prefabs_TrackComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterwayComposition> __Game_Prefabs_WaterwayComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathwayComposition> __Game_Prefabs_PathwayComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiwayComposition> __Game_Prefabs_TaxiwayComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> __Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetObjectData> __Game_Prefabs_NetObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CutRange> __Game_Net_CutRange_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> __Game_Prefabs_NetCompositionLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionCrosswalk> __Game_Prefabs_NetCompositionCrosswalk_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<DefaultNetLane> __Game_Prefabs_DefaultNetLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubLane> __Game_Prefabs_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AuxiliaryNetLane> __Game_Prefabs_AuxiliaryNetLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionPiece> __Game_Prefabs_NetCompositionPiece_RO_BufferLookup;

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
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_NodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeGeometry>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Orphan_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Orphan>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Common_Destroyed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(true);
			__Game_Tools_EditorContainer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Tools.EditorContainer>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Elevation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Elevation>(true);
			__Game_Objects_UnderConstruction_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnderConstruction>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(true);
			__Game_Net_ConnectedNode_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedNode>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_NodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeGeometry>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SecondaryLane>(true);
			__Game_Net_LaneSignal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(true);
			__Game_Net_BuildOrder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildOrder>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Overridden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Overridden>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Areas_Clear_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Clear>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_RoadComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadComposition>(true);
			__Game_Prefabs_TrackComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackComposition>(true);
			__Game_Prefabs_WaterwayComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterwayComposition>(true);
			__Game_Prefabs_PathwayComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathwayComposition>(true);
			__Game_Prefabs_TaxiwayComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiwayComposition>(true);
			__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneArchetypeData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_NetObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetObjectData>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Net_CutRange_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CutRange>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Prefabs_NetCompositionLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionLane>(true);
			__Game_Prefabs_NetCompositionCrosswalk_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionCrosswalk>(true);
			__Game_Prefabs_DefaultNetLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DefaultNetLane>(true);
			__Game_Prefabs_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubLane>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
			__Game_Prefabs_AuxiliaryNetLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AuxiliaryNetLane>(true);
			__Game_Prefabs_NetCompositionPiece_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionPiece>(true);
		}
	}

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ToolSystem m_ToolSystem;

	private LaneReferencesSystem m_LaneReferencesSystem;

	private TerrainSystem m_TerrainSystem;

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_OwnerQuery;

	private EntityQuery m_BuildingSettingsQuery;

	private ComponentTypeSet m_AppliedTypes;

	private ComponentTypeSet m_DeletedTempTypes;

	private ComponentTypeSet m_TempOwnerTypes;

	private ComponentTypeSet m_HideLaneTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_LaneReferencesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LaneReferencesSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SubLane>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<OutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<Area>()
		};
		array[0] = val;
		m_OwnerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_BuildingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		m_DeletedTempTypes = new ComponentTypeSet(ComponentType.ReadWrite<Deleted>(), ComponentType.ReadWrite<Temp>());
		m_TempOwnerTypes = new ComponentTypeSet(ComponentType.ReadWrite<Temp>(), ComponentType.ReadWrite<Owner>());
		m_HideLaneTypes = new ComponentTypeSet(ComponentType.ReadWrite<CullingInfo>(), ComponentType.ReadWrite<MeshBatch>(), ComponentType.ReadWrite<MeshColor>());
		((ComponentSystemBase)this).RequireForUpdate(m_OwnerQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0952: Unknown result type (might be due to invalid IL or missing references)
		//IL_0957: Unknown result type (might be due to invalid IL or missing references)
		//IL_095f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0971: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		UpdateLanesJob updateLanesJob = new UpdateLanesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OrphanType = InternalCompilerInterface.GetComponentTypeHandle<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnderConstructionType = InternalCompilerInterface.GetComponentTypeHandle<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNodeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildOrderData = InternalCompilerInterface.GetComponentLookup<BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OverriddenData = InternalCompilerInterface.GetComponentLookup<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaClearData = InternalCompilerInterface.GetComponentLookup<Clear>(ref __TypeHandle.__Game_Areas_Clear_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<RoadComposition>(ref __TypeHandle.__Game_Prefabs_RoadComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackData = InternalCompilerInterface.GetComponentLookup<TrackComposition>(ref __TypeHandle.__Game_Prefabs_TrackComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterwayData = InternalCompilerInterface.GetComponentLookup<WaterwayComposition>(ref __TypeHandle.__Game_Prefabs_WaterwayComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathwayData = InternalCompilerInterface.GetComponentLookup<PathwayComposition>(ref __TypeHandle.__Game_Prefabs_PathwayComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiwayData = InternalCompilerInterface.GetComponentLookup<TaxiwayComposition>(ref __TypeHandle.__Game_Prefabs_TaxiwayComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneArchetypeData = InternalCompilerInterface.GetComponentLookup<NetLaneArchetypeData>(ref __TypeHandle.__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetObjectData = InternalCompilerInterface.GetComponentLookup<NetObjectData>(ref __TypeHandle.__Game_Prefabs_NetObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CutRanges = InternalCompilerInterface.GetBufferLookup<CutRange>(ref __TypeHandle.__Game_Net_CutRange_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionLanes = InternalCompilerInterface.GetBufferLookup<NetCompositionLane>(ref __TypeHandle.__Game_Prefabs_NetCompositionLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionCrosswalks = InternalCompilerInterface.GetBufferLookup<NetCompositionCrosswalk>(ref __TypeHandle.__Game_Prefabs_NetCompositionCrosswalk_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefaultNetLanes = InternalCompilerInterface.GetBufferLookup<DefaultNetLane>(ref __TypeHandle.__Game_Prefabs_DefaultNetLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubLanes = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubLane>(ref __TypeHandle.__Game_Prefabs_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceholderObjects = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectRequirements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAuxiliaryLanes = InternalCompilerInterface.GetBufferLookup<AuxiliaryNetLane>(ref __TypeHandle.__Game_Prefabs_AuxiliaryNetLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionPieces = InternalCompilerInterface.GetBufferLookup<NetCompositionPiece>(ref __TypeHandle.__Game_Prefabs_NetCompositionPiece_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_RandomSeed = RandomSeed.Next(),
			m_DefaultTheme = m_CityConfigurationSystem.defaultTheme,
			m_AppliedTypes = m_AppliedTypes,
			m_DeletedTempTypes = m_DeletedTempTypes,
			m_TempOwnerTypes = m_TempOwnerTypes,
			m_HideLaneTypes = m_HideLaneTypes,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_BuildingConfigurationData = ((EntityQuery)(ref m_BuildingSettingsQuery)).GetSingleton<BuildingConfigurationData>(),
			m_SkipLaneQueue = m_LaneReferencesSystem.GetSkipLaneQueue().AsParallelWriter()
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		updateLanesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateLanesJob>(updateLanesJob, m_OwnerQuery, ((SystemBase)this).Dependency);
		m_TerrainSystem.AddCPUHeightReader(val2);
		m_LaneReferencesSystem.AddSkipLaneWriter(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
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
	public LaneSystem()
	{
	}
}

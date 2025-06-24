using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
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
public class SecondaryLaneSystem : GameSystemBase
{
	private struct LaneKey : IEquatable<LaneKey>
	{
		private Lane m_Lane;

		private Entity m_Prefab;

		public LaneKey(Lane lane, Entity prefab)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
			m_Prefab = prefab;
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
			if (m_Lane.Equals(other.m_Lane))
			{
				return ((Entity)(ref m_Prefab)).Equals(other.m_Prefab);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_Lane.GetHashCode();
		}
	}

	private struct LaneBuffer
	{
		public NativeParallelHashMap<LaneKey, Entity> m_OldLanes;

		public NativeParallelHashMap<LaneKey, Entity> m_OriginalLanes;

		public NativeParallelHashSet<Entity> m_Requirements;

		public NativeList<LaneCorner> m_LaneCorners;

		public NativeList<CutRange> m_CutRanges;

		public NativeList<CrossingLane> m_CrossingLanes;

		public bool m_RequirementsSearched;

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
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			m_OldLanes = new NativeParallelHashMap<LaneKey, Entity>(32, AllocatorHandle.op_Implicit(allocator));
			m_OriginalLanes = new NativeParallelHashMap<LaneKey, Entity>(32, AllocatorHandle.op_Implicit(allocator));
			m_Requirements = default(NativeParallelHashSet<Entity>);
			m_LaneCorners = new NativeList<LaneCorner>(32, AllocatorHandle.op_Implicit(allocator));
			m_CutRanges = new NativeList<CutRange>(32, AllocatorHandle.op_Implicit(allocator));
			m_CrossingLanes = new NativeList<CrossingLane>(32, AllocatorHandle.op_Implicit(allocator));
			m_RequirementsSearched = false;
		}

		public void Clear()
		{
			m_OldLanes.Clear();
			m_OriginalLanes.Clear();
			if (m_Requirements.IsCreated)
			{
				m_Requirements.Clear();
			}
			m_LaneCorners.Clear();
			m_CutRanges.Clear();
			m_CrossingLanes.Clear();
			m_RequirementsSearched = false;
		}

		public void Dispose()
		{
			m_OldLanes.Dispose();
			m_OriginalLanes.Dispose();
			if (m_Requirements.IsCreated)
			{
				m_Requirements.Dispose();
			}
			m_LaneCorners.Dispose();
			m_CutRanges.Dispose();
			m_CrossingLanes.Dispose();
		}
	}

	private struct LaneCorner
	{
		public float3 m_StartPosition;

		public float3 m_EndPosition;

		public float4 m_Tangents;

		public float2 m_Width;

		public Entity m_Lane;

		public PathNode m_StartNode;

		public PathNode m_EndNode;

		public LaneFlags m_Flags;

		public bool m_Inverted;

		public bool m_Duplicates;

		public bool m_Hidden;
	}

	private struct CutRange : IComparable<CutRange>
	{
		public Bounds1 m_Bounds;

		public uint m_Group;

		public int CompareTo(CutRange other)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			return math.select(0, math.select(-1, 1, m_Bounds.min > other.m_Bounds.min), m_Bounds.min != other.m_Bounds.min);
		}
	}

	private struct CrossingLane
	{
		public Entity m_Prefab;

		public float3 m_StartPos;

		public float2 m_StartTangent;

		public float3 m_EndPos;

		public float2 m_EndTangent;

		public bool m_Optional;

		public bool m_Hidden;
	}

	[BurstCompile]
	private struct UpdateLanesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<NodeLane> m_NodeLaneData;

		[ReadOnly]
		public ComponentLookup<HangingLane> m_HangingLaneData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<LaneGeometry> m_LaneGeometryData;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> m_PrefabLaneArchetypeData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public ComponentLookup<SecondaryLaneData> m_PrefabSecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> m_PrefabLaneGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public BufferLookup<SecondaryNetLane> m_PrefabSecondaryLanes;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_LaneRequirements;

		[ReadOnly]
		public Entity m_DefaultTheme;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		[ReadOnly]
		public ComponentTypeSet m_DeletedTempTypes;

		[ReadOnly]
		public ComponentTypeSet m_HideLaneTypes;

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
					if (m_SecondaryLaneData.HasComponent(subLane))
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
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e60: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1023: Unknown result type (might be due to invalid IL or missing references)
			//IL_1028: Unknown result type (might be due to invalid IL or missing references)
			//IL_18e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_18e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_108e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1864: Unknown result type (might be due to invalid IL or missing references)
			//IL_186b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1870: Unknown result type (might be due to invalid IL or missing references)
			//IL_1875: Unknown result type (might be due to invalid IL or missing references)
			//IL_187e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1891: Unknown result type (might be due to invalid IL or missing references)
			//IL_1895: Unknown result type (might be due to invalid IL or missing references)
			//IL_189f: Unknown result type (might be due to invalid IL or missing references)
			//IL_18a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_18b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ede: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_103f: Unknown result type (might be due to invalid IL or missing references)
			//IL_104b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f19: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fdd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1127: Unknown result type (might be due to invalid IL or missing references)
			//IL_1111: Unknown result type (might be due to invalid IL or missing references)
			//IL_116c: Unknown result type (might be due to invalid IL or missing references)
			//IL_113b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f40: Unknown result type (might be due to invalid IL or missing references)
			//IL_1194: Unknown result type (might be due to invalid IL or missing references)
			//IL_1180: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0719: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_1256: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_126d: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1317: Unknown result type (might be due to invalid IL or missing references)
			//IL_129d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0765: Unknown result type (might be due to invalid IL or missing references)
			//IL_132e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_135e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_081a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1498: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0859: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_086e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0852: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_14da: Unknown result type (might be due to invalid IL or missing references)
			//IL_14df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
			//IL_15bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1539: Unknown result type (might be due to invalid IL or missing references)
			//IL_154d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1552: Unknown result type (might be due to invalid IL or missing references)
			//IL_1557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
			//IL_15e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_15fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_1619: Unknown result type (might be due to invalid IL or missing references)
			//IL_1625: Unknown result type (might be due to invalid IL or missing references)
			//IL_16f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_16f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_16fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1705: Unknown result type (might be due to invalid IL or missing references)
			//IL_170a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1712: Unknown result type (might be due to invalid IL or missing references)
			//IL_1719: Unknown result type (might be due to invalid IL or missing references)
			//IL_171e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1720: Unknown result type (might be due to invalid IL or missing references)
			//IL_1725: Unknown result type (might be due to invalid IL or missing references)
			//IL_1727: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0972: Unknown result type (might be due to invalid IL or missing references)
			//IL_0977: Unknown result type (might be due to invalid IL or missing references)
			//IL_17b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_17b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_17c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_17c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_17cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_17dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_17df: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_175c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1760: Unknown result type (might be due to invalid IL or missing references)
			//IL_1767: Unknown result type (might be due to invalid IL or missing references)
			//IL_176e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1773: Unknown result type (might be due to invalid IL or missing references)
			//IL_1778: Unknown result type (might be due to invalid IL or missing references)
			//IL_177f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1784: Unknown result type (might be due to invalid IL or missing references)
			//IL_1786: Unknown result type (might be due to invalid IL or missing references)
			//IL_178b: Unknown result type (might be due to invalid IL or missing references)
			//IL_178d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_1683: Unknown result type (might be due to invalid IL or missing references)
			//IL_1685: Unknown result type (might be due to invalid IL or missing references)
			//IL_1687: Unknown result type (might be due to invalid IL or missing references)
			//IL_1694: Unknown result type (might be due to invalid IL or missing references)
			//IL_169b: Unknown result type (might be due to invalid IL or missing references)
			//IL_16a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_16af: Unknown result type (might be due to invalid IL or missing references)
			LaneBuffer laneBuffer = new LaneBuffer((Allocator)2);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<EdgeGeometry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
			NativeArray<Composition> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
			bool isNode = ((ArchetypeChunk)(ref chunk)).Has<Node>(ref m_NodeType);
			bool flag = nativeArray4.Length != 0;
			DynamicBuffer<SecondaryNetLane> val5 = default(DynamicBuffer<SecondaryNetLane>);
			NodeLane nodeLane = default(NodeLane);
			EdgeLane edgeLane = default(EdgeLane);
			bool2 val11 = default(bool2);
			SecondaryLaneData secondaryLaneData = default(SecondaryLaneData);
			ParkingLane parkingLane = default(ParkingLane);
			bool2 val13 = default(bool2);
			float slotAngle2 = default(float);
			Line2 val20 = default(Line2);
			Line2 val21 = default(Line2);
			float2 val23 = default(float2);
			float2 val24 = default(float2);
			Line2 val25 = default(Line2);
			Line2 val26 = default(Line2);
			float2 val28 = default(float2);
			float2 val29 = default(float2);
			Segment val30 = default(Segment);
			Segment val31 = default(Segment);
			CarLane carLane3 = default(CarLane);
			CarLane carLane4 = default(CarLane);
			bool2 val37 = default(bool2);
			bool2 val38 = default(bool2);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity owner = nativeArray[i];
				DynamicBuffer<SubLane> lanes = bufferAccessor[i];
				int laneIndex = 0;
				Temp ownerTemp = default(Temp);
				if (flag)
				{
					ownerTemp = nativeArray4[i];
					if (m_SubLanes.HasBuffer(ownerTemp.m_Original))
					{
						DynamicBuffer<SubLane> lanes2 = m_SubLanes[ownerTemp.m_Original];
						FillOldLaneBuffer(lanes2, laneBuffer.m_OriginalLanes);
					}
				}
				FillOldLaneBuffer(lanes, laneBuffer.m_OldLanes);
				EdgeGeometry edgeGeometry = default(EdgeGeometry);
				Line3 val = default(Line3);
				Line3 val2 = default(Line3);
				float2 val3 = default(float2);
				float2 val4 = default(float2);
				if (nativeArray2.Length != 0)
				{
					edgeGeometry = nativeArray2[i];
					((Line3)(ref val))._002Ector(edgeGeometry.m_Start.m_Right.a, edgeGeometry.m_Start.m_Left.a);
					((Line3)(ref val2))._002Ector(edgeGeometry.m_End.m_Left.d, edgeGeometry.m_End.m_Right.d);
					val3 = MathUtils.Right(math.normalizesafe(((float3)(ref val.b)).xz - ((float3)(ref val.a)).xz, default(float2)));
					val4 = MathUtils.Right(math.normalizesafe(((float3)(ref val2.b)).xz - ((float3)(ref val2.a)).xz, default(float2)));
				}
				NetCompositionData netCompositionData = default(NetCompositionData);
				NetCompositionData netCompositionData2 = default(NetCompositionData);
				if (nativeArray3.Length != 0)
				{
					Composition composition = nativeArray3[i];
					netCompositionData = m_PrefabCompositionData[composition.m_StartNode];
					netCompositionData2 = m_PrefabCompositionData[composition.m_EndNode];
				}
				for (int j = 0; j < lanes.Length; j++)
				{
					Entity subLane = lanes[j].m_SubLane;
					if (m_MasterLaneData.HasComponent(subLane) || m_SecondaryLaneData.HasComponent(subLane))
					{
						continue;
					}
					Curve curve = m_CurveData[subLane];
					Lane lane = m_LaneData[subLane];
					PrefabRef prefabRef = m_PrefabRefData[subLane];
					NetLaneData netLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
					if ((netLaneData.m_Flags & LaneFlags.Secondary) != 0 || !m_PrefabSecondaryLanes.TryGetBuffer(prefabRef.m_Prefab, ref val5) || val5.Length == 0)
					{
						continue;
					}
					float3 val6 = MathUtils.StartTangent(curve.m_Bezier);
					float2 val7 = math.normalizesafe(((float3)(ref val6)).xz, default(float2));
					val6 = MathUtils.EndTangent(curve.m_Bezier);
					float2 val8 = math.normalizesafe(((float3)(ref val6)).xz, default(float2));
					float3 a = curve.m_Bezier.a;
					float3 d = curve.m_Bezier.d;
					float3 d2 = curve.m_Bezier.d;
					float3 a2 = curve.m_Bezier.a;
					float2 val9 = float2.op_Implicit(netLaneData.m_Width);
					if (m_NodeLaneData.TryGetComponent(subLane, ref nodeLane))
					{
						val9 += nodeLane.m_WidthOffset;
					}
					((float3)(ref a)).xz = ((float3)(ref a)).xz + MathUtils.Right(val7) * (val9.x * 0.5f);
					((float3)(ref d)).xz = ((float3)(ref d)).xz + MathUtils.Left(val8) * (val9.x * 0.5f);
					((float3)(ref d2)).xz = ((float3)(ref d2)).xz + MathUtils.Right(val8) * (val9.y * 0.5f);
					((float3)(ref a2)).xz = ((float3)(ref a2)).xz + MathUtils.Left(val7) * (val9.y * 0.5f);
					bool flag2 = false;
					bool flag3 = !m_CullingInfoData.HasComponent(subLane) && m_LaneGeometryData.HasComponent(subLane);
					for (int k = 0; k < val5.Length; k++)
					{
						flag2 |= (val5[k].m_Flags & SecondaryNetLaneFlags.DuplicateSides) != 0;
					}
					ref NativeList<LaneCorner> reference = ref laneBuffer.m_LaneCorners;
					LaneCorner laneCorner = new LaneCorner
					{
						m_StartPosition = a,
						m_EndPosition = d2,
						m_Tangents = new float4(val7, val8),
						m_Lane = subLane,
						m_StartNode = lane.m_StartNode,
						m_EndNode = lane.m_EndNode,
						m_Width = val9,
						m_Inverted = false,
						m_Duplicates = flag2,
						m_Hidden = flag3,
						m_Flags = netLaneData.m_Flags
					};
					reference.Add(ref laneCorner);
					ref NativeList<LaneCorner> reference2 = ref laneBuffer.m_LaneCorners;
					laneCorner = new LaneCorner
					{
						m_StartPosition = d,
						m_EndPosition = a2,
						m_Tangents = new float4(val8, val7),
						m_Lane = subLane,
						m_StartNode = lane.m_EndNode,
						m_EndNode = lane.m_StartNode,
						m_Width = val9,
						m_Inverted = true,
						m_Duplicates = flag2,
						m_Hidden = flag3,
						m_Flags = netLaneData.m_Flags
					};
					reference2.Add(ref laneCorner);
					if (!m_EdgeLaneData.TryGetComponent(subLane, ref edgeLane))
					{
						continue;
					}
					bool4 val10 = ((float2)(ref edgeLane.m_EdgeDelta)).xxyy == new float4(0f, 1f, 0f, 1f);
					if (!math.any(val10))
					{
						continue;
					}
					CarLaneFlags carLaneFlags = ~(CarLaneFlags.Unsafe | CarLaneFlags.UTurnLeft | CarLaneFlags.Invert | CarLaneFlags.SideConnection | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.LevelCrossing | CarLaneFlags.Twoway | CarLaneFlags.IsSecured | CarLaneFlags.Runway | CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.ForbidCombustionEngines | CarLaneFlags.ForbidTransitTraffic | CarLaneFlags.ForbidHeavyTraffic | CarLaneFlags.PublicOnly | CarLaneFlags.Highway | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight | CarLaneFlags.Forward | CarLaneFlags.Approach | CarLaneFlags.Roundabout | CarLaneFlags.RightLimit | CarLaneFlags.LeftLimit | CarLaneFlags.ForbidPassing | CarLaneFlags.RightOfWay | CarLaneFlags.TrafficLights | CarLaneFlags.ParkingLeft | CarLaneFlags.ParkingRight | CarLaneFlags.Forbidden | CarLaneFlags.AllowEnter);
					if (m_CarLaneData.HasComponent(subLane))
					{
						carLaneFlags = m_CarLaneData[subLane].m_Flags;
					}
					for (int l = 0; l < val5.Length; l++)
					{
						SecondaryNetLane secondaryNetLane = val5[l];
						if ((secondaryNetLane.m_Flags & SecondaryNetLaneFlags.Crossing) == 0)
						{
							continue;
						}
						bool flag4 = false;
						((bool2)(ref val11))._002Ector(math.any(((bool4)(ref val10)).xy), math.any(((bool4)(ref val10)).zw));
						if ((secondaryNetLane.m_Flags & SecondaryNetLaneFlags.RequireStop) != 0)
						{
							flag4 = flag4 || (carLaneFlags & (CarLaneFlags.LevelCrossing | CarLaneFlags.Stop | CarLaneFlags.TrafficLights)) == 0;
							val11.x = false;
						}
						if ((secondaryNetLane.m_Flags & SecondaryNetLaneFlags.RequireYield) != 0)
						{
							flag4 = flag4 || (carLaneFlags & CarLaneFlags.Yield) == 0;
							val11.x = false;
						}
						if ((secondaryNetLane.m_Flags & SecondaryNetLaneFlags.RequirePavement) != 0)
						{
							val11 &= (((bool4)(ref val10)).xz & ((netCompositionData.m_Flags.m_General & CompositionFlags.General.Pavement) != 0)) | (((bool4)(ref val10)).yw & ((netCompositionData2.m_Flags.m_General & CompositionFlags.General.Pavement) != 0));
						}
						if (!math.any(val11) || !CheckRequirements(ref laneBuffer, secondaryNetLane.m_Lane))
						{
							continue;
						}
						float3 val12 = float3.op_Implicit(0f);
						if (m_PrefabSecondaryLaneData.TryGetComponent(secondaryNetLane.m_Lane, ref secondaryLaneData))
						{
							val12 = secondaryLaneData.m_PositionOffset;
							if ((secondaryLaneData.m_Flags & SecondaryLaneDataFlags.FitToParkingSpaces) != 0)
							{
								m_ParkingLaneData.TryGetComponent(subLane, ref parkingLane);
								FitToParkingLane(subLane, curve, prefabRef, float2.op_Implicit(0f), out var curveBounds, out var blockedMask, out var slotCount, out var slotAngle, out var skipStartEnd);
								((Bounds1)(ref curveBounds))._002Ector(math.max(0f, curveBounds.min), math.min(1f, curveBounds.max));
								if ((secondaryNetLane.m_Flags & SecondaryNetLaneFlags.RequireContinue) != 0)
								{
									((bool2)(ref val13))._002Ector((parkingLane.m_Flags & ParkingLaneFlags.StartingLane) != 0, (parkingLane.m_Flags & ParkingLaneFlags.EndingLane) != 0);
									float2 val14 = new float2(curveBounds.min - 0.01f, 0.99f - curveBounds.max) * curve.m_Length;
									if (math.abs(slotAngle) > 0.25f)
									{
										val14 -= netLaneData.m_Width * 0.5f / math.tan(slotAngle);
									}
									skipStartEnd |= val13 & (val14 < 0.2f);
								}
								float num = 1f / (float)math.max(1, slotCount);
								int2 val15 = math.select(new int2(0, slotCount), new int2(1, slotCount - 1), skipStartEnd);
								float num2 = ((!(math.abs(slotAngle) <= 0.25f)) ? (netLaneData.m_Width * 0.5f / math.cos((float)Math.PI / 2f - math.abs(slotAngle))) : (netLaneData.m_Width * 0.5f));
								for (int m = val15.x; m <= val15.y; m++)
								{
									if (m == 0)
									{
										if (((int)blockedMask & 1) == 1)
										{
											continue;
										}
									}
									else if (m == slotCount)
									{
										if (((int)(blockedMask >> m - 1) & 1) == 1)
										{
											if ((parkingLane.m_Flags & ParkingLaneFlags.EndingLane) != 0)
											{
												continue;
											}
											ulong blockedMask2 = 0uL;
											for (int n = 0; n < lanes.Length; n++)
											{
												SubLane subLane2 = lanes[n];
												if ((subLane2.m_PathMethods & PathMethod.Parking) != 0)
												{
													Curve curve2 = m_CurveData[subLane2.m_SubLane];
													if (!(math.distancesq(curve2.m_Bezier.a, curve.m_Bezier.d) > 0.0001f))
													{
														PrefabRef prefabRef2 = m_PrefabRefData[subLane2.m_SubLane];
														FitToParkingLane(subLane2.m_SubLane, curve2, prefabRef2, float2.op_Implicit(0f), out var _, out blockedMask2, out var _, out slotAngle2, out var _);
														break;
													}
												}
											}
											if (((int)blockedMask2 & 1) == 1)
											{
												continue;
											}
										}
									}
									else if (((int)(blockedMask >> m - 1) & 3) == 3)
									{
										continue;
									}
									float num3 = math.lerp(curveBounds.min, curveBounds.max, (float)m * num);
									val6 = MathUtils.Tangent(curve.m_Bezier, num3);
									float2 xz = ((float3)(ref val6)).xz;
									float2 val16 = ((!(math.abs(slotAngle) <= 0.25f)) ? MathUtils.RotateRight(xz, slotAngle) : ((slotAngle < 0f) ? MathUtils.Left(xz) : MathUtils.Right(xz)));
									if (MathUtils.TryNormalize(ref val16, num2))
									{
										float3 val17 = MathUtils.Position(curve.m_Bezier, num3);
										AddCrossingLane(laneBuffer, secondaryNetLane.m_Lane, val17 - new float3(val16.x, 0f, val16.y), val17 + new float3(val16.x, 0f, val16.y), math.normalizesafe(xz, default(float2)), flag4, flag3);
									}
								}
								continue;
							}
						}
						Line3 val18 = val;
						Line3 val19 = val2;
						((Line3)(ref val18)).xz = ((Line3)(ref val18)).xz + (val3 * val12.z + MathUtils.Right(val3) * val12.x);
						((Line3)(ref val19)).xz = ((Line3)(ref val19)).xz + (val4 * val12.z + MathUtils.Right(val4) * val12.x);
						((Line3)(ref val18)).y = ((Line3)(ref val18)).y + val12.y;
						((Line3)(ref val19)).y = ((Line3)(ref val19)).y + val12.y;
						if (val11.x)
						{
							((Line2)(ref val20))._002Ector(((float3)(ref a2)).xz, ((float3)(ref a2)).xz + val7);
							((Line2)(ref val21))._002Ector(((float3)(ref a)).xz, ((float3)(ref a)).xz + val7);
							Line3 val22 = (val10.x ? val18 : val19);
							if (MathUtils.Intersect(((Line3)(ref val22)).xz, val20, ref val23) && MathUtils.Intersect(((Line3)(ref val22)).xz, val21, ref val24))
							{
								AddCrossingLane(laneBuffer, secondaryNetLane.m_Lane, MathUtils.Position(val22, val23.x), MathUtils.Position(val22, val24.x), val7, flag4, flag3);
							}
						}
						if (val11.y)
						{
							((Line2)(ref val25))._002Ector(((float3)(ref d2)).xz, ((float3)(ref d2)).xz + val8);
							((Line2)(ref val26))._002Ector(((float3)(ref d)).xz, ((float3)(ref d)).xz + val8);
							Line3 val27 = (val10.z ? val18 : val19);
							if (MathUtils.Intersect(((Line3)(ref val27)).xz, val25, ref val28) && MathUtils.Intersect(((Line3)(ref val27)).xz, val26, ref val29))
							{
								AddCrossingLane(laneBuffer, secondaryNetLane.m_Lane, MathUtils.Position(val27, val28.x), MathUtils.Position(val27, val29.x), val8, flag4, flag3);
							}
						}
					}
				}
				for (int num4 = 0; num4 < laneBuffer.m_LaneCorners.Length; num4++)
				{
					LaneCorner laneCorner2 = laneBuffer.m_LaneCorners[num4];
					LaneCorner laneCorner3 = default(LaneCorner);
					float num5 = math.distance(((float3)(ref laneCorner2.m_StartPosition)).xz, ((float3)(ref laneCorner2.m_EndPosition)).xz) * 0.5f;
					((Segment)(ref val30))._002Ector(laneCorner2.m_StartPosition, laneCorner2.m_StartPosition);
					((Segment)(ref val31))._002Ector(laneCorner2.m_EndPosition, laneCorner2.m_EndPosition);
					ref float3 a3 = ref val30.a;
					((float3)(ref a3)).xz = ((float3)(ref a3)).xz - ((float4)(ref laneCorner2.m_Tangents)).xy * num5;
					ref float3 b = ref val30.b;
					((float3)(ref b)).xz = ((float3)(ref b)).xz + ((float4)(ref laneCorner2.m_Tangents)).xy * num5;
					ref float3 a4 = ref val31.a;
					((float3)(ref a4)).xz = ((float3)(ref a4)).xz - ((float4)(ref laneCorner2.m_Tangents)).zw * num5;
					ref float3 b2 = ref val31.b;
					((float3)(ref b2)).xz = ((float3)(ref b2)).xz + ((float4)(ref laneCorner2.m_Tangents)).zw * num5;
					float num6 = float.MaxValue;
					bool flag5 = false;
					bool flag6 = false;
					bool flag7 = false;
					float2 val32 = math.select(laneCorner2.m_Width, ((float2)(ref laneCorner2.m_Width)).yx, laneCorner2.m_Inverted);
					for (int num7 = 0; num7 < laneBuffer.m_LaneCorners.Length; num7++)
					{
						LaneCorner laneCorner4 = laneBuffer.m_LaneCorners[num7];
						if (((laneCorner2.m_Flags ^ laneCorner4.m_Flags) & (LaneFlags.Utility | LaneFlags.Underground)) != 0)
						{
							continue;
						}
						bool flag8 = laneCorner2.m_StartNode.Equals(laneCorner4.m_EndNode);
						bool flag9 = laneCorner2.m_EndNode.Equals(laneCorner4.m_StartNode);
						if ((laneCorner2.m_Flags & LaneFlags.Utility) == 0)
						{
							float2 val33 = math.select(((float2)(ref laneCorner4.m_Width)).yx, laneCorner4.m_Width, laneCorner4.m_Inverted);
							float2 val34 = (val32 + val33) * 0.25f;
							val34 *= val34;
							if ((flag8 ? 0f : MathUtils.DistanceSquared(val30, laneCorner4.m_EndPosition, ref slotAngle2)) > val34.x || (flag9 ? 0f : MathUtils.DistanceSquared(val31, laneCorner4.m_StartPosition, ref slotAngle2)) > val34.y)
							{
								continue;
							}
						}
						if (laneCorner2.m_Lane == laneCorner4.m_Lane)
						{
							continue;
						}
						bool num8 = math.distancesq(laneCorner2.m_Tangents, ((float4)(ref laneCorner4.m_Tangents)).zwxy) < 0.01f;
						bool flag10 = math.distancesq(laneCorner2.m_Tangents, -((float4)(ref laneCorner4.m_Tangents)).zwxy) < 0.01f;
						if (num8 || flag10)
						{
							float num9 = math.max(math.distancesq(laneCorner2.m_StartPosition, laneCorner4.m_EndPosition), math.distancesq(laneCorner2.m_EndPosition, laneCorner4.m_StartPosition));
							if (num9 < num6)
							{
								num6 = num9;
								laneCorner3 = laneCorner4;
								flag5 = flag10;
								flag6 = flag8;
								flag7 = flag9;
							}
						}
					}
					if (laneCorner3.m_Lane != Entity.Null && !laneCorner2.m_Duplicates && laneCorner2.m_Lane.Index > laneCorner3.m_Lane.Index)
					{
						continue;
					}
					SecondaryNetLaneFlags secondaryNetLaneFlags = (SecondaryNetLaneFlags)0;
					SecondaryNetLaneFlags secondaryNetLaneFlags2 = (SecondaryNetLaneFlags)0;
					SecondaryNetLaneFlags secondaryNetLaneFlags3 = (SecondaryNetLaneFlags)0;
					SecondaryNetLaneFlags secondaryNetLaneFlags4 = (SecondaryNetLaneFlags)0;
					CarLane carLane = default(CarLane);
					CarLane carLane2 = default(CarLane);
					PedestrianLane pedestrianLane = default(PedestrianLane);
					PedestrianLane pedestrianLane2 = default(PedestrianLane);
					if (m_CarLaneData.HasComponent(laneCorner2.m_Lane))
					{
						carLane = m_CarLaneData[laneCorner2.m_Lane];
					}
					else if (m_TrackLaneData.HasComponent(laneCorner2.m_Lane))
					{
						secondaryNetLaneFlags = (((m_TrackLaneData[laneCorner2.m_Lane].m_Flags & TrackLaneFlags.Switch) == 0) ? (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireMerge) : (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireContinue));
					}
					if (m_CarLaneData.HasComponent(laneCorner3.m_Lane))
					{
						carLane2 = m_CarLaneData[laneCorner3.m_Lane];
					}
					else if (m_TrackLaneData.HasComponent(laneCorner3.m_Lane))
					{
						secondaryNetLaneFlags2 = (((m_TrackLaneData[laneCorner3.m_Lane].m_Flags & TrackLaneFlags.Switch) == 0) ? (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireMerge) : (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireContinue));
					}
					if (m_PedestrianLaneData.HasComponent(laneCorner2.m_Lane))
					{
						pedestrianLane = m_PedestrianLaneData[laneCorner2.m_Lane];
					}
					if (m_PedestrianLaneData.HasComponent(laneCorner3.m_Lane))
					{
						pedestrianLane2 = m_PedestrianLaneData[laneCorner3.m_Lane];
					}
					secondaryNetLaneFlags = (((carLane.m_Flags & CarLaneFlags.Unsafe) == 0 && (pedestrianLane.m_Flags & PedestrianLaneFlags.Unsafe) == 0) ? (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireUnsafe) : (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireSafe));
					secondaryNetLaneFlags2 = (((carLane2.m_Flags & CarLaneFlags.Unsafe) == 0 && (pedestrianLane2.m_Flags & PedestrianLaneFlags.Unsafe) == 0) ? (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireUnsafe) : (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireSafe));
					secondaryNetLaneFlags = (((carLane.m_Flags & CarLaneFlags.ForbidPassing) == 0) ? (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireForbidPassing) : (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireAllowPassing));
					secondaryNetLaneFlags2 = (((carLane2.m_Flags & CarLaneFlags.ForbidPassing) == 0) ? (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireForbidPassing) : (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireAllowPassing));
					if (m_SlaveLaneData.HasComponent(laneCorner2.m_Lane))
					{
						SlaveLane slaveLane = m_SlaveLaneData[laneCorner2.m_Lane];
						if (lanes.Length > slaveLane.m_MasterIndex && m_CarLaneData.TryGetComponent(lanes[(int)slaveLane.m_MasterIndex].m_SubLane, ref carLane3) && (carLane3.m_Flags & CarLaneFlags.Unsafe) != 0)
						{
							secondaryNetLaneFlags |= SecondaryNetLaneFlags.RequireSafeMaster;
						}
						secondaryNetLaneFlags = (((slaveLane.m_Flags & SlaveLaneFlags.MultipleLanes) == 0) ? (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireMultiple) : (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireSingle));
						secondaryNetLaneFlags = (((slaveLane.m_Flags & SlaveLaneFlags.MergingLane) == 0) ? (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireMerge) : (secondaryNetLaneFlags | SecondaryNetLaneFlags.RequireContinue));
					}
					else
					{
						secondaryNetLaneFlags |= SecondaryNetLaneFlags.RequireMultiple | SecondaryNetLaneFlags.RequireMerge;
					}
					if (m_SlaveLaneData.HasComponent(laneCorner3.m_Lane))
					{
						SlaveLane slaveLane2 = m_SlaveLaneData[laneCorner3.m_Lane];
						if (lanes.Length > slaveLane2.m_MasterIndex && m_CarLaneData.TryGetComponent(lanes[(int)slaveLane2.m_MasterIndex].m_SubLane, ref carLane4) && (carLane4.m_Flags & CarLaneFlags.Unsafe) != 0)
						{
							secondaryNetLaneFlags2 |= SecondaryNetLaneFlags.RequireSafeMaster;
						}
						secondaryNetLaneFlags2 = (((slaveLane2.m_Flags & SlaveLaneFlags.MultipleLanes) == 0) ? (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireMultiple) : (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireSingle));
						secondaryNetLaneFlags2 = ((((uint)slaveLane2.m_Flags & (uint)(laneCorner3.m_Inverted ? 512 : 1024)) != 0) ? (((secondaryNetLaneFlags & SecondaryNetLaneFlags.RequireContinue) == 0) ? (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireContinue) : (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireMerge)) : (((slaveLane2.m_Flags & SlaveLaneFlags.MergingLane) == 0) ? (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireMerge) : (secondaryNetLaneFlags2 | SecondaryNetLaneFlags.RequireContinue)));
					}
					else
					{
						secondaryNetLaneFlags2 |= SecondaryNetLaneFlags.RequireMultiple | SecondaryNetLaneFlags.RequireMerge;
					}
					bool flag11 = false;
					if (flag6 || flag7)
					{
						int num10 = 0;
						int num11 = 0;
						num10 += math.select(0, 1, (secondaryNetLaneFlags & SecondaryNetLaneFlags.RequireSafe) != 0);
						num10 += math.select(0, 2, (secondaryNetLaneFlags & SecondaryNetLaneFlags.RequireContinue) != 0);
						num11 += math.select(0, 1, (secondaryNetLaneFlags2 & SecondaryNetLaneFlags.RequireSafe) != 0);
						num11 += math.select(0, 2, (secondaryNetLaneFlags2 & SecondaryNetLaneFlags.RequireContinue) != 0);
						if (num10 == 0 && num11 == 0)
						{
							continue;
						}
						flag11 = (num10 > num11) ^ laneCorner2.m_Inverted;
					}
					PrefabRef prefabRef3 = m_PrefabRefData[laneCorner2.m_Lane];
					DynamicBuffer<SecondaryNetLane> val35 = m_PrefabSecondaryLanes[prefabRef3.m_Prefab];
					DynamicBuffer<SecondaryNetLane> val36 = default(DynamicBuffer<SecondaryNetLane>);
					secondaryNetLaneFlags3 = (SecondaryNetLaneFlags)((int)secondaryNetLaneFlags3 | ((laneCorner2.m_Inverted == m_LeftHandTraffic) ? 1 : 2));
					if (laneCorner3.m_Lane != Entity.Null)
					{
						secondaryNetLaneFlags4 = (SecondaryNetLaneFlags)((int)secondaryNetLaneFlags4 | ((laneCorner2.m_Inverted != m_LeftHandTraffic) ? 1 : 2));
						secondaryNetLaneFlags |= SecondaryNetLaneFlags.OneSided;
						secondaryNetLaneFlags2 |= SecondaryNetLaneFlags.OneSided;
						if (flag5)
						{
							secondaryNetLaneFlags |= SecondaryNetLaneFlags.RequireParallel;
							secondaryNetLaneFlags2 |= SecondaryNetLaneFlags.RequireParallel;
						}
						else
						{
							secondaryNetLaneFlags |= SecondaryNetLaneFlags.RequireOpposite;
							secondaryNetLaneFlags2 |= SecondaryNetLaneFlags.RequireOpposite;
						}
						PrefabRef prefabRef4 = m_PrefabRefData[laneCorner3.m_Lane];
						val36 = m_PrefabSecondaryLanes[prefabRef4.m_Prefab];
					}
					else
					{
						secondaryNetLaneFlags3 |= SecondaryNetLaneFlags.OneSided;
						secondaryNetLaneFlags2 |= SecondaryNetLaneFlags.Left | SecondaryNetLaneFlags.Right;
					}
					SecondaryNetLaneFlags secondaryNetLaneFlags5 = (secondaryNetLaneFlags3 ^ (SecondaryNetLaneFlags.Left | SecondaryNetLaneFlags.Right)) | SecondaryNetLaneFlags.CanFlipSides;
					SecondaryNetLaneFlags secondaryNetLaneFlags6 = (secondaryNetLaneFlags4 ^ (SecondaryNetLaneFlags.Left | SecondaryNetLaneFlags.Right)) | SecondaryNetLaneFlags.CanFlipSides;
					for (int num12 = 0; num12 < val35.Length; num12++)
					{
						SecondaryNetLane secondaryNetLane2 = val35[num12];
						((bool2)(ref val37))._002Ector((secondaryNetLane2.m_Flags & secondaryNetLaneFlags3) == secondaryNetLaneFlags3, (secondaryNetLane2.m_Flags & secondaryNetLaneFlags5) == secondaryNetLaneFlags5);
						if ((((secondaryNetLane2.m_Flags & secondaryNetLaneFlags) != 0) | !math.any(val37)) || !CheckRequirements(ref laneBuffer, secondaryNetLane2.m_Lane))
						{
							continue;
						}
						bool flag12 = laneCorner2.m_Hidden;
						bool flag13 = !val37.x;
						if (laneCorner3.m_Lane != Entity.Null)
						{
							flag12 |= laneCorner3.m_Hidden;
							if (laneCorner2.m_Lane.Index > laneCorner3.m_Lane.Index && (secondaryNetLane2.m_Flags & SecondaryNetLaneFlags.DuplicateSides) == 0)
							{
								continue;
							}
							int num13 = 0;
							while (num13 < val36.Length)
							{
								SecondaryNetLane secondaryNetLane3 = val36[num13];
								((bool2)(ref val38))._002Ector((secondaryNetLane3.m_Flags & secondaryNetLaneFlags4) == secondaryNetLaneFlags4, (secondaryNetLane3.m_Flags & secondaryNetLaneFlags6) == secondaryNetLaneFlags6);
								if (!(((secondaryNetLane3.m_Flags & secondaryNetLaneFlags2) == 0) & math.any(val37 & val38) & (secondaryNetLane2.m_Lane == secondaryNetLane3.m_Lane)))
								{
									num13++;
									continue;
								}
								goto IL_16a8;
							}
							continue;
						}
						goto IL_16d7;
						IL_16d7:
						flag13 ^= m_LeftHandTraffic;
						if ((secondaryNetLane2.m_Flags & SecondaryNetLaneFlags.DuplicateSides) != 0)
						{
							CreateSecondaryLane(chunkIndex, ref laneIndex, owner, Entity.Null, laneCorner2.m_Lane, secondaryNetLane2.m_Lane, lanes, laneBuffer, float2.op_Implicit(0f), laneCorner2.m_Width, MathUtils.Left(val3), MathUtils.Left(val4), flag12, isNode, invertLeft: false, !laneCorner2.m_Inverted, mergeStart: false, mergeEnd: false, mergeLeft: false, flag, ownerTemp);
						}
						else if (laneCorner2.m_Inverted ^ flag13)
						{
							CreateSecondaryLane(chunkIndex, ref laneIndex, owner, laneCorner3.m_Lane, laneCorner2.m_Lane, secondaryNetLane2.m_Lane, lanes, laneBuffer, laneCorner3.m_Width, laneCorner2.m_Width, MathUtils.Left(val3), MathUtils.Left(val4), flag12, isNode, flag5 ^ flag13, flag13, flag7, flag6, flag11 ^ flag13, flag, ownerTemp);
						}
						else
						{
							CreateSecondaryLane(chunkIndex, ref laneIndex, owner, laneCorner2.m_Lane, laneCorner3.m_Lane, secondaryNetLane2.m_Lane, lanes, laneBuffer, laneCorner2.m_Width, laneCorner3.m_Width, MathUtils.Left(val3), MathUtils.Left(val4), flag12, isNode, flag13, flag5 ^ flag13, flag6, flag7, flag11 ^ flag13, flag, ownerTemp);
						}
						break;
						IL_16a8:
						flag13 = !(val37.x & val38.x);
						goto IL_16d7;
					}
				}
				for (int num14 = 0; num14 < laneBuffer.m_CrossingLanes.Length; num14++)
				{
					CrossingLane crossingLane = laneBuffer.m_CrossingLanes[num14];
					if (!crossingLane.m_Optional)
					{
						Curve curveData = default(Curve);
						curveData.m_Bezier = NetUtils.StraightCurve(crossingLane.m_StartPos, crossingLane.m_EndPos);
						curveData.m_Length = MathUtils.Length(curveData.m_Bezier);
						CreateSecondaryLane(chunkIndex, ref laneIndex, owner, crossingLane.m_Prefab, laneBuffer, curveData, crossingLane.m_StartTangent, crossingLane.m_EndTangent, float2.op_Implicit(0f), crossingLane.m_Hidden, flag, ownerTemp);
					}
				}
				RemoveUnusedOldLanes(chunkIndex, lanes, laneBuffer.m_OldLanes);
				laneBuffer.Clear();
			}
			laneBuffer.Dispose();
		}

		private void FitToParkingLane(Entity lane, Curve curve, PrefabRef prefabRef, float2 sideOffset, out Bounds1 curveBounds, out ulong blockedMask, out int slotCount, out float slotAngle, out bool2 skipStartEnd)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			curveBounds = new Bounds1(0f, 1f);
			blockedMask = 0uL;
			slotCount = 1;
			slotAngle = 0f;
			skipStartEnd = bool2.op_Implicit(false);
			ParkingLane parkingLane = default(ParkingLane);
			if (!m_ParkingLaneData.TryGetComponent(lane, ref parkingLane) || (parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
			{
				return;
			}
			ParkingLaneData prefabParkingLane = m_PrefabParkingLaneData[prefabRef.m_Prefab];
			if (prefabParkingLane.m_SlotInterval != 0f)
			{
				slotCount = NetUtils.GetParkingSlotCount(curve, parkingLane, prefabParkingLane);
				float parkingSlotInterval = NetUtils.GetParkingSlotInterval(curve, parkingLane, prefabParkingLane, slotCount);
				slotAngle = prefabParkingLane.m_SlotAngle;
				float4 val = float4.op_Implicit(0f);
				val.y = (float)slotCount * parkingSlotInterval;
				val.x = 0f - val.y;
				((float4)(ref val)).zw = math.select(float2.op_Implicit(0f), sideOffset * math.tan((float)Math.PI / 2f - slotAngle), slotAngle > 0.25f);
				val /= curve.m_Length;
				DynamicBuffer<LaneOverlap> val2 = m_LaneOverlaps[lane];
				float num;
				switch (parkingLane.m_Flags & (ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane))
				{
				case ParkingLaneFlags.StartingLane:
					num = curve.m_Length - (float)slotCount * parkingSlotInterval;
					curveBounds.min = 1f + val.x + val.z;
					curveBounds.max = 1f + val.w;
					break;
				case ParkingLaneFlags.EndingLane:
					num = 0f;
					curveBounds.min = val.z;
					curveBounds.max = val.y + val.w;
					skipStartEnd.x = true;
					break;
				default:
					num = (curve.m_Length - (float)slotCount * parkingSlotInterval) * 0.5f;
					curveBounds.min = 0.5f + val.x * 0.5f + val.z;
					curveBounds.max = 0.5f + val.y * 0.5f + val.w;
					break;
				}
				float3 val3 = curve.m_Bezier.a;
				float num2 = 0f;
				int i = -1;
				val = float4.op_Implicit(0f);
				num = math.max(num, 0f);
				float2 val4 = float2.op_Implicit(2f);
				int num3 = 0;
				if (num3 < val2.Length)
				{
					LaneOverlap laneOverlap = val2[num3++];
					val4 = new float2((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd) * 0.003921569f;
				}
				for (int j = 1; j <= 16; j++)
				{
					float num4 = (float)j * 0.0625f;
					float3 val5 = MathUtils.Position(curve.m_Bezier, num4);
					for (num2 += math.distance(val3, val5); num2 >= num || (j == 16 && i < slotCount); i++)
					{
						val.y = math.select(num4, math.lerp(val.x, num4, num / num2), num < num2);
						bool flag = false;
						if (val4.x < val.y)
						{
							flag = true;
							if (val4.y <= val.y)
							{
								val4 = float2.op_Implicit(2f);
								while (num3 < val2.Length)
								{
									LaneOverlap laneOverlap2 = val2[num3++];
									float2 val6 = new float2((float)(int)laneOverlap2.m_ThisStart, (float)(int)laneOverlap2.m_ThisEnd) * 0.003921569f;
									if (val6.y > val.y)
									{
										val4 = val6;
										break;
									}
								}
							}
						}
						if (flag && i >= 0 && i < slotCount)
						{
							blockedMask |= (ulong)(1L << i);
						}
						num2 -= num;
						val.x = val.y;
						num = parkingSlotInterval;
					}
					val3 = val5;
				}
			}
			else
			{
				skipStartEnd.x = (parkingLane.m_Flags & ParkingLaneFlags.StartingLane) == 0;
				skipStartEnd.y = (parkingLane.m_Flags & ParkingLaneFlags.EndingLane) == 0;
			}
			slotAngle = math.select(slotAngle, 0f - slotAngle, (parkingLane.m_Flags & ParkingLaneFlags.ParkingLeft) != 0);
		}

		private void FillOldLaneBuffer(DynamicBuffer<SubLane> lanes, NativeParallelHashMap<LaneKey, Entity> laneBuffer)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < lanes.Length; i++)
			{
				Entity subLane = lanes[i].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane))
				{
					LaneKey laneKey = new LaneKey(m_LaneData[subLane], m_PrefabRefData[subLane].m_Prefab);
					laneBuffer.TryAdd(laneKey, subLane);
				}
			}
		}

		private void RemoveUnusedOldLanes(int jobIndex, DynamicBuffer<SubLane> lanes, NativeParallelHashMap<LaneKey, Entity> laneBuffer)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			for (int i = 0; i < lanes.Length; i++)
			{
				Entity subLane = lanes[i].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane))
				{
					LaneKey laneKey = new LaneKey(m_LaneData[subLane], m_PrefabRefData[subLane].m_Prefab);
					if (laneBuffer.TryGetValue(laneKey, ref val))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, subLane, ref m_AppliedTypes);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, subLane, default(Deleted));
						laneBuffer.Remove(laneKey);
					}
				}
			}
		}

		private void AddCrossingLane(LaneBuffer laneBuffer, Entity prefab, float3 startPos, float3 endPos, float2 tangent, bool isOptional, bool isHidden)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			float2 startTangent = tangent;
			float2 endTangent = tangent;
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int i = 0; i < laneBuffer.m_CrossingLanes.Length; i++)
				{
					CrossingLane crossingLane = laneBuffer.m_CrossingLanes[i];
					if (!(crossingLane.m_Prefab != prefab))
					{
						if (math.distancesq(crossingLane.m_EndPos, startPos) < 1f)
						{
							startPos = crossingLane.m_StartPos;
							startTangent = crossingLane.m_StartTangent;
							isOptional &= crossingLane.m_Optional;
							laneBuffer.m_CrossingLanes.RemoveAtSwapBack(i);
							flag = true;
							break;
						}
						if (math.distancesq(crossingLane.m_StartPos, endPos) < 1f)
						{
							endPos = crossingLane.m_EndPos;
							endTangent = crossingLane.m_EndTangent;
							isOptional &= crossingLane.m_Optional;
							laneBuffer.m_CrossingLanes.RemoveAtSwapBack(i);
							flag = true;
							break;
						}
					}
				}
			}
			ref NativeList<CrossingLane> reference = ref laneBuffer.m_CrossingLanes;
			CrossingLane crossingLane2 = new CrossingLane
			{
				m_Prefab = prefab,
				m_StartPos = startPos,
				m_StartTangent = startTangent,
				m_EndPos = endPos,
				m_EndTangent = endTangent,
				m_Optional = isOptional,
				m_Hidden = isHidden
			};
			reference.Add(ref crossingLane2);
		}

		private void CreateSecondaryLane(int jobIndex, ref int laneIndex, Entity owner, Entity leftLane, Entity rightLane, Entity prefab, DynamicBuffer<SubLane> lanes, LaneBuffer laneBuffer, float2 leftWidth, float2 rightWidth, float2 startTangent, float2 endTangent, bool isHidden, bool isNode, bool invertLeft, bool invertRight, bool mergeStart, bool mergeEnd, bool mergeLeft, bool isTemp, Temp ownerTemp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_0765: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0852: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_0949: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_095f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_0970: Unknown result type (might be due to invalid IL or missing references)
			//IL_0972: Unknown result type (might be due to invalid IL or missing references)
			//IL_097c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_098b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			//IL_0991: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_0829: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_091f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_0927: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			SecondaryLaneData secondaryLaneData = m_PrefabSecondaryLaneData[prefab];
			NetLaneGeometryData netLaneGeometryData = default(NetLaneGeometryData);
			if (m_PrefabLaneGeometryData.HasComponent(prefab))
			{
				netLaneGeometryData = m_PrefabLaneGeometryData[prefab];
			}
			Bezier4x3 val = default(Bezier4x3);
			float num = math.max(0.01f, netLaneGeometryData.m_Size.x);
			laneBuffer.m_CutRanges.Clear();
			Curve curve = default(Curve);
			Curve curve2 = default(Curve);
			float slotAngle;
			bool2 skipStartEnd;
			if (leftLane != Entity.Null && rightLane != Entity.Null)
			{
				curve = m_CurveData[leftLane];
				curve2 = m_CurveData[rightLane];
				if ((secondaryLaneData.m_Flags & SecondaryLaneDataFlags.FitToParkingSpaces) != 0)
				{
					FitToParkingLane(leftLane, curve, m_PrefabRefData[leftLane], leftWidth * 0.5f, out var curveBounds, out var blockedMask, out var slotCount, out slotAngle, out skipStartEnd);
					FitToParkingLane(rightLane, curve2, m_PrefabRefData[rightLane], rightWidth * 0.5f, out var curveBounds2, out var blockedMask2, out var slotCount2, out slotAngle, out skipStartEnd);
					GetCutRanges(leftLane, lanes, laneBuffer, curveBounds, blockedMask, slotCount, invertLeft);
					GetCutRanges(rightLane, lanes, laneBuffer, curveBounds2, blockedMask2, slotCount2, invertRight);
				}
				if (invertLeft)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
					leftWidth = ((float2)(ref leftWidth)).yx;
				}
				if (invertRight)
				{
					curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
					rightWidth = ((float2)(ref rightWidth)).yx;
				}
				float2 val2 = math.select(leftWidth / (leftWidth + rightWidth), float2.op_Implicit(0.5f), (leftWidth == 0f) & (rightWidth == 0f));
				Bezier4x1 val3 = default(Bezier4x1);
				((Bezier4x1)(ref val3))._002Ector(val2.x, val2.x, val2.y, val2.y);
				val = MathUtils.Lerp(curve.m_Bezier, curve2.m_Bezier, val3);
				if (mergeStart || mergeEnd)
				{
					Bezier4x3 val4 = ((!mergeLeft) ? NetUtils.OffsetCurveLeftSmooth(curve.m_Bezier, leftWidth * -0.5f - secondaryLaneData.m_CutOffset) : NetUtils.OffsetCurveLeftSmooth(curve2.m_Bezier, rightWidth * 0.5f - secondaryLaneData.m_CutOffset));
					if (!ValidateCurve(val4))
					{
						return;
					}
					if (mergeStart)
					{
						val.a = val4.a;
						val.b = val4.b;
					}
					if (mergeEnd)
					{
						val.c = val4.c;
						val.d = val4.d;
					}
				}
				GetCutRanges(leftLane, secondaryLaneData.m_Flags, laneBuffer, val, leftWidth, secondaryLaneData.m_CutOverlap, invertLeft, isRight: false, rightLane);
				GetCutRanges(rightLane, secondaryLaneData.m_Flags, laneBuffer, val, rightWidth, secondaryLaneData.m_CutOverlap, invertRight, isRight: true, leftLane);
				num = math.min(num, math.min(curve.m_Length, curve2.m_Length) * 0.5f);
			}
			else if (leftLane != Entity.Null)
			{
				curve = m_CurveData[leftLane];
				if ((secondaryLaneData.m_Flags & SecondaryLaneDataFlags.FitToParkingSpaces) != 0)
				{
					FitToParkingLane(leftLane, curve, m_PrefabRefData[leftLane], leftWidth * 0.5f, out var curveBounds3, out var blockedMask3, out var slotCount3, out slotAngle, out skipStartEnd);
					GetCutRanges(leftLane, lanes, laneBuffer, curveBounds3, blockedMask3, slotCount3, invertLeft);
				}
				if (invertLeft)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
					leftWidth = ((float2)(ref leftWidth)).yx;
				}
				val = NetUtils.OffsetCurveLeftSmooth(curve.m_Bezier, leftWidth * -0.5f - secondaryLaneData.m_CutOffset);
				if (!ValidateCurve(val))
				{
					return;
				}
				GetCutRanges(leftLane, secondaryLaneData.m_Flags, laneBuffer, val, leftWidth, secondaryLaneData.m_CutOverlap, invertLeft, isRight: false, Entity.Null);
				num = math.min(num, curve.m_Length * 0.5f);
			}
			else if (rightLane != Entity.Null)
			{
				curve2 = m_CurveData[rightLane];
				if ((secondaryLaneData.m_Flags & SecondaryLaneDataFlags.FitToParkingSpaces) != 0)
				{
					FitToParkingLane(rightLane, curve2, m_PrefabRefData[rightLane], rightWidth * 0.5f, out var curveBounds4, out var blockedMask4, out var slotCount4, out slotAngle, out skipStartEnd);
					GetCutRanges(rightLane, lanes, laneBuffer, curveBounds4, blockedMask4, slotCount4, invertRight);
				}
				if (invertRight)
				{
					curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
					rightWidth = ((float2)(ref rightWidth)).yx;
				}
				val = NetUtils.OffsetCurveLeftSmooth(curve2.m_Bezier, rightWidth * 0.5f - secondaryLaneData.m_CutOffset);
				if (!ValidateCurve(val))
				{
					return;
				}
				GetCutRanges(rightLane, secondaryLaneData.m_Flags, laneBuffer, val, rightWidth, secondaryLaneData.m_CutOverlap, invertRight, isRight: true, Entity.Null);
				num = math.min(num, curve2.m_Length * 0.5f);
			}
			if (secondaryLaneData.m_PositionOffset.x != secondaryLaneData.m_CutOffset)
			{
				val = NetUtils.OffsetCurveLeftSmooth(val, float2.op_Implicit(secondaryLaneData.m_CutOffset - secondaryLaneData.m_PositionOffset.x));
			}
			if (secondaryLaneData.m_PositionOffset.y != 0f)
			{
				val.a.y += secondaryLaneData.m_PositionOffset.y;
				val.b.y += secondaryLaneData.m_PositionOffset.y;
				val.c.y += secondaryLaneData.m_PositionOffset.y;
				val.d.y += secondaryLaneData.m_PositionOffset.y;
			}
			Bounds1 val5 = default(Bounds1);
			((Bounds1)(ref val5))._002Ector(0f, 1f);
			if (laneBuffer.m_CutRanges.Length >= 2)
			{
				NativeSortExtension.Sort<CutRange>(laneBuffer.m_CutRanges);
			}
			Bezier4x2 val6 = default(Bezier4x2);
			PrefabRef prefabRef = default(PrefabRef);
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			if (m_PrefabRefData.TryGetComponent(leftLane, ref prefabRef) && m_PrefabUtilityLaneData.TryGetComponent((Entity)prefabRef, ref utilityLaneData) && utilityLaneData.m_Hanging != 0f)
			{
				HangingLane hangingLane = default(HangingLane);
				m_HangingLaneData.TryGetComponent(leftLane, ref hangingLane);
				val6.a.x = hangingLane.m_Distances.x;
				val6.b.x = (hangingLane.m_Distances.x + utilityLaneData.m_Hanging * curve.m_Length) * (2f / 3f);
				val6.c.x = (hangingLane.m_Distances.y + utilityLaneData.m_Hanging * curve.m_Length) * (2f / 3f);
				val6.d.x = hangingLane.m_Distances.y;
			}
			PrefabRef prefabRef2 = default(PrefabRef);
			UtilityLaneData utilityLaneData2 = default(UtilityLaneData);
			if (m_PrefabRefData.TryGetComponent(rightLane, ref prefabRef2) && m_PrefabUtilityLaneData.TryGetComponent((Entity)prefabRef2, ref utilityLaneData2) && utilityLaneData2.m_Hanging != 0f)
			{
				HangingLane hangingLane2 = default(HangingLane);
				m_HangingLaneData.TryGetComponent(rightLane, ref hangingLane2);
				val6.a.y = hangingLane2.m_Distances.x;
				val6.b.y = (hangingLane2.m_Distances.x + utilityLaneData2.m_Hanging * curve2.m_Length) * (2f / 3f);
				val6.c.y = (hangingLane2.m_Distances.y + utilityLaneData2.m_Hanging * curve2.m_Length) * (2f / 3f);
				val6.d.y = hangingLane2.m_Distances.y;
			}
			Bounds1 val7 = default(Bounds1);
			for (int i = 0; i < laneBuffer.m_CutRanges.Length; i++)
			{
				CutRange cutRange = laneBuffer.m_CutRanges[i];
				if (cutRange.m_Bounds.min > val5.min)
				{
					((Bounds1)(ref val7))._002Ector(val5.min, math.min(val5.max, cutRange.m_Bounds.min));
					if (val7.max > val7.min)
					{
						if (secondaryLaneData.m_CutMargin > 0.001f)
						{
							if (val7.min > 0.001f)
							{
								Bounds1 val8 = val7;
								MathUtils.ClampLength(val, ref val8, secondaryLaneData.m_CutMargin);
								val7.min = val8.max;
							}
							if (val7.max < 0.999f)
							{
								Bounds1 val9 = val7;
								MathUtils.ClampLengthInverse(val, ref val9, secondaryLaneData.m_CutMargin);
								val7.max = val9.min;
							}
						}
						Curve curve3 = default(Curve);
						curve3.m_Bezier = MathUtils.Cut(val, val7);
						curve3.m_Length = MathUtils.Length(curve3.m_Bezier);
						if (curve3.m_Length >= num)
						{
							if (secondaryLaneData.m_Spacing > 0.1f)
							{
								CalculateSpacing(secondaryLaneData, curve3, out var count, out var offset, out var factor);
								for (int j = 0; j < count; j++)
								{
									float num2 = math.lerp(val7.min, val7.max, ((float)j + offset) * factor);
									float2 hangingDistances = MathUtils.Position(val6, num2);
									curve3.m_Bezier = NetUtils.StraightCurve(MathUtils.Position(curve.m_Bezier, num2), MathUtils.Position(curve2.m_Bezier, num2));
									curve3.m_Length = math.distance(curve3.m_Bezier.a, curve3.m_Bezier.d);
									CreateSecondaryLane(jobIndex, ref laneIndex, owner, prefab, laneBuffer, curve3, startTangent, endTangent, hangingDistances, isHidden, isTemp, ownerTemp);
								}
							}
							else
							{
								float2 hangingDistances2 = math.lerp(new float2(val6.a.x, val6.d.x), new float2(val6.a.y, val6.d.y), 0.5f);
								CreateSecondaryLane(jobIndex, ref laneIndex, owner, prefab, laneBuffer, curve3, startTangent, endTangent, hangingDistances2, isHidden, isTemp, ownerTemp);
							}
						}
					}
				}
				val5.min = math.max(val5.min, cutRange.m_Bounds.max);
				if (val5.min >= val5.max)
				{
					break;
				}
			}
			if (!(val5.max > val5.min))
			{
				return;
			}
			if (secondaryLaneData.m_CutMargin > 0.001f)
			{
				if (val5.min > 0.001f)
				{
					Bounds1 val10 = val5;
					MathUtils.ClampLength(val, ref val10, secondaryLaneData.m_CutMargin);
					val5.min = val10.max;
				}
				if (val5.max < 0.999f)
				{
					Bounds1 val11 = val5;
					MathUtils.ClampLengthInverse(val, ref val11, secondaryLaneData.m_CutMargin);
					val5.max = val11.min;
				}
			}
			Curve curve4 = default(Curve);
			curve4.m_Bezier = MathUtils.Cut(val, val5);
			curve4.m_Length = MathUtils.Length(curve4.m_Bezier);
			if (!(curve4.m_Length >= num))
			{
				return;
			}
			if (secondaryLaneData.m_Spacing > 0.1f)
			{
				CalculateSpacing(secondaryLaneData, curve4, out var count2, out var offset2, out var factor2);
				for (int k = 0; k < count2; k++)
				{
					float num3 = math.lerp(val5.min, val5.max, ((float)k + offset2) * factor2);
					float2 hangingDistances3 = MathUtils.Position(val6, num3);
					curve4.m_Bezier = NetUtils.StraightCurve(MathUtils.Position(curve.m_Bezier, num3), MathUtils.Position(curve2.m_Bezier, num3));
					curve4.m_Length = math.distance(curve4.m_Bezier.a, curve4.m_Bezier.d);
					CreateSecondaryLane(jobIndex, ref laneIndex, owner, prefab, laneBuffer, curve4, startTangent, endTangent, hangingDistances3, isHidden, isTemp, ownerTemp);
				}
			}
			else
			{
				float2 hangingDistances4 = math.lerp(new float2(val6.a.x, val6.d.x), new float2(val6.a.y, val6.d.y), 0.5f);
				CreateSecondaryLane(jobIndex, ref laneIndex, owner, prefab, laneBuffer, curve4, startTangent, endTangent, hangingDistances4, isHidden, isTemp, ownerTemp);
			}
		}

		private void CalculateSpacing(SecondaryLaneData secondaryLaneData, Curve curve, out int count, out float offset, out float factor)
		{
			count = Mathf.RoundToInt(curve.m_Length / secondaryLaneData.m_Spacing);
			factor = 1f / (float)count;
			if ((secondaryLaneData.m_Flags & SecondaryLaneDataFlags.EvenSpacing) != 0)
			{
				count--;
				offset = 1f;
			}
			else
			{
				offset = 0.5f;
			}
		}

		private bool ValidateCurve(Bezier4x3 curve)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.StartTangent(curve);
			float2 xz = ((float3)(ref val)).xz;
			val = MathUtils.EndTangent(curve);
			float2 xz2 = ((float3)(ref val)).xz;
			float2 val2 = ((float3)(ref curve.d)).xz - ((float3)(ref curve.a)).xz;
			if (MathUtils.TryNormalize(ref xz) && MathUtils.TryNormalize(ref xz2) && MathUtils.TryNormalize(ref val2))
			{
				float2 val3 = default(float2);
				((float2)(ref val3))._002Ector(math.dot(xz, val2), math.dot(xz2, val2));
				if (!(math.dot(xz, xz2) >= -0.99f))
				{
					return math.cmax(math.abs(val3)) <= 0.99f;
				}
				return true;
			}
			return false;
		}

		private void CreateSecondaryLane(int jobIndex, ref int laneIndex, Entity owner, Entity prefab, LaneBuffer laneBuffer, Curve curveData, float2 startTangent, float2 endTangent, float2 hangingDistances, bool isHidden, bool isTemp, Temp ownerTemp)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = new PrefabRef(prefab);
			SecondaryLaneData secondaryLaneData = m_PrefabSecondaryLaneData[prefab];
			float2 val = float2.op_Implicit(secondaryLaneData.m_LengthOffset.x);
			if (secondaryLaneData.m_LengthOffset.y != 0f)
			{
				float3 val2 = MathUtils.StartTangent(curveData.m_Bezier);
				float2 val3 = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
				val2 = MathUtils.StartTangent(curveData.m_Bezier);
				float2 val4 = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
				float2 val5 = default(float2);
				((float2)(ref val5))._002Ector(math.dot(startTangent, val3), math.dot(endTangent, val4));
				val5 = math.tan((float)Math.PI / 2f - math.acos(math.saturate(math.abs(val5))));
				val += val5 * secondaryLaneData.m_LengthOffset.y * 0.5f;
			}
			if (math.any(val < 0f))
			{
				float2 val6 = math.min(float2.op_Implicit(0.5f), -val / math.max(0.001f, curveData.m_Length));
				curveData.m_Length -= curveData.m_Length * math.csum(val6);
				curveData.m_Bezier = MathUtils.Cut(curveData.m_Bezier, new float2(val6.x, 1f - val6.y));
			}
			Owner owner2 = new Owner
			{
				m_Owner = owner
			};
			Elevation elevation = default(Elevation);
			Lane lane = new Lane
			{
				m_StartNode = new PathNode(new PathNode(owner, (ushort)laneIndex++), secondaryNode: true),
				m_MiddleNode = new PathNode(new PathNode(owner, (ushort)laneIndex++), secondaryNode: true),
				m_EndNode = new PathNode(new PathNode(owner, (ushort)laneIndex++), secondaryNode: true)
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
			LaneKey laneKey = new LaneKey(lane, prefabRef.m_Prefab);
			LaneKey laneKey2 = laneKey;
			if (isTemp)
			{
				ReplaceTempOwner(ref laneKey2, owner);
				GetOriginalLane(laneBuffer, laneKey2, ref temp);
			}
			HangingLane hangingLane = default(HangingLane);
			bool flag = false;
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			if (m_PrefabUtilityLaneData.TryGetComponent(prefab, ref utilityLaneData) && utilityLaneData.m_Hanging != 0f)
			{
				hangingLane.m_Distances = hangingDistances;
				flag = true;
			}
			Entity val7 = default(Entity);
			if (laneBuffer.m_OldLanes.TryGetValue(laneKey, ref val7))
			{
				laneBuffer.m_OldLanes.Remove(laneKey);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val7, curveData);
				if (flag)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<HangingLane>(jobIndex, val7, hangingLane);
				}
				if (isTemp)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, val7);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val7, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Temp>(jobIndex, val7, temp);
				}
				else if (m_TempData.HasComponent(val7))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val7, ref m_DeletedTempTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val7, ref m_AppliedTypes);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, val7);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val7, default(Updated));
				}
				return;
			}
			NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[prefabRef.m_Prefab];
			Entity val8 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netLaneArchetypeData.m_LaneArchetype);
			if (isHidden)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val8, ref m_HideLaneTypes);
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val8, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val8, lane);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val8, curveData);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val8, owner2);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Elevation>(jobIndex, val8, elevation);
			if (flag)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HangingLane>(jobIndex, val8, hangingLane);
			}
			if (isTemp)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val8, temp);
			}
		}

		private void GetCutRanges(Entity lane, DynamicBuffer<SubLane> lanes, LaneBuffer laneBuffer, Bounds1 bounds, ulong blockedMask, int slotCount, bool invert)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(0f, 1f);
			ParkingLane parkingLane = default(ParkingLane);
			if (bounds.min > 0.0001f && m_ParkingLaneData.TryGetComponent(lane, ref parkingLane) && (parkingLane.m_Flags & ParkingLaneFlags.StartingLane) == 0)
			{
				Curve curve = m_CurveData[lane];
				for (int i = 0; i < lanes.Length; i++)
				{
					SubLane subLane = lanes[i];
					if ((subLane.m_PathMethods & PathMethod.Parking) == 0)
					{
						continue;
					}
					Curve curve2 = m_CurveData[subLane.m_SubLane];
					if (!(math.distancesq(curve2.m_Bezier.d, curve.m_Bezier.a) > 0.0001f))
					{
						PrefabRef prefabRef = m_PrefabRefData[subLane.m_SubLane];
						FitToParkingLane(subLane.m_SubLane, curve2, prefabRef, float2.op_Implicit(0f), out var _, out var blockedMask2, out var slotCount2, out var _, out var _);
						if (slotCount2 != 0 && ((blockedMask2 >> slotCount2 - 1) & 1) == 0L)
						{
							val.min = bounds.min;
						}
						break;
					}
				}
			}
			if (invert)
			{
				blockedMask = math.reversebits(blockedMask) >> 64 - slotCount;
				bounds = 1f - MathUtils.Invert(bounds);
				val = 1f - MathUtils.Invert(val);
			}
			Bounds1 val2 = default(Bounds1);
			((Bounds1)(ref val2))._002Ector(val.min, bounds.min);
			float num = MathUtils.Size(bounds) / (float)math.max(1, slotCount);
			for (int j = 0; j < slotCount; j++)
			{
				if (((blockedMask >> j) & 1) != 0L)
				{
					val2.min = math.min(val2.min, bounds.min + (float)j * num);
					val2.max = bounds.min + (float)(j + 1) * num;
					continue;
				}
				if (val2.max > val2.min)
				{
					ref NativeList<CutRange> reference = ref laneBuffer.m_CutRanges;
					CutRange cutRange = new CutRange
					{
						m_Bounds = val2,
						m_Group = uint.MaxValue
					};
					reference.Add(ref cutRange);
				}
				((Bounds1)(ref val2))._002Ector(bounds.max, 0f);
			}
			val2.max = val.max;
			if (val2.max > val2.min)
			{
				ref NativeList<CutRange> reference2 = ref laneBuffer.m_CutRanges;
				CutRange cutRange = new CutRange
				{
					m_Bounds = val2,
					m_Group = uint.MaxValue
				};
				reference2.Add(ref cutRange);
			}
		}

		private void GetCutRanges(Entity lane, SecondaryLaneDataFlags flags, LaneBuffer laneBuffer, Bezier4x3 curve, float2 width, float cutOverlap, bool invert, bool isRight, Entity ignore)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (flags & SecondaryLaneDataFlags.SkipSafePedestrianOverlap) != 0;
			bool flag2 = (flags & (SecondaryLaneDataFlags.SkipSafeCarOverlap | SecondaryLaneDataFlags.SkipUnsafeCarOverlap)) != 0;
			bool flag3 = (flags & SecondaryLaneDataFlags.SkipTrackOverlap) != 0;
			bool flag4 = (flags & SecondaryLaneDataFlags.SkipMergeOverlap) != 0;
			if (flag2 && m_CarLaneData.HasComponent(lane))
			{
				CarLane carLane = m_CarLaneData[lane];
				CarLaneFlags carLaneFlags = ((isRight != invert) ? (CarLaneFlags.Roundabout | CarLaneFlags.LeftLimit) : (CarLaneFlags.Roundabout | CarLaneFlags.RightLimit));
				flag2 = (carLane.m_Flags & (carLaneFlags | CarLaneFlags.Approach)) != carLaneFlags;
			}
			if ((!flag && !flag2 && !flag3) || !m_LaneOverlaps.HasBuffer(lane))
			{
				return;
			}
			DynamicBuffer<LaneOverlap> val = m_LaneOverlaps[lane];
			int length = laneBuffer.m_CutRanges.Length;
			Owner owner = default(Owner);
			DynamicBuffer<SubLane> val2 = default(DynamicBuffer<SubLane>);
			CarLane carLane3 = default(CarLane);
			Bounds1 val4 = default(Bounds1);
			NodeLane nodeLane = default(NodeLane);
			float4 val6 = default(float4);
			for (int i = 0; i < val.Length; i++)
			{
				LaneOverlap laneOverlap = val[i];
				if (((uint)laneOverlap.m_Flags & (uint)((isRight != invert) ? 4 : 8)) == 0 || laneOverlap.m_Other == ignore)
				{
					continue;
				}
				if (!flag || !m_PedestrianLaneData.HasComponent(laneOverlap.m_Other) || (m_PedestrianLaneData[laneOverlap.m_Other].m_Flags & PedestrianLaneFlags.Unsafe) != 0)
				{
					if (flag2 && m_CarLaneData.HasComponent(laneOverlap.m_Other))
					{
						CarLane carLane2 = m_CarLaneData[laneOverlap.m_Other];
						if (((flags & SecondaryLaneDataFlags.SkipSafeCarOverlap) != 0 && (carLane2.m_Flags & CarLaneFlags.Unsafe) == 0) || ((flags & SecondaryLaneDataFlags.SkipUnsafeCarOverlap) != 0 && (carLane2.m_Flags & CarLaneFlags.Unsafe) != 0))
						{
							if ((carLane2.m_Flags & (CarLaneFlags.Approach | CarLaneFlags.Roundabout | CarLaneFlags.LeftLimit)) == (CarLaneFlags.Roundabout | CarLaneFlags.LeftLimit))
							{
								laneOverlap.m_Flags &= ~OverlapFlags.OverlapRight;
							}
							if ((carLane2.m_Flags & (CarLaneFlags.Approach | CarLaneFlags.Roundabout | CarLaneFlags.RightLimit)) == (CarLaneFlags.Roundabout | CarLaneFlags.RightLimit))
							{
								laneOverlap.m_Flags &= ~OverlapFlags.OverlapLeft;
							}
							goto IL_0284;
						}
					}
					if (!flag3 || !m_TrackLaneData.HasComponent(laneOverlap.m_Other))
					{
						if (!flag4 || !m_SlaveLaneData.HasComponent(laneOverlap.m_Other))
						{
							continue;
						}
						SlaveLane slaveLane = m_SlaveLaneData[laneOverlap.m_Other];
						if ((slaveLane.m_Flags & SlaveLaneFlags.MergingLane) == 0 || ((flags & (SecondaryLaneDataFlags.SkipSafeCarOverlap | SecondaryLaneDataFlags.SkipUnsafeCarOverlap)) == SecondaryLaneDataFlags.SkipSafeCarOverlap && (!m_OwnerData.TryGetComponent(laneOverlap.m_Other, ref owner) || !m_SubLanes.TryGetBuffer(owner.m_Owner, ref val2) || val2.Length <= slaveLane.m_MasterIndex || !m_CarLaneData.TryGetComponent(val2[(int)slaveLane.m_MasterIndex].m_SubLane, ref carLane3) || (carLane3.m_Flags & CarLaneFlags.Unsafe) != 0)))
						{
							continue;
						}
					}
				}
				goto IL_0284;
				IL_0284:
				Curve curve2 = m_CurveData[laneOverlap.m_Other];
				PrefabRef prefabRef = m_PrefabRefData[laneOverlap.m_Other];
				NetLaneData netLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
				float2 val3 = new float2((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd) * 0.003921569f;
				val3 = math.select(val3, 1f - val3, invert);
				((Bounds1)(ref val4))._002Ector(1f, 0f);
				int num = 0;
				if ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart)) != 0)
				{
					val4 |= val3.x;
					num++;
				}
				if ((laneOverlap.m_Flags & (OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleEnd)) != 0)
				{
					val4 |= val3.y;
					num++;
				}
				float2 val5 = float2.op_Implicit(netLaneData.m_Width);
				if (m_NodeLaneData.TryGetComponent(laneOverlap.m_Other, ref nodeLane))
				{
					val5 += nodeLane.m_WidthOffset;
				}
				((float4)(ref val6))._002Ector(math.min(float2.op_Implicit(0f), cutOverlap - val5 * 0.5f), math.max(float2.op_Implicit(0f), val5 * 0.5f - cutOverlap));
				val6 = math.select(val6, ((float4)(ref val6)).zwxy, (laneOverlap.m_Flags & OverlapFlags.MergeFlip) != 0);
				if ((laneOverlap.m_Flags & OverlapFlags.OverlapLeft) != 0)
				{
					Bezier4x3 curve3 = NetUtils.OffsetCurveLeftSmooth(curve2.m_Bezier, ((float4)(ref val6)).xy);
					if (ValidateCurve(curve3) && ExtendedIntersect(((Bezier4x3)(ref curve)).xz, ((Bezier4x3)(ref curve3)).xz, width, val5, out var t))
					{
						val4 |= t.x;
						num++;
					}
				}
				if ((laneOverlap.m_Flags & OverlapFlags.OverlapRight) != 0)
				{
					Bezier4x3 curve4 = NetUtils.OffsetCurveLeftSmooth(curve2.m_Bezier, ((float4)(ref val6)).zw);
					if (ValidateCurve(curve4) && ExtendedIntersect(((Bezier4x3)(ref curve)).xz, ((Bezier4x3)(ref curve4)).xz, width, val5, out var t2))
					{
						val4 |= t2.x;
						num++;
					}
				}
				if (num == 1)
				{
					val4 |= val3.x;
					val4 |= val3.y;
				}
				if (!(val4.max > val4.min))
				{
					continue;
				}
				uint num2 = uint.MaxValue;
				int num3;
				CutRange cutRange;
				if (m_SlaveLaneData.HasComponent(laneOverlap.m_Other))
				{
					num2 = m_SlaveLaneData[laneOverlap.m_Other].m_Group;
					num3 = length;
					while (num3 < laneBuffer.m_CutRanges.Length)
					{
						cutRange = laneBuffer.m_CutRanges[num3];
						if (cutRange.m_Group != num2)
						{
							num3++;
							continue;
						}
						goto IL_052c;
					}
				}
				ref NativeList<CutRange> reference = ref laneBuffer.m_CutRanges;
				CutRange cutRange2 = new CutRange
				{
					m_Bounds = val4,
					m_Group = num2
				};
				reference.Add(ref cutRange2);
				continue;
				IL_052c:
				ref Bounds1 reference2 = ref cutRange.m_Bounds;
				reference2 |= val4;
				laneBuffer.m_CutRanges[num3] = cutRange;
			}
		}

		private bool ExtendedIntersect(Bezier4x2 curve1, Bezier4x2 curve2, float2 width1, float2 width2, out float2 t)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.max(new float2(width1.x, width2.x), new float2(width1.y, width2.y));
			if (MathUtils.Intersect(curve1, curve2, ref t, 4))
			{
				return true;
			}
			if (MathUtils.Intersect(curve1, new Segment(curve2.a, curve2.a - math.normalizesafe(MathUtils.StartTangent(curve2), default(float2)) * (val.x * 0.5f)), ref t, 4) && t.y * val.x <= math.lerp(width1.x, width1.y, t.x))
			{
				t.y = 0f;
				return true;
			}
			if (MathUtils.Intersect(curve2, new Segment(curve1.a, curve1.a - math.normalizesafe(MathUtils.StartTangent(curve1), default(float2)) * (val.y * 0.5f)), ref t, 4) && t.y * val.y <= math.lerp(width2.x, width2.y, t.x))
			{
				t = new float2(0f, t.x);
				return true;
			}
			if (MathUtils.Intersect(curve1, new Segment(curve2.d, curve2.d + math.normalizesafe(MathUtils.EndTangent(curve2), default(float2)) * (val.x * 0.5f)), ref t, 4) && t.y * val.x <= math.lerp(width1.x, width1.y, t.x))
			{
				t.y = 1f;
				return true;
			}
			if (MathUtils.Intersect(curve2, new Segment(curve1.d, curve1.d + math.normalizesafe(MathUtils.EndTangent(curve1), default(float2)) * (val.y * 0.5f)), ref t, 4) && t.y * val.y <= math.lerp(width2.x, width2.y, t.x))
			{
				t = new float2(1f, t.x);
				return true;
			}
			return false;
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

		private bool CheckRequirements(ref LaneBuffer laneBuffer, Entity lanePrefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if (!m_LaneRequirements.HasBuffer(lanePrefab))
			{
				return true;
			}
			DynamicBuffer<ObjectRequirementElement> val = m_LaneRequirements[lanePrefab];
			if (!laneBuffer.m_RequirementsSearched)
			{
				if (!laneBuffer.m_Requirements.IsCreated)
				{
					laneBuffer.m_Requirements = new NativeParallelHashSet<Entity>(10, AllocatorHandle.op_Implicit((Allocator)2));
				}
				if (0 == 0 && m_DefaultTheme != Entity.Null)
				{
					laneBuffer.m_Requirements.Add(m_DefaultTheme);
				}
				laneBuffer.m_RequirementsSearched = true;
			}
			int num = -1;
			bool flag = true;
			for (int i = 0; i < val.Length; i++)
			{
				ObjectRequirementElement objectRequirementElement = val[i];
				if (objectRequirementElement.m_Group != num)
				{
					if (!flag)
					{
						break;
					}
					num = objectRequirementElement.m_Group;
					flag = false;
				}
				flag |= laneBuffer.m_Requirements.Contains(objectRequirementElement.m_Requirement);
			}
			return flag;
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
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeLane> __Game_Net_NodeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HangingLane> __Game_Net_HangingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneGeometry> __Game_Net_LaneGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> __Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SecondaryLaneData> __Game_Prefabs_SecondaryLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> __Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SecondaryNetLane> __Game_Prefabs_SecondaryNetLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PedestrianLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SecondaryLane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_NodeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeLane>(true);
			__Game_Net_HangingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HangingLane>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_LaneGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneGeometry>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneArchetypeData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_SecondaryLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SecondaryLaneData>(true);
			__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Prefabs_SecondaryNetLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SecondaryNetLane>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
		}
	}

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ModificationBarrier4B m_ModificationBarrier;

	private EntityQuery m_OwnerQuery;

	private ComponentTypeSet m_AppliedTypes;

	private ComponentTypeSet m_DeletedTempTypes;

	private ComponentTypeSet m_HideLaneTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SubLane>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<OutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Area>()
		};
		array[0] = val;
		m_OwnerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		m_DeletedTempTypes = new ComponentTypeSet(ComponentType.ReadWrite<Deleted>(), ComponentType.ReadWrite<Temp>());
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
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		UpdateLanesJob updateLanesJob = new UpdateLanesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeLaneData = InternalCompilerInterface.GetComponentLookup<NodeLane>(ref __TypeHandle.__Game_Net_NodeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HangingLaneData = InternalCompilerInterface.GetComponentLookup<HangingLane>(ref __TypeHandle.__Game_Net_HangingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneGeometryData = InternalCompilerInterface.GetComponentLookup<LaneGeometry>(ref __TypeHandle.__Game_Net_LaneGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneArchetypeData = InternalCompilerInterface.GetComponentLookup<NetLaneArchetypeData>(ref __TypeHandle.__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLaneData>(ref __TypeHandle.__Game_Prefabs_SecondaryLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneGeometryData = InternalCompilerInterface.GetComponentLookup<NetLaneGeometryData>(ref __TypeHandle.__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSecondaryLanes = InternalCompilerInterface.GetBufferLookup<SecondaryNetLane>(ref __TypeHandle.__Game_Prefabs_SecondaryNetLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneRequirements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefaultTheme = m_CityConfigurationSystem.defaultTheme,
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_AppliedTypes = m_AppliedTypes,
			m_DeletedTempTypes = m_DeletedTempTypes,
			m_HideLaneTypes = m_HideLaneTypes
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		updateLanesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateLanesJob>(updateLanesJob, m_OwnerQuery, ((SystemBase)this).Dependency);
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
	public SecondaryLaneSystem()
	{
	}
}

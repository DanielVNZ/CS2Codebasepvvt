using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Areas;

[CompilerGenerated]
public class AreaConnectionSystem : GameSystemBase
{
	private enum LaneType
	{
		Road,
		Pedestrian,
		Border
	}

	private struct AreaLaneKey : IEquatable<AreaLaneKey>
	{
		private LaneType m_LaneType;

		private float2 m_Position1;

		private float2 m_Position2;

		public AreaLaneKey(LaneType laneType, float2 position1, float2 position2)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			m_LaneType = laneType;
			bool2 val = position1 < position2;
			bool flag = position1.x == position2.x;
			if (val.x | (flag & val.y))
			{
				m_Position1 = position1;
				m_Position2 = position2;
			}
			else
			{
				m_Position1 = position2;
				m_Position2 = position1;
			}
		}

		public bool Equals(AreaLaneKey other)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (m_LaneType == other.m_LaneType && ((float2)(ref m_Position1)).Equals(other.m_Position1))
			{
				return ((float2)(ref m_Position2)).Equals(other.m_Position2);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 17 * 31;
			int num2 = (int)m_LaneType;
			return ((num + num2.GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float2, float2>(ref m_Position1)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float2, float2>(ref m_Position2)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct AreaLaneValue
	{
		public Entity m_Lane;

		public float2 m_Heights;

		public AreaLaneValue(Entity lane, float a, float b)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
			m_Heights = new float2(a, b);
		}
	}

	private struct TriangleSideKey : IEquatable<TriangleSideKey>
	{
		private float3 m_Position1;

		private float3 m_Position2;

		public TriangleSideKey(float3 position1, float3 position2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Position1 = position1;
			m_Position2 = position2;
		}

		public bool Equals(TriangleSideKey other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (((float3)(ref m_Position1)).Equals(other.m_Position1))
			{
				return ((float3)(ref m_Position2)).Equals(other.m_Position2);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position1)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position2)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	[BurstCompile]
	private struct UpdateSecondaryLanesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public ComponentTypeHandle<Lot> m_LotType;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Overridden> m_OverriddenData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<AreaLane> m_AreaLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> m_TakeoffLocationData;

		[ReadOnly]
		public ComponentLookup<AccessLane> m_AccessLaneData;

		[ReadOnly]
		public ComponentLookup<RouteLane> m_RouteLaneData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> m_PrefabNavigationAreaData;

		[ReadOnly]
		public ComponentLookup<EnclosedAreaData> m_PrefabEnclosedAreaData;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> m_PrefabNetLaneArchetypeData;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<CutRange> m_CutRanges;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeList<Entity> m_ConnectionPrefabs;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Area> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Area>(ref m_AreaType);
			NativeArray<PseudoRandomSeed> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
			bool isLot = ((ArchetypeChunk)(ref chunk)).Has<Lot>(ref m_LotType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Area area = nativeArray2[i];
				Temp temp = default(Temp);
				Temp subTemp = default(Temp);
				bool isTemp = false;
				bool isDeleted = m_DeletedData.HasComponent(val);
				bool isCounterClockwise = (area.m_Flags & AreaFlags.CounterClockwise) != 0;
				if (!CollectionUtils.TryGet<PseudoRandomSeed>(nativeArray3, i, ref pseudoRandomSeed))
				{
					pseudoRandomSeed = new PseudoRandomSeed((ushort)((Random)(ref random)).NextUInt(65536u));
				}
				if (m_TempData.HasComponent(val))
				{
					temp = m_TempData[val];
					subTemp.m_Flags = temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden);
					if ((temp.m_Flags & (TempFlags.Replace | TempFlags.Upgrade)) != 0)
					{
						subTemp.m_Flags |= TempFlags.Modify;
					}
					isTemp = true;
				}
				FindOriginalLanes(temp.m_Original, out var originalConnections);
				UpdateLanes(unfilteredChunkIndex, val, pseudoRandomSeed, isCounterClockwise, isLot, isDeleted, isTemp, subTemp, originalConnections);
				if (originalConnections.IsCreated)
				{
					originalConnections.Dispose();
				}
			}
		}

		private void GetLaneFlags(RouteConnectionType connectionType, RoadTypes areaRoadTypes, ref ConnectionLaneFlags laneFlags, ref RoadTypes roadTypes, ref int indexOffset)
		{
			switch (connectionType)
			{
			case RouteConnectionType.Pedestrian:
				laneFlags |= ConnectionLaneFlags.Pedestrian;
				indexOffset++;
				break;
			case RouteConnectionType.Road:
				laneFlags |= ConnectionLaneFlags.Road;
				roadTypes = areaRoadTypes;
				indexOffset++;
				break;
			}
		}

		private void UpdateLanes(int jobIndex, Entity area, PseudoRandomSeed pseudoRandomSeed, bool isCounterClockwise, bool isLot, bool isDeleted, bool isTemp, Temp subTemp, NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> originalLanes)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0950: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_098c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_0780: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_086f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_0883: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0902: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_090e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0910: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06da: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> oldLanes = default(NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue>);
			NativeParallelHashSet<Entity> updatedSet = default(NativeParallelHashSet<Entity>);
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[area];
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane))
				{
					Curve curve = m_CurveData[subLane];
					if (!oldLanes.IsCreated)
					{
						oldLanes._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
					}
					LaneType laneType = ((!m_ConnectionLaneData.TryGetComponent(subLane, ref connectionLane)) ? LaneType.Border : (((connectionLane.m_Flags & ConnectionLaneFlags.Road) == 0) ? LaneType.Pedestrian : LaneType.Road));
					oldLanes.Add(new AreaLaneKey(laneType, ((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve.m_Bezier.d)).xz), new AreaLaneValue(subLane, curve.m_Bezier.a.y, curve.m_Bezier.d.y));
				}
			}
			if (!isDeleted)
			{
				ConnectionLaneFlags laneFlags = (ConnectionLaneFlags)0;
				RoadTypes roadTypes = RoadTypes.None;
				Entity val2 = Entity.Null;
				int num = 0;
				int indexOffset = 0;
				bool flag = false;
				PrefabRef prefabRef = m_PrefabRefData[area];
				Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kAreaBorder);
				NavigationAreaData navigationAreaData = default(NavigationAreaData);
				if (m_PrefabNavigationAreaData.TryGetComponent(prefabRef.m_Prefab, ref navigationAreaData))
				{
					GetLaneFlags(navigationAreaData.m_ConnectionType, navigationAreaData.m_RoadTypes, ref laneFlags, ref roadTypes, ref indexOffset);
					GetLaneFlags(navigationAreaData.m_SecondaryType, navigationAreaData.m_RoadTypes, ref laneFlags, ref roadTypes, ref indexOffset);
				}
				EnclosedAreaData enclosedAreaData = default(EnclosedAreaData);
				if (m_PrefabEnclosedAreaData.TryGetComponent(prefabRef.m_Prefab, ref enclosedAreaData))
				{
					val2 = enclosedAreaData.m_BorderLanePrefab;
					flag = enclosedAreaData.m_CounterClockWise != isCounterClockwise;
				}
				if (laneFlags != 0)
				{
					DynamicBuffer<Node> nodes = m_Nodes[area];
					DynamicBuffer<Triangle> val3 = m_Triangles[area];
					if (val3.Length == 1)
					{
						Triangle triangle = val3[0];
						float3 trianglePosition = GetTrianglePosition(nodes, triangle);
						float3 trianglePosition2 = GetTrianglePosition(nodes, triangle);
						trianglePosition += (nodes[triangle.m_Indices.x].m_Position - trianglePosition) * 0.5f;
						trianglePosition2 += (nodes[triangle.m_Indices.y].m_Position - trianglePosition2) * 0.25f + (nodes[triangle.m_Indices.z].m_Position - trianglePosition2) * 0.25f;
						float3 middlePosition = (trianglePosition + trianglePosition2) * 0.5f;
						int4 xyyz = ((int3)(ref triangle.m_Indices)).xyyz;
						CheckConnection(jobIndex, area, isTemp, subTemp, 0, 2 * indexOffset, indexOffset, trianglePosition, middlePosition, trianglePosition2, xyyz, laneFlags, roadTypes, originalLanes, oldLanes, ref updatedSet);
						num = 3 * indexOffset;
					}
					else if (val3.Length >= 2)
					{
						NativeParallelHashMap<TriangleSideKey, int2> val4 = default(NativeParallelHashMap<TriangleSideKey, int2>);
						val4._002Ector(val3.Length * 3, AllocatorHandle.op_Implicit((Allocator)2));
						for (int j = 0; j < val3.Length; j++)
						{
							Triangle triangle2 = val3[j];
							val4.TryAdd(new TriangleSideKey(nodes[triangle2.m_Indices.y].m_Position, nodes[triangle2.m_Indices.z].m_Position), new int2(j, triangle2.m_Indices.x));
							val4.TryAdd(new TriangleSideKey(nodes[triangle2.m_Indices.z].m_Position, nodes[triangle2.m_Indices.x].m_Position), new int2(j, triangle2.m_Indices.y));
							val4.TryAdd(new TriangleSideKey(nodes[triangle2.m_Indices.x].m_Position, nodes[triangle2.m_Indices.y].m_Position), new int2(j, triangle2.m_Indices.z));
						}
						int num2 = val3.Length * indexOffset;
						int2 val5 = default(int2);
						int4 nodeIndex = default(int4);
						int2 val6 = default(int2);
						int4 nodeIndex2 = default(int4);
						int2 val7 = default(int2);
						int4 nodeIndex3 = default(int4);
						for (int k = 0; k < val3.Length; k++)
						{
							Triangle triangle3 = val3[k];
							if (val4.TryGetValue(new TriangleSideKey(nodes[triangle3.m_Indices.z].m_Position, nodes[triangle3.m_Indices.y].m_Position), ref val5) && val5.x > k)
							{
								float3 trianglePosition3 = GetTrianglePosition(nodes, triangle3);
								float3 edgePosition = GetEdgePosition(nodes, ((int3)(ref triangle3.m_Indices)).zy);
								float3 trianglePosition4 = GetTrianglePosition(nodes, val3[val5.x]);
								((int4)(ref nodeIndex))._002Ector(math.select(((int3)(ref triangle3.m_Indices)).xyz, ((int3)(ref triangle3.m_Indices)).xzy, isCounterClockwise), val5.y);
								CheckConnection(jobIndex, area, isTemp, subTemp, k * indexOffset, num2, val5.x * indexOffset, trianglePosition3, edgePosition, trianglePosition4, nodeIndex, laneFlags, roadTypes, originalLanes, oldLanes, ref updatedSet);
								num2 += indexOffset;
							}
							if (val4.TryGetValue(new TriangleSideKey(nodes[triangle3.m_Indices.x].m_Position, nodes[triangle3.m_Indices.z].m_Position), ref val6) && val6.x > k)
							{
								float3 trianglePosition5 = GetTrianglePosition(nodes, triangle3);
								float3 edgePosition2 = GetEdgePosition(nodes, ((int3)(ref triangle3.m_Indices)).xz);
								float3 trianglePosition6 = GetTrianglePosition(nodes, val3[val6.x]);
								((int4)(ref nodeIndex2))._002Ector(math.select(((int3)(ref triangle3.m_Indices)).yzx, ((int3)(ref triangle3.m_Indices)).yxz, isCounterClockwise), val6.y);
								CheckConnection(jobIndex, area, isTemp, subTemp, k * indexOffset, num2, val6.x * indexOffset, trianglePosition5, edgePosition2, trianglePosition6, nodeIndex2, laneFlags, roadTypes, originalLanes, oldLanes, ref updatedSet);
								num2 += indexOffset;
							}
							if (val4.TryGetValue(new TriangleSideKey(nodes[triangle3.m_Indices.y].m_Position, nodes[triangle3.m_Indices.x].m_Position), ref val7) && val7.x > k)
							{
								float3 trianglePosition7 = GetTrianglePosition(nodes, triangle3);
								float3 edgePosition3 = GetEdgePosition(nodes, ((int3)(ref triangle3.m_Indices)).yx);
								float3 trianglePosition8 = GetTrianglePosition(nodes, val3[val7.x]);
								((int4)(ref nodeIndex3))._002Ector(math.select(((int3)(ref triangle3.m_Indices)).zxy, ((int3)(ref triangle3.m_Indices)).zyx, isCounterClockwise), val7.y);
								CheckConnection(jobIndex, area, isTemp, subTemp, k * indexOffset, num2, val7.x * indexOffset, trianglePosition7, edgePosition3, trianglePosition8, nodeIndex3, laneFlags, roadTypes, originalLanes, oldLanes, ref updatedSet);
								num2 += indexOffset;
							}
						}
						val4.Dispose();
						num = num2;
					}
				}
				if (val2 != Entity.Null)
				{
					DynamicBuffer<Node> val8 = m_Nodes[area];
					int num3 = num + val8.Length;
					int num4 = math.select(0, 1, isLot);
					PseudoRandomSeed pseudoRandomSeed2 = new PseudoRandomSeed((ushort)((Random)(ref random)).NextUInt(65536u));
					NativeParallelHashSet<TriangleSideKey> val9 = default(NativeParallelHashSet<TriangleSideKey>);
					val9._002Ector(val8.Length, AllocatorHandle.op_Implicit((Allocator)2));
					int2 val10 = default(int2);
					for (int l = num4; l < val8.Length; l++)
					{
						((int2)(ref val10))._002Ector(l, l + 1);
						val10.y = math.select(val10.y, 0, val10.y == val8.Length);
						val10 = math.select(val10, ((int2)(ref val10)).yx, flag);
						Node node = val8[val10.x];
						Node node2 = val8[val10.y];
						val9.Add(new TriangleSideKey(node.m_Position, node2.m_Position));
					}
					int2 val11 = default(int2);
					for (int m = num4; m < val8.Length; m++)
					{
						((int2)(ref val11))._002Ector(m, m + 1);
						val11.y = math.select(val11.y, 0, val11.y == val8.Length);
						val11 = math.select(val11, ((int2)(ref val11)).yx, flag);
						Node node3 = val8[val11.x];
						Node node4 = val8[val11.y];
						if (!val9.Contains(new TriangleSideKey(node4.m_Position, node3.m_Position)))
						{
							float3 middlePosition2 = (node3.m_Position + node4.m_Position) * 0.5f;
							val11 += num;
							CheckBorder(jobIndex, area, val2, isTemp, subTemp, pseudoRandomSeed2, val11.x, num3++, val11.y, node3.m_Position, middlePosition2, node4.m_Position, ((int2)(ref val11)).xxyy, originalLanes, oldLanes);
						}
					}
					val9.Dispose();
				}
			}
			if (oldLanes.IsCreated)
			{
				int num5 = oldLanes.Count();
				if (num5 != 0)
				{
					NativeArray<AreaLaneValue> valueArray = oldLanes.GetValueArray(AllocatorHandle.op_Implicit((Allocator)2));
					for (int n = 0; n < valueArray.Length; n++)
					{
						Entity val12 = valueArray[n].m_Lane;
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val12, ref m_AppliedTypes);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, val12);
						if (!updatedSet.IsCreated)
						{
							updatedSet._002Ector(num5, AllocatorHandle.op_Implicit((Allocator)2));
						}
						updatedSet.Add(val12);
					}
					valueArray.Dispose();
				}
				oldLanes.Dispose();
			}
			if (!updatedSet.IsCreated)
			{
				return;
			}
			Entity val13 = area;
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(val13, ref owner))
			{
				val13 = owner.m_Owner;
				if (m_UpdatedData.HasComponent(val13) || m_DeletedData.HasComponent(val13))
				{
					val13 = Entity.Null;
					break;
				}
			}
			DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(val13, ref subObjects))
			{
				UpdateConnections(jobIndex, updatedSet, subObjects);
			}
			updatedSet.Dispose();
		}

		private void UpdateConnections(int jobIndex, NativeParallelHashSet<Entity> updatedSet, DynamicBuffer<Game.Objects.SubObject> subObjects)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_UpdatedData.HasComponent(subObject) || m_DeletedData.HasComponent(subObject))
				{
					continue;
				}
				if (m_SpawnLocationData.HasComponent(subObject))
				{
					Game.Objects.SpawnLocation spawnLocation = m_SpawnLocationData[subObject];
					if (updatedSet.Contains(spawnLocation.m_ConnectedLane1) || updatedSet.Contains(spawnLocation.m_ConnectedLane2))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, subObject, default(Updated));
					}
				}
				else if (m_TakeoffLocationData.HasComponent(subObject))
				{
					AccessLane accessLane = m_AccessLaneData[subObject];
					RouteLane routeLane = m_RouteLaneData[subObject];
					if (updatedSet.Contains(accessLane.m_Lane) || updatedSet.Contains(routeLane.m_StartLane) || updatedSet.Contains(routeLane.m_EndLane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, subObject, default(Updated));
					}
				}
				if (m_SubObjects.TryGetBuffer(subObject, ref subObjects2))
				{
					UpdateConnections(jobIndex, updatedSet, subObjects2);
				}
			}
		}

		private float3 GetEdgePosition(DynamicBuffer<Node> nodes, int2 indices)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return (nodes[indices.x].m_Position + nodes[indices.y].m_Position) / 2f;
		}

		private float3 GetTrianglePosition(DynamicBuffer<Node> nodes, Triangle triangle)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			return (nodes[triangle.m_Indices.x].m_Position + nodes[triangle.m_Indices.y].m_Position + nodes[triangle.m_Indices.z].m_Position) / 3f;
		}

		private void CheckConnection(int jobIndex, Entity area, bool isTemp, Temp temp, int startIndex, int middleIndex, int endIndex, float3 startPosition, float3 middlePosition, float3 endPosition, int4 nodeIndex, ConnectionLaneFlags laneFlags, RoadTypes roadTypes, NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> originalLanes, NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> oldLanes, ref NativeParallelHashSet<Entity> updatedSet)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			Lane lane = default(Lane);
			lane.m_StartNode = new PathNode(area, (ushort)startIndex);
			lane.m_MiddleNode = new PathNode(area, (ushort)middleIndex);
			lane.m_EndNode = new PathNode(area, (ushort)endIndex);
			float3 val = middlePosition - startPosition;
			float3 val2 = endPosition - middlePosition;
			val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			Curve curve = default(Curve);
			curve.m_Bezier = NetUtils.FitCurve(startPosition, val, val2, endPosition);
			curve.m_Length = MathUtils.Length(curve.m_Bezier);
			AreaLane areaLane = default(AreaLane);
			areaLane.m_Nodes = nodeIndex;
			float2 val3 = default(float2);
			((float2)(ref val3))._002Ector(startPosition.y, endPosition.y);
			ref Bezier4x3 bezier;
			for (; laneFlags != 0; lane.m_StartNode = new PathNode(area, (ushort)(++startIndex)), lane.m_MiddleNode = new PathNode(area, (ushort)(++middleIndex)), lane.m_EndNode = new PathNode(area, (ushort)(++endIndex)), bezier = ref curve.m_Bezier, ((Bezier4x3)(ref bezier)).y = ((Bezier4x3)(ref bezier)).y + 0.5f, val3 += 0.5f)
			{
				ConnectionLaneFlags connectionLaneFlags;
				AreaLaneKey laneKey;
				if ((laneFlags & ConnectionLaneFlags.Road) != 0)
				{
					connectionLaneFlags = ConnectionLaneFlags.Road;
					laneFlags &= ~ConnectionLaneFlags.Road;
					laneKey = new AreaLaneKey(LaneType.Road, ((float3)(ref startPosition)).xz, ((float3)(ref endPosition)).xz);
				}
				else
				{
					connectionLaneFlags = ConnectionLaneFlags.Pedestrian;
					laneFlags = (ConnectionLaneFlags)0;
					laneKey = new AreaLaneKey(LaneType.Pedestrian, ((float3)(ref startPosition)).xz, ((float3)(ref endPosition)).xz);
				}
				Entity val4 = SelectOld(oldLanes, laneKey, val3);
				if (val4 != Entity.Null)
				{
					if (m_DeletedData.HasComponent(val4))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, val4);
					}
					Lane lane2 = m_LaneData[val4];
					Curve curve2 = m_CurveData[val4];
					AreaLane areaLane2 = default(AreaLane);
					bool flag = m_AreaLaneData.HasComponent(val4);
					if (flag)
					{
						areaLane2 = m_AreaLaneData[val4];
					}
					Lane other = lane2;
					CommonUtils.Swap(ref other.m_StartNode, ref other.m_EndNode);
					if (lane.Equals(lane2) && ((Bezier4x3)(ref curve.m_Bezier)).Equals(curve2.m_Bezier) && ((int4)(ref areaLane.m_Nodes)).Equals(areaLane2.m_Nodes))
					{
						continue;
					}
					if (lane.Equals(other))
					{
						Bezier4x3 val5 = MathUtils.Invert(curve.m_Bezier);
						if (((Bezier4x3)(ref val5)).Equals(curve2.m_Bezier))
						{
							int4 wzyx = ((int4)(ref areaLane.m_Nodes)).wzyx;
							if (((int4)(ref wzyx)).Equals(areaLane2.m_Nodes))
							{
								continue;
							}
						}
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val4, lane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val4, curve);
					if (flag)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AreaLane>(jobIndex, val4, areaLane);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<AreaLane>(jobIndex, val4, areaLane);
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val4, default(Updated));
					if (!updatedSet.IsCreated)
					{
						updatedSet = new NativeParallelHashSet<Entity>(oldLanes.Count() + 1, AllocatorHandle.op_Implicit((Allocator)2));
					}
					updatedSet.Add(val4);
				}
				else
				{
					Entity val6 = m_ConnectionPrefabs[0];
					NetLaneArchetypeData netLaneArchetypeData = m_PrefabNetLaneArchetypeData[val6];
					Owner owner = new Owner(area);
					PrefabRef prefabRef = new PrefabRef(val6);
					Game.Net.SecondaryLane secondaryLane = default(Game.Net.SecondaryLane);
					Game.Net.ConnectionLane connectionLane = new Game.Net.ConnectionLane
					{
						m_Flags = (connectionLaneFlags | (ConnectionLaneFlags.AllowMiddle | ConnectionLaneFlags.Area)),
						m_RoadTypes = roadTypes
					};
					Entity val7 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netLaneArchetypeData.m_AreaLaneArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val7, prefabRef);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val7, lane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val7, curve);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AreaLane>(jobIndex, val7, areaLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Net.ConnectionLane>(jobIndex, val7, connectionLane);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val7, owner);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.SecondaryLane>(jobIndex, val7, secondaryLane);
					if (isTemp)
					{
						temp.m_Original = SelectOriginal(originalLanes, laneKey, val3);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val7, temp);
					}
				}
			}
		}

		private void CheckBorder(int jobIndex, Entity area, Entity lanePrefab, bool isTemp, Temp temp, PseudoRandomSeed pseudoRandomSeed, int startIndex, int middleIndex, int endIndex, float3 startPosition, float3 middlePosition, float3 endPosition, int4 nodeIndex, NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> originalLanes, NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> oldLanes)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			Lane lane = default(Lane);
			lane.m_StartNode = new PathNode(area, (ushort)startIndex);
			lane.m_MiddleNode = new PathNode(area, (ushort)middleIndex);
			lane.m_EndNode = new PathNode(area, (ushort)endIndex);
			float3 val = middlePosition - startPosition;
			float3 val2 = endPosition - middlePosition;
			val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			Curve curve = default(Curve);
			curve.m_Bezier = NetUtils.FitCurve(startPosition, val, val2, endPosition);
			curve.m_Length = MathUtils.Length(curve.m_Bezier);
			AreaLane areaLane = default(AreaLane);
			areaLane.m_Nodes = nodeIndex;
			AreaLaneKey laneKey = new AreaLaneKey(LaneType.Border, ((float3)(ref startPosition)).xz, ((float3)(ref endPosition)).xz);
			float2 heights = default(float2);
			((float2)(ref heights))._002Ector(startPosition.y, endPosition.y);
			Entity val3 = SelectOld(oldLanes, laneKey, heights);
			if (val3 != Entity.Null)
			{
				if (m_DeletedData.HasComponent(val3))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(jobIndex, val3);
				}
				Lane lane2 = m_LaneData[val3];
				Curve curve2 = m_CurveData[val3];
				AreaLane areaLane2 = default(AreaLane);
				bool flag = m_AreaLaneData.HasComponent(val3);
				if (flag)
				{
					areaLane2 = m_AreaLaneData[val3];
				}
				Lane other = lane2;
				CommonUtils.Swap(ref other.m_StartNode, ref other.m_EndNode);
				if (lane.Equals(lane2) && ((Bezier4x3)(ref curve.m_Bezier)).Equals(curve2.m_Bezier) && ((int4)(ref areaLane.m_Nodes)).Equals(areaLane2.m_Nodes))
				{
					return;
				}
				if (lane.Equals(other))
				{
					Bezier4x3 val4 = MathUtils.Invert(curve.m_Bezier);
					if (((Bezier4x3)(ref val4)).Equals(curve2.m_Bezier))
					{
						int4 wzyx = ((int4)(ref areaLane.m_Nodes)).wzyx;
						if (((int4)(ref wzyx)).Equals(areaLane2.m_Nodes))
						{
							return;
						}
					}
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val3, lane);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val3, curve);
				if (flag)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AreaLane>(jobIndex, val3, areaLane);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<AreaLane>(jobIndex, val3, areaLane);
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val3, default(Updated));
				return;
			}
			NetLaneArchetypeData netLaneArchetypeData = m_PrefabNetLaneArchetypeData[lanePrefab];
			Owner owner = new Owner(area);
			PrefabRef prefabRef = new PrefabRef(lanePrefab);
			Game.Net.SecondaryLane secondaryLane = default(Game.Net.SecondaryLane);
			Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, netLaneArchetypeData.m_AreaLaneArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val5, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Lane>(jobIndex, val5, lane);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Curve>(jobIndex, val5, curve);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AreaLane>(jobIndex, val5, areaLane);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val5, owner);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Game.Net.SecondaryLane>(jobIndex, val5, secondaryLane);
			NetLaneData netLaneData = default(NetLaneData);
			if (m_PrefabNetLaneData.TryGetComponent(lanePrefab, ref netLaneData) && (netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val5, pseudoRandomSeed);
			}
			if (!isTemp)
			{
				return;
			}
			temp.m_Original = SelectOriginal(originalLanes, laneKey, heights);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(jobIndex, val5, temp);
			if (temp.m_Original != Entity.Null)
			{
				if (m_OverriddenData.HasComponent(temp.m_Original))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Overridden>(jobIndex, val5, default(Overridden));
				}
				DynamicBuffer<CutRange> val6 = default(DynamicBuffer<CutRange>);
				if (m_CutRanges.TryGetBuffer(temp.m_Original, ref val6))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<CutRange>(jobIndex, val5).CopyFrom(val6);
				}
			}
		}

		private Entity SelectOld(NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> oldLanes, AreaLaneKey laneKey, float2 heights)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			AreaLaneValue areaLaneValue = default(AreaLaneValue);
			NativeParallelMultiHashMapIterator<AreaLaneKey> val = default(NativeParallelMultiHashMapIterator<AreaLaneKey>);
			if (oldLanes.IsCreated && oldLanes.TryGetFirstValue(laneKey, ref areaLaneValue, ref val))
			{
				NativeParallelMultiHashMapIterator<AreaLaneKey> val2 = val;
				float num = float.MaxValue;
				result = areaLaneValue.m_Lane;
				do
				{
					float num2 = math.csum(math.abs(heights - areaLaneValue.m_Heights));
					if (num2 < num)
					{
						val2 = val;
						num = num2;
						result = areaLaneValue.m_Lane;
					}
				}
				while (oldLanes.TryGetNextValue(ref areaLaneValue, ref val));
				oldLanes.Remove(val2);
			}
			return result;
		}

		private Entity SelectOriginal(NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> originalLanes, AreaLaneKey laneKey, float2 heights)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			AreaLaneValue areaLaneValue = default(AreaLaneValue);
			NativeParallelMultiHashMapIterator<AreaLaneKey> val = default(NativeParallelMultiHashMapIterator<AreaLaneKey>);
			if (originalLanes.IsCreated && originalLanes.TryGetFirstValue(laneKey, ref areaLaneValue, ref val))
			{
				NativeParallelMultiHashMapIterator<AreaLaneKey> val2 = val;
				float num = float.MaxValue;
				do
				{
					float num2 = math.csum(math.abs(heights - areaLaneValue.m_Heights));
					if (num2 < num)
					{
						val2 = val;
						num = num2;
						result = areaLaneValue.m_Lane;
					}
				}
				while (originalLanes.TryGetNextValue(ref areaLaneValue, ref val));
				originalLanes.Remove(val2);
			}
			return result;
		}

		private void FindOriginalLanes(Entity originalArea, out NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue> originalConnections)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			originalConnections = default(NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue>);
			if (!m_SubLanes.HasBuffer(originalArea))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[originalArea];
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_SecondaryLaneData.HasComponent(subLane))
				{
					Curve curve = m_CurveData[subLane];
					if (!originalConnections.IsCreated)
					{
						originalConnections = new NativeParallelMultiHashMap<AreaLaneKey, AreaLaneValue>(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
					}
					LaneType laneType = ((!m_ConnectionLaneData.TryGetComponent(subLane, ref connectionLane)) ? LaneType.Border : (((connectionLane.m_Flags & ConnectionLaneFlags.Road) == 0) ? LaneType.Pedestrian : LaneType.Road));
					originalConnections.Add(new AreaLaneKey(laneType, ((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve.m_Bezier.d)).xz), new AreaLaneValue(subLane, curve.m_Bezier.a.y, curve.m_Bezier.d.y));
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
		public ComponentTypeHandle<Area> __Game_Areas_Area_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lot> __Game_Areas_Lot_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Overridden> __Game_Common_Overridden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaLane> __Game_Net_AreaLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> __Game_Routes_TakeoffLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccessLane> __Game_Routes_AccessLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> __Game_Prefabs_NavigationAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EnclosedAreaData> __Game_Prefabs_EnclosedAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> __Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CutRange> __Game_Net_CutRange_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Areas_Area_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Area>(true);
			__Game_Areas_Lot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lot>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Overridden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Overridden>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.SecondaryLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_AreaLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaLane>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Routes_TakeoffLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TakeoffLocation>(true);
			__Game_Routes_AccessLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccessLane>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_NavigationAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NavigationAreaData>(true);
			__Game_Prefabs_EnclosedAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EnclosedAreaData>(true);
			__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneArchetypeData>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_CutRange_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CutRange>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
		}
	}

	private ModificationBarrier4B m_ModificationBarrier;

	private EntityQuery m_ModificationQuery;

	private EntityQuery m_ConnectionQuery;

	private ComponentTypeSet m_AppliedTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Game.Net.SubLane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_ModificationQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ConnectionLaneData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_ModificationQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<Entity> connectionPrefabs = ((EntityQuery)(ref m_ConnectionQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		UpdateSecondaryLanesJob updateSecondaryLanesJob = new UpdateSecondaryLanesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LotType = InternalCompilerInterface.GetComponentTypeHandle<Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OverriddenData = InternalCompilerInterface.GetComponentLookup<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaLaneData = InternalCompilerInterface.GetComponentLookup<AreaLane>(ref __TypeHandle.__Game_Net_AreaLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TakeoffLocationData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccessLaneData = InternalCompilerInterface.GetComponentLookup<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNavigationAreaData = InternalCompilerInterface.GetComponentLookup<NavigationAreaData>(ref __TypeHandle.__Game_Prefabs_NavigationAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEnclosedAreaData = InternalCompilerInterface.GetComponentLookup<EnclosedAreaData>(ref __TypeHandle.__Game_Prefabs_EnclosedAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneArchetypeData = InternalCompilerInterface.GetComponentLookup<NetLaneArchetypeData>(ref __TypeHandle.__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CutRanges = InternalCompilerInterface.GetBufferLookup<CutRange>(ref __TypeHandle.__Game_Net_CutRange_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_ConnectionPrefabs = connectionPrefabs,
			m_AppliedTypes = m_AppliedTypes
		};
		EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
		updateSecondaryLanesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateSecondaryLanesJob>(updateSecondaryLanesJob, m_ModificationQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		connectionPrefabs.Dispose(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
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
	public AreaConnectionSystem()
	{
	}
}

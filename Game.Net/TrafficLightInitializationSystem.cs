using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class TrafficLightInitializationSystem : GameSystemBase
{
	private struct LaneGroup
	{
		public float2 m_StartDirection;

		public float2 m_EndDirection;

		public int2 m_LaneRange;

		public int m_GroupIndex;

		public ushort m_GroupMask;

		public bool m_IsStraight;

		public bool m_IsCombined;

		public bool m_IsUnsafe;

		public bool m_IsTrack;

		public bool m_IsWaterway;

		public bool m_IsPedestrian;
	}

	[BurstCompile]
	private struct InitializeTrafficLightsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> m_SubObjectType;

		public ComponentTypeHandle<TrafficLights> m_TrafficLightsType;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<PointOfInterest> m_PointOfInterestData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<MoveableBridgeData> m_PrefabMoveableBridgeData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_Overlaps;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			NativeList<LaneGroup> groups = default(NativeList<LaneGroup>);
			groups._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<LaneGroup> vehicleLanes = default(NativeList<LaneGroup>);
			vehicleLanes._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<LaneGroup> pedestrianLanes = default(NativeList<LaneGroup>);
			pedestrianLanes._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			NativeHashMap<PathNode, int> groupIndexMap = default(NativeHashMap<PathNode, int>);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<TrafficLights> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrafficLights>(ref m_TrafficLightsType);
			BufferAccessor<SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
			BufferAccessor<Game.Objects.SubObject> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Objects.SubObject>(ref m_SubObjectType);
			Owner owner = default(Owner);
			DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				Entity val = nativeArray[i];
				TrafficLights trafficLights = nativeArray3[i];
				DynamicBuffer<SubLane> subLanes = bufferAccessor[i];
				bool flag = (trafficLights.m_Flags & TrafficLightFlags.LevelCrossing) != 0;
				bool flag2 = false;
				bool isSubNode = false;
				int groupCount = 0;
				MoveableBridgeData moveableBridgeData = default(MoveableBridgeData);
				if (flag && CollectionUtils.TryGet<Owner>(nativeArray2, i, ref owner) && FindMoveableBridgeData(val, owner.m_Owner, out moveableBridgeData))
				{
					flag2 = true;
					isSubNode = true;
					trafficLights.m_Flags |= TrafficLightFlags.MoveableBridge | TrafficLightFlags.IsSubNode;
				}
				else if (flag && CollectionUtils.TryGet<Game.Objects.SubObject>(bufferAccessor2, i, ref subObjects) && FindMoveableBridgeData(subObjects, out moveableBridgeData))
				{
					flag2 = true;
					trafficLights.m_Flags &= ~TrafficLightFlags.IsSubNode;
					trafficLights.m_Flags |= TrafficLightFlags.MoveableBridge;
				}
				else
				{
					trafficLights.m_Flags &= ~(TrafficLightFlags.MoveableBridge | TrafficLightFlags.IsSubNode);
				}
				FillLaneBuffers(subLanes, vehicleLanes, pedestrianLanes);
				if (flag2)
				{
					ProcessMoveableBridgeLanes(val, vehicleLanes, pedestrianLanes, groups, subLanes, moveableBridgeData, isSubNode, ref groupIndexMap, out groupCount);
				}
				else
				{
					ProcessVehicleLaneGroups(vehicleLanes, groups, flag, out groupCount);
					ProcessPedestrianLaneGroups(subLanes, pedestrianLanes, groups, flag, ref groupCount);
				}
				InitializeTrafficLights(subLanes, groups, groupCount, flag, flag2, ref trafficLights);
				nativeArray3[i] = trafficLights;
				groups.Clear();
				vehicleLanes.Clear();
				pedestrianLanes.Clear();
			}
			groups.Dispose();
			vehicleLanes.Dispose();
			pedestrianLanes.Dispose();
			if (groupIndexMap.IsCreated)
			{
				groupIndexMap.Dispose();
			}
		}

		private bool FindMoveableBridgeData(Entity node, Entity owner, out MoveableBridgeData moveableBridgeData)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			moveableBridgeData = default(MoveableBridgeData);
			Edge edge = default(Edge);
			if (!m_EdgeData.TryGetComponent(owner, ref edge))
			{
				return false;
			}
			Node node2 = m_NodeData[node];
			Curve curve = m_CurveData[owner];
			float num = math.distancesq(node2.m_Position, curve.m_Bezier.a);
			float num2 = math.distancesq(node2.m_Position, curve.m_Bezier.d);
			Entity val = ((num <= num2) ? edge.m_Start : edge.m_End);
			DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
			if (!m_SubObjects.TryGetBuffer(val, ref subObjects))
			{
				return false;
			}
			return FindMoveableBridgeData(subObjects, out moveableBridgeData);
		}

		private bool FindMoveableBridgeData(DynamicBuffer<Game.Objects.SubObject> subObjects, out MoveableBridgeData moveableBridgeData)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_PointOfInterestData.HasComponent(subObject))
				{
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					if (m_PrefabMoveableBridgeData.TryGetComponent(prefabRef.m_Prefab, ref moveableBridgeData))
					{
						return true;
					}
				}
			}
			moveableBridgeData = default(MoveableBridgeData);
			return false;
		}

		private void ProcessMoveableBridgeLanes(Entity entity, NativeList<LaneGroup> vehicleLanes, NativeList<LaneGroup> pedestrianLanes, NativeList<LaneGroup> groups, DynamicBuffer<SubLane> subLanes, MoveableBridgeData moveableBridgeData, bool isSubNode, ref NativeHashMap<PathNode, int> groupIndexMap, out int groupCount)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			if (groupIndexMap.IsCreated)
			{
				groupIndexMap.Clear();
			}
			else
			{
				groupIndexMap = new NativeHashMap<PathNode, int>(32, AllocatorHandle.op_Implicit((Allocator)2));
			}
			PrefabRef prefabRef = m_PrefabRefData[entity];
			bool2 val = moveableBridgeData.m_LiftOffsets.z != ((float3)(ref moveableBridgeData.m_LiftOffsets)).xy;
			groupCount = math.select(2, 3, math.all(val));
			int num = math.select(math.select(0, 1, val.x), 2, math.all(val));
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, entity, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
			EdgeIteratorValue value;
			DynamicBuffer<SubLane> val2 = default(DynamicBuffer<SubLane>);
			while (edgeIterator.GetNext(out value))
			{
				if (m_SubLanes.TryGetBuffer(value.m_Edge, ref val2))
				{
					int num2 = math.select(1, 0, m_PrefabRefData[value.m_Edge].m_Prefab == prefabRef.m_Prefab);
					num2 = math.select(num2, num, isSubNode);
					for (int i = 0; i < val2.Length; i++)
					{
						Lane lane = m_LaneData[val2[i].m_SubLane];
						groupIndexMap.TryAdd(lane.m_StartNode, num2);
						groupIndexMap.TryAdd(lane.m_EndNode, num2);
					}
				}
			}
			for (int j = 0; j < vehicleLanes.Length; j++)
			{
				LaneGroup laneGroup = vehicleLanes[j];
				if (!groupIndexMap.TryGetValue(m_LaneData[subLanes[laneGroup.m_LaneRange.x].m_SubLane].m_StartNode, ref laneGroup.m_GroupIndex))
				{
					laneGroup.m_GroupIndex = groupCount;
				}
				laneGroup.m_GroupMask = (ushort)(1 << (laneGroup.m_GroupIndex & 0xF));
				groups.Add(ref laneGroup);
			}
			for (int k = 0; k < pedestrianLanes.Length; k++)
			{
				LaneGroup laneGroup2 = pedestrianLanes[k];
				if (laneGroup2.m_IsWaterway)
				{
					laneGroup2.m_GroupMask = (ushort)(~(-1 << groupCount));
				}
				else
				{
					if (!groupIndexMap.TryGetValue(m_LaneData[subLanes[laneGroup2.m_LaneRange.x].m_SubLane].m_StartNode, ref laneGroup2.m_GroupIndex))
					{
						laneGroup2.m_GroupIndex = groupCount;
					}
					laneGroup2.m_GroupMask = (ushort)(1 << (laneGroup2.m_GroupIndex & 0xF));
				}
				groups.Add(ref laneGroup2);
			}
		}

		private void FillLaneBuffers(DynamicBuffer<SubLane> subLanes, NativeList<LaneGroup> vehicleLanes, NativeList<LaneGroup> pedestrianLanes)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			PedestrianLane pedestrianLane = default(PedestrianLane);
			MasterLane masterLane = default(MasterLane);
			CarLane carLane = default(CarLane);
			CarLaneData carLaneData = default(CarLaneData);
			CarLane carLane2 = default(CarLane);
			CarLaneData carLaneData2 = default(CarLaneData);
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				if (!m_LaneSignalData.HasComponent(subLane) || m_SecondaryLaneData.HasComponent(subLane))
				{
					continue;
				}
				float3 val;
				if (m_PedestrianLaneData.TryGetComponent(subLane, ref pedestrianLane))
				{
					Curve curve = m_CurveData[subLane];
					LaneGroup laneGroup = default(LaneGroup);
					val = MathUtils.StartTangent(curve.m_Bezier);
					laneGroup.m_StartDirection = math.normalizesafe(((float3)(ref val)).xz, default(float2));
					val = MathUtils.EndTangent(curve.m_Bezier);
					laneGroup.m_EndDirection = math.normalizesafe(-((float3)(ref val)).xz, default(float2));
					laneGroup.m_LaneRange = new int2(i, i);
					laneGroup.m_IsUnsafe = (pedestrianLane.m_Flags & PedestrianLaneFlags.Unsafe) != 0;
					laneGroup.m_IsWaterway = (pedestrianLane.m_Flags & PedestrianLaneFlags.OnWater) != 0;
					laneGroup.m_IsPedestrian = true;
					LaneGroup laneGroup2 = laneGroup;
					pedestrianLanes.Add(ref laneGroup2);
				}
				else if (m_MasterLaneData.TryGetComponent(subLane, ref masterLane))
				{
					Curve curve2 = m_CurveData[subLane];
					PrefabRef prefabRef = m_PrefabRefData[subLane];
					LaneGroup laneGroup = default(LaneGroup);
					val = MathUtils.StartTangent(curve2.m_Bezier);
					laneGroup.m_StartDirection = math.normalizesafe(((float3)(ref val)).xz, default(float2));
					val = MathUtils.EndTangent(curve2.m_Bezier);
					laneGroup.m_EndDirection = math.normalizesafe(-((float3)(ref val)).xz, default(float2));
					laneGroup.m_LaneRange = new int2(masterLane.m_MinIndex - 1, (int)masterLane.m_MaxIndex);
					LaneGroup laneGroup3 = laneGroup;
					if (m_CarLaneData.TryGetComponent(subLane, ref carLane))
					{
						laneGroup3.m_IsStraight = (carLane.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight)) == 0;
						laneGroup3.m_IsUnsafe = (carLane.m_Flags & CarLaneFlags.Unsafe) != 0;
						if (m_PrefabCarLaneData.TryGetComponent(prefabRef.m_Prefab, ref carLaneData))
						{
							laneGroup3.m_IsWaterway = (carLaneData.m_RoadTypes & RoadTypes.Watercraft) != 0;
						}
					}
					else
					{
						laneGroup3.m_IsStraight = true;
					}
					vehicleLanes.Add(ref laneGroup3);
				}
				else
				{
					if (m_SlaveLaneData.HasComponent(subLane))
					{
						continue;
					}
					Curve curve3 = m_CurveData[subLane];
					PrefabRef prefabRef2 = m_PrefabRefData[subLane];
					LaneGroup laneGroup = default(LaneGroup);
					val = MathUtils.StartTangent(curve3.m_Bezier);
					laneGroup.m_StartDirection = math.normalizesafe(((float3)(ref val)).xz, default(float2));
					val = MathUtils.EndTangent(curve3.m_Bezier);
					laneGroup.m_EndDirection = math.normalizesafe(-((float3)(ref val)).xz, default(float2));
					laneGroup.m_LaneRange = new int2(i, i);
					LaneGroup laneGroup4 = laneGroup;
					if (m_CarLaneData.TryGetComponent(subLane, ref carLane2))
					{
						laneGroup4.m_IsStraight = (carLane2.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight)) == 0;
						laneGroup4.m_IsUnsafe = (carLane2.m_Flags & CarLaneFlags.Unsafe) != 0;
						if (m_PrefabCarLaneData.TryGetComponent(prefabRef2.m_Prefab, ref carLaneData2))
						{
							laneGroup4.m_IsWaterway = (carLaneData2.m_RoadTypes & RoadTypes.Watercraft) != 0;
						}
					}
					else
					{
						laneGroup4.m_IsStraight = true;
						laneGroup4.m_IsTrack = true;
					}
					vehicleLanes.Add(ref laneGroup4);
				}
			}
		}

		private void ProcessVehicleLaneGroups(NativeList<LaneGroup> vehicleLanes, NativeList<LaneGroup> groups, bool isLevelCrossing, out int groupCount)
		{
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			groupCount = 0;
			while (vehicleLanes.Length > 0)
			{
				LaneGroup laneGroup = vehicleLanes[0];
				laneGroup.m_GroupIndex = groupCount++;
				groups.Add(ref laneGroup);
				vehicleLanes.RemoveAtSwapBack(0);
				int num = 0;
				while (num < vehicleLanes.Length)
				{
					LaneGroup laneGroup2 = vehicleLanes[num];
					if ((!isLevelCrossing | (laneGroup.m_IsTrack == laneGroup2.m_IsTrack)) && math.dot(laneGroup.m_StartDirection, laneGroup2.m_StartDirection) > 0.999f)
					{
						laneGroup2.m_GroupIndex = laneGroup.m_GroupIndex;
						groups.Add(ref laneGroup2);
						vehicleLanes.RemoveAtSwapBack(num);
					}
					else
					{
						num++;
					}
				}
			}
			int i = 0;
			groupCount = 0;
			while (i < groups.Length)
			{
				LaneGroup laneGroup3 = groups[i++];
				groupCount = math.select(laneGroup3.m_GroupIndex + 1, groupCount, laneGroup3.m_IsCombined);
				float2 val = default(float2);
				float2 val2 = default(float2);
				float num2 = 1f;
				if (laneGroup3.m_IsStraight && !laneGroup3.m_IsCombined)
				{
					val = laneGroup3.m_StartDirection;
					val2 = laneGroup3.m_EndDirection;
					num2 = math.dot(laneGroup3.m_StartDirection, laneGroup3.m_EndDirection);
				}
				for (; i < groups.Length; i++)
				{
					LaneGroup laneGroup4 = groups[i];
					if (laneGroup4.m_GroupIndex != laneGroup3.m_GroupIndex)
					{
						break;
					}
					if (laneGroup4.m_IsStraight && !laneGroup4.m_IsCombined)
					{
						float num3 = math.dot(laneGroup4.m_StartDirection, laneGroup4.m_EndDirection);
						if (num3 < num2)
						{
							val = laneGroup4.m_StartDirection;
							val2 = laneGroup4.m_EndDirection;
							num2 = num3;
						}
					}
				}
				if (num2 >= 0f)
				{
					continue;
				}
				int num4 = i;
				while (num4 < groups.Length)
				{
					int num5 = num4;
					int num6 = num4;
					LaneGroup laneGroup5 = groups[num4++];
					bool flag = false;
					if (!laneGroup5.m_IsCombined)
					{
						if (isLevelCrossing)
						{
							if (laneGroup3.m_IsTrack == laneGroup5.m_IsTrack)
							{
								flag = true;
							}
						}
						else if (laneGroup5.m_IsStraight && math.dot(val, laneGroup5.m_EndDirection) > 0.999f && math.dot(val2, laneGroup5.m_StartDirection) > 0.999f)
						{
							flag = true;
						}
					}
					while (num4 < groups.Length)
					{
						LaneGroup laneGroup6 = groups[num4];
						if (laneGroup6.m_GroupIndex != laneGroup5.m_GroupIndex)
						{
							break;
						}
						if (!laneGroup6.m_IsCombined)
						{
							if (isLevelCrossing)
							{
								if (laneGroup3.m_IsTrack == laneGroup6.m_IsTrack)
								{
									flag = true;
								}
							}
							else if (laneGroup6.m_IsStraight && math.dot(val, laneGroup6.m_EndDirection) > 0.999f && math.dot(val2, laneGroup6.m_StartDirection) > 0.999f)
							{
								flag = true;
							}
						}
						num6 = num4++;
					}
					if (!flag)
					{
						continue;
					}
					for (int j = num5; j <= num6; j++)
					{
						laneGroup5 = groups[j];
						laneGroup5.m_GroupIndex = laneGroup3.m_GroupIndex;
						laneGroup5.m_IsCombined = true;
						groups[j] = laneGroup5;
					}
					for (int k = num6 + 1; k < groups.Length; k++)
					{
						laneGroup5 = groups[k];
						if (!laneGroup5.m_IsCombined)
						{
							laneGroup5.m_GroupIndex--;
							groups[k] = laneGroup5;
						}
					}
				}
			}
			for (int l = 0; l < groups.Length; l++)
			{
				LaneGroup laneGroup7 = groups[l];
				laneGroup7.m_GroupMask = (ushort)(1 << (laneGroup7.m_GroupIndex & 0xF));
				groups[l] = laneGroup7;
			}
		}

		private void ProcessPedestrianLaneGroups(DynamicBuffer<SubLane> subLanes, NativeList<LaneGroup> pedestrianLanes, NativeList<LaneGroup> groups, bool isLevelCrossing, ref int groupCount)
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			if (groupCount <= 1)
			{
				int num = groupCount++;
				for (int i = 0; i < pedestrianLanes.Length; i++)
				{
					LaneGroup laneGroup = pedestrianLanes[i];
					laneGroup.m_GroupMask = (ushort)(1 << (num & 0xF));
					groups.Add(ref laneGroup);
				}
				return;
			}
			int length = groups.Length;
			int num2 = -1;
			float4 val2 = default(float4);
			for (int j = 0; j < pedestrianLanes.Length; j++)
			{
				LaneGroup laneGroup2 = pedestrianLanes[j];
				laneGroup2.m_GroupMask = (ushort)((1 << math.min(16, groupCount)) - 1);
				Entity subLane = subLanes[laneGroup2.m_LaneRange.x].m_SubLane;
				if (!laneGroup2.m_IsUnsafe && m_Overlaps.HasBuffer(subLane))
				{
					DynamicBuffer<LaneOverlap> val = m_Overlaps[subLane];
					for (int k = 0; k < length; k++)
					{
						LaneGroup laneGroup3 = groups[k];
						bool flag;
						if (isLevelCrossing)
						{
							flag = !laneGroup3.m_IsTrack;
						}
						else
						{
							flag = !laneGroup3.m_IsStraight;
							if (flag)
							{
								val2.x = math.dot(laneGroup2.m_StartDirection, laneGroup3.m_StartDirection);
								val2.y = math.dot(laneGroup2.m_StartDirection, laneGroup3.m_EndDirection);
								val2.z = math.dot(laneGroup2.m_EndDirection, laneGroup3.m_StartDirection);
								val2.w = math.dot(laneGroup2.m_EndDirection, laneGroup3.m_EndDirection);
								val2 = math.abs(val2);
								flag = val2.x + val2.z > val2.y + val2.w;
							}
						}
						bool flag2 = false;
						if (!flag)
						{
							for (int l = laneGroup3.m_LaneRange.x; l <= laneGroup3.m_LaneRange.y; l++)
							{
								for (int m = 0; m < val.Length; m++)
								{
									flag2 |= val[m].m_Other == subLanes[l].m_SubLane;
								}
							}
						}
						if (!flag && flag2)
						{
							laneGroup2.m_GroupMask &= (ushort)(~laneGroup3.m_GroupMask);
						}
					}
				}
				if (laneGroup2.m_GroupMask == 0)
				{
					if (num2 == -1)
					{
						num2 = groupCount++;
					}
					laneGroup2.m_GroupMask = (ushort)(1 << (num2 & 0xF));
				}
				groups.Add(ref laneGroup2);
			}
		}

		private void InitializeTrafficLights(DynamicBuffer<SubLane> subLanes, NativeList<LaneGroup> groups, int groupCount, bool isLevelCrossing, bool isMoveableBridge, ref TrafficLights trafficLights)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			trafficLights.m_SignalGroupCount = (byte)math.min(16, groupCount);
			if (trafficLights.m_CurrentSignalGroup > trafficLights.m_SignalGroupCount || trafficLights.m_NextSignalGroup > trafficLights.m_SignalGroupCount)
			{
				trafficLights.m_CurrentSignalGroup = 0;
				trafficLights.m_NextSignalGroup = 0;
				trafficLights.m_Timer = 0;
				trafficLights.m_State = TrafficLightState.None;
			}
			for (int i = 0; i < groups.Length; i++)
			{
				LaneGroup laneGroup = groups[i];
				sbyte b = (sbyte)math.select(0, -1, isLevelCrossing & ((laneGroup.m_IsTrack && !isMoveableBridge) | (laneGroup.m_IsWaterway & !laneGroup.m_IsPedestrian)));
				for (int j = laneGroup.m_LaneRange.x; j <= laneGroup.m_LaneRange.y; j++)
				{
					Entity subLane = subLanes[j].m_SubLane;
					LaneSignal laneSignal = m_LaneSignalData[subLane];
					laneSignal.m_GroupMask = laneGroup.m_GroupMask;
					laneSignal.m_Default = b;
					if (!isLevelCrossing && m_CarLaneData.HasComponent(subLane))
					{
						laneSignal.m_Flags |= LaneSignalFlags.CanExtend;
					}
					if (isMoveableBridge)
					{
						laneSignal.m_Flags |= LaneSignalFlags.Physical;
					}
					TrafficLightSystem.UpdateLaneSignal(trafficLights, ref laneSignal);
					m_LaneSignalData[subLane] = laneSignal;
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
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		public ComponentTypeHandle<TrafficLights> __Game_Net_TrafficLights_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PointOfInterest> __Game_Common_PointOfInterest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MoveableBridgeData> __Game_Prefabs_MoveableBridgeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Objects.SubObject>(true);
			__Game_Net_TrafficLights_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrafficLights>(false);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PedestrianLane>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SecondaryLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Common_PointOfInterest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PointOfInterest>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_MoveableBridgeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MoveableBridgeData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_LaneSignal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(false);
		}
	}

	private EntityQuery m_TrafficLightsQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrafficLights>() };
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		array[0] = val;
		m_TrafficLightsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_TrafficLightsQuery);
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
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependency = JobChunkExtensions.ScheduleParallel<InitializeTrafficLightsJob>(new InitializeTrafficLightsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightsType = InternalCompilerInterface.GetComponentTypeHandle<TrafficLights>(ref __TypeHandle.__Game_Net_TrafficLights_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PointOfInterestData = InternalCompilerInterface.GetComponentLookup<PointOfInterest>(ref __TypeHandle.__Game_Common_PointOfInterest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMoveableBridgeData = InternalCompilerInterface.GetComponentLookup<MoveableBridgeData>(ref __TypeHandle.__Game_Prefabs_MoveableBridgeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Overlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		}, m_TrafficLightsQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = dependency;
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
	public TrafficLightInitializationSystem()
	{
	}
}

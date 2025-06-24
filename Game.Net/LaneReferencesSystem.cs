using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Pathfind;
using Game.Prefabs;
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
public class LaneReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateLaneReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PedestrianLane> m_PedestrianLaneType;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> m_CarLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ParkingLane> m_ParkingLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> m_ConnectionLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		public BufferLookup<SubLane> m_Lanes;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (m_Lanes.TryGetBuffer(nativeArray2[i].m_Owner, ref val))
					{
						CollectionUtils.RemoveValue<SubLane>(val, new SubLane(nativeArray[i], (PathMethod)0));
					}
				}
				return;
			}
			NativeArray<ParkingLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkingLane>(ref m_ParkingLaneType);
			NativeArray<ConnectionLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ConnectionLane>(ref m_ConnectionLaneType);
			PathMethod pathMethod = (PathMethod)0;
			if (((ArchetypeChunk)(ref chunk)).Has<PedestrianLane>(ref m_PedestrianLaneType))
			{
				pathMethod |= PathMethod.Pedestrian;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<CarLane>(ref m_CarLaneType))
			{
				pathMethod |= PathMethod.Road;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<TrackLane>(ref m_TrackLaneType))
			{
				pathMethod |= PathMethod.Track;
			}
			ParkingLane parkingLane = default(ParkingLane);
			ConnectionLane connectionLane = default(ConnectionLane);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				DynamicBuffer<SubLane> val2 = m_Lanes[nativeArray2[j].m_Owner];
				PathMethod pathMethod2 = pathMethod;
				if (CollectionUtils.TryGet<ParkingLane>(nativeArray3, j, ref parkingLane))
				{
					pathMethod2 = (((parkingLane.m_Flags & ParkingLaneFlags.SpecialVehicles) == 0) ? (pathMethod2 | (PathMethod.Parking | PathMethod.Boarding)) : (pathMethod2 | (PathMethod.Boarding | PathMethod.SpecialParking)));
				}
				if (CollectionUtils.TryGet<ConnectionLane>(nativeArray4, j, ref connectionLane))
				{
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						pathMethod2 |= PathMethod.Pedestrian;
					}
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0)
					{
						pathMethod2 |= PathMethod.Road;
					}
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Track) != 0)
					{
						pathMethod2 |= PathMethod.Track;
					}
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
					{
						pathMethod2 |= PathMethod.Parking | PathMethod.Boarding;
					}
				}
				CollectionUtils.TryAddUniqueValue<SubLane>(val2, new SubLane(nativeArray[j], pathMethod2));
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillNodeMapJob : IJob
	{
		public NativeQueue<Lane> m_SkipLaneQueue;

		public NativeHashMap<PathNode, PathNode> m_PathNodeMap;

		public void Execute()
		{
			Lane lane = default(Lane);
			while (m_SkipLaneQueue.TryDequeue(ref lane))
			{
				PathNode pathNode = new PathNode(lane.m_MiddleNode, secondaryNode: false);
				m_PathNodeMap.TryAdd(lane.m_StartNode, pathNode);
				m_PathNodeMap.TryAdd(lane.m_MiddleNode, pathNode);
				m_PathNodeMap.TryAdd(lane.m_EndNode, pathNode);
			}
		}
	}

	[BurstCompile]
	private struct FixSkippedLanesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		[ReadOnly]
		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public NativeHashMap<PathNode, PathNode> m_PathNodeMap;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
			if (bufferAccessor.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<SubLane> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
			bool2 val4 = default(bool2);
			EdgeLane edgeLane = default(EdgeLane);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<ConnectedEdge> val2 = bufferAccessor[i];
				DynamicBuffer<SubLane> val3 = bufferAccessor2[i];
				RefRW<Lane> refRW;
				for (int j = 0; j < val3.Length; j++)
				{
					SubLane subLane = val3[j];
					if ((subLane.m_PathMethods & (PathMethod.Pedestrian | PathMethod.Road)) != 0)
					{
						refRW = m_LaneData.GetRefRW(subLane.m_SubLane);
						ref Lane valueRW = ref refRW.ValueRW;
						UpdatePathNode(ref valueRW.m_StartNode, nativeArray2.Length != 0);
						UpdatePathNode(ref valueRW.m_EndNode, nativeArray2.Length != 0);
					}
				}
				for (int k = 0; k < val2.Length; k++)
				{
					ConnectedEdge connectedEdge = val2[k];
					if (!m_SubLanes.TryGetBuffer(connectedEdge.m_Edge, ref val3))
					{
						continue;
					}
					Edge edge = m_EdgeData[connectedEdge.m_Edge];
					((bool2)(ref val4))._002Ector(edge.m_Start == val, edge.m_End == val);
					if (!math.any(val4))
					{
						continue;
					}
					float num = math.select(0f, 1f, val4.y);
					int segmentIndex = math.select(0, 4, val4.y);
					bool flag = m_UpdatedData.HasComponent(connectedEdge.m_Edge);
					for (int l = 0; l < val3.Length; l++)
					{
						SubLane subLane2 = val3[l];
						if ((subLane2.m_PathMethods & (PathMethod.Pedestrian | PathMethod.Road | PathMethod.Track)) == 0 || !m_EdgeLaneData.TryGetComponent(subLane2.m_SubLane, ref edgeLane))
						{
							continue;
						}
						bool2 val5 = edgeLane.m_EdgeDelta == num;
						if (math.any(val5))
						{
							refRW = m_LaneData.GetRefRW(subLane2.m_SubLane);
							ref Lane valueRW2 = ref refRW.ValueRW;
							bool flag2 = ((!val5.x) ? UpdatePathNode(ref valueRW2.m_EndNode, valueRW2.m_MiddleNode, segmentIndex, val, nativeArray2.Length != 0) : UpdatePathNode(ref valueRW2.m_StartNode, valueRW2.m_MiddleNode, segmentIndex, val, nativeArray2.Length != 0));
							if (flag2 && !flag)
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, subLane2.m_SubLane, default(PathfindUpdated));
							}
						}
					}
				}
			}
		}

		private void UpdatePathNode(ref PathNode pathNode, bool isTemp)
		{
			PathNode pathNode2 = pathNode.StripCurvePos();
			PathNode pathNode3 = default(PathNode);
			if (m_PathNodeMap.TryGetValue(new PathNode(pathNode2, isTemp), ref pathNode3))
			{
				pathNode = pathNode3;
			}
		}

		private bool UpdatePathNode(ref PathNode pathNode, PathNode middleNode, int segmentIndex, Entity ownerNode, bool isTemp)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			PathNode pathNode2 = pathNode;
			if (pathNode.OwnerEquals(new PathNode(ownerNode, 0)))
			{
				pathNode2 = middleNode;
				pathNode2.SetSegmentIndex((byte)segmentIndex);
			}
			PathNode pathNode3 = default(PathNode);
			if (!m_PathNodeMap.TryGetValue(new PathNode(pathNode2, isTemp), ref pathNode3))
			{
				pathNode3 = pathNode2;
			}
			if (!pathNode.Equals(pathNode3))
			{
				pathNode = pathNode3;
				return true;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLaneIndicesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Road> m_RoadType;

		[ReadOnly]
		public ComponentTypeHandle<TramTrack> m_TramTrackType;

		[ReadOnly]
		public ComponentTypeHandle<TrainTrack> m_TrainTrackType;

		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			NativeList<SubLaneOrder> val = default(NativeList<SubLaneOrder>);
			val._002Ector(256, AllocatorHandle.op_Implicit((Allocator)2));
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
			bool flag = false;
			TrackTypes trackTypes = TrackTypes.None;
			if (((ArchetypeChunk)(ref chunk)).Has<Node>(ref m_NodeType))
			{
				flag = ((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType);
				if (flag)
				{
					if (((ArchetypeChunk)(ref chunk)).Has<TramTrack>(ref m_TramTrackType))
					{
						trackTypes |= TrackTypes.Tram;
					}
					if (((ArchetypeChunk)(ref chunk)).Has<TrainTrack>(ref m_TrainTrackType))
					{
						trackTypes |= TrackTypes.Train;
					}
				}
			}
			TrackLaneData trackLaneData = default(TrackLaneData);
			MasterLane masterLane = default(MasterLane);
			SlaveLane slaveLane = default(SlaveLane);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<SubLane> val2 = bufferAccessor[i];
				TrackTypes trackTypes2 = TrackTypes.None;
				for (int j = 0; j < val2.Length; j++)
				{
					SubLaneOrder subLaneOrder = new SubLaneOrder
					{
						m_SubLane = val2[j]
					};
					if (flag && (subLaneOrder.m_SubLane.m_PathMethods & PathMethod.Track) != 0)
					{
						PrefabRef prefabRef = m_PrefabRefData[subLaneOrder.m_SubLane.m_SubLane];
						if (m_PrefabTrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
						{
							trackTypes2 |= trackLaneData.m_TrackTypes;
						}
					}
					if (m_MasterLaneData.TryGetComponent(subLaneOrder.m_SubLane.m_SubLane, ref masterLane))
					{
						subLaneOrder.m_Group = masterLane.m_Group;
						subLaneOrder.m_Index = -1;
						subLaneOrder.m_FullLane = false;
					}
					else if (m_SlaveLaneData.TryGetComponent(subLaneOrder.m_SubLane.m_SubLane, ref slaveLane))
					{
						subLaneOrder.m_Group = slaveLane.m_Group;
						subLaneOrder.m_Index = (slaveLane.m_SubIndex << 16) | j;
						subLaneOrder.m_FullLane = (slaveLane.m_Flags & (SlaveLaneFlags.StartingLane | SlaveLaneFlags.EndingLane)) == 0;
						subLaneOrder.m_MergeLane = (slaveLane.m_Flags & SlaveLaneFlags.MergingLane) != 0;
					}
					else if (m_SecondaryLaneData.HasComponent(subLaneOrder.m_SubLane.m_SubLane))
					{
						subLaneOrder.m_Group = uint.MaxValue;
						subLaneOrder.m_Index = j;
					}
					else
					{
						subLaneOrder.m_Group = 4294967294u;
						subLaneOrder.m_Index = j;
					}
					val.Add(ref subLaneOrder);
				}
				NativeSortExtension.Sort<SubLaneOrder>(val);
				int num = 0;
				while (num < val.Length)
				{
					SubLaneOrder subLaneOrder2 = val[num];
					if (subLaneOrder2.m_Group < 4294967294u)
					{
						int num2 = num;
						int k = num + 1;
						int num3 = math.select(0, 1, subLaneOrder2.m_FullLane);
						for (; k < val.Length; k++)
						{
							SubLaneOrder subLaneOrder3 = val[k];
							if (subLaneOrder3.m_Group != subLaneOrder2.m_Group)
							{
								break;
							}
							num3 += math.select(0, 1, subLaneOrder3.m_FullLane);
						}
						int num4 = -1;
						if (subLaneOrder2.m_Index == -1)
						{
							num2++;
							MasterLane masterLane2 = m_MasterLaneData[subLaneOrder2.m_SubLane.m_SubLane];
							masterLane2.m_MinIndex = (ushort)num2;
							masterLane2.m_MaxIndex = (ushort)(k - 1);
							m_MasterLaneData[subLaneOrder2.m_SubLane.m_SubLane] = masterLane2;
							num4 = num;
							val2[num++] = subLaneOrder2.m_SubLane;
						}
						Lane lane = default(Lane);
						Lane lane2 = default(Lane);
						bool flag2 = false;
						if (num < k)
						{
							SubLaneOrder subLaneOrder4 = val[num];
							lane2 = m_LaneData[subLaneOrder4.m_SubLane.m_SubLane];
						}
						while (num < k)
						{
							subLaneOrder2 = val[num];
							SlaveLane slaveLane2 = m_SlaveLaneData[subLaneOrder2.m_SubLane.m_SubLane];
							slaveLane2.m_Flags &= ~(SlaveLaneFlags.MultipleLanes | SlaveLaneFlags.OpenStartLeft | SlaveLaneFlags.OpenStartRight | SlaveLaneFlags.OpenEndLeft | SlaveLaneFlags.OpenEndRight);
							Lane lane3 = lane2;
							if (num > num2 && subLaneOrder2.m_MergeLane == flag2)
							{
								if (!lane.m_StartNode.Equals(lane3.m_StartNode))
								{
									slaveLane2.m_Flags |= SlaveLaneFlags.OpenStartLeft;
								}
								if (!lane.m_EndNode.Equals(lane3.m_EndNode))
								{
									slaveLane2.m_Flags |= SlaveLaneFlags.OpenEndLeft;
								}
							}
							if (num + 1 < k)
							{
								SubLaneOrder subLaneOrder5 = val[num + 1];
								lane2 = m_LaneData[subLaneOrder5.m_SubLane.m_SubLane];
								if (subLaneOrder2.m_MergeLane == subLaneOrder5.m_MergeLane)
								{
									if (!lane2.m_StartNode.Equals(lane3.m_StartNode))
									{
										slaveLane2.m_Flags |= SlaveLaneFlags.OpenStartRight;
									}
									if (!lane2.m_EndNode.Equals(lane3.m_EndNode))
									{
										slaveLane2.m_Flags |= SlaveLaneFlags.OpenEndRight;
									}
								}
							}
							lane = lane3;
							flag2 = subLaneOrder2.m_MergeLane;
							slaveLane2.m_MinIndex = (ushort)num2;
							slaveLane2.m_MaxIndex = (ushort)(k - 1);
							slaveLane2.m_SubIndex = (ushort)num;
							slaveLane2.m_MasterIndex = (ushort)math.select(num4, num, num4 == -1);
							if (num3 >= 2)
							{
								slaveLane2.m_Flags |= SlaveLaneFlags.MultipleLanes;
							}
							m_SlaveLaneData[subLaneOrder2.m_SubLane.m_SubLane] = slaveLane2;
							val2[num++] = subLaneOrder2.m_SubLane;
						}
					}
					else
					{
						val2[num++] = subLaneOrder2.m_SubLane;
					}
				}
				if (trackTypes != trackTypes2)
				{
					Entity val3 = nativeArray[i];
					if (((uint)trackTypes & (uint)(byte)(~(int)trackTypes2) & 2) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TramTrack>(unfilteredChunkIndex, val3);
					}
					if (((uint)trackTypes2 & (uint)(byte)(~(int)trackTypes) & 2) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TramTrack>(unfilteredChunkIndex, val3);
					}
					if (((uint)trackTypes & (uint)(byte)(~(int)trackTypes2) & 1) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TrainTrack>(unfilteredChunkIndex, val3);
					}
					if (((uint)trackTypes2 & (uint)(byte)(~(int)trackTypes) & 1) != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TrainTrack>(unfilteredChunkIndex, val3);
					}
				}
				val.Clear();
			}
			val.Dispose();
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct SubLaneOrder : IComparable<SubLaneOrder>
	{
		public uint m_Group;

		public int m_Index;

		public SubLane m_SubLane;

		public bool m_FullLane;

		public bool m_MergeLane;

		public int CompareTo(SubLaneOrder other)
		{
			int num = math.select(0, math.select(1, -1, other.m_Group > m_Group), other.m_Group != m_Group);
			return math.select(num, m_Index - other.m_Index, num == 0);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> __Game_Net_CarLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> __Game_Net_TrackLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkingLane> __Game_Net_ParkingLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		public BufferLookup<SubLane> __Game_Net_SubLane_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		public ComponentLookup<Lane> __Game_Net_Lane_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Road> __Game_Net_Road_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TramTrack> __Game_Net_TramTrack_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainTrack> __Game_Net_TrainTrack_RO_ComponentTypeHandle;

		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RW_ComponentLookup;

		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RW_ComponentLookup;

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
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_PedestrianLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PedestrianLane>(true);
			__Game_Net_CarLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackLane>(true);
			__Game_Net_ParkingLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConnectionLane>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Net_SubLane_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(false);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_ConnectedEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Net_Lane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(false);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Net_Road_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(true);
			__Game_Net_TramTrack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TramTrack>(true);
			__Game_Net_TrainTrack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainTrack>(true);
			__Game_Net_SubLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(false);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SecondaryLane>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Net_MasterLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(false);
			__Game_Net_SlaveLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(false);
		}
	}

	private ModificationBarrier4B m_ModificationBarrier;

	private EntityQuery m_LanesQuery;

	private EntityQuery m_UpdatedOwnersQuery;

	private EntityQuery m_AllOwnersQuery;

	private NativeQueue<Lane> m_SkipLaneQueue;

	private JobHandle m_SkipLaneDeps;

	private bool m_Loaded;

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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadOnly<Owner>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SecondaryLane>() };
		array[0] = val;
		m_LanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_UpdatedOwnersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<SubLane>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_AllOwnersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<SubLane>() });
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	public NativeQueue<Lane> GetSkipLaneQueue()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_SkipLaneQueue = new NativeQueue<Lane>(AllocatorHandle.op_Implicit((Allocator)3));
		return m_SkipLaneQueue;
	}

	public void AddSkipLaneWriter(JobHandle dependency)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SkipLaneDeps = dependency;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllOwnersQuery : m_UpdatedOwnersQuery);
		if (!((EntityQuery)(ref m_LanesQuery)).IsEmptyIgnoreFilter)
		{
			UpdateLaneReferencesJob updateLaneReferencesJob = new UpdateLaneReferencesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneType = InternalCompilerInterface.GetComponentTypeHandle<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Lanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			((SystemBase)this).Dependency = JobChunkExtensions.Schedule<UpdateLaneReferencesJob>(updateLaneReferencesJob, m_LanesQuery, ((SystemBase)this).Dependency);
		}
		EntityCommandBuffer val2;
		if (m_SkipLaneQueue.IsCreated)
		{
			NativeHashMap<PathNode, PathNode> pathNodeMap = default(NativeHashMap<PathNode, PathNode>);
			pathNodeMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
			FillNodeMapJob fillNodeMapJob = new FillNodeMapJob
			{
				m_SkipLaneQueue = m_SkipLaneQueue,
				m_PathNodeMap = pathNodeMap
			};
			FixSkippedLanesJob fixSkippedLanesJob = new FixSkippedLanesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathNodeMap = pathNodeMap
			};
			val2 = m_ModificationBarrier.CreateCommandBuffer();
			fixSkippedLanesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			FixSkippedLanesJob fixSkippedLanesJob2 = fixSkippedLanesJob;
			JobHandle val3 = IJobExtensions.Schedule<FillNodeMapJob>(fillNodeMapJob, m_SkipLaneDeps);
			JobHandle val4 = JobChunkExtensions.ScheduleParallel<FixSkippedLanesJob>(fixSkippedLanesJob2, m_UpdatedOwnersQuery, JobHandle.CombineDependencies(val3, ((SystemBase)this).Dependency));
			m_SkipLaneQueue.Dispose(val3);
			pathNodeMap.Dispose(val4);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val4);
			((SystemBase)this).Dependency = val4;
		}
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			UpdateLaneIndicesJob updateLaneIndicesJob = new UpdateLaneIndicesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TramTrackType = InternalCompilerInterface.GetComponentTypeHandle<TramTrack>(ref __TypeHandle.__Game_Net_TramTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrainTrackType = InternalCompilerInterface.GetComponentTypeHandle<TrainTrack>(ref __TypeHandle.__Game_Net_TrainTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			val2 = m_ModificationBarrier.CreateCommandBuffer();
			updateLaneIndicesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			UpdateLaneIndicesJob updateLaneIndicesJob2 = updateLaneIndicesJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateLaneIndicesJob>(updateLaneIndicesJob2, val, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
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
	public LaneReferencesSystem()
	{
	}
}

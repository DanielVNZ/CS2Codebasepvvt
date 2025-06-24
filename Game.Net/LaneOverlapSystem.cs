using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Common;
using Game.Pathfind;
using Game.Prefabs;
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
public class LaneOverlapSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddNonUpdatedEdgesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public NativeList<Entity> m_Entities;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			NativeHashSet<Entity> val = default(NativeHashSet<Entity>);
			val._002Ector(m_Entities.Length, AllocatorHandle.op_Implicit((Allocator)2));
			DynamicBuffer<ConnectedEdge> val2 = default(DynamicBuffer<ConnectedEdge>);
			for (int i = 0; i < m_Entities.Length; i++)
			{
				if (!m_ConnectedEdges.TryGetBuffer(m_Entities[i], ref val2))
				{
					continue;
				}
				for (int j = 0; j < val2.Length; j++)
				{
					Entity edge = val2[j].m_Edge;
					if (!m_UpdatedData.HasComponent(edge) && val.Add(edge))
					{
						m_Entities.Add(ref edge);
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct UpdateLaneFlagsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<MasterLane> m_MasterLaneType;

		[ReadOnly]
		public BufferTypeHandle<LaneOverlap> m_LaneOverlapType;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		public ComponentTypeHandle<NodeLane> m_NodeLaneType;

		public ComponentTypeHandle<CarLane> m_CarLaneType;

		public ComponentTypeHandle<TrackLane> m_TrackLaneType;

		public ComponentTypeHandle<SlaveLane> m_SlaveLaneType;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlapData;

		[ReadOnly]
		public NativeParallelMultiHashMap<PathNode, LaneSourceData> m_SourceMap;

		[ReadOnly]
		public NativeParallelMultiHashMap<PathNode, LaneTargetData> m_TargetMap;

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
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e82: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f48: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1029: Unknown result type (might be due to invalid IL or missing references)
			//IL_10be: Unknown result type (might be due to invalid IL or missing references)
			//IL_105c: Unknown result type (might be due to invalid IL or missing references)
			//IL_103c: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1072: Unknown result type (might be due to invalid IL or missing references)
			//IL_1107: Unknown result type (might be due to invalid IL or missing references)
			//IL_108e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0880: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_1123: Unknown result type (might be due to invalid IL or missing references)
			//IL_111a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0897: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Lane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Lane>(ref m_LaneType);
			NativeArray<CarLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarLane>(ref m_CarLaneType);
			NativeArray<TrackLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrackLane>(ref m_TrackLaneType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<EdgeLane> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeLane>(ref m_EdgeLaneType);
			if (nativeArray6.Length != 0)
			{
				Edge edge = default(Edge);
				Edge edge2 = default(Edge);
				for (int i = 0; i < nativeArray6.Length; i++)
				{
					Lane lane = nativeArray2[i];
					EdgeLane edgeLane = nativeArray6[i];
					bool flag = nativeArray3.Length != 0;
					bool trackLanes = nativeArray4.Length != 0 && !flag;
					if (lane.m_StartNode.OwnerEquals(lane.m_MiddleNode))
					{
						edgeLane.m_ConnectedStartCount = (byte)math.clamp(CalculateConnectedSources(lane.m_StartNode, flag, trackLanes), 0, 255);
					}
					else
					{
						edgeLane.m_ConnectedStartCount = 1;
					}
					if (lane.m_EndNode.OwnerEquals(lane.m_MiddleNode))
					{
						edgeLane.m_ConnectedEndCount = (byte)math.clamp(CalculateConnectedTargets(lane.m_EndNode, flag, trackLanes), 0, 255);
					}
					else
					{
						edgeLane.m_ConnectedEndCount = 1;
					}
					if (edgeLane.m_ConnectedStartCount == 0 && nativeArray5.Length != 0 && (edgeLane.m_EdgeDelta.x == 0f || edgeLane.m_EdgeDelta.x == 1f) && m_EdgeData.TryGetComponent(nativeArray5[i].m_Owner, ref edge) && m_OutsideConnectionData.HasComponent((edgeLane.m_EdgeDelta.x == 0f) ? edge.m_Start : edge.m_End))
					{
						edgeLane.m_ConnectedStartCount = 1;
					}
					if (edgeLane.m_ConnectedEndCount == 0 && nativeArray5.Length != 0 && (edgeLane.m_EdgeDelta.y == 0f || edgeLane.m_EdgeDelta.y == 1f) && m_EdgeData.TryGetComponent(nativeArray5[i].m_Owner, ref edge2) && m_OutsideConnectionData.HasComponent((edgeLane.m_EdgeDelta.y == 0f) ? edge2.m_Start : edge2.m_End))
					{
						edgeLane.m_ConnectedEndCount = 1;
					}
					nativeArray6[i] = edgeLane;
				}
				if (nativeArray3.Length != 0)
				{
					NativeParallelHashSet<PathNode> val = default(NativeParallelHashSet<PathNode>);
					NativeList<LaneTargetData> val2 = default(NativeList<LaneTargetData>);
					NativeArray<Curve> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
					NativeArray<SlaveLane> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SlaveLane>(ref m_SlaveLaneType);
					bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<MasterLane>(ref m_MasterLaneType);
					Owner owner = default(Owner);
					Composition composition = default(Composition);
					NetCompositionData netCompositionData = default(NetCompositionData);
					LaneSourceData laneSourceData = default(LaneSourceData);
					NativeParallelMultiHashMapIterator<PathNode> val3 = default(NativeParallelMultiHashMapIterator<PathNode>);
					LaneTargetData laneTargetData = default(LaneTargetData);
					NativeParallelMultiHashMapIterator<PathNode> val4 = default(NativeParallelMultiHashMapIterator<PathNode>);
					for (int j = 0; j < nativeArray2.Length; j++)
					{
						Lane lane2 = nativeArray2[j];
						EdgeLane edgeLane2 = nativeArray6[j];
						Curve curve = nativeArray7[j];
						CarLane carLane = nativeArray3[j];
						SlaveLane slaveLane = default(SlaveLane);
						if (nativeArray8.Length != 0)
						{
							slaveLane = nativeArray8[j];
							slaveLane.m_Flags &= ~(SlaveLaneFlags.MergingLane | SlaveLaneFlags.SplitLeft | SlaveLaneFlags.SplitRight | SlaveLaneFlags.MergeLeft | SlaveLaneFlags.MergeRight);
						}
						carLane.m_Flags &= ~(CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.LevelCrossing | CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight | CarLaneFlags.Forward | CarLaneFlags.Approach | CarLaneFlags.Roundabout | CarLaneFlags.ForbidPassing | CarLaneFlags.RightOfWay | CarLaneFlags.TrafficLights);
						bool flag3 = (carLane.m_Flags & CarLaneFlags.Highway) != 0;
						if (flag3 && CollectionUtils.TryGet<Owner>(nativeArray5, j, ref owner) && m_CompositionData.TryGetComponent(owner.m_Owner, ref composition) && m_PrefabCompositionData.TryGetComponent(composition.m_Edge, ref netCompositionData) && (netCompositionData.m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes | CompositionState.Multilane)) == (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes | CompositionState.Multilane))
						{
							carLane.m_Flags |= CarLaneFlags.ForbidPassing;
						}
						if (carLane.m_Curviness > math.select((float)Math.PI / 180f, (float)Math.PI / 360f, flag3) || edgeLane2.m_ConnectedEndCount == 0)
						{
							carLane.m_Flags |= CarLaneFlags.ForbidPassing;
						}
						if ((edgeLane2.m_ConnectedStartCount == 0) | (edgeLane2.m_ConnectedEndCount == 0))
						{
							if (nativeArray5.Length != 0 && edgeLane2.m_ConnectedEndCount == 0)
							{
								slaveLane.m_Flags |= GetMergeLaneFlags(nativeArray5[j].m_Owner, slaveLane, lane2, (carLane.m_Flags & CarLaneFlags.Invert) != 0);
								if ((slaveLane.m_Flags & (SlaveLaneFlags.MergeLeft | SlaveLaneFlags.MergeRight)) != 0)
								{
									carLane.m_Flags |= CarLaneFlags.Approach;
								}
							}
							else
							{
								slaveLane.m_Flags |= SlaveLaneFlags.MergingLane;
							}
						}
						bool flag4 = false;
						if (lane2.m_StartNode.OwnerEquals(lane2.m_MiddleNode) && m_SourceMap.TryGetFirstValue(lane2.m_StartNode, ref laneSourceData, ref val3))
						{
							do
							{
								if (laneSourceData.m_IsTrack)
								{
									continue;
								}
								if (!laneSourceData.m_IsEdge)
								{
									bool flag5 = false;
									bool flag6 = false;
									do
									{
										flag5 |= (laneSourceData.m_SlaveFlags & SlaveLaneFlags.OpenEndLeft) != 0;
										flag6 |= (laneSourceData.m_SlaveFlags & SlaveLaneFlags.OpenEndRight) != 0;
									}
									while (m_SourceMap.TryGetNextValue(ref laneSourceData, ref val3));
									if (!flag5)
									{
										slaveLane.m_Flags |= SlaveLaneFlags.SplitLeft;
									}
									if (!flag6)
									{
										slaveLane.m_Flags |= SlaveLaneFlags.SplitRight;
									}
								}
								break;
							}
							while (m_SourceMap.TryGetNextValue(ref laneSourceData, ref val3));
						}
						if (lane2.m_EndNode.OwnerEquals(lane2.m_MiddleNode) && m_TargetMap.TryGetFirstValue(lane2.m_EndNode, ref laneTargetData, ref val4))
						{
							do
							{
								if (laneTargetData.m_IsTrack)
								{
									continue;
								}
								bool flag7 = !laneTargetData.m_IsEdge;
								if (laneTargetData.m_IsEdge)
								{
									if (!flag3)
									{
										laneTargetData.m_CarFlags &= ~CarLaneFlags.ForbidPassing;
									}
									carLane.m_Flags |= laneTargetData.m_CarFlags;
									bool flag8 = !laneTargetData.m_EndNode.OwnerEquals(lane2.m_MiddleNode);
									if (!flag8 && m_TargetMap.TryGetFirstValue(laneTargetData.m_EndNode, ref laneTargetData, ref val4))
									{
										do
										{
											if (!laneTargetData.m_IsTrack)
											{
												flag8 = true;
												break;
											}
										}
										while (m_TargetMap.TryGetNextValue(ref laneTargetData, ref val4));
									}
									if (!flag8 && flag3)
									{
										carLane.m_Flags |= CarLaneFlags.ForbidPassing;
									}
								}
								if (laneTargetData.m_IsEdge)
								{
									break;
								}
								bool flag9 = false;
								bool flag10 = false;
								do
								{
									if (laneTargetData.m_IsTrack)
									{
										continue;
									}
									if ((laneTargetData.m_CarFlags & CarLaneFlags.Roundabout) != 0)
									{
										if (!flag4)
										{
											if (val.IsCreated)
											{
												val.Clear();
												val2.Clear();
											}
											else
											{
												val._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
												val2._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
											}
											val.Add(lane2.m_EndNode);
											flag4 = true;
										}
										val2.Add(ref laneTargetData);
										CarLaneFlags carLaneFlags = laneTargetData.m_CarFlags & (CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.RightOfWay);
										if (carLaneFlags != 0)
										{
											carLane.m_Flags |= carLaneFlags | CarLaneFlags.Approach;
										}
									}
									else
									{
										CarLaneFlags carLaneFlags2 = laneTargetData.m_CarFlags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.LevelCrossing | CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight | CarLaneFlags.Forward | CarLaneFlags.ForbidPassing | CarLaneFlags.RightOfWay | CarLaneFlags.TrafficLights);
										if (!flag2 && laneTargetData.m_IsMaster)
										{
											carLaneFlags2 = (CarLaneFlags)((uint)carLaneFlags2 & 0xFFE1FFCDu);
										}
										if (!flag2 && (carLaneFlags2 & CarLaneFlags.Yield) != 0 && (laneTargetData.m_IsMaster || !HasRoadOverlaps(laneTargetData.m_Entity)))
										{
											carLaneFlags2 = (CarLaneFlags)((uint)carLaneFlags2 & 0xFFFFFBFFu);
										}
										if (((uint)carLaneFlags2 & 0xFDFFFFFFu) != 0)
										{
											carLane.m_Flags |= CarLaneFlags.Approach;
										}
										carLane.m_Flags |= carLaneFlags2;
									}
									flag9 |= (laneTargetData.m_SlaveFlags & SlaveLaneFlags.OpenStartLeft) != 0;
									flag10 |= (laneTargetData.m_SlaveFlags & SlaveLaneFlags.OpenStartRight) != 0;
								}
								while (m_TargetMap.TryGetNextValue(ref laneTargetData, ref val4));
								if (flag7)
								{
									if (!flag9)
									{
										slaveLane.m_Flags |= SlaveLaneFlags.SplitLeft;
									}
									if (!flag10)
									{
										slaveLane.m_Flags |= SlaveLaneFlags.SplitRight;
									}
								}
								break;
							}
							while (m_TargetMap.TryGetNextValue(ref laneTargetData, ref val4));
						}
						if (flag4)
						{
							float3 val5 = MathUtils.EndTangent(curve.m_Bezier);
							float2 val6 = math.normalizesafe(((float3)(ref val5)).xz, default(float2));
							CarLaneFlags carLaneFlags3 = CarLaneFlags.Approach | CarLaneFlags.Roundabout | CarLaneFlags.ForbidPassing;
							int num = 0;
							while (num < val2.Length)
							{
								LaneTargetData laneTargetData2 = val2[num++];
								if (!val.Add(laneTargetData2.m_EndNode))
								{
									continue;
								}
								bool flag11 = false;
								if (m_TargetMap.TryGetFirstValue(laneTargetData2.m_EndNode, ref laneTargetData, ref val4))
								{
									do
									{
										if ((laneTargetData.m_CarFlags & CarLaneFlags.Roundabout) != 0)
										{
											laneTargetData.m_TurnAmount += laneTargetData2.m_TurnAmount;
											if (math.abs(laneTargetData2.m_TurnAmount) < (float)Math.PI * 2f)
											{
												val2.Add(ref laneTargetData);
											}
											flag11 = true;
										}
									}
									while (m_TargetMap.TryGetNextValue(ref laneTargetData, ref val4));
								}
								if (flag11)
								{
									continue;
								}
								Curve curve2 = m_CurveData[laneTargetData2.m_Entity];
								float2 endDirection = -val6;
								if (curve2.m_Length > 0.1f)
								{
									val5 = MathUtils.EndTangent(curve2.m_Bezier);
									endDirection = math.normalizesafe(-((float3)(ref val5)).xz, default(float2));
								}
								if (NetUtils.IsTurn(((float3)(ref curve.m_Bezier.d)).xz, val6, ((float3)(ref curve2.m_Bezier.d)).xz, endDirection, out var right, out var gentle, out var uturn))
								{
									if (laneTargetData2.m_TurnAmount > 0f == right)
									{
										carLaneFlags3 = (CarLaneFlags)((!gentle) ? ((!uturn) ? ((uint)carLaneFlags3 | (uint)(right ? 32 : 16)) : ((uint)carLaneFlags3 | (uint)(right ? 131072 : 2))) : ((uint)carLaneFlags3 | (uint)(right ? 524288 : 262144)));
									}
								}
								else if (math.abs(laneTargetData2.m_TurnAmount) < (float)Math.PI / 2f)
								{
									carLaneFlags3 |= CarLaneFlags.Forward;
								}
							}
							if ((carLaneFlags3 & (CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight | CarLaneFlags.Forward)) != 0)
							{
								carLaneFlags3 = (CarLaneFlags)((uint)carLaneFlags3 & 0xFFFDFFFDu);
							}
							carLane.m_Flags |= carLaneFlags3;
						}
						nativeArray3[j] = carLane;
						if (nativeArray8.Length != 0)
						{
							nativeArray8[j] = slaveLane;
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
				}
			}
			else if (nativeArray3.Length != 0)
			{
				NativeArray<NodeLane> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeLane>(ref m_NodeLaneType);
				NativeArray<MasterLane> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MasterLane>(ref m_MasterLaneType);
				BufferAccessor<LaneOverlap> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneOverlap>(ref m_LaneOverlapType);
				if (nativeArray9.Length != 0)
				{
					for (int k = 0; k < nativeArray3.Length; k++)
					{
						if ((nativeArray3[k].m_Flags & CarLaneFlags.Roundabout) != 0)
						{
							Lane lane3 = nativeArray2[k];
							NodeLane nodeLane = nativeArray9[k];
							if (lane3.m_EndNode.OwnerEquals(lane3.m_MiddleNode) && CalculateConnectedTargets(lane3.m_EndNode, carLanes: true, trackLanes: false) == 0)
							{
								nodeLane.m_SharedEndCount = byte.MaxValue;
							}
							if (lane3.m_StartNode.OwnerEquals(lane3.m_MiddleNode) && CalculateConnectedSources(lane3.m_StartNode, carLanes: true, trackLanes: false) == 0)
							{
								nodeLane.m_SharedStartCount = byte.MaxValue;
							}
							nativeArray9[k] = nodeLane;
						}
					}
				}
				DynamicBuffer<LaneOverlap> val8 = default(DynamicBuffer<LaneOverlap>);
				for (int l = 0; l < nativeArray3.Length; l++)
				{
					CarLane carLane2 = nativeArray3[l];
					carLane2.m_LaneCrossCount = 0;
					if (nativeArray10.Length != 0)
					{
						MasterLane masterLane = nativeArray10[l];
						Owner owner2 = nativeArray5[l];
						int num2 = 256;
						if (m_SubLanes.HasBuffer(owner2.m_Owner))
						{
							DynamicBuffer<SubLane> val7 = m_SubLanes[owner2.m_Owner];
							for (int m = masterLane.m_MinIndex; m <= masterLane.m_MaxIndex; m++)
							{
								Entity subLane = val7[m].m_SubLane;
								if (m_LaneOverlapData.TryGetBuffer(subLane, ref val8))
								{
									int num3 = 0;
									for (int n = 0; n < val8.Length; n++)
									{
										LaneOverlap laneOverlap = val8[n];
										num3 += math.select(0, 1, ((laneOverlap.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleStart | OverlapFlags.MergeMiddleEnd | OverlapFlags.Unsafe | OverlapFlags.Road)) == OverlapFlags.Road) | ((laneOverlap.m_Flags & (OverlapFlags.Road | OverlapFlags.MergeFlip)) == (OverlapFlags.Road | OverlapFlags.MergeFlip)));
									}
									num3 = math.min(255, num3);
									num2 = math.min(num3, num2);
								}
							}
						}
						carLane2.m_LaneCrossCount = (byte)math.select(num2, 0, num2 == 256);
					}
					else if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<LaneOverlap> val9 = bufferAccessor[l];
						int num4 = 0;
						for (int num5 = 0; num5 < val9.Length; num5++)
						{
							LaneOverlap laneOverlap2 = val9[num5];
							num4 += math.select(0, 1, ((laneOverlap2.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd | OverlapFlags.MergeMiddleStart | OverlapFlags.MergeMiddleEnd | OverlapFlags.Unsafe | OverlapFlags.Road)) == OverlapFlags.Road) | ((laneOverlap2.m_Flags & (OverlapFlags.Road | OverlapFlags.MergeFlip)) == (OverlapFlags.Road | OverlapFlags.MergeFlip)));
						}
						carLane2.m_LaneCrossCount = (byte)math.min(255, num4);
					}
					nativeArray3[l] = carLane2;
				}
			}
			if (nativeArray4.Length == 0)
			{
				return;
			}
			BufferAccessor<LaneOverlap> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneOverlap>(ref m_LaneOverlapType);
			LaneSourceData laneSourceData2 = default(LaneSourceData);
			NativeParallelMultiHashMapIterator<PathNode> val12 = default(NativeParallelMultiHashMapIterator<PathNode>);
			LaneTargetData laneTargetData3 = default(LaneTargetData);
			NativeParallelMultiHashMapIterator<PathNode> val13 = default(NativeParallelMultiHashMapIterator<PathNode>);
			LaneSourceData laneSourceData3 = default(LaneSourceData);
			NativeParallelMultiHashMapIterator<PathNode> val14 = default(NativeParallelMultiHashMapIterator<PathNode>);
			LaneTargetData laneTargetData4 = default(LaneTargetData);
			NativeParallelMultiHashMapIterator<PathNode> val15 = default(NativeParallelMultiHashMapIterator<PathNode>);
			Edge edge3 = default(Edge);
			Edge edge4 = default(Edge);
			for (int num6 = 0; num6 < nativeArray4.Length; num6++)
			{
				Entity val10 = nativeArray[num6];
				TrackLane trackLane = nativeArray4[num6];
				Lane lane4 = nativeArray2[num6];
				trackLane.m_Flags &= ~(TrackLaneFlags.Switch | TrackLaneFlags.DiamondCrossing | TrackLaneFlags.CrossingTraffic | TrackLaneFlags.MergingTraffic | TrackLaneFlags.DoubleSwitch);
				trackLane.m_Flags |= TrackLaneFlags.StartingLane | TrackLaneFlags.EndingLane;
				if (bufferAccessor2.Length != 0)
				{
					DynamicBuffer<LaneOverlap> val11 = bufferAccessor2[num6];
					OverlapFlags overlapFlags = (OverlapFlags)0;
					for (int num7 = 0; num7 < val11.Length; num7++)
					{
						LaneOverlap laneOverlap3 = val11[num7];
						if ((laneOverlap3.m_Flags & OverlapFlags.Track) != 0)
						{
							OverlapFlags overlapFlags2 = laneOverlap3.m_Flags & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd);
							overlapFlags |= overlapFlags2;
							if (overlapFlags2 == (OverlapFlags)0)
							{
								trackLane.m_Flags |= TrackLaneFlags.DiamondCrossing;
							}
							else if (overlapFlags == (OverlapFlags.MergeStart | OverlapFlags.MergeEnd))
							{
								trackLane.m_Flags |= TrackLaneFlags.Switch | TrackLaneFlags.DoubleSwitch;
							}
							else
							{
								trackLane.m_Flags |= TrackLaneFlags.Switch;
							}
						}
						else if (nativeArray6.Length == 0)
						{
							if ((laneOverlap3.m_Flags & OverlapFlags.MergeEnd) != 0)
							{
								trackLane.m_Flags |= TrackLaneFlags.MergingTraffic;
							}
							else if ((laneOverlap3.m_Flags & OverlapFlags.MergeStart) == 0)
							{
								trackLane.m_Flags |= TrackLaneFlags.CrossingTraffic;
							}
						}
					}
				}
				if (!lane4.m_StartNode.OwnerEquals(lane4.m_MiddleNode))
				{
					trackLane.m_Flags &= ~TrackLaneFlags.StartingLane;
				}
				if (!lane4.m_EndNode.OwnerEquals(lane4.m_MiddleNode))
				{
					trackLane.m_Flags &= ~TrackLaneFlags.EndingLane;
				}
				if ((trackLane.m_Flags & TrackLaneFlags.StartingLane) != 0 && m_SourceMap.TryGetFirstValue(lane4.m_StartNode, ref laneSourceData2, ref val12))
				{
					do
					{
						if (laneSourceData2.m_Entity != val10 && laneSourceData2.m_IsTrack)
						{
							trackLane.m_Flags &= ~TrackLaneFlags.StartingLane;
							break;
						}
					}
					while (m_SourceMap.TryGetNextValue(ref laneSourceData2, ref val12));
				}
				if ((trackLane.m_Flags & TrackLaneFlags.StartingLane) != 0 && m_TargetMap.TryGetFirstValue(lane4.m_StartNode, ref laneTargetData3, ref val13))
				{
					do
					{
						if (laneTargetData3.m_Entity != val10 && laneTargetData3.m_IsTrack)
						{
							trackLane.m_Flags &= ~TrackLaneFlags.StartingLane;
							break;
						}
					}
					while (m_TargetMap.TryGetNextValue(ref laneTargetData3, ref val13));
				}
				if ((trackLane.m_Flags & TrackLaneFlags.EndingLane) != 0 && m_SourceMap.TryGetFirstValue(lane4.m_EndNode, ref laneSourceData3, ref val14))
				{
					do
					{
						if (laneSourceData3.m_Entity != val10 && laneSourceData3.m_IsTrack)
						{
							trackLane.m_Flags &= ~TrackLaneFlags.EndingLane;
							break;
						}
					}
					while (m_SourceMap.TryGetNextValue(ref laneSourceData3, ref val14));
				}
				if ((trackLane.m_Flags & TrackLaneFlags.EndingLane) != 0 && m_TargetMap.TryGetFirstValue(lane4.m_EndNode, ref laneTargetData4, ref val15))
				{
					do
					{
						if (laneTargetData4.m_Entity != val10 && laneTargetData4.m_IsTrack)
						{
							trackLane.m_Flags &= ~TrackLaneFlags.EndingLane;
							break;
						}
					}
					while (m_TargetMap.TryGetNextValue(ref laneTargetData4, ref val15));
				}
				if (nativeArray6.Length != 0 && nativeArray5.Length != 0 && (trackLane.m_Flags & (TrackLaneFlags.StartingLane | TrackLaneFlags.EndingLane)) != 0)
				{
					EdgeLane edgeLane3 = nativeArray6[num6];
					if ((trackLane.m_Flags & TrackLaneFlags.StartingLane) != 0 && (edgeLane3.m_EdgeDelta.x == 0f || edgeLane3.m_EdgeDelta.x == 1f) && m_EdgeData.TryGetComponent(nativeArray5[num6].m_Owner, ref edge3) && m_OutsideConnectionData.HasComponent((edgeLane3.m_EdgeDelta.x == 0f) ? edge3.m_Start : edge3.m_End))
					{
						trackLane.m_Flags &= ~TrackLaneFlags.StartingLane;
					}
					if ((trackLane.m_Flags & TrackLaneFlags.EndingLane) != 0 && (edgeLane3.m_EdgeDelta.y == 0f || edgeLane3.m_EdgeDelta.y == 1f) && m_EdgeData.TryGetComponent(nativeArray5[num6].m_Owner, ref edge4) && m_OutsideConnectionData.HasComponent((edgeLane3.m_EdgeDelta.y == 0f) ? edge4.m_Start : edge4.m_End))
					{
						trackLane.m_Flags &= ~TrackLaneFlags.EndingLane;
					}
				}
				nativeArray4[num6] = trackLane;
			}
		}

		private SlaveLaneFlags GetMergeLaneFlags(Entity owner, SlaveLane slaveLane, Lane lane, bool invert)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = false;
			if (m_SubLanes.HasBuffer(owner))
			{
				DynamicBuffer<SubLane> val = m_SubLanes[owner];
				for (int i = slaveLane.m_MinIndex; i <= slaveLane.m_MaxIndex; i++)
				{
					Entity subLane = val[i].m_SubLane;
					Lane other = m_LaneData[subLane];
					if (lane.Equals(other))
					{
						if (flag)
						{
							if (!invert)
							{
								return SlaveLaneFlags.MergingLane | SlaveLaneFlags.MergeLeft;
							}
							return SlaveLaneFlags.MergingLane | SlaveLaneFlags.MergeRight;
						}
						flag2 = true;
					}
					if (!other.m_EndNode.OwnerEquals(other.m_MiddleNode) || !m_TargetMap.ContainsKey(other.m_EndNode))
					{
						continue;
					}
					if (flag2)
					{
						if (!invert)
						{
							return SlaveLaneFlags.MergingLane | SlaveLaneFlags.MergeRight;
						}
						return SlaveLaneFlags.MergingLane | SlaveLaneFlags.MergeLeft;
					}
					flag = true;
				}
			}
			return SlaveLaneFlags.MergingLane;
		}

		private bool HasRoadOverlaps(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_LaneOverlapData.HasBuffer(entity))
			{
				DynamicBuffer<LaneOverlap> val = m_LaneOverlapData[entity];
				for (int i = 0; i < val.Length; i++)
				{
					if ((val[i].m_Flags & (OverlapFlags.Unsafe | OverlapFlags.Road)) == OverlapFlags.Road)
					{
						return true;
					}
				}
			}
			return false;
		}

		private int CalculateConnectedSources(PathNode node, bool carLanes, bool trackLanes)
		{
			int num = 0;
			LaneSourceData laneSourceData = default(LaneSourceData);
			NativeParallelMultiHashMapIterator<PathNode> val = default(NativeParallelMultiHashMapIterator<PathNode>);
			if (m_SourceMap.TryGetFirstValue(node, ref laneSourceData, ref val))
			{
				do
				{
					if ((laneSourceData.m_IsRoad && carLanes) | (laneSourceData.m_IsTrack && trackLanes))
					{
						num++;
					}
				}
				while (m_SourceMap.TryGetNextValue(ref laneSourceData, ref val));
			}
			return num;
		}

		private int CalculateConnectedTargets(PathNode node, bool carLanes, bool trackLanes)
		{
			int num = 0;
			LaneTargetData laneTargetData = default(LaneTargetData);
			NativeParallelMultiHashMapIterator<PathNode> val = default(NativeParallelMultiHashMapIterator<PathNode>);
			if (m_TargetMap.TryGetFirstValue(node, ref laneTargetData, ref val))
			{
				do
				{
					if ((laneTargetData.m_IsRoad && carLanes) | (laneTargetData.m_IsTrack && trackLanes))
					{
						num++;
					}
				}
				while (m_TargetMap.TryGetNextValue(ref laneTargetData, ref val));
			}
			return num;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct LaneSourceData
	{
		public Entity m_Entity;

		public PathNode m_StartNode;

		public SlaveLaneFlags m_SlaveFlags;

		public bool m_IsEdge;

		public bool m_IsRoad;

		public bool m_IsTrack;
	}

	private struct LaneTargetData
	{
		public Entity m_Entity;

		public PathNode m_EndNode;

		public CarLaneFlags m_CarFlags;

		public SlaveLaneFlags m_SlaveFlags;

		public float m_TurnAmount;

		public bool m_IsEdge;

		public bool m_IsMaster;

		public bool m_IsRoad;

		public bool m_IsTrack;
	}

	[BurstCompile]
	private struct CollectLaneDirectionsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> m_CarLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> m_SlaveLaneType;

		[ReadOnly]
		public ComponentTypeHandle<MasterLane> m_MasterLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> m_ConnectionLaneType;

		public ParallelWriter<PathNode, LaneSourceData> m_SourceMap;

		public ParallelWriter<PathNode, LaneTargetData> m_TargetMap;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Lane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Lane>(ref m_LaneType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<EdgeLane>(ref m_EdgeLaneType);
			NativeArray<CarLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarLane>(ref m_CarLaneType);
			if (nativeArray3.Length != 0)
			{
				NativeArray<Curve> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<SlaveLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SlaveLane>(ref m_SlaveLaneType);
				bool isMaster = ((ArchetypeChunk)(ref chunk)).Has<MasterLane>(ref m_MasterLaneType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity entity = nativeArray[i];
					Lane lane = nativeArray2[i];
					Curve curve = nativeArray4[i];
					CarLane carLane = nativeArray3[i];
					if ((carLane.m_Flags & CarLaneFlags.Unsafe) != 0)
					{
						continue;
					}
					SlaveLaneFlags slaveFlags = (SlaveLaneFlags)0;
					SlaveLaneFlags slaveFlags2 = (SlaveLaneFlags)0;
					CarLaneFlags carLaneFlags = ~(CarLaneFlags.Unsafe | CarLaneFlags.UTurnLeft | CarLaneFlags.Invert | CarLaneFlags.SideConnection | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.LevelCrossing | CarLaneFlags.Twoway | CarLaneFlags.IsSecured | CarLaneFlags.Runway | CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.ForbidCombustionEngines | CarLaneFlags.ForbidTransitTraffic | CarLaneFlags.ForbidHeavyTraffic | CarLaneFlags.PublicOnly | CarLaneFlags.Highway | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight | CarLaneFlags.Forward | CarLaneFlags.Approach | CarLaneFlags.Roundabout | CarLaneFlags.RightLimit | CarLaneFlags.LeftLimit | CarLaneFlags.ForbidPassing | CarLaneFlags.RightOfWay | CarLaneFlags.TrafficLights | CarLaneFlags.ParkingLeft | CarLaneFlags.ParkingRight | CarLaneFlags.Forbidden | CarLaneFlags.AllowEnter);
					if (flag)
					{
						if ((carLane.m_Flags & CarLaneFlags.Highway) != 0 && carLane.m_Curviness > (float)Math.PI / 360f)
						{
							carLaneFlags |= CarLaneFlags.ForbidPassing;
						}
					}
					else
					{
						if (nativeArray5.Length != 0)
						{
							SlaveLane slaveLane = nativeArray5[i];
							slaveFlags = slaveLane.m_Flags & (SlaveLaneFlags.OpenEndLeft | SlaveLaneFlags.OpenEndRight);
							slaveFlags2 = slaveLane.m_Flags & (SlaveLaneFlags.OpenStartLeft | SlaveLaneFlags.OpenStartRight);
						}
						carLaneFlags = carLane.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.TurnLeft | CarLaneFlags.TurnRight | CarLaneFlags.LevelCrossing | CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.UTurnRight | CarLaneFlags.GentleTurnLeft | CarLaneFlags.GentleTurnRight | CarLaneFlags.Forward | CarLaneFlags.Roundabout | CarLaneFlags.ForbidPassing | CarLaneFlags.RightOfWay | CarLaneFlags.TrafficLights);
					}
					m_SourceMap.Add(lane.m_EndNode, new LaneSourceData
					{
						m_Entity = entity,
						m_StartNode = lane.m_StartNode,
						m_SlaveFlags = slaveFlags,
						m_IsEdge = flag,
						m_IsRoad = true
					});
					m_TargetMap.Add(lane.m_StartNode, new LaneTargetData
					{
						m_Entity = entity,
						m_EndNode = lane.m_EndNode,
						m_SlaveFlags = slaveFlags2,
						m_CarFlags = carLaneFlags,
						m_TurnAmount = CalculateTurnAmount(curve),
						m_IsEdge = flag,
						m_IsMaster = isMaster,
						m_IsRoad = true
					});
				}
			}
			else if (!((ArchetypeChunk)(ref chunk)).Has<ConnectionLane>(ref m_ConnectionLaneType))
			{
				bool isTrack = ((ArchetypeChunk)(ref chunk)).Has<TrackLane>(ref m_TrackLaneType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity entity2 = nativeArray[j];
					Lane lane2 = nativeArray2[j];
					m_SourceMap.Add(lane2.m_EndNode, new LaneSourceData
					{
						m_Entity = entity2,
						m_StartNode = lane2.m_StartNode,
						m_IsEdge = flag,
						m_IsTrack = isTrack
					});
					m_TargetMap.Add(lane2.m_StartNode, new LaneTargetData
					{
						m_Entity = entity2,
						m_EndNode = lane2.m_EndNode,
						m_IsEdge = flag,
						m_IsTrack = isTrack
					});
				}
			}
		}

		private float CalculateTurnAmount(Curve curve)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.StartTangent(curve.m_Bezier);
			float2 val2 = math.normalizesafe(((float3)(ref val)).xz, default(float2));
			val = MathUtils.EndTangent(curve.m_Bezier);
			float2 val3 = math.normalizesafe(((float3)(ref val)).xz, default(float2));
			float num = math.acos(math.clamp(math.dot(val2, val3), -1f, 1f));
			return math.select(num, 0f - num, math.dot(MathUtils.Right(val2), val3) < 0f);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct OverlapData
	{
		public Entity m_Entity;

		public LaneOverlap m_Overlap;

		public byte m_SharedStartDelta;

		public byte m_SharedEndDelta;
	}

	[BurstCompile]
	private struct ApplyExtraOverlapsJob : IJob
	{
		public ComponentLookup<NodeLane> m_NodeLaneData;

		public BufferLookup<LaneOverlap> m_Overlaps;

		public NativeQueue<OverlapData> m_ExtraOverlaps;

		public void Execute()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			OverlapData overlapData = default(OverlapData);
			while (m_ExtraOverlaps.TryDequeue(ref overlapData))
			{
				if (overlapData.m_Overlap.m_Other != Entity.Null)
				{
					m_Overlaps[overlapData.m_Entity].Add(overlapData.m_Overlap);
				}
				if (overlapData.m_SharedStartDelta != 0 || overlapData.m_SharedEndDelta != 0)
				{
					ref NodeLane valueRW = ref m_NodeLaneData.GetRefRW(overlapData.m_Entity).ValueRW;
					valueRW.m_SharedStartCount = (byte)math.min(254, valueRW.m_SharedStartCount + overlapData.m_SharedStartDelta);
					valueRW.m_SharedEndCount = (byte)math.min(254, valueRW.m_SharedEndCount + overlapData.m_SharedEndDelta);
				}
			}
		}
	}

	[BurstCompile]
	private struct SortLaneOverlapsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LaneOverlap> m_Overlaps;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			DynamicBuffer<SubLane> val2 = m_SubLanes[val];
			DynamicBuffer<LaneOverlap> val3 = default(DynamicBuffer<LaneOverlap>);
			for (int i = 0; i < val2.Length; i++)
			{
				Entity subLane = val2[i].m_SubLane;
				if (m_Overlaps.TryGetBuffer(subLane, ref val3) && !m_SecondaryLaneData.HasComponent(subLane))
				{
					NativeSortExtension.Sort<LaneOverlap>(val3.AsNativeArray());
				}
			}
		}
	}

	[BurstCompile]
	private struct UpdateLaneOverlapsJob : IJobParallelForDefer
	{
		private struct FindOverlapStackItem
		{
			public Bezier4x2 m_Curve1;

			public Bezier4x2 m_Curve2;

			public float2 m_CurvePos1;

			public float2 m_CurvePos2;

			public int2 m_Iterations;
		}

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<NodeLane> m_NodeLaneData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<LaneOverlap> m_Overlaps;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public bool m_UpdateAll;

		public ParallelWriter<OverlapData> m_ExtraOverlaps;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			Edge edge = default(Edge);
			bool isEdge = m_EdgeData.TryGetComponent(val, ref edge);
			UpdateOverlaps(val, m_SubLanes[val], edge, isEdge);
		}

		private void UpdateOverlaps(Entity entity, DynamicBuffer<SubLane> lanes, Edge edge, bool isEdge)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LaneOverlap> overlaps = default(DynamicBuffer<LaneOverlap>);
			ParkingLane parkingLane = default(ParkingLane);
			ParkingLaneData parkingLaneData = default(ParkingLaneData);
			TrackLaneData trackLaneData = default(TrackLaneData);
			NodeLane nodeLane = default(NodeLane);
			DynamicBuffer<SubLane> lanes2 = default(DynamicBuffer<SubLane>);
			DynamicBuffer<SubLane> lanes3 = default(DynamicBuffer<SubLane>);
			for (int i = 0; i < lanes.Length; i++)
			{
				Entity subLane = lanes[i].m_SubLane;
				if (!m_Overlaps.TryGetBuffer(subLane, ref overlaps) || m_SecondaryLaneData.HasComponent(subLane))
				{
					continue;
				}
				PrefabRef prefabRef = m_PrefabRefData[subLane];
				Curve curve = m_CurveData[subLane];
				Lane laneData = m_LaneData[subLane];
				NetLaneData prefabLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
				if ((prefabLaneData.m_Flags & (LaneFlags.Parking | LaneFlags.Virtual)) == (LaneFlags.Parking | LaneFlags.Virtual))
				{
					continue;
				}
				CarLane carLane = default(CarLane);
				if ((prefabLaneData.m_Flags & LaneFlags.Road) != 0)
				{
					carLane = m_CarLaneData[subLane];
				}
				PedestrianLane pedestrianLane = default(PedestrianLane);
				if ((prefabLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					pedestrianLane = m_PedestrianLaneData[subLane];
				}
				float angle = 0f;
				if ((prefabLaneData.m_Flags & LaneFlags.Parking) != 0 && m_ParkingLaneData.TryGetComponent(subLane, ref parkingLane) && m_PrefabParkingLaneData.TryGetComponent(prefabRef.m_Prefab, ref parkingLaneData) && parkingLaneData.m_SlotAngle > 0.25f)
				{
					angle = math.select((float)Math.PI / 2f - parkingLaneData.m_SlotAngle, parkingLaneData.m_SlotAngle - (float)Math.PI / 2f, (parkingLane.m_Flags & ParkingLaneFlags.ParkingLeft) != 0);
				}
				bool isTrain = false;
				if ((prefabLaneData.m_Flags & LaneFlags.Track) != 0 && m_PrefabTrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
				{
					isTrain = (trackLaneData.m_TrackTypes & (TrackTypes.Train | TrackTypes.Subway)) != 0;
				}
				bool flag = m_NodeLaneData.TryGetComponent(subLane, ref nodeLane);
				overlaps.Clear();
				nodeLane.m_SharedStartCount = 0;
				nodeLane.m_SharedEndCount = 0;
				CheckOverlaps(subLane, laneData, curve, carLane, pedestrianLane, overlaps, angle, flag, isEdge, differentOwner: false, isTrain, ref nodeLane, prefabLaneData, lanes, i);
				if (isEdge && !flag)
				{
					if ((laneData.m_StartNode.OwnerEquals(new PathNode(edge.m_Start, 0)) || laneData.m_EndNode.OwnerEquals(new PathNode(edge.m_Start, 0))) && (m_UpdateAll || m_UpdatedData.HasComponent(edge.m_Start)) && m_SubLanes.TryGetBuffer(edge.m_Start, ref lanes2))
					{
						CheckOverlaps(subLane, laneData, curve, carLane, pedestrianLane, overlaps, angle, isNodeLane1: false, isEdge: true, differentOwner: true, isTrain, ref nodeLane, prefabLaneData, lanes2, lanes2.Length);
					}
					if ((laneData.m_StartNode.OwnerEquals(new PathNode(edge.m_End, 0)) || laneData.m_EndNode.OwnerEquals(new PathNode(edge.m_End, 0))) && (m_UpdateAll || m_UpdatedData.HasComponent(edge.m_End)) && m_SubLanes.TryGetBuffer(edge.m_End, ref lanes3))
					{
						CheckOverlaps(subLane, laneData, curve, carLane, pedestrianLane, overlaps, angle, isNodeLane1: false, isEdge: true, differentOwner: true, isTrain, ref nodeLane, prefabLaneData, lanes3, lanes3.Length);
					}
				}
				if (flag)
				{
					m_NodeLaneData[subLane] = nodeLane;
				}
			}
		}

		private void CheckOverlaps(Entity lane1, Lane laneData1, Curve curve1, CarLane carLane1, PedestrianLane pedestrianLane1, DynamicBuffer<LaneOverlap> overlaps1, float angle1, bool isNodeLane1, bool isEdge, bool differentOwner, bool isTrain1, ref NodeLane nodeLane1, NetLaneData prefabLaneData1, DynamicBuffer<SubLane> lanes, int count)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0750: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_082e: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0849: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0803: Unknown result type (might be due to invalid IL or missing references)
			//IL_0808: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0870: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_0890: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0902: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0910: Unknown result type (might be due to invalid IL or missing references)
			//IL_0913: Unknown result type (might be due to invalid IL or missing references)
			//IL_0918: Unknown result type (might be due to invalid IL or missing references)
			//IL_091f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0954: Unknown result type (might be due to invalid IL or missing references)
			//IL_0956: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0960: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0984: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
			ParkingLane parkingLane = default(ParkingLane);
			ParkingLaneData parkingLaneData = default(ParkingLaneData);
			TrackLaneData trackLaneData = default(TrackLaneData);
			NodeLane nodeLane2 = default(NodeLane);
			float2 val5 = default(float2);
			float2 val15 = default(float2);
			for (int i = 0; i < count; i++)
			{
				Entity subLane = lanes[i].m_SubLane;
				DynamicBuffer<LaneOverlap> val = default(DynamicBuffer<LaneOverlap>);
				if (differentOwner)
				{
					if (!m_Overlaps.HasBuffer(subLane))
					{
						continue;
					}
				}
				else if (!m_Overlaps.TryGetBuffer(subLane, ref val))
				{
					continue;
				}
				if (m_SecondaryLaneData.HasComponent(subLane))
				{
					continue;
				}
				Curve curve2 = m_CurveData[subLane];
				Lane lane2 = m_LaneData[subLane];
				float3 val2;
				if (laneData1.m_StartNode.Equals(lane2.m_EndNode))
				{
					val2 = MathUtils.StartTangent(curve1.m_Bezier);
					float2 val3 = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
					val2 = MathUtils.EndTangent(curve2.m_Bezier);
					if (math.dot(val3, math.normalizesafe(((float3)(ref val2)).xz, default(float2))) >= 0f)
					{
						continue;
					}
				}
				if (laneData1.m_EndNode.Equals(lane2.m_StartNode))
				{
					val2 = MathUtils.EndTangent(curve1.m_Bezier);
					float2 val4 = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
					val2 = MathUtils.StartTangent(curve2.m_Bezier);
					if (math.dot(val4, math.normalizesafe(((float3)(ref val2)).xz, default(float2))) >= 0f)
					{
						continue;
					}
				}
				PrefabRef prefabRef = m_PrefabRefData[subLane];
				NetLaneData netLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
				if ((netLaneData.m_Flags & LaneFlags.Parking) != 0)
				{
					if ((prefabLaneData1.m_Flags & LaneFlags.Road) == 0 || !isNodeLane1 || (netLaneData.m_Flags & LaneFlags.Virtual) != 0)
					{
						continue;
					}
				}
				else if ((prefabLaneData1.m_Flags & LaneFlags.Parking) != 0 && (netLaneData.m_Flags & LaneFlags.Road) == 0)
				{
					continue;
				}
				CarLane carLane2 = default(CarLane);
				if ((netLaneData.m_Flags & LaneFlags.Road) != 0)
				{
					carLane2 = m_CarLaneData[subLane];
				}
				PedestrianLane pedestrianLane2 = default(PedestrianLane);
				if ((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
				{
					pedestrianLane2 = m_PedestrianLaneData[subLane];
				}
				float angle2 = 0f;
				if ((netLaneData.m_Flags & LaneFlags.Parking) != 0 && m_ParkingLaneData.TryGetComponent(subLane, ref parkingLane) && m_PrefabParkingLaneData.TryGetComponent(prefabRef.m_Prefab, ref parkingLaneData) && parkingLaneData.m_SlotAngle > 0.25f)
				{
					angle2 = math.select((float)Math.PI / 2f - parkingLaneData.m_SlotAngle, parkingLaneData.m_SlotAngle - (float)Math.PI / 2f, (parkingLane.m_Flags & ParkingLaneFlags.ParkingLeft) != 0);
				}
				bool flag = false;
				if ((netLaneData.m_Flags & LaneFlags.Parking) != 0 && m_PrefabTrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
				{
					flag = (trackLaneData.m_TrackTypes & (TrackTypes.Train | TrackTypes.Subway)) != 0;
				}
				bool flag2 = m_NodeLaneData.TryGetComponent(subLane, ref nodeLane2);
				if (!flag2 && ((prefabLaneData1.m_Flags & LaneFlags.Parking) != 0 || (isEdge && !isNodeLane1)))
				{
					continue;
				}
				Bezier4x2 xz = ((Bezier4x3)(ref curve1.m_Bezier)).xz;
				Bezier4x2 xz2 = ((Bezier4x3)(ref curve2.m_Bezier)).xz;
				float2 radius = (prefabLaneData1.m_Width + nodeLane1.m_WidthOffset) * 0.4f;
				float2 radius2 = (netLaneData.m_Width + nodeLane2.m_WidthOffset) * 0.4f;
				if (!FindOverlapRange(xz, xz2, radius, radius2, angle1, angle2, out var t))
				{
					continue;
				}
				float num = CalculateParallelism(xz, xz2, t);
				OverlapFlags overlapFlags = (OverlapFlags)0;
				OverlapFlags overlapFlags2 = (OverlapFlags)0;
				if ((prefabLaneData1.m_Flags & LaneFlags.Road) != 0)
				{
					overlapFlags2 |= OverlapFlags.Road;
				}
				if ((prefabLaneData1.m_Flags & LaneFlags.Track) != 0)
				{
					overlapFlags2 |= OverlapFlags.Track;
				}
				if ((prefabLaneData1.m_Flags & LaneFlags.OnWater) != 0)
				{
					overlapFlags2 |= OverlapFlags.Water;
				}
				if ((netLaneData.m_Flags & LaneFlags.Road) != 0)
				{
					overlapFlags |= OverlapFlags.Road;
				}
				if ((netLaneData.m_Flags & LaneFlags.Track) != 0)
				{
					overlapFlags |= OverlapFlags.Track;
				}
				if ((netLaneData.m_Flags & LaneFlags.OnWater) != 0)
				{
					overlapFlags |= OverlapFlags.Water;
				}
				if (laneData1.m_StartNode.Equals(lane2.m_StartNode))
				{
					overlapFlags |= OverlapFlags.MergeStart;
					overlapFlags2 |= OverlapFlags.MergeStart;
				}
				else if (laneData1.m_StartNode.EqualsIgnoreCurvePos(lane2.m_MiddleNode))
				{
					overlapFlags |= OverlapFlags.MergeStart;
					overlapFlags2 |= OverlapFlags.MergeMiddleStart;
				}
				else if (lane2.m_StartNode.EqualsIgnoreCurvePos(laneData1.m_MiddleNode))
				{
					overlapFlags |= OverlapFlags.MergeMiddleStart;
					overlapFlags2 |= OverlapFlags.MergeStart;
				}
				else if (laneData1.m_StartNode.Equals(lane2.m_EndNode))
				{
					overlapFlags |= OverlapFlags.MergeStart | OverlapFlags.MergeFlip;
					overlapFlags2 |= OverlapFlags.MergeEnd | OverlapFlags.MergeFlip;
				}
				if (laneData1.m_EndNode.Equals(lane2.m_EndNode))
				{
					overlapFlags |= OverlapFlags.MergeEnd;
					overlapFlags2 |= OverlapFlags.MergeEnd;
				}
				else if (laneData1.m_EndNode.EqualsIgnoreCurvePos(lane2.m_MiddleNode))
				{
					overlapFlags |= OverlapFlags.MergeEnd;
					overlapFlags2 |= OverlapFlags.MergeMiddleEnd;
				}
				else if (lane2.m_EndNode.EqualsIgnoreCurvePos(laneData1.m_MiddleNode))
				{
					overlapFlags |= OverlapFlags.MergeMiddleEnd;
					overlapFlags2 |= OverlapFlags.MergeEnd;
				}
				else if (laneData1.m_EndNode.Equals(lane2.m_StartNode))
				{
					overlapFlags |= OverlapFlags.MergeEnd | OverlapFlags.MergeFlip;
					overlapFlags2 |= OverlapFlags.MergeStart | OverlapFlags.MergeFlip;
				}
				if ((carLane1.m_Flags & CarLaneFlags.Unsafe) != 0 || (pedestrianLane1.m_Flags & PedestrianLaneFlags.Unsafe) != 0)
				{
					overlapFlags2 |= OverlapFlags.Unsafe;
				}
				if ((carLane2.m_Flags & CarLaneFlags.Unsafe) != 0 || (pedestrianLane2.m_Flags & PedestrianLaneFlags.Unsafe) != 0)
				{
					overlapFlags |= OverlapFlags.Unsafe;
				}
				if ((carLane1.m_Flags & CarLaneFlags.Approach) != 0)
				{
					carLane1.m_Flags &= ~(CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.RightOfWay);
				}
				if ((carLane2.m_Flags & CarLaneFlags.Approach) != 0)
				{
					carLane2.m_Flags &= ~(CarLaneFlags.Yield | CarLaneFlags.Stop | CarLaneFlags.RightOfWay);
				}
				LaneFlags laneFlags = prefabLaneData1.m_Flags ^ netLaneData.m_Flags;
				CarLaneFlags carLaneFlags = carLane1.m_Flags ^ carLane2.m_Flags;
				int num2 = 0;
				int num3 = 0;
				if (((overlapFlags | overlapFlags2) & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd)) == 0)
				{
					((float2)(ref val5))._002Ector(curve1.m_Length, curve2.m_Length);
					val5 = math.max(float2.op_Implicit(1f), val5 * (((float4)(ref t)).yw - ((float4)(ref t)).xz));
					val5 *= num / ((float2)(ref val5)).yx;
					overlapFlags |= OverlapFlags.OverlapLeft | OverlapFlags.OverlapRight;
					overlapFlags2 |= OverlapFlags.OverlapLeft | OverlapFlags.OverlapRight;
				}
				else
				{
					val5 = float2.op_Implicit(num);
					if ((carLaneFlags & CarLaneFlags.Unsafe) == 0 && (laneFlags & LaneFlags.Road) == 0)
					{
						if (isNodeLane1 && (overlapFlags & OverlapFlags.MergeStart) != 0)
						{
							nodeLane1.m_SharedStartCount = (byte)math.min(254, nodeLane1.m_SharedStartCount + 1);
						}
						if (flag2 && (overlapFlags2 & OverlapFlags.MergeStart) != 0)
						{
							num2 = 1;
						}
						if (isNodeLane1 && (overlapFlags & OverlapFlags.MergeEnd) != 0)
						{
							nodeLane1.m_SharedEndCount = (byte)math.min(254, nodeLane1.m_SharedEndCount + 1);
						}
						if (flag2 && (overlapFlags2 & OverlapFlags.MergeEnd) != 0)
						{
							num3 = 1;
						}
						if (flag2 && !differentOwner)
						{
							nodeLane2.m_SharedStartCount = (byte)math.min(254, nodeLane2.m_SharedStartCount + num2);
							nodeLane2.m_SharedEndCount = (byte)math.min(254, nodeLane2.m_SharedEndCount + num3);
							m_NodeLaneData[subLane] = nodeLane2;
						}
					}
					float2 xz3;
					if ((overlapFlags & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd)) == OverlapFlags.MergeStart)
					{
						val2 = MathUtils.Tangent(curve1.m_Bezier, t.y);
						xz3 = ((float3)(ref val2)).xz;
						float2 xz4 = ((float3)(ref curve1.m_Bezier.d)).xz;
						val2 = MathUtils.Position(curve1.m_Bezier, math.lerp(t.x, t.y, 0.5f));
						float2 val6 = xz4 - ((float3)(ref val2)).xz;
						float2 val7 = xz3;
						val2 = MathUtils.StartTangent(curve1.m_Bezier);
						xz3 = math.select(val7, val6, math.dot(((float3)(ref val2)).xz, val6) >= 0f);
					}
					else if ((overlapFlags & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd)) == OverlapFlags.MergeEnd)
					{
						val2 = MathUtils.Tangent(curve1.m_Bezier, t.x);
						xz3 = ((float3)(ref val2)).xz;
						val2 = MathUtils.Position(curve1.m_Bezier, math.lerp(t.x, t.y, 0.5f));
						float2 val8 = ((float3)(ref val2)).xz - ((float3)(ref curve1.m_Bezier.a)).xz;
						float2 val9 = xz3;
						val2 = MathUtils.EndTangent(curve1.m_Bezier);
						xz3 = math.select(val9, val8, math.dot(((float3)(ref val2)).xz, val8) >= 0f);
					}
					else
					{
						xz3 = ((float3)(ref curve1.m_Bezier.d)).xz - ((float3)(ref curve1.m_Bezier.a)).xz;
					}
					float2 xz5;
					if ((overlapFlags2 & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd)) == OverlapFlags.MergeStart)
					{
						val2 = MathUtils.Tangent(curve2.m_Bezier, t.w);
						xz5 = ((float3)(ref val2)).xz;
						float2 xz6 = ((float3)(ref curve2.m_Bezier.d)).xz;
						val2 = MathUtils.Position(curve2.m_Bezier, math.lerp(t.z, t.w, 0.5f));
						float2 val10 = xz6 - ((float3)(ref val2)).xz;
						float2 val11 = xz5;
						val2 = MathUtils.StartTangent(curve2.m_Bezier);
						xz5 = math.select(val11, val10, math.dot(((float3)(ref val2)).xz, val10) >= 0f);
					}
					else if ((overlapFlags2 & (OverlapFlags.MergeStart | OverlapFlags.MergeEnd)) == OverlapFlags.MergeEnd)
					{
						val2 = MathUtils.Tangent(curve2.m_Bezier, t.z);
						xz5 = ((float3)(ref val2)).xz;
						val2 = MathUtils.Position(curve2.m_Bezier, math.lerp(t.z, t.w, 0.5f));
						float2 val12 = ((float3)(ref val2)).xz - ((float3)(ref curve2.m_Bezier.a)).xz;
						float2 val13 = xz5;
						val2 = MathUtils.EndTangent(curve2.m_Bezier);
						xz5 = math.select(val13, val12, math.dot(((float3)(ref val2)).xz, val12) >= 0f);
					}
					else
					{
						xz5 = ((float3)(ref curve2.m_Bezier.d)).xz - ((float3)(ref curve2.m_Bezier.a)).xz;
					}
					bool flag3 = math.dot(MathUtils.Right(xz3), xz5) > 0f == ((overlapFlags & OverlapFlags.MergeFlip) == 0);
					if ((overlapFlags & OverlapFlags.MergeStart) != 0)
					{
						t.x = 0f;
						overlapFlags = (OverlapFlags)((uint)overlapFlags | (uint)(flag3 ? 8 : 4));
					}
					if ((overlapFlags2 & OverlapFlags.MergeStart) != 0)
					{
						t.z = 0f;
						overlapFlags2 = (OverlapFlags)((uint)overlapFlags2 | (uint)(flag3 ? 4 : 8));
					}
					if ((overlapFlags & OverlapFlags.MergeEnd) != 0)
					{
						t.y = 1f;
						overlapFlags = (OverlapFlags)((uint)overlapFlags | (uint)(flag3 ? 4 : 8));
					}
					if ((overlapFlags2 & OverlapFlags.MergeEnd) != 0)
					{
						t.w = 1f;
						overlapFlags2 = (OverlapFlags)((uint)overlapFlags2 | (uint)(flag3 ? 8 : 4));
					}
				}
				if ((prefabLaneData1.m_Flags & LaneFlags.Road) != 0 && flag)
				{
					overlapFlags |= OverlapFlags.Slow;
				}
				if ((netLaneData.m_Flags & LaneFlags.Road) != 0 && isTrain1)
				{
					overlapFlags2 |= OverlapFlags.Slow;
				}
				int num4 = 0;
				if (((prefabLaneData1.m_Flags | netLaneData.m_Flags) & LaneFlags.Pedestrian) != 0)
				{
					if ((prefabLaneData1.m_Flags & LaneFlags.Road) != 0 && (pedestrianLane2.m_Flags & PedestrianLaneFlags.Unsafe) == 0)
					{
						num4 = 1;
					}
					else if ((netLaneData.m_Flags & LaneFlags.Road) != 0 && (pedestrianLane1.m_Flags & PedestrianLaneFlags.Unsafe) == 0)
					{
						num4 = -1;
					}
				}
				else if ((overlapFlags & (OverlapFlags.MergeStart | OverlapFlags.MergeMiddleStart)) == 0)
				{
					if ((carLaneFlags & CarLaneFlags.Stop) != 0)
					{
						num4 = math.select(1, -1, (carLane2.m_Flags & CarLaneFlags.Stop) != 0);
					}
					else if ((carLaneFlags & CarLaneFlags.Yield) != 0)
					{
						num4 = math.select(1, -1, (carLane2.m_Flags & CarLaneFlags.Yield) != 0);
					}
					else if ((carLaneFlags & CarLaneFlags.RightOfWay) != 0)
					{
						num4 = math.select(1, -1, (carLane1.m_Flags & CarLaneFlags.RightOfWay) != 0);
					}
					else if ((carLaneFlags & CarLaneFlags.Unsafe) != 0)
					{
						num4 = math.select(1, -1, (carLane2.m_Flags & CarLaneFlags.Unsafe) != 0);
					}
					else
					{
						float2 val14 = math.lerp(((float4)(ref t)).xz, ((float4)(ref t)).yw, 0.5f);
						val2 = MathUtils.Tangent(curve1.m_Bezier, val14.x);
						float2 xz7 = ((float3)(ref val2)).xz;
						val2 = MathUtils.Tangent(curve2.m_Bezier, val14.y);
						float2 xz8 = ((float3)(ref val2)).xz;
						if (m_LeftHandTraffic)
						{
							((float2)(ref val15))._002Ector(math.dot(MathUtils.Right(xz7), xz8), math.dot(MathUtils.Right(xz8), xz7));
						}
						else
						{
							((float2)(ref val15))._002Ector(math.dot(MathUtils.Left(xz7), xz8), math.dot(MathUtils.Left(xz8), xz7));
						}
						num4 = math.csum(math.select(default(int2), new int2(1, -1), val15 > 0f));
					}
				}
				if ((netLaneData.m_Flags & LaneFlags.Parking) == 0)
				{
					overlaps1.Add(new LaneOverlap(subLane, t, overlapFlags, val5.x, num4));
				}
				if (differentOwner)
				{
					if (num2 != 0 || num3 != 0 || (prefabLaneData1.m_Flags & LaneFlags.Parking) == 0)
					{
						OverlapData overlapData = new OverlapData
						{
							m_Entity = subLane,
							m_SharedStartDelta = (byte)num2,
							m_SharedEndDelta = (byte)num3
						};
						if ((prefabLaneData1.m_Flags & LaneFlags.Parking) == 0)
						{
							overlapData.m_Overlap = new LaneOverlap(lane1, ((float4)(ref t)).zwxy, overlapFlags2, val5.y, -num4);
						}
						m_ExtraOverlaps.Enqueue(overlapData);
					}
				}
				else if ((prefabLaneData1.m_Flags & LaneFlags.Parking) == 0)
				{
					val.Add(new LaneOverlap(lane1, ((float4)(ref t)).zwxy, overlapFlags2, val5.y, -num4));
				}
			}
		}

		private static float CalculateParallelism(Bezier4x2 curve1, Bezier4x2 curve2, float4 overlapRange)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.lerp(((float4)(ref overlapRange)).xz, ((float4)(ref overlapRange)).yw, 0.5f);
			float2 val2 = math.normalizesafe(MathUtils.Tangent(curve1, overlapRange.x), default(float2));
			float2 val3 = math.normalizesafe(MathUtils.Tangent(curve1, val.x), default(float2));
			float2 val4 = math.normalizesafe(MathUtils.Tangent(curve1, overlapRange.y), default(float2));
			float2 val5 = math.normalizesafe(MathUtils.Tangent(curve2, overlapRange.z), default(float2));
			float2 val6 = math.normalizesafe(MathUtils.Tangent(curve2, val.y), default(float2));
			float2 val7 = math.normalizesafe(MathUtils.Tangent(curve2, overlapRange.w), default(float2));
			return math.max(0f, (math.dot(val2, val5) + math.dot(val3, val6) + math.dot(val4, val7)) * (1f / 3f));
		}

		private unsafe static bool FindOverlapRange(Bezier4x2 curve1, Bezier4x2 curve2, float2 radius1, float2 radius2, float angle1, float angle2, out float4 t)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ded: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_081a: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_082f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087c: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0891: Unknown result type (might be due to invalid IL or missing references)
			//IL_073a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_070c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0719: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
			//IL_090d: Unknown result type (might be due to invalid IL or missing references)
			//IL_090f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0901: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_093d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_0959: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_080e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0995: Unknown result type (might be due to invalid IL or missing references)
			//IL_099a: Unknown result type (might be due to invalid IL or missing references)
			//IL_099c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			t = new float4(2f, -1f, 2f, -1f);
			FindOverlapStackItem* ptr = stackalloc FindOverlapStackItem[13];
			int num = 0;
			float2 val = default(float2);
			val.x = MathUtils.Length(curve1);
			val.y = MathUtils.Length(curve2);
			val = math.sqrt(val / math.max(float2.op_Implicit(0.1f), math.min(new float2(radius1.x, radius2.x), new float2(radius1.y, radius2.y))));
			float2 val2 = 1f / math.cos(new float2(angle1, angle2));
			float2 val3 = math.tan(new float2(angle1, angle2));
			int2 val4 = default(int2);
			val4.x = Mathf.RoundToInt(val.x);
			val4.y = Mathf.RoundToInt(val.y);
			val4 = math.min(val4, int2.op_Implicit(4));
			System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
			{
				m_Curve1 = curve1,
				m_Curve2 = curve2,
				m_CurvePos1 = new float2(0f, 1f),
				m_CurvePos2 = new float2(0f, 1f),
				m_Iterations = val4
			});
			Quad2 val14 = default(Quad2);
			Quad2 val15 = default(Quad2);
			float2 val16 = default(float2);
			Bezier4x2 curve3 = default(Bezier4x2);
			Bezier4x2 curve4 = default(Bezier4x2);
			Bezier4x2 curve5 = default(Bezier4x2);
			Bezier4x2 curve6 = default(Bezier4x2);
			float3 val17 = default(float3);
			float3 val18 = default(float3);
			Bezier4x2 curve7 = default(Bezier4x2);
			Bezier4x2 curve8 = default(Bezier4x2);
			float3 val19 = default(float3);
			Bezier4x2 curve9 = default(Bezier4x2);
			Bezier4x2 curve10 = default(Bezier4x2);
			float3 val20 = default(float3);
			while (num != 0)
			{
				ref FindOverlapStackItem reference = ref ptr[--num];
				float2 val5 = math.lerp(((float2)(ref radius1)).xx, ((float2)(ref radius1)).yy, reference.m_CurvePos1);
				float2 val6 = math.lerp(((float2)(ref radius2)).xx, ((float2)(ref radius2)).yy, reference.m_CurvePos2);
				Bounds2 val7 = MathUtils.Bounds(reference.m_Curve1);
				Bounds2 val8 = MathUtils.Bounds(reference.m_Curve2);
				float2 val9 = new float2(math.cmax(val5), math.cmax(val6)) * val2;
				float num2 = math.csum(val9);
				if (!(MathUtils.DistanceSquared(val7, val8) < num2 * num2))
				{
					continue;
				}
				val4 = reference.m_Iterations - 1;
				if (math.all(val4 < 0))
				{
					float2 val10 = MathUtils.Right(MathUtils.StartTangent(reference.m_Curve1));
					float2 val11 = MathUtils.Right(MathUtils.EndTangent(reference.m_Curve1));
					float2 val12 = MathUtils.Right(MathUtils.StartTangent(reference.m_Curve2));
					float2 val13 = MathUtils.Right(MathUtils.EndTangent(reference.m_Curve2));
					MathUtils.TryNormalize(ref val10);
					MathUtils.TryNormalize(ref val11);
					MathUtils.TryNormalize(ref val12);
					MathUtils.TryNormalize(ref val13);
					val10 *= val5.x;
					val11 *= val5.y;
					val12 *= val6.x;
					val13 *= val6.y;
					val10 += MathUtils.Left(val10) * val3.x;
					val11 += MathUtils.Left(val11) * val3.x;
					val12 += MathUtils.Left(val12) * val3.y;
					val13 += MathUtils.Left(val13) * val3.y;
					((Quad2)(ref val14))._002Ector(reference.m_Curve1.a + val10, reference.m_Curve1.d + val11, reference.m_Curve1.d - val11, reference.m_Curve1.a - val10);
					((Quad2)(ref val15))._002Ector(reference.m_Curve2.a + val12, reference.m_Curve2.d + val13, reference.m_Curve2.d - val13, reference.m_Curve2.a - val12);
					Segment ab = ((Quad2)(ref val14)).ab;
					Segment dc = ((Quad2)(ref val14)).dc;
					Segment ad = ((Quad2)(ref val14)).ad;
					Segment bc = ((Quad2)(ref val14)).bc;
					Segment ab2 = ((Quad2)(ref val15)).ab;
					Segment dc2 = ((Quad2)(ref val15)).dc;
					Segment ad2 = ((Quad2)(ref val15)).ad;
					Segment bc2 = ((Quad2)(ref val15)).bc;
					val7 = MathUtils.Expand(val7, float2.op_Implicit(val9.x));
					val8 = MathUtils.Expand(val8, float2.op_Implicit(val9.y));
					if (MathUtils.Intersect(MathUtils.Bounds(ab), val8))
					{
						if (MathUtils.Intersect(ab, ab2, ref val16))
						{
							val16 = math.lerp(new float2(reference.m_CurvePos1.x, reference.m_CurvePos2.x), new float2(reference.m_CurvePos1.y, reference.m_CurvePos2.y), val16);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(ab, dc2, ref val16))
						{
							val16 = math.lerp(new float2(reference.m_CurvePos1.x, reference.m_CurvePos2.x), new float2(reference.m_CurvePos1.y, reference.m_CurvePos2.y), val16);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(ab, ad2, ref val16))
						{
							((float2)(ref val16))._002Ector(math.lerp(reference.m_CurvePos1.x, reference.m_CurvePos1.y, val16.x), reference.m_CurvePos2.x);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(ab, bc2, ref val16))
						{
							((float2)(ref val16))._002Ector(math.lerp(reference.m_CurvePos1.x, reference.m_CurvePos1.y, val16.x), reference.m_CurvePos2.y);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
					}
					if (MathUtils.Intersect(MathUtils.Bounds(dc), val8))
					{
						if (MathUtils.Intersect(dc, ab2, ref val16))
						{
							val16 = math.lerp(new float2(reference.m_CurvePos1.x, reference.m_CurvePos2.x), new float2(reference.m_CurvePos1.y, reference.m_CurvePos2.y), val16);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(dc, dc2, ref val16))
						{
							val16 = math.lerp(new float2(reference.m_CurvePos1.x, reference.m_CurvePos2.x), new float2(reference.m_CurvePos1.y, reference.m_CurvePos2.y), val16);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(dc, ad2, ref val16))
						{
							((float2)(ref val16))._002Ector(math.lerp(reference.m_CurvePos1.x, reference.m_CurvePos1.y, val16.x), reference.m_CurvePos2.x);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(dc, bc2, ref val16))
						{
							((float2)(ref val16))._002Ector(math.lerp(reference.m_CurvePos1.x, reference.m_CurvePos1.y, val16.x), reference.m_CurvePos2.y);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
					}
					if (MathUtils.Intersect(MathUtils.Bounds(ad), val8))
					{
						if (MathUtils.Intersect(ad, ab2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.x, math.lerp(reference.m_CurvePos2.x, reference.m_CurvePos2.y, val16.y));
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(ad, dc2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.x, math.lerp(reference.m_CurvePos2.x, reference.m_CurvePos2.y, val16.y));
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(ad, ad2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.x, reference.m_CurvePos2.x);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(ad, bc2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.x, reference.m_CurvePos2.y);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
					}
					if (MathUtils.Intersect(MathUtils.Bounds(bc), val8))
					{
						if (MathUtils.Intersect(bc, ab2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.y, math.lerp(reference.m_CurvePos2.x, reference.m_CurvePos2.y, val16.y));
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(bc, dc2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.y, math.lerp(reference.m_CurvePos2.x, reference.m_CurvePos2.y, val16.y));
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(bc, ad2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.y, reference.m_CurvePos2.x);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
						if (MathUtils.Intersect(bc, bc2, ref val16))
						{
							((float2)(ref val16))._002Ector(reference.m_CurvePos1.y, reference.m_CurvePos2.y);
							((float4)(ref t)).xz = math.min(((float4)(ref t)).xz, val16);
							((float4)(ref t)).yw = math.max(((float4)(ref t)).yw, val16);
							result = true;
						}
					}
				}
				else if (math.all(val4 >= 0))
				{
					MathUtils.Divide(reference.m_Curve1, ref curve3, ref curve4, 0.5f);
					MathUtils.Divide(reference.m_Curve2, ref curve5, ref curve6, 0.5f);
					((float3)(ref val17))._002Ector(reference.m_CurvePos1.x, math.lerp(reference.m_CurvePos1.x, reference.m_CurvePos1.y, 0.5f), reference.m_CurvePos1.y);
					((float3)(ref val18))._002Ector(reference.m_CurvePos2.x, math.lerp(reference.m_CurvePos2.x, reference.m_CurvePos2.y, 0.5f), reference.m_CurvePos2.y);
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve3,
						m_Curve2 = curve5,
						m_CurvePos1 = ((float3)(ref val17)).xy,
						m_CurvePos2 = ((float3)(ref val18)).xy,
						m_Iterations = val4
					});
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve3,
						m_Curve2 = curve6,
						m_CurvePos1 = ((float3)(ref val17)).xy,
						m_CurvePos2 = ((float3)(ref val18)).yz,
						m_Iterations = val4
					});
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve4,
						m_Curve2 = curve5,
						m_CurvePos1 = ((float3)(ref val17)).yz,
						m_CurvePos2 = ((float3)(ref val18)).xy,
						m_Iterations = val4
					});
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve4,
						m_Curve2 = curve6,
						m_CurvePos1 = ((float3)(ref val17)).yz,
						m_CurvePos2 = ((float3)(ref val18)).yz,
						m_Iterations = val4
					});
				}
				else if (val4.x >= 0)
				{
					MathUtils.Divide(reference.m_Curve1, ref curve7, ref curve8, 0.5f);
					curve2 = reference.m_Curve2;
					((float3)(ref val19))._002Ector(reference.m_CurvePos1.x, math.lerp(reference.m_CurvePos1.x, reference.m_CurvePos1.y, 0.5f), reference.m_CurvePos1.y);
					float2 curvePos = reference.m_CurvePos2;
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve7,
						m_Curve2 = curve2,
						m_CurvePos1 = ((float3)(ref val19)).xy,
						m_CurvePos2 = curvePos,
						m_Iterations = val4
					});
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve8,
						m_Curve2 = curve2,
						m_CurvePos1 = ((float3)(ref val19)).yz,
						m_CurvePos2 = curvePos,
						m_Iterations = val4
					});
				}
				else
				{
					curve1 = reference.m_Curve1;
					MathUtils.Divide(reference.m_Curve2, ref curve9, ref curve10, 0.5f);
					float2 curvePos2 = reference.m_CurvePos1;
					((float3)(ref val20))._002Ector(reference.m_CurvePos2.x, math.lerp(reference.m_CurvePos2.x, reference.m_CurvePos2.y, 0.5f), reference.m_CurvePos2.y);
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve1,
						m_Curve2 = curve9,
						m_CurvePos1 = curvePos2,
						m_CurvePos2 = ((float3)(ref val20)).xy,
						m_Iterations = val4
					});
					System.Runtime.CompilerServices.Unsafe.Write(ptr + num++, new FindOverlapStackItem
					{
						m_Curve1 = curve1,
						m_Curve2 = curve10,
						m_CurvePos1 = curvePos2,
						m_CurvePos2 = ((float3)(ref val20)).yz,
						m_Iterations = val4
					});
				}
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		public ComponentLookup<NodeLane> __Game_Net_NodeLane_RW_ComponentLookup;

		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RW_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> __Game_Net_EdgeLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> __Game_Net_CarLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> __Game_Net_TrackLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> __Game_Net_SlaveLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MasterLane> __Game_Net_MasterLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		public ComponentTypeHandle<EdgeLane> __Game_Net_EdgeLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NodeLane> __Game_Net_NodeLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarLane> __Game_Net_CarLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TrackLane> __Game_Net_TrackLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<SlaveLane> __Game_Net_SlaveLane_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

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
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PedestrianLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLane>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SecondaryLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Net_NodeLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeLane>(false);
			__Game_Net_LaneOverlap_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeLane>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_CarLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackLane>(true);
			__Game_Net_SlaveLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SlaveLane>(true);
			__Game_Net_MasterLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MasterLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConnectionLane>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_LaneOverlap_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneOverlap>(true);
			__Game_Net_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnection>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Net_EdgeLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeLane>(false);
			__Game_Net_NodeLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeLane>(false);
			__Game_Net_CarLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarLane>(false);
			__Game_Net_TrackLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackLane>(false);
			__Game_Net_SlaveLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SlaveLane>(false);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
		}
	}

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_UpdatedOwnersQuery;

	private EntityQuery m_UpdatedLanesQuery;

	private EntityQuery m_AllOwnersQuery;

	private EntityQuery m_AllLanesQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_UpdatedOwnersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<SubLane>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>()
		});
		m_UpdatedLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<SecondaryLane>()
		});
		m_AllOwnersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<SubLane>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AllLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<SecondaryLane>()
		});
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

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllOwnersQuery : m_UpdatedOwnersQuery);
		EntityQuery val2 = (loaded ? m_AllLanesQuery : m_UpdatedLanesQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			int num = ((EntityQuery)(ref val2)).CalculateEntityCount();
			NativeParallelMultiHashMap<PathNode, LaneSourceData> sourceMap = default(NativeParallelMultiHashMap<PathNode, LaneSourceData>);
			sourceMap._002Ector(num, AllocatorHandle.op_Implicit((Allocator)3));
			NativeParallelMultiHashMap<PathNode, LaneTargetData> targetMap = default(NativeParallelMultiHashMap<PathNode, LaneTargetData>);
			targetMap._002Ector(num, AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val4 = default(JobHandle);
			NativeList<Entity> val3 = ((EntityQuery)(ref val)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
			NativeQueue<OverlapData> extraOverlaps = default(NativeQueue<OverlapData>);
			extraOverlaps._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			if (!loaded)
			{
				val4 = IJobExtensions.Schedule<AddNonUpdatedEdgesJob>(new AddNonUpdatedEdgesJob
				{
					m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Entities = val3
				}, JobHandle.CombineDependencies(val4, ((SystemBase)this).Dependency));
			}
			UpdateLaneOverlapsJob updateLaneOverlapsJob = new UpdateLaneOverlapsJob
			{
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeLaneData = InternalCompilerInterface.GetComponentLookup<NodeLane>(ref __TypeHandle.__Game_Net_NodeLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Overlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Entities = val3.AsDeferredJobArray(),
				m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
				m_UpdateAll = loaded,
				m_ExtraOverlaps = extraOverlaps.AsParallelWriter()
			};
			ApplyExtraOverlapsJob applyExtraOverlapsJob = new ApplyExtraOverlapsJob
			{
				m_NodeLaneData = InternalCompilerInterface.GetComponentLookup<NodeLane>(ref __TypeHandle.__Game_Net_NodeLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Overlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ExtraOverlaps = extraOverlaps
			};
			SortLaneOverlapsJob sortLaneOverlapsJob = new SortLaneOverlapsJob
			{
				m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Overlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Entities = val3.AsDeferredJobArray()
			};
			CollectLaneDirectionsJob collectLaneDirectionsJob = new CollectLaneDirectionsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneType = InternalCompilerInterface.GetComponentTypeHandle<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MasterLaneType = InternalCompilerInterface.GetComponentTypeHandle<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SourceMap = sourceMap.AsParallelWriter(),
				m_TargetMap = targetMap.AsParallelWriter()
			};
			UpdateLaneFlagsJob obj = new UpdateLaneFlagsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MasterLaneType = InternalCompilerInterface.GetComponentTypeHandle<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LaneOverlapType = InternalCompilerInterface.GetBufferTypeHandle<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeLaneType = InternalCompilerInterface.GetComponentTypeHandle<NodeLane>(ref __TypeHandle.__Game_Net_NodeLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneType = InternalCompilerInterface.GetComponentTypeHandle<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneOverlapData = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SourceMap = sourceMap,
				m_TargetMap = targetMap
			};
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<UpdateLaneOverlapsJob, Entity>(updateLaneOverlapsJob, val3, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val4));
			JobHandle val6 = IJobExtensions.Schedule<ApplyExtraOverlapsJob>(applyExtraOverlapsJob, val5);
			JobHandle val7 = IJobParallelForDeferExtensions.Schedule<SortLaneOverlapsJob, Entity>(sortLaneOverlapsJob, val3, 1, val6);
			JobHandle val8 = JobChunkExtensions.ScheduleParallel<CollectLaneDirectionsJob>(collectLaneDirectionsJob, val2, ((SystemBase)this).Dependency);
			JobHandle val9 = JobChunkExtensions.ScheduleParallel<UpdateLaneFlagsJob>(obj, val2, JobHandle.CombineDependencies(val7, val8));
			sourceMap.Dispose(val9);
			targetMap.Dispose(val9);
			val3.Dispose(val7);
			extraOverlaps.Dispose(val6);
			((SystemBase)this).Dependency = val9;
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
	public LaneOverlapSystem()
	{
	}
}

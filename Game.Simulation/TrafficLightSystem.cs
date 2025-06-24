using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
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

namespace Game.Simulation;

[CompilerGenerated]
public class TrafficLightSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateTrafficLightsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> m_SubLaneType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> m_SubObjectType;

		public ComponentTypeHandle<TrafficLights> m_TrafficLightsType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<MoveableBridgeData> m_PrefabMoveableBridgeData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrafficLight> m_TrafficLightData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PointOfInterest> m_PointOfInterestData;

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
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<TrafficLights> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrafficLights>(ref m_TrafficLightsType);
			BufferAccessor<Game.Net.SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubLane>(ref m_SubLaneType);
			BufferAccessor<ConnectedEdge> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
			BufferAccessor<Game.Objects.SubObject> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Objects.SubObject>(ref m_SubObjectType);
			NativeList<Entity> laneSignals = default(NativeList<Entity>);
			laneSignals._002Ector(30, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				TrafficLights trafficLights = nativeArray2[i];
				DynamicBuffer<Game.Net.SubLane> subLanes = bufferAccessor[i];
				DynamicBuffer<Game.Objects.SubObject> subObjects = bufferAccessor3[i];
				if ((trafficLights.m_Flags & TrafficLightFlags.IsSubNode) != 0)
				{
					continue;
				}
				Entity entity = default(Entity);
				MoveableBridgeData moveableBridgeData = default(MoveableBridgeData);
				FillLaneSignals(subLanes, laneSignals);
				if ((trafficLights.m_Flags & TrafficLightFlags.MoveableBridge) != 0)
				{
					FindMoveableBridge(subObjects, out entity, out moveableBridgeData);
					FillLaneSignals(nativeArray[i], bufferAccessor2[i], laneSignals);
				}
				if (UpdateTrafficLightState(laneSignals, moveableBridgeData, ref trafficLights))
				{
					UpdateLaneSignals(laneSignals, trafficLights);
					UpdateTrafficLightObjects(subObjects, trafficLights);
					if (entity != Entity.Null)
					{
						ref PointOfInterest valueRW = ref m_PointOfInterestData.GetRefRW(entity).ValueRW;
						UpdateMoveableBridge(trafficLights, m_TransformData[entity], moveableBridgeData, ref valueRW);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(unfilteredChunkIndex, nativeArray[i]);
					}
				}
				nativeArray2[i] = trafficLights;
				laneSignals.Clear();
			}
			laneSignals.Dispose();
		}

		private void FillLaneSignals(DynamicBuffer<Game.Net.SubLane> subLanes, NativeList<Entity> laneSignals)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				if (m_LaneSignalData.HasComponent(subLane))
				{
					laneSignals.Add(ref subLane);
				}
			}
		}

		private void FillLaneSignals(Entity node, DynamicBuffer<ConnectedEdge> connectedEdges, NativeList<Entity> laneSignals)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubNet> subNets = default(DynamicBuffer<Game.Net.SubNet>);
			for (int i = 0; i < connectedEdges.Length; i++)
			{
				Entity edge = connectedEdges[i].m_Edge;
				if (m_SubNets.TryGetBuffer(edge, ref subNets))
				{
					FillLaneSignals(node, edge, subNets, laneSignals);
				}
			}
		}

		private void FillLaneSignals(Entity node, Entity edge, DynamicBuffer<Game.Net.SubNet> subNets, NativeList<Entity> laneSignals)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			Node node2 = m_NodeData[node];
			Curve curve = m_CurveData[edge];
			float num = math.distancesq(node2.m_Position, curve.m_Bezier.a);
			float num2 = math.distancesq(node2.m_Position, curve.m_Bezier.d);
			bool flag = num <= num2;
			DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (m_NodeData.TryGetComponent(subNet, ref node2))
				{
					float num3 = math.distancesq(node2.m_Position, curve.m_Bezier.a);
					num2 = math.distancesq(node2.m_Position, curve.m_Bezier.d);
					bool flag2 = num3 <= num2;
					if (flag == flag2 && m_SubLanes.TryGetBuffer(subNet, ref subLanes))
					{
						FillLaneSignals(subLanes, laneSignals);
					}
				}
			}
		}

		private bool UpdateTrafficLightState(NativeList<Entity> laneSignals, MoveableBridgeData moveableBridgeData, ref TrafficLights trafficLights)
		{
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			bool canExtend;
			switch (trafficLights.m_State)
			{
			case Game.Net.TrafficLightState.None:
				if (++trafficLights.m_Timer >= 1)
				{
					trafficLights.m_State = Game.Net.TrafficLightState.Beginning;
					trafficLights.m_CurrentSignalGroup = 0;
					trafficLights.m_NextSignalGroup = (byte)GetNextSignalGroup(laneSignals, trafficLights, preferChange: true, out canExtend);
					trafficLights.m_Timer = 0;
					return true;
				}
				break;
			case Game.Net.TrafficLightState.Beginning:
				if (++trafficLights.m_Timer >= 1)
				{
					trafficLights.m_State = Game.Net.TrafficLightState.Ongoing;
					trafficLights.m_CurrentSignalGroup = trafficLights.m_NextSignalGroup;
					trafficLights.m_NextSignalGroup = 0;
					trafficLights.m_Timer = 0;
					return true;
				}
				break;
			case Game.Net.TrafficLightState.Ongoing:
				if (++trafficLights.m_Timer >= 2)
				{
					int num2 = 6;
					if (moveableBridgeData.m_MovingTime != 0f)
					{
						num2 = math.clamp((int)(moveableBridgeData.m_MovingTime * 1.875f + 0.5f), num2, 255);
					}
					bool canExtend2;
					int nextSignalGroup2 = GetNextSignalGroup(laneSignals, trafficLights, trafficLights.m_Timer >= num2, out canExtend2);
					if (nextSignalGroup2 != trafficLights.m_CurrentSignalGroup)
					{
						trafficLights.m_State = (canExtend2 ? Game.Net.TrafficLightState.Extending : Game.Net.TrafficLightState.Ending);
						trafficLights.m_NextSignalGroup = (byte)nextSignalGroup2;
						trafficLights.m_Timer = 0;
						return true;
					}
					return false;
				}
				break;
			case Game.Net.TrafficLightState.Extending:
				if (++trafficLights.m_Timer >= 2)
				{
					bool canExtend4;
					int nextSignalGroup4 = GetNextSignalGroup(laneSignals, trafficLights, preferChange: true, out canExtend4);
					if (nextSignalGroup4 == trafficLights.m_CurrentSignalGroup)
					{
						trafficLights.m_State = Game.Net.TrafficLightState.Beginning;
						trafficLights.m_CurrentSignalGroup = 0;
					}
					else
					{
						trafficLights.m_State = (canExtend4 ? Game.Net.TrafficLightState.Extended : Game.Net.TrafficLightState.Ending);
					}
					trafficLights.m_NextSignalGroup = (byte)nextSignalGroup4;
					trafficLights.m_Timer = 0;
					return true;
				}
				break;
			case Game.Net.TrafficLightState.Extended:
				if (++trafficLights.m_Timer >= 2)
				{
					bool canExtend3;
					int nextSignalGroup3 = GetNextSignalGroup(laneSignals, trafficLights, preferChange: true, out canExtend3);
					if (nextSignalGroup3 == trafficLights.m_CurrentSignalGroup)
					{
						trafficLights.m_State = Game.Net.TrafficLightState.Beginning;
						trafficLights.m_CurrentSignalGroup = 0;
						trafficLights.m_NextSignalGroup = (byte)nextSignalGroup3;
						trafficLights.m_Timer = 0;
						return true;
					}
					if (trafficLights.m_Timer >= 4 || !canExtend3)
					{
						trafficLights.m_State = Game.Net.TrafficLightState.Ending;
						trafficLights.m_NextSignalGroup = (byte)nextSignalGroup3;
						trafficLights.m_Timer = 0;
						return true;
					}
					return false;
				}
				break;
			case Game.Net.TrafficLightState.Ending:
			{
				if (++trafficLights.m_Timer < 2)
				{
					break;
				}
				int nextSignalGroup5 = GetNextSignalGroup(laneSignals, trafficLights, preferChange: true, out canExtend);
				if ((trafficLights.m_Flags & TrafficLightFlags.MoveableBridge) != 0 && !IsEmpty(laneSignals, nextSignalGroup5))
				{
					return false;
				}
				if (nextSignalGroup5 != trafficLights.m_NextSignalGroup)
				{
					if (RequireEnding(laneSignals, nextSignalGroup5))
					{
						trafficLights.m_CurrentSignalGroup = trafficLights.m_NextSignalGroup;
					}
					else
					{
						trafficLights.m_State = Game.Net.TrafficLightState.Changing;
					}
					trafficLights.m_NextSignalGroup = (byte)nextSignalGroup5;
				}
				else
				{
					trafficLights.m_State = Game.Net.TrafficLightState.Changing;
				}
				trafficLights.m_Timer = 0;
				return true;
			}
			case Game.Net.TrafficLightState.Changing:
			{
				int num = 1;
				if (moveableBridgeData.m_MovingTime != 0f && trafficLights.m_CurrentSignalGroup != trafficLights.m_NextSignalGroup)
				{
					num = math.clamp((int)(moveableBridgeData.m_MovingTime * 0.9375f + 0.5f), num, 255);
				}
				if (++trafficLights.m_Timer < num)
				{
					break;
				}
				int nextSignalGroup = GetNextSignalGroup(laneSignals, trafficLights, preferChange: true, out canExtend);
				if (nextSignalGroup != trafficLights.m_NextSignalGroup)
				{
					if (RequireEnding(laneSignals, nextSignalGroup))
					{
						trafficLights.m_CurrentSignalGroup = trafficLights.m_NextSignalGroup;
						trafficLights.m_State = Game.Net.TrafficLightState.Ending;
					}
					else if (moveableBridgeData.m_MovingTime == 0f)
					{
						trafficLights.m_State = Game.Net.TrafficLightState.Beginning;
					}
					else
					{
						trafficLights.m_CurrentSignalGroup = trafficLights.m_NextSignalGroup;
					}
					trafficLights.m_NextSignalGroup = (byte)nextSignalGroup;
				}
				else
				{
					trafficLights.m_State = Game.Net.TrafficLightState.Beginning;
				}
				trafficLights.m_Timer = 0;
				return true;
			}
			}
			ClearPriority(laneSignals);
			return false;
		}

		private bool RequireEnding(NativeList<Entity> laneSignals, int nextSignalGroup)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (nextSignalGroup > 0)
			{
				num |= 1 << nextSignalGroup - 1;
			}
			for (int i = 0; i < laneSignals.Length; i++)
			{
				LaneSignal laneSignal = m_LaneSignalData[laneSignals[i]];
				if (laneSignal.m_Signal == LaneSignalType.Go && (laneSignal.m_GroupMask & num) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private int GetNextSignalGroup(NativeList<Entity> laneSignals, TrafficLights trafficLights, bool preferChange, out bool canExtend)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = math.select(127, 1, (trafficLights.m_Flags & TrafficLightFlags.MoveableBridge) != 0);
			for (int i = 0; i < laneSignals.Length; i++)
			{
				Entity val3 = laneSignals[i];
				LaneSignal laneSignal = m_LaneSignalData[val3];
				int num6 = math.min((int)laneSignal.m_Priority, num5);
				if (num6 > num)
				{
					val = laneSignal.m_Petitioner;
					num = num6;
					num2 = laneSignal.m_GroupMask;
					num3 = math.select(0, (int)laneSignal.m_GroupMask, (laneSignal.m_Flags & LaneSignalFlags.CanExtend) != 0);
				}
				else if (num6 == num)
				{
					num2 |= laneSignal.m_GroupMask;
					num3 |= math.select(0, (int)laneSignal.m_GroupMask, (laneSignal.m_Flags & LaneSignalFlags.CanExtend) != 0);
				}
				else if (num6 < 0)
				{
					num4 |= laneSignal.m_GroupMask;
				}
				if (laneSignal.m_Blocker != Entity.Null)
				{
					val2 = laneSignal.m_Blocker;
				}
				laneSignal.m_Petitioner = Entity.Null;
				laneSignal.m_Priority = laneSignal.m_Default;
				m_LaneSignalData[val3] = laneSignal;
			}
			if (val != val2)
			{
				for (int j = 0; j < laneSignals.Length; j++)
				{
					Entity val4 = laneSignals[j];
					LaneSignal laneSignal2 = m_LaneSignalData[val4];
					if ((num2 & laneSignal2.m_GroupMask) != 0)
					{
						laneSignal2.m_Blocker = Entity.Null;
					}
					else
					{
						laneSignal2.m_Blocker = val;
					}
					m_LaneSignalData[val4] = laneSignal2;
				}
			}
			if (num == 0)
			{
				preferChange = false;
				num2 &= ~num4;
			}
			int num7 = (byte)math.select(trafficLights.m_CurrentSignalGroup + 1, 1, trafficLights.m_CurrentSignalGroup >= trafficLights.m_SignalGroupCount);
			int num8 = math.select(math.max(1, (int)trafficLights.m_CurrentSignalGroup), num7, preferChange);
			int num9 = math.select(trafficLights.m_CurrentSignalGroup - 1, (int)trafficLights.m_CurrentSignalGroup, preferChange);
			canExtend = preferChange && trafficLights.m_CurrentSignalGroup >= 1 && (num3 & (1 << trafficLights.m_CurrentSignalGroup - 1)) != 0;
			for (int k = num8; k <= trafficLights.m_SignalGroupCount; k++)
			{
				if ((num2 & (1 << k - 1)) != 0)
				{
					return k;
				}
			}
			for (int l = 1; l <= num9; l++)
			{
				if ((num2 & (1 << l - 1)) != 0)
				{
					return l;
				}
			}
			return trafficLights.m_CurrentSignalGroup;
		}

		private void UpdateLaneSignals(NativeList<Entity> laneSignals, TrafficLights trafficLights)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < laneSignals.Length; i++)
			{
				Entity val = laneSignals[i];
				LaneSignal laneSignal = m_LaneSignalData[val];
				UpdateLaneSignal(trafficLights, ref laneSignal);
				laneSignal.m_Petitioner = Entity.Null;
				laneSignal.m_Priority = laneSignal.m_Default;
				m_LaneSignalData[val] = laneSignal;
			}
		}

		private bool FindMoveableBridge(DynamicBuffer<Game.Objects.SubObject> subObjects, out Entity entity, out MoveableBridgeData moveableBridgeData)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_PointOfInterestData.HasComponent(subObject))
				{
					PrefabRef prefabRef = m_PrefabRefData[subObject];
					if (m_PrefabMoveableBridgeData.TryGetComponent(prefabRef.m_Prefab, ref moveableBridgeData))
					{
						entity = subObject;
						return true;
					}
				}
			}
			entity = default(Entity);
			moveableBridgeData = default(MoveableBridgeData);
			return false;
		}

		private void UpdateTrafficLightObjects(DynamicBuffer<Game.Objects.SubObject> subObjects, TrafficLights trafficLights)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			TrafficLight trafficLight = default(TrafficLight);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_TrafficLightData.TryGetComponent(subObject, ref trafficLight))
				{
					TrafficLightSystem.UpdateTrafficLightState(trafficLights, ref trafficLight);
					m_TrafficLightData[subObject] = trafficLight;
				}
			}
		}

		private void ClearPriority(NativeList<Entity> laneSignals)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < laneSignals.Length; i++)
			{
				Entity val = laneSignals[i];
				LaneSignal laneSignal = m_LaneSignalData[val];
				laneSignal.m_Petitioner = Entity.Null;
				laneSignal.m_Priority = laneSignal.m_Default;
				m_LaneSignalData[val] = laneSignal;
			}
		}

		private bool IsEmpty(NativeList<Entity> laneSignals, int nextSignalGroup)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			if (nextSignalGroup > 0)
			{
				int num = 1 << nextSignalGroup - 1;
				Entity blocker = Entity.Null;
				DynamicBuffer<LaneObject> val2 = default(DynamicBuffer<LaneObject>);
				LaneReservation laneReservation = default(LaneReservation);
				PrefabRef prefabRef = default(PrefabRef);
				CarLaneData carLaneData = default(CarLaneData);
				for (int i = 0; i < laneSignals.Length; i++)
				{
					Entity val = laneSignals[i];
					if ((m_LaneSignalData[val].m_GroupMask & num) == 0)
					{
						if (m_LaneObjects.TryGetBuffer(val, ref val2) && val2.Length != 0)
						{
							blocker = val2[0].m_LaneObject;
							break;
						}
						if (m_LaneReservationData.TryGetComponent(val, ref laneReservation) && laneReservation.GetPriority() >= 100)
						{
							blocker = laneReservation.m_Blocker;
							break;
						}
						if (m_PrefabRefData.TryGetComponent(val, ref prefabRef) && m_PrefabCarLaneData.TryGetComponent(prefabRef.m_Prefab, ref carLaneData) && (carLaneData.m_RoadTypes & RoadTypes.Watercraft) != RoadTypes.None && CheckNextLane(Entity.Null, val, 0f, 0, out blocker))
						{
							break;
						}
					}
				}
				if (blocker != Entity.Null)
				{
					for (int j = 0; j < laneSignals.Length; j++)
					{
						Entity val3 = laneSignals[j];
						LaneSignal laneSignal = m_LaneSignalData[val3];
						if (laneSignal.m_Blocker == Entity.Null)
						{
							laneSignal.m_Blocker = blocker;
							m_LaneSignalData[val3] = laneSignal;
						}
					}
					return false;
				}
			}
			return true;
		}

		private bool CheckNextLane(Entity prevOwner, Entity lane, float distance, int depth, out Entity blocker)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(lane, ref owner))
			{
				DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
				Edge edge = default(Edge);
				if (m_ConnectedEdges.TryGetBuffer(owner.m_Owner, ref val))
				{
					for (int i = 0; i < val.Length; i++)
					{
						ConnectedEdge connectedEdge = val[i];
						if (!(connectedEdge.m_Edge == prevOwner) && CheckNextLane(owner.m_Owner, connectedEdge.m_Edge, lane, distance, depth, out blocker))
						{
							return true;
						}
					}
				}
				else if (m_EdgeData.TryGetComponent(owner.m_Owner, ref edge) && (edge.m_Start == prevOwner || edge.m_End == prevOwner))
				{
					if (CheckNextLane(owner.m_Owner, (edge.m_End == prevOwner) ? edge.m_Start : edge.m_End, lane, distance, depth, out blocker))
					{
						return true;
					}
					if (CheckNextLane(prevOwner, owner.m_Owner, lane, distance, depth, out blocker))
					{
						return true;
					}
				}
			}
			blocker = Entity.Null;
			return false;
		}

		private bool CheckNextLane(Entity prevOwner, Entity nextOwner, Entity lane, float distance, int depth, out Entity blocker)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			Lane lane2 = default(Lane);
			if (m_SubLanes.TryGetBuffer(nextOwner, ref val) && m_LaneData.TryGetComponent(lane, ref lane2))
			{
				Lane lane3 = default(Lane);
				Curve curve = default(Curve);
				DynamicBuffer<LaneObject> val2 = default(DynamicBuffer<LaneObject>);
				PrefabRef prefabRef = default(PrefabRef);
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (!m_LaneData.TryGetComponent(subLane, ref lane3) || !lane2.m_EndNode.Equals(lane3.m_StartNode) || !m_CurveData.TryGetComponent(subLane, ref curve) || !m_LaneObjects.TryGetBuffer(subLane, ref val2))
					{
						continue;
					}
					for (int j = 0; j < val2.Length; j++)
					{
						LaneObject laneObject = val2[j];
						if (m_PrefabRefData.TryGetComponent(laneObject.m_LaneObject, ref prefabRef) && m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
						{
							float3 val3 = MathUtils.Position(curve.m_Bezier, laneObject.m_CurvePosition.x);
							float3 val4 = MathUtils.Size(objectGeometryData.m_Bounds);
							if (math.distance(val3, curve.m_Bezier.a) + distance < val4.z - val4.x * 0.25f)
							{
								blocker = laneObject.m_LaneObject;
								return true;
							}
						}
					}
					float num = distance + curve.m_Length;
					if (num < 150f && depth < 3 && CheckNextLane(prevOwner, subLane, num, depth + 1, out blocker))
					{
						return true;
					}
				}
			}
			blocker = Entity.Null;
			return false;
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
		public BufferTypeHandle<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		public ComponentTypeHandle<TrafficLights> __Game_Net_TrafficLights_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MoveableBridgeData> __Game_Prefabs_MoveableBridgeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RW_ComponentLookup;

		public ComponentLookup<TrafficLight> __Game_Objects_TrafficLight_RW_ComponentLookup;

		public ComponentLookup<PointOfInterest> __Game_Common_PointOfInterest_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubLane>(true);
			__Game_Net_ConnectedEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Objects.SubObject>(true);
			__Game_Net_TrafficLights_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrafficLights>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_MoveableBridgeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MoveableBridgeData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_LaneSignal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(false);
			__Game_Objects_TrafficLight_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficLight>(false);
			__Game_Common_PointOfInterest_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PointOfInterest>(false);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_TrafficLightQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 4;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TrafficLightQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<TrafficLights>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TrafficLightQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		((EntityQuery)(ref m_TrafficLightQuery)).ResetFilter();
		((EntityQuery)(ref m_TrafficLightQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16)));
		UpdateTrafficLightsJob updateTrafficLightsJob = new UpdateTrafficLightsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightsType = InternalCompilerInterface.GetComponentTypeHandle<TrafficLights>(ref __TypeHandle.__Game_Net_TrafficLights_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMoveableBridgeData = InternalCompilerInterface.GetComponentLookup<MoveableBridgeData>(ref __TypeHandle.__Game_Prefabs_MoveableBridgeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightData = InternalCompilerInterface.GetComponentLookup<TrafficLight>(ref __TypeHandle.__Game_Objects_TrafficLight_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PointOfInterestData = InternalCompilerInterface.GetComponentLookup<PointOfInterest>(ref __TypeHandle.__Game_Common_PointOfInterest_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		updateTrafficLightsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle dependency = JobChunkExtensions.ScheduleParallel<UpdateTrafficLightsJob>(updateTrafficLightsJob, m_TrafficLightQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = dependency;
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
	}

	public static void UpdateLaneSignal(TrafficLights trafficLights, ref LaneSignal laneSignal)
	{
		int num = 0;
		int num2 = 0;
		if (trafficLights.m_CurrentSignalGroup > 0)
		{
			num |= 1 << trafficLights.m_CurrentSignalGroup - 1;
		}
		if (trafficLights.m_NextSignalGroup > 0)
		{
			num2 |= 1 << trafficLights.m_NextSignalGroup - 1;
		}
		switch (trafficLights.m_State)
		{
		case Game.Net.TrafficLightState.Beginning:
			if ((laneSignal.m_GroupMask & num2) != 0)
			{
				if (laneSignal.m_Signal != LaneSignalType.Go)
				{
					laneSignal.m_Signal = LaneSignalType.Yield;
				}
			}
			else
			{
				laneSignal.m_Signal = LaneSignalType.Stop;
			}
			break;
		case Game.Net.TrafficLightState.Ongoing:
			if ((laneSignal.m_GroupMask & num) != 0)
			{
				laneSignal.m_Signal = LaneSignalType.Go;
			}
			else
			{
				laneSignal.m_Signal = LaneSignalType.Stop;
			}
			break;
		case Game.Net.TrafficLightState.Extending:
			if ((laneSignal.m_Flags & LaneSignalFlags.CanExtend) != 0)
			{
				if ((laneSignal.m_GroupMask & num) != 0)
				{
					laneSignal.m_Signal = LaneSignalType.Go;
				}
				else
				{
					laneSignal.m_Signal = LaneSignalType.Stop;
				}
			}
			else if (laneSignal.m_Signal == LaneSignalType.Go)
			{
				if ((laneSignal.m_GroupMask & num2) == 0)
				{
					laneSignal.m_Signal = LaneSignalType.SafeStop;
				}
			}
			else
			{
				laneSignal.m_Signal = LaneSignalType.Stop;
			}
			break;
		case Game.Net.TrafficLightState.Extended:
			if ((laneSignal.m_Flags & LaneSignalFlags.CanExtend) != 0 && (laneSignal.m_GroupMask & num) != 0)
			{
				laneSignal.m_Signal = LaneSignalType.Go;
			}
			else
			{
				laneSignal.m_Signal = LaneSignalType.Stop;
			}
			break;
		case Game.Net.TrafficLightState.Ending:
			if (laneSignal.m_Signal == LaneSignalType.Go)
			{
				if ((laneSignal.m_GroupMask & num2) == 0)
				{
					laneSignal.m_Signal = LaneSignalType.SafeStop;
				}
			}
			else
			{
				laneSignal.m_Signal = LaneSignalType.Stop;
			}
			break;
		case Game.Net.TrafficLightState.Changing:
			if (laneSignal.m_Signal != LaneSignalType.Go || (laneSignal.m_GroupMask & num2) == 0)
			{
				laneSignal.m_Signal = LaneSignalType.Stop;
			}
			break;
		default:
			laneSignal.m_Signal = LaneSignalType.None;
			break;
		}
	}

	public static void UpdateTrafficLightState(TrafficLights trafficLights, ref TrafficLight trafficLight)
	{
		int num = 0;
		int num2 = 0;
		if (trafficLights.m_CurrentSignalGroup > 0)
		{
			num |= 1 << trafficLights.m_CurrentSignalGroup - 1;
		}
		if (trafficLights.m_NextSignalGroup > 0)
		{
			num2 |= 1 << trafficLights.m_NextSignalGroup - 1;
		}
		Game.Objects.TrafficLightState trafficLightState = trafficLight.m_State & (Game.Objects.TrafficLightState.Red | Game.Objects.TrafficLightState.Yellow | Game.Objects.TrafficLightState.Green | Game.Objects.TrafficLightState.Flashing);
		Game.Objects.TrafficLightState trafficLightState2 = (Game.Objects.TrafficLightState)(((int)trafficLight.m_State >> 4) & 0xF);
		Game.Objects.TrafficLightState trafficLightState3 = (((trafficLights.m_Flags & TrafficLightFlags.LevelCrossing) != 0) ? (Game.Objects.TrafficLightState.Yellow | Game.Objects.TrafficLightState.Flashing) : Game.Objects.TrafficLightState.Yellow);
		Game.Objects.TrafficLightState trafficLightState4 = (((trafficLights.m_Flags & TrafficLightFlags.LevelCrossing) == 0) ? Game.Objects.TrafficLightState.Red : (Game.Objects.TrafficLightState.Red | Game.Objects.TrafficLightState.Flashing));
		switch (trafficLights.m_State)
		{
		case Game.Net.TrafficLightState.Beginning:
			if ((trafficLight.m_GroupMask0 & num2) != 0)
			{
				if (trafficLightState != Game.Objects.TrafficLightState.Green)
				{
					trafficLightState = trafficLightState4 | trafficLightState3;
				}
			}
			else
			{
				trafficLightState = trafficLightState4;
			}
			trafficLightState2 = (((trafficLight.m_GroupMask1 & num2) == 0) ? Game.Objects.TrafficLightState.Red : Game.Objects.TrafficLightState.Green);
			break;
		case Game.Net.TrafficLightState.Ongoing:
			trafficLightState = (((trafficLight.m_GroupMask0 & num) == 0) ? trafficLightState4 : Game.Objects.TrafficLightState.Green);
			trafficLightState2 = (((trafficLight.m_GroupMask1 & num) == 0) ? Game.Objects.TrafficLightState.Red : Game.Objects.TrafficLightState.Green);
			break;
		case Game.Net.TrafficLightState.Extending:
			trafficLightState = (((trafficLight.m_GroupMask0 & num) == 0) ? trafficLightState4 : Game.Objects.TrafficLightState.Green);
			if (trafficLightState2 == Game.Objects.TrafficLightState.Green)
			{
				if ((trafficLight.m_GroupMask1 & num2) == 0)
				{
					trafficLightState2 = Game.Objects.TrafficLightState.Green | Game.Objects.TrafficLightState.Flashing;
				}
			}
			else
			{
				trafficLightState2 = Game.Objects.TrafficLightState.Red;
			}
			break;
		case Game.Net.TrafficLightState.Extended:
			trafficLightState = (((trafficLight.m_GroupMask0 & num) == 0) ? trafficLightState4 : Game.Objects.TrafficLightState.Green);
			if (trafficLightState2 != Game.Objects.TrafficLightState.Green || (trafficLight.m_GroupMask1 & num2) == 0)
			{
				trafficLightState2 = Game.Objects.TrafficLightState.Red;
			}
			break;
		case Game.Net.TrafficLightState.Ending:
			if (trafficLightState == Game.Objects.TrafficLightState.Green)
			{
				if ((trafficLight.m_GroupMask0 & num2) == 0)
				{
					trafficLightState = trafficLightState3;
				}
			}
			else
			{
				trafficLightState = trafficLightState4;
			}
			if (trafficLightState2 == Game.Objects.TrafficLightState.Green)
			{
				if ((trafficLight.m_GroupMask1 & num2) == 0)
				{
					trafficLightState2 = Game.Objects.TrafficLightState.Green | Game.Objects.TrafficLightState.Flashing;
				}
			}
			else
			{
				trafficLightState2 = Game.Objects.TrafficLightState.Red;
			}
			break;
		case Game.Net.TrafficLightState.Changing:
			if (trafficLightState != Game.Objects.TrafficLightState.Green || (trafficLight.m_GroupMask0 & num2) == 0)
			{
				trafficLightState = trafficLightState4;
			}
			if (trafficLightState2 != Game.Objects.TrafficLightState.Green || (trafficLight.m_GroupMask1 & num2) == 0)
			{
				trafficLightState2 = Game.Objects.TrafficLightState.Red;
			}
			break;
		default:
			trafficLightState = Game.Objects.TrafficLightState.None;
			trafficLightState2 = Game.Objects.TrafficLightState.None;
			break;
		}
		trafficLight.m_State = (Game.Objects.TrafficLightState)((uint)trafficLightState | ((uint)trafficLightState2 << 4));
	}

	public static void UpdateMoveableBridge(TrafficLights trafficLights, Transform transform, MoveableBridgeData moveableBridgeData, ref PointOfInterest pointOfInterest)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		if (trafficLights.m_State == Game.Net.TrafficLightState.Beginning || trafficLights.m_State == Game.Net.TrafficLightState.Changing)
		{
			if (trafficLights.m_NextSignalGroup > 0)
			{
				num = trafficLights.m_NextSignalGroup - 1;
			}
		}
		else if (trafficLights.m_State != Game.Net.TrafficLightState.Ending && trafficLights.m_CurrentSignalGroup > 0)
		{
			num = trafficLights.m_CurrentSignalGroup - 1;
		}
		pointOfInterest.m_IsValid = false;
		if (num >= 0 && num <= 2)
		{
			pointOfInterest.m_Position = transform.m_Position;
			pointOfInterest.m_Position.y += ((float3)(ref moveableBridgeData.m_LiftOffsets))[num];
			pointOfInterest.m_IsValid = true;
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
	public TrafficLightSystem()
	{
	}
}

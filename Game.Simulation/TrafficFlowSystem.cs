using System.Runtime.CompilerServices;
using Game.Common;
using Game.Net;
using Game.Pathfind;
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

namespace Game.Simulation;

[CompilerGenerated]
public class TrafficFlowSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateLaneFlowJob : IJobChunk
	{
		[ReadOnly]
		public float4 m_TimeFactors;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<LaneFlow> m_LaneFlowType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<LaneFlow> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneFlow>(ref m_LaneFlowType);
			float4 val = m_TimeFactors * 0.125f;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				LaneFlow laneFlow = nativeArray[i];
				laneFlow.m_Duration = math.lerp(laneFlow.m_Duration, float4.op_Implicit(laneFlow.m_Next.x), val);
				laneFlow.m_Distance = math.lerp(laneFlow.m_Distance, float4.op_Implicit(laneFlow.m_Next.y), val);
				laneFlow.m_Next = default(float2);
				nativeArray[i] = laneFlow;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateRoadFlowJob : IJobChunk
	{
		[ReadOnly]
		public float4 m_TimeFactors;

		[ReadOnly]
		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public ComponentLookup<LaneFlow> m_LaneFlowData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		public ComponentTypeHandle<Road> m_RoadType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CarLane> m_CarLaneData;

		public ParallelWriter<FlowActionData> m_FlowActions;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Road> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
			BufferAccessor<SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubLane>(ref m_SubLaneType);
			EdgeLane edgeLane = default(EdgeLane);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Road road = nativeArray[i];
				DynamicBuffer<SubLane> val = bufferAccessor[i];
				road.m_TrafficFlowDuration0 = default(float4);
				road.m_TrafficFlowDuration1 = default(float4);
				road.m_TrafficFlowDistance0 = default(float4);
				road.m_TrafficFlowDistance1 = default(float4);
				int num = -1;
				MasterLane masterLane = default(MasterLane);
				float4 val2 = default(float4);
				float4 val3 = default(float4);
				bool isRoundabout;
				for (int j = 0; j < val.Length; j++)
				{
					Entity subLane = val[j].m_SubLane;
					if (num != -1 && j > masterLane.m_MaxIndex)
					{
						Entity subLane2 = val[num].m_SubLane;
						float4 trafficFlowSpeed = NetUtils.GetTrafficFlowSpeed(val2, val3);
						UpdateLaneFlow(subLane2, trafficFlowSpeed, out isRoundabout);
						num = -1;
					}
					if (m_LaneFlowData.HasComponent(subLane))
					{
						LaneFlow laneFlow = m_LaneFlowData[subLane];
						float4 trafficFlowSpeed2 = NetUtils.GetTrafficFlowSpeed(laneFlow.m_Duration, laneFlow.m_Distance);
						UpdateLaneFlow(subLane, trafficFlowSpeed2, out var isRoundabout2);
						float2 val4 = ((!m_EdgeLaneData.TryGetComponent(subLane, ref edgeLane)) ? float2.op_Implicit(math.select(1f, 1f / 3f, isRoundabout2)) : math.select(float2.op_Implicit(0f), float2.op_Implicit(1f), new bool2(math.any(edgeLane.m_EdgeDelta == 0f), math.any(edgeLane.m_EdgeDelta == 1f))));
						ref float4 trafficFlowDuration = ref road.m_TrafficFlowDuration0;
						trafficFlowDuration += laneFlow.m_Duration * val4.x;
						ref float4 trafficFlowDuration2 = ref road.m_TrafficFlowDuration1;
						trafficFlowDuration2 += laneFlow.m_Duration * val4.y;
						ref float4 trafficFlowDistance = ref road.m_TrafficFlowDistance0;
						trafficFlowDistance += laneFlow.m_Distance * val4.x;
						ref float4 trafficFlowDistance2 = ref road.m_TrafficFlowDistance1;
						trafficFlowDistance2 += laneFlow.m_Distance * val4.y;
						val2 += laneFlow.m_Duration;
						val3 += laneFlow.m_Distance;
					}
					else if (m_MasterLaneData.HasComponent(subLane))
					{
						num = j;
						masterLane = m_MasterLaneData[subLane];
						val2 = default(float4);
						val3 = default(float4);
					}
				}
				if (num != -1)
				{
					Entity subLane3 = val[num].m_SubLane;
					float4 trafficFlowSpeed3 = NetUtils.GetTrafficFlowSpeed(val2, val3);
					UpdateLaneFlow(subLane3, trafficFlowSpeed3, out isRoundabout);
				}
				nativeArray[i] = road;
			}
		}

		private void UpdateLaneFlow(Entity lane, float4 flowSpeed, out bool isRoundabout)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			isRoundabout = false;
			if (m_CarLaneData.HasComponent(lane))
			{
				CarLane carLane = m_CarLaneData[lane];
				isRoundabout = (carLane.m_Flags & (CarLaneFlags.Approach | CarLaneFlags.Roundabout)) == CarLaneFlags.Roundabout;
				byte b = (byte)math.clamp(256 - Mathf.RoundToInt(math.dot(flowSpeed, m_TimeFactors) * 256f), 0, 255);
				if (b != carLane.m_FlowOffset)
				{
					carLane.m_FlowOffset = b;
					m_CarLaneData[lane] = carLane;
					m_FlowActions.Enqueue(new FlowActionData
					{
						m_Owner = lane,
						m_FlowOffset = b
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

		public ComponentTypeHandle<LaneFlow> __Game_Net_LaneFlow_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<LaneFlow> __Game_Net_LaneFlow_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		public ComponentTypeHandle<Road> __Game_Net_Road_RW_ComponentTypeHandle;

		public ComponentLookup<CarLane> __Game_Net_CarLane_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_LaneFlow_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LaneFlow>(false);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(true);
			__Game_Net_LaneFlow_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneFlow>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Net_Road_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(false);
			__Game_Net_CarLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(false);
		}
	}

	public const int UPDATES_PER_DAY = 32;

	private SimulationSystem m_SimulationSystem;

	private PathfindQueueSystem m_PathfindQueueSystem;

	private TimeSystem m_TimeSystem;

	private EntityQuery m_LaneQuery;

	private EntityQuery m_RoadQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 512;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<LaneFlow>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_RoadQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<Road>(),
			ComponentType.ReadOnly<SubLane>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_LaneQuery, m_RoadQuery });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		float num = m_TimeSystem.normalizedTime * 4f;
		float4 val = default(float4);
		((float4)(ref val))._002Ector(math.max(num - 3f, 1f - num), 1f - math.abs(num - new float3(1f, 2f, 3f)));
		val = math.saturate(val);
		FlowAction action = new FlowAction((Allocator)4);
		((EntityQuery)(ref m_LaneQuery)).ResetFilter();
		((EntityQuery)(ref m_LaneQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, 32, 16)));
		((EntityQuery)(ref m_RoadQuery)).ResetFilter();
		((EntityQuery)(ref m_RoadQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, 32, 16)));
		UpdateLaneFlowJob updateLaneFlowJob = new UpdateLaneFlowJob
		{
			m_TimeFactors = val,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneFlowType = InternalCompilerInterface.GetComponentTypeHandle<LaneFlow>(ref __TypeHandle.__Game_Net_LaneFlow_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		UpdateRoadFlowJob obj = new UpdateRoadFlowJob
		{
			m_TimeFactors = val,
			m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneFlowData = InternalCompilerInterface.GetComponentLookup<LaneFlow>(ref __TypeHandle.__Game_Net_LaneFlow_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowActions = action.m_FlowData.AsParallelWriter()
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateLaneFlowJob>(updateLaneFlowJob, m_LaneQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateRoadFlowJob>(obj, m_RoadQuery, val2);
		m_PathfindQueueSystem.Enqueue(action, val3);
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
	public TrafficFlowSystem()
	{
	}
}

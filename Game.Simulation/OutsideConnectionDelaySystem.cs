using System.Runtime.CompilerServices;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
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
public class OutsideConnectionDelaySystem : GameSystemBase
{
	private struct AccumulationData
	{
		public Entity m_Lane;

		public float2 m_Delay;
	}

	[BurstCompile]
	private struct OutsideConnectionDelayJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> m_SubLaneType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> m_PrefabPathfindConnectionData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Net.OutsideConnection> m_OutsideConnectionData;

		public ParallelWriter<TimeActionData> m_TimeActions;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Game.Net.SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubLane>(ref m_SubLaneType);
			BufferAccessor<ConnectedEdge> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
			NativeParallelHashMap<PathNode, AccumulationData> val = default(NativeParallelHashMap<PathNode, AccumulationData>);
			SlaveLane slaveLane = default(SlaveLane);
			AccumulationData accumulationData = default(AccumulationData);
			Game.Net.OutsideConnection outsideConnection = default(Game.Net.OutsideConnection);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<Game.Net.SubLane> val2 = bufferAccessor[i];
				bool flag = false;
				for (int j = 0; j < val2.Length; j++)
				{
					Game.Net.SubLane subLane = val2[j];
					if ((subLane.m_PathMethods & PathMethod.Road) == 0 || !m_ConnectionLaneData.HasComponent(subLane.m_SubLane))
					{
						continue;
					}
					Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[subLane.m_SubLane];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Start) != 0 && (connectionLane.m_RoadTypes & RoadTypes.Car) != RoadTypes.None)
					{
						Lane lane = m_LaneData[subLane.m_SubLane];
						if (!val.IsCreated)
						{
							val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
						}
						val.TryAdd(lane.m_StartNode, new AccumulationData
						{
							m_Lane = subLane.m_SubLane
						});
						flag = true;
					}
				}
				if (!flag)
				{
					continue;
				}
				DynamicBuffer<ConnectedEdge> val3 = bufferAccessor2[i];
				for (int k = 0; k < val3.Length; k++)
				{
					ConnectedEdge connectedEdge = val3[k];
					DynamicBuffer<Game.Net.SubLane> val4 = m_SubLanes[connectedEdge.m_Edge];
					for (int l = 0; l < val4.Length; l++)
					{
						Game.Net.SubLane subLane2 = val4[l];
						if ((subLane2.m_PathMethods & PathMethod.Road) != 0)
						{
							Lane lane2;
							if (m_SlaveLaneData.TryGetComponent(subLane2.m_SubLane, ref slaveLane))
							{
								Game.Net.SubLane subLane3 = val4[(int)slaveLane.m_MasterIndex];
								lane2 = m_LaneData[subLane3.m_SubLane];
							}
							else
							{
								lane2 = m_LaneData[subLane2.m_SubLane];
							}
							if (val.TryGetValue(lane2.m_StartNode, ref accumulationData))
							{
								accumulationData.m_Delay.x += CalculateDelay(subLane2.m_SubLane);
								accumulationData.m_Delay.y += 1f;
								val[lane2.m_StartNode] = accumulationData;
							}
						}
					}
				}
				Enumerator<PathNode, AccumulationData> enumerator = val.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AccumulationData value = enumerator.Current.Value;
					if (m_OutsideConnectionData.TryGetComponent(value.m_Lane, ref outsideConnection))
					{
						outsideConnection.m_Delay = math.select(value.m_Delay.x / value.m_Delay.y, 0f, value.m_Delay.y == 0f);
						m_OutsideConnectionData[value.m_Lane] = outsideConnection;
						Lane lane3 = m_LaneData[value.m_Lane];
						PrefabRef prefabRef = m_PrefabRefData[value.m_Lane];
						NetLaneData netLaneData = m_PrefabNetLaneData[prefabRef.m_Prefab];
						PathfindConnectionData pathfindConnectionData = m_PrefabPathfindConnectionData[netLaneData.m_PathfindPrefab];
						TimeActionData timeActionData = new TimeActionData
						{
							m_Owner = value.m_Lane,
							m_StartNode = lane3.m_StartNode,
							m_EndNode = lane3.m_EndNode,
							m_SecondaryStartNode = lane3.m_StartNode,
							m_SecondaryEndNode = lane3.m_EndNode,
							m_Flags = (TimeActionFlags.SetPrimary | TimeActionFlags.SetSecondary | TimeActionFlags.EnableForward | TimeActionFlags.EnableBackward),
							m_Time = pathfindConnectionData.m_BorderCost.m_Value.x + outsideConnection.m_Delay
						};
						m_TimeActions.Enqueue(timeActionData);
					}
				}
				enumerator.Dispose();
				val.Clear();
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
		}

		private float CalculateDelay(Entity lane)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
			if (m_LaneObjects.TryGetBuffer(lane, ref val))
			{
				CarCurrentLane carCurrentLane = default(CarCurrentLane);
				for (int i = 0; i < val.Length; i++)
				{
					LaneObject laneObject = val[i];
					if (m_CarCurrentLaneData.TryGetComponent(laneObject.m_LaneObject, ref carCurrentLane) && (carCurrentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ResetSpeed) != 0)
					{
						num += carCurrentLane.m_Duration;
					}
				}
			}
			return num;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> __Game_Prefabs_PathfindConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		public ComponentLookup<Game.Net.OutsideConnection> __Game_Net_OutsideConnection_RW_ComponentLookup;

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
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubLane>(true);
			__Game_Net_ConnectedEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindConnectionData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_OutsideConnection_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.OutsideConnection>(false);
		}
	}

	public const int UPDATES_PER_DAY = 64;

	private PathfindQueueSystem m_PathfindQueueSystem;

	private EntityQuery m_NodeQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 4096;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_NodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Game.Net.OutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_NodeQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		TimeAction action = new TimeAction((Allocator)4);
		JobHandle val = JobChunkExtensions.ScheduleParallel<OutsideConnectionDelayJob>(new OutsideConnectionDelayJob
		{
			m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPathfindConnectionData = InternalCompilerInterface.GetComponentLookup<PathfindConnectionData>(ref __TypeHandle.__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TimeActions = action.m_TimeData.AsParallelWriter()
		}, m_NodeQuery, ((SystemBase)this).Dependency);
		m_PathfindQueueSystem.Enqueue(action, val);
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
	public OutsideConnectionDelaySystem()
	{
	}
}

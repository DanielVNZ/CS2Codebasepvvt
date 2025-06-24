using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Serialization;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class PathfindResultSystem : GameSystemBase, IPreDeserialize
{
	public enum QueryType
	{
		Pathfind,
		Coverage,
		Availability
	}

	public struct ResultKey : IEquatable<ResultKey>
	{
		public object m_System;

		public QueryType m_QueryType;

		public SetupTargetType m_OriginType;

		public SetupTargetType m_DestinationType;

		public bool Equals(ResultKey other)
		{
			return (m_System == other.m_System) & (m_QueryType == other.m_QueryType) & (m_OriginType == other.m_OriginType) & (m_DestinationType == other.m_DestinationType);
		}

		public override int GetHashCode()
		{
			return ((m_System.GetHashCode() * 31 + m_QueryType.GetHashCode()) * 31 + m_OriginType.GetHashCode()) * 31 + m_DestinationType.GetHashCode();
		}
	}

	public struct ResultValue
	{
		public int m_QueryCount;

		public int m_SuccessCount;

		public float m_GraphTraversal;

		public float m_Efficiency;
	}

	private struct TypeHandle
	{
		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentLookup;

		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RW_ComponentLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		public BufferLookup<PathInformations> __Game_Pathfind_PathInformations_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		public BufferLookup<CoverageElement> __Game_Pathfind_CoverageElement_RW_BufferLookup;

		public BufferLookup<AvailabilityElement> __Game_Pathfind_AvailabilityElement_RW_BufferLookup;

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
			__Game_Pathfind_PathOwner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(false);
			__Game_Pathfind_PathInformation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			__Game_Pathfind_PathInformations_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathInformations>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Pathfind_CoverageElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CoverageElement>(false);
			__Game_Pathfind_AvailabilityElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AvailabilityElement>(false);
		}
	}

	private PathfindQueueSystem m_PathfindQueueSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityCommandBuffer m_CommandBuffer;

	private EntityArchetype m_PathEventArchetype;

	private EntityArchetype m_CoverageEventArchetype;

	private uint m_PendingSimulationFrameIndex;

	private int m_PendingRequestCount;

	private Dictionary<Entity, int> m_ResultListIndex;

	private Dictionary<ResultKey, ResultValue> m_QueryStats;

	private NativeList<PathfindJobs.ResultItem> m_PathfindResultBuffer;

	private NativeList<CoverageJobs.ResultItem> m_CoverageResultBuffer;

	private NativeList<AvailabilityJobs.ResultItem> m_AvailabilityResultBuffer;

	private TypeHandle __TypeHandle;

	public uint pendingSimulationFrame => math.min(m_PendingSimulationFrameIndex, m_PathfindSetupSystem.pendingSimulationFrame);

	public int pendingRequestCount => m_PendingRequestCount + m_PathfindSetupSystem.pendingRequestCount;

	public Dictionary<ResultKey, ResultValue> queryStats => m_QueryStats;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PathEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<PathUpdated>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_CoverageEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<CoverageUpdated>()
		});
		m_ResultListIndex = new Dictionary<Entity, int>(10);
		m_QueryStats = new Dictionary<ResultKey, ResultValue>(10);
		m_PathfindResultBuffer = new NativeList<PathfindJobs.ResultItem>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_CoverageResultBuffer = new NativeList<CoverageJobs.ResultItem>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_AvailabilityResultBuffer = new NativeList<AvailabilityJobs.ResultItem>(10, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_PathfindResultBuffer.Dispose();
		m_CoverageResultBuffer.Dispose();
		m_AvailabilityResultBuffer.Dispose();
		base.OnDestroy();
	}

	public void PreDeserialize(Context context)
	{
		m_PendingSimulationFrameIndex = uint.MaxValue;
		m_PendingRequestCount = 0;
		m_QueryStats.Clear();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		JobHandle outputDeps = ((SystemBase)this).Dependency;
		m_PendingSimulationFrameIndex = uint.MaxValue;
		m_PendingRequestCount = 0;
		m_CommandBuffer = default(EntityCommandBuffer);
		ProcessResults(m_PathfindQueueSystem.GetPathfindActions(), ref outputDeps, ((SystemBase)this).Dependency);
		ProcessResults(m_PathfindQueueSystem.GetCoverageActions(), ref outputDeps, ((SystemBase)this).Dependency);
		ProcessResults(m_PathfindQueueSystem.GetAvailabilityActions(), ref outputDeps, ((SystemBase)this).Dependency);
		ProcessResults(m_PathfindQueueSystem.GetCreateActions());
		ProcessResults(m_PathfindQueueSystem.GetUpdateActions());
		ProcessResults(m_PathfindQueueSystem.GetDeleteActions());
		ProcessResults(m_PathfindQueueSystem.GetDensityActions());
		ProcessResults(m_PathfindQueueSystem.GetTimeActions());
		ProcessResults(m_PathfindQueueSystem.GetFlowActions());
		((SystemBase)this).Dependency = outputDeps;
	}

	private void AddQueryStats(object system, QueryType queryType, SetupTargetType originType, SetupTargetType destinationType, int resultLength, int graphTraversal)
	{
		ResultKey key = new ResultKey
		{
			m_System = system,
			m_QueryType = queryType,
			m_OriginType = originType,
			m_DestinationType = destinationType
		};
		if (m_QueryStats.TryGetValue(key, out var value))
		{
			value.m_QueryCount++;
			value.m_SuccessCount += math.min(1, resultLength);
			value.m_GraphTraversal += (float)graphTraversal / math.max(1f, (float)m_PathfindQueueSystem.GetGraphSize());
			value.m_Efficiency += (float)resultLength / math.max(1f, (float)graphTraversal);
			m_QueryStats[key] = value;
		}
		else
		{
			m_QueryStats.Add(key, new ResultValue
			{
				m_QueryCount = 1,
				m_SuccessCount = math.min(1, resultLength),
				m_GraphTraversal = (float)graphTraversal / math.max(1f, (float)m_PathfindQueueSystem.GetGraphSize()),
				m_Efficiency = (float)resultLength / math.max(1f, (float)graphTraversal)
			});
		}
	}

	private void ProcessResults(PathfindQueueSystem.ActionList<PathfindAction> list, ref JobHandle outputDeps, JobHandle inputDeps)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		m_ResultListIndex.Clear();
		m_PathfindResultBuffer.Clear();
		int num = 0;
		PathfindJobs.ResultItem resultItem = default(PathfindJobs.ResultItem);
		for (int i = 0; i < list.m_Items.Count; i++)
		{
			PathfindQueueSystem.ActionListItem<PathfindAction> value = list.m_Items[i];
			if ((value.m_Flags & PathFlags.Scheduled) != 0)
			{
				if (value.m_Action.data.m_State == PathfindActionState.Completed)
				{
					value.m_Flags &= ~PathFlags.Scheduled;
					ErrorCode errorCode = value.m_Action.data.m_Result[0].m_ErrorCode;
					int graphTraversal = value.m_Action.data.m_Result[value.m_Action.data.m_Result.Length - 1].m_GraphTraversal;
					int pathLength = value.m_Action.data.m_Result[value.m_Action.data.m_Result.Length - 1].m_PathLength;
					resultItem.m_Owner = value.m_Owner;
					resultItem.m_Result = value.m_Action.data.m_Result;
					resultItem.m_Path = value.m_Action.data.m_Path;
					if (m_ResultListIndex.TryGetValue(value.m_Owner, out var value2))
					{
						m_PathfindResultBuffer[value2] = resultItem;
					}
					else
					{
						m_ResultListIndex.Add(value.m_Owner, m_PathfindResultBuffer.Length);
						m_PathfindResultBuffer.Add(ref resultItem);
					}
					if (errorCode != ErrorCode.None)
					{
						COSystemBase.baseLog.ErrorFormat("Pathfind error ({0}: {1} -> {2}): {3} (Request: {4})", (object)value.m_System.GetType().Name, (object)value.m_Action.data.m_OriginType, (object)value.m_Action.data.m_DestinationType, (object)errorCode, (object)value.m_Owner);
					}
					AddQueryStats(value.m_System, QueryType.Pathfind, value.m_Action.data.m_OriginType, value.m_Action.data.m_DestinationType, pathLength, graphTraversal);
					if ((value.m_Flags & PathFlags.WantsEvent) != 0)
					{
						if (!((EntityCommandBuffer)(ref m_CommandBuffer)).IsCreated)
						{
							m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer();
						}
						Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_PathEventArchetype);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PathUpdated>(val, new PathUpdated(value.m_Owner, value.m_EventData));
					}
				}
				else
				{
					m_PendingSimulationFrameIndex = math.min(m_PendingSimulationFrameIndex, value.m_ResultFrame);
					m_PendingRequestCount++;
				}
			}
			else
			{
				if ((value.m_Flags & PathFlags.Pending) == 0)
				{
					value.Dispose();
					list.m_NextIndex--;
					continue;
				}
				m_PendingSimulationFrameIndex = math.min(m_PendingSimulationFrameIndex, value.m_ResultFrame);
				m_PendingRequestCount++;
			}
			list.m_Items[num++] = value;
		}
		if (num < list.m_Items.Count)
		{
			list.m_Items.RemoveRange(num, list.m_Items.Count - num);
		}
		if (m_PathfindResultBuffer.Length > 0)
		{
			JobHandle val2 = IJobParallelForExtensions.Schedule<PathfindJobs.ProcessResultsJob>(new PathfindJobs.ProcessResultsJob
			{
				m_ResultItems = m_PathfindResultBuffer,
				m_PathOwner = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformation = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformations = InternalCompilerInterface.GetBufferLookup<PathInformations>(ref __TypeHandle.__Game_Pathfind_PathInformations_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, m_PathfindResultBuffer.Length, 1, inputDeps);
			outputDeps = JobHandle.CombineDependencies(outputDeps, val2);
		}
	}

	private void ProcessResults(PathfindQueueSystem.ActionList<CoverageAction> list, ref JobHandle outputDeps, JobHandle inputDeps)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		m_ResultListIndex.Clear();
		m_CoverageResultBuffer.Clear();
		int num = 0;
		CoverageJobs.ResultItem resultItem = default(CoverageJobs.ResultItem);
		for (int i = 0; i < list.m_Items.Count; i++)
		{
			PathfindQueueSystem.ActionListItem<CoverageAction> value = list.m_Items[i];
			if ((value.m_Flags & PathFlags.Scheduled) != 0)
			{
				if (value.m_Action.data.m_State == PathfindActionState.Completed)
				{
					value.m_Flags &= ~PathFlags.Scheduled;
					resultItem.m_Owner = value.m_Owner;
					resultItem.m_Results = value.m_Action.data.m_Results;
					if (m_ResultListIndex.TryGetValue(value.m_Owner, out var value2))
					{
						m_CoverageResultBuffer[value2] = resultItem;
					}
					else
					{
						m_ResultListIndex.Add(value.m_Owner, m_CoverageResultBuffer.Length);
						m_CoverageResultBuffer.Add(ref resultItem);
					}
					AddQueryStats(value.m_System, QueryType.Coverage, SetupTargetType.None, SetupTargetType.None, resultItem.m_Results.Length, resultItem.m_Results.Length);
					if ((value.m_Flags & PathFlags.WantsEvent) != 0)
					{
						if (!((EntityCommandBuffer)(ref m_CommandBuffer)).IsCreated)
						{
							m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer();
						}
						Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_CoverageEventArchetype);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<CoverageUpdated>(val, new CoverageUpdated(value.m_Owner, value.m_EventData));
					}
				}
				else
				{
					m_PendingSimulationFrameIndex = math.min(m_PendingSimulationFrameIndex, value.m_ResultFrame);
					m_PendingRequestCount++;
				}
			}
			else
			{
				if ((value.m_Flags & PathFlags.Pending) == 0)
				{
					value.Dispose();
					list.m_NextIndex--;
					continue;
				}
				m_PendingSimulationFrameIndex = math.min(m_PendingSimulationFrameIndex, value.m_ResultFrame);
				m_PendingRequestCount++;
			}
			list.m_Items[num++] = value;
		}
		if (num < list.m_Items.Count)
		{
			list.m_Items.RemoveRange(num, list.m_Items.Count - num);
		}
		if (m_CoverageResultBuffer.Length > 0)
		{
			JobHandle val2 = IJobParallelForExtensions.Schedule<CoverageJobs.ProcessResultsJob>(new CoverageJobs.ProcessResultsJob
			{
				m_ResultItems = m_CoverageResultBuffer,
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CoverageElements = InternalCompilerInterface.GetBufferLookup<CoverageElement>(ref __TypeHandle.__Game_Pathfind_CoverageElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, m_CoverageResultBuffer.Length, 1, inputDeps);
			outputDeps = JobHandle.CombineDependencies(outputDeps, val2);
		}
	}

	private void ProcessResults(PathfindQueueSystem.ActionList<AvailabilityAction> list, ref JobHandle outputDeps, JobHandle inputDeps)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		m_ResultListIndex.Clear();
		m_AvailabilityResultBuffer.Clear();
		int num = 0;
		AvailabilityJobs.ResultItem resultItem = default(AvailabilityJobs.ResultItem);
		for (int i = 0; i < list.m_Items.Count; i++)
		{
			PathfindQueueSystem.ActionListItem<AvailabilityAction> value = list.m_Items[i];
			if ((value.m_Flags & PathFlags.Scheduled) != 0)
			{
				if (value.m_Action.data.m_State == PathfindActionState.Completed)
				{
					value.m_Flags &= ~PathFlags.Scheduled;
					resultItem.m_Owner = value.m_Owner;
					resultItem.m_Results = value.m_Action.data.m_Results;
					if (m_ResultListIndex.TryGetValue(value.m_Owner, out var value2))
					{
						m_AvailabilityResultBuffer[value2] = resultItem;
					}
					else
					{
						m_ResultListIndex.Add(value.m_Owner, m_AvailabilityResultBuffer.Length);
						m_AvailabilityResultBuffer.Add(ref resultItem);
					}
					AddQueryStats(value.m_System, QueryType.Availability, SetupTargetType.None, SetupTargetType.None, resultItem.m_Results.Length, resultItem.m_Results.Length);
				}
				else
				{
					m_PendingSimulationFrameIndex = math.min(m_PendingSimulationFrameIndex, value.m_ResultFrame);
					m_PendingRequestCount++;
				}
			}
			else
			{
				if ((value.m_Flags & PathFlags.Pending) == 0)
				{
					value.Dispose();
					list.m_NextIndex--;
					continue;
				}
				m_PendingSimulationFrameIndex = math.min(m_PendingSimulationFrameIndex, value.m_ResultFrame);
				m_PendingRequestCount++;
			}
			list.m_Items[num++] = value;
		}
		if (num < list.m_Items.Count)
		{
			list.m_Items.RemoveRange(num, list.m_Items.Count - num);
		}
		if (m_AvailabilityResultBuffer.Length > 0)
		{
			JobHandle val = IJobParallelForExtensions.Schedule<AvailabilityJobs.ProcessResultsJob>(new AvailabilityJobs.ProcessResultsJob
			{
				m_ResultItems = m_AvailabilityResultBuffer,
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AvailabilityElements = InternalCompilerInterface.GetBufferLookup<AvailabilityElement>(ref __TypeHandle.__Game_Pathfind_AvailabilityElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, m_AvailabilityResultBuffer.Length, 1, inputDeps);
			outputDeps = JobHandle.CombineDependencies(outputDeps, val);
		}
	}

	private void ProcessResults<T>(PathfindQueueSystem.ActionList<T> list) where T : struct, IDisposable
	{
		int num = 0;
		for (int i = 0; i < list.m_Items.Count; i++)
		{
			PathfindQueueSystem.ActionListItem<T> value = list.m_Items[i];
			if ((value.m_Flags & PathFlags.Pending) == 0 && ((JobHandle)(ref value.m_Dependencies)).IsCompleted)
			{
				((JobHandle)(ref value.m_Dependencies)).Complete();
				value.Dispose();
				list.m_NextIndex--;
			}
			else
			{
				list.m_Items[num++] = value;
			}
		}
		if (num < list.m_Items.Count)
		{
			list.m_Items.RemoveRange(num, list.m_Items.Count - num);
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
	public PathfindResultSystem()
	{
	}
}

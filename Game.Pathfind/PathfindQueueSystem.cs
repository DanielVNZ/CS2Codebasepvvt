using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class PathfindQueueSystem : GameSystemBase, IPreDeserialize
{
	public struct ActionListItem<T> : IDisposable where T : struct, IDisposable
	{
		public T m_Action;

		public Entity m_Owner;

		public JobHandle m_Dependencies;

		public PathFlags m_Flags;

		public uint m_ResultFrame;

		public PathEventData m_EventData;

		public object m_System;

		public ActionListItem(T action, Entity owner, JobHandle dependencies, PathFlags flags, uint frameIndex, PathEventData eventData, object system)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			m_Action = action;
			m_Owner = owner;
			m_Dependencies = dependencies;
			m_Flags = flags;
			m_ResultFrame = frameIndex;
			m_EventData = eventData;
			m_System = system;
		}

		public void Dispose()
		{
			((JobHandle)(ref m_Dependencies)).Complete();
			m_Action.Dispose();
		}
	}

	public class ActionList<T> : IDisposable where T : struct, IDisposable
	{
		public List<ActionListItem<T>> m_Items;

		public int m_NextIndex;

		public int m_PriorityCount;

		public ActionList()
		{
			m_Items = new List<ActionListItem<T>>();
		}

		public void Clear()
		{
			for (int i = 0; i < m_Items.Count; i++)
			{
				m_Items[i].Dispose();
			}
			m_Items.Clear();
			m_NextIndex = 0;
			m_PriorityCount = 0;
		}

		public void Dispose()
		{
			for (int i = 0; i < m_Items.Count; i++)
			{
				m_Items[i].Dispose();
			}
			m_Items = null;
		}
	}

	public enum ActionType
	{
		Create,
		Update,
		Delete,
		Pathfind,
		Coverage,
		Availability,
		Density,
		Time,
		Flow
	}

	private class WorkerData : IDisposable
	{
		public NativePathfindData m_PathfindData;

		public JobHandle m_WriteHandle;

		public JobHandle m_ReadHandle;

		public WorkerData(Allocator allocator)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			m_PathfindData = new NativePathfindData(allocator);
			m_WriteHandle = default(JobHandle);
			m_ReadHandle = default(JobHandle);
		}

		public void Clear()
		{
			((JobHandle)(ref m_WriteHandle)).Complete();
			((JobHandle)(ref m_ReadHandle)).Complete();
			m_PathfindData.Clear();
		}

		public void Dispose()
		{
			((JobHandle)(ref m_WriteHandle)).Complete();
			((JobHandle)(ref m_ReadHandle)).Complete();
			m_PathfindData.Dispose();
		}
	}

	private class WorkerActions : IDisposable
	{
		public NativeList<WorkerAction> m_Actions;

		public NativeReference<int> m_ActionIndex;

		public JobHandle m_ReadHandle;

		public int m_HighPriorityCount;

		public WorkerActions(Allocator allocator)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			m_Actions = new NativeList<WorkerAction>(100, AllocatorHandle.op_Implicit(allocator));
			m_ActionIndex = new NativeReference<int>(0, AllocatorHandle.op_Implicit(allocator));
			m_ReadHandle = default(JobHandle);
			m_HighPriorityCount = 0;
		}

		public unsafe void Add<T>(ActionType type, bool isHighPriority, ref T data) where T : struct
		{
			ref NativeList<WorkerAction> reference = ref m_Actions;
			WorkerAction workerAction = new WorkerAction
			{
				m_Type = type,
				m_ActionData = UnsafeUtility.AddressOf<T>(ref data)
			};
			reference.Add(ref workerAction);
			if (isHighPriority)
			{
				m_HighPriorityCount++;
			}
		}

		public void Clear()
		{
			((JobHandle)(ref m_ReadHandle)).Complete();
			m_Actions.Clear();
			m_ActionIndex.Value = 0;
			m_HighPriorityCount = 0;
		}

		public void Dispose()
		{
			((JobHandle)(ref m_ReadHandle)).Complete();
			m_Actions.Dispose();
			m_ActionIndex.Dispose();
		}
	}

	private struct ThreadData
	{
		public JobHandle m_JobHandle;

		public AllocatorHelper<UnsafeLinearAllocator> m_Allocator;
	}

	public struct WorkerAction
	{
		public ActionType m_Type;

		public unsafe void* m_ActionData;
	}

	[BurstCompile]
	public struct PathfindWorkerJob : IJob
	{
		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativePathfindData m_PathfindData;

		[ReadOnly]
		public PathfindHeuristicData m_PathfindHeuristicData;

		[ReadOnly]
		public float m_MaxPassengerTransportSpeed;

		[ReadOnly]
		public float m_MaxCargoTransportSpeed;

		[ReadOnly]
		public NativeArray<WorkerAction> m_Actions;

		[NativeDisableContainerSafetyRestriction]
		public NativeReference<int> m_ActionIndex;

		[NativeDisableUnsafePtrRestriction]
		public AllocatorHelper<UnsafeLinearAllocator> m_Allocator;

		public unsafe void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			ref int location = ref CollectionUtils.ValueAsRef<int>(m_ActionIndex);
			ref UnsafeLinearAllocator allocator = ref m_Allocator.Allocator;
			AllocatorHandle handle = ((UnsafeLinearAllocator)(ref allocator)).Handle;
			Allocator toAllocator = ((AllocatorHandle)(ref handle)).ToAllocator;
			while (true)
			{
				int num = Interlocked.Increment(ref location) - 1;
				if (num >= m_Actions.Length)
				{
					break;
				}
				WorkerAction workerAction = m_Actions[num];
				switch (workerAction.m_Type)
				{
				case ActionType.Pathfind:
					Execute(ref UnsafeUtility.AsRef<PathfindActionData>(workerAction.m_ActionData), num, toAllocator);
					break;
				case ActionType.Coverage:
					Execute(ref UnsafeUtility.AsRef<CoverageActionData>(workerAction.m_ActionData), toAllocator);
					break;
				case ActionType.Availability:
					Execute(ref UnsafeUtility.AsRef<AvailabilityActionData>(workerAction.m_ActionData), toAllocator);
					break;
				}
				((UnsafeLinearAllocator)(ref allocator)).Rewind(false);
			}
			((UnsafeLinearAllocator)(ref allocator)).Rewind(true);
		}

		private void Execute(ref PathfindActionData actionData, int index, Allocator allocator)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			PathfindJobs.PathfindJob.Execute(m_PathfindData, allocator, m_RandomSeed.GetRandom(index), m_PathfindHeuristicData, m_MaxPassengerTransportSpeed, m_MaxCargoTransportSpeed, ref actionData);
			Interlocked.MemoryBarrier();
			actionData.m_State = PathfindActionState.Completed;
		}

		private void Execute(ref CoverageActionData actionData, Allocator allocator)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CoverageJobs.CoverageJob.Execute(m_PathfindData, allocator, ref actionData);
			Interlocked.MemoryBarrier();
			actionData.m_State = PathfindActionState.Completed;
		}

		private void Execute(ref AvailabilityActionData actionData, Allocator allocator)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			AvailabilityJobs.AvailabilityJob.Execute(m_PathfindData, allocator, ref actionData);
			Interlocked.MemoryBarrier();
			actionData.m_State = PathfindActionState.Completed;
		}
	}

	private const int WORKER_DATA_COUNT = 2;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private TransportLineSystem m_TransportLineSystem;

	private NetInitializeSystem m_NetInitializeSystem;

	private ActionList<CreateAction> m_CreateActions;

	private ActionList<UpdateAction> m_UpdateActions;

	private ActionList<DeleteAction> m_DeleteActions;

	private ActionList<PathfindAction> m_PathfindActions;

	private ActionList<CoverageAction> m_CoverageActions;

	private ActionList<AvailabilityAction> m_AvailabilityActions;

	private ActionList<DensityAction> m_DensityActions;

	private ActionList<TimeAction> m_TimeActions;

	private ActionList<FlowAction> m_FlowActions;

	private Queue<ActionType> m_ActionTypes;

	private Queue<ActionType> m_HighPriorityTypes;

	private Queue<ActionType> m_ModificationTypes;

	private Queue<WorkerActions> m_WorkerActions;

	private Queue<WorkerActions> m_WorkerActionPool;

	private List<WorkerData> m_WorkerData;

	private List<ThreadData> m_ThreadData;

	private List<AllocatorHelper<UnsafeLinearAllocator>> m_AllocatorPool;

	private int m_MaxThreadCount;

	private int m_NextWorkerIndex;

	private int m_LastWorkerIndex;

	private int m_DependencyIndex;

	private bool m_RequireDebug;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_TransportLineSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TransportLineSystem>();
		m_NetInitializeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetInitializeSystem>();
		m_MaxThreadCount = math.max(1, JobsUtility.JobWorkerCount / 2);
		m_CreateActions = new ActionList<CreateAction>();
		m_UpdateActions = new ActionList<UpdateAction>();
		m_DeleteActions = new ActionList<DeleteAction>();
		m_PathfindActions = new ActionList<PathfindAction>();
		m_CoverageActions = new ActionList<CoverageAction>();
		m_AvailabilityActions = new ActionList<AvailabilityAction>();
		m_DensityActions = new ActionList<DensityAction>();
		m_TimeActions = new ActionList<TimeAction>();
		m_FlowActions = new ActionList<FlowAction>();
		m_ActionTypes = new Queue<ActionType>();
		m_HighPriorityTypes = new Queue<ActionType>();
		m_ModificationTypes = new Queue<ActionType>();
		m_WorkerActions = new Queue<WorkerActions>();
		m_WorkerActionPool = new Queue<WorkerActions>();
		m_WorkerData = new List<WorkerData>(2);
		for (int i = 0; i < 2; i++)
		{
			m_WorkerData.Add(new WorkerData((Allocator)4));
		}
		m_ThreadData = new List<ThreadData>(m_MaxThreadCount);
		m_AllocatorPool = new List<AllocatorHelper<UnsafeLinearAllocator>>(m_MaxThreadCount);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ThreadData.Count; i++)
		{
			ThreadData threadData = m_ThreadData[i];
			((JobHandle)(ref threadData.m_JobHandle)).Complete();
			UnsafeLinearAllocator allocator = threadData.m_Allocator.Allocator;
			threadData.m_Allocator.Dispose();
			((UnsafeLinearAllocator)(ref allocator)).Dispose();
		}
		for (int j = 0; j < m_AllocatorPool.Count; j++)
		{
			UnsafeLinearAllocator allocator2 = m_AllocatorPool[j].Allocator;
			m_AllocatorPool[j].Dispose();
			((UnsafeLinearAllocator)(ref allocator2)).Dispose();
		}
		m_CreateActions.Dispose();
		m_UpdateActions.Dispose();
		m_DeleteActions.Dispose();
		m_PathfindActions.Dispose();
		m_CoverageActions.Dispose();
		m_AvailabilityActions.Dispose();
		m_DensityActions.Dispose();
		m_TimeActions.Dispose();
		m_FlowActions.Dispose();
		WorkerActions workerActions = default(WorkerActions);
		while (m_WorkerActions.TryDequeue(ref workerActions))
		{
			workerActions.Dispose();
		}
		WorkerActions workerActions2 = default(WorkerActions);
		while (m_WorkerActionPool.TryDequeue(ref workerActions2))
		{
			workerActions2.Dispose();
		}
		for (int k = 0; k < m_WorkerData.Count; k++)
		{
			m_WorkerData[k].Dispose();
		}
		base.OnDestroy();
	}

	public void PreDeserialize(Context context)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ThreadData.Count; i++)
		{
			ThreadData threadData = m_ThreadData[i];
			((JobHandle)(ref threadData.m_JobHandle)).Complete();
			m_AllocatorPool.Add(threadData.m_Allocator);
		}
		m_ThreadData.Clear();
		m_DependencyIndex = 0;
		m_CreateActions.Clear();
		m_UpdateActions.Clear();
		m_DeleteActions.Clear();
		m_PathfindActions.Clear();
		m_CoverageActions.Clear();
		m_AvailabilityActions.Clear();
		m_DensityActions.Clear();
		m_TimeActions.Clear();
		m_FlowActions.Clear();
		m_ActionTypes.Clear();
		m_HighPriorityTypes.Clear();
		m_ModificationTypes.Clear();
		WorkerActions workerActions = default(WorkerActions);
		while (m_WorkerActions.TryDequeue(ref workerActions))
		{
			workerActions.Clear();
			m_WorkerActionPool.Enqueue(workerActions);
		}
		for (int j = 0; j < m_WorkerData.Count; j++)
		{
			m_WorkerData[j].Clear();
		}
	}

	public NativePathfindData GetDataContainer(out JobHandle dependencies)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		WorkerData workerData = m_WorkerData[m_NextWorkerIndex];
		dependencies = workerData.m_WriteHandle;
		return workerData.m_PathfindData;
	}

	public void AddDataReader(JobHandle handle)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		WorkerData workerData = m_WorkerData[m_NextWorkerIndex];
		workerData.m_ReadHandle = JobHandle.CombineDependencies(workerData.m_ReadHandle, handle);
	}

	public void RequireDebug()
	{
		m_RequireDebug = true;
	}

	public int GetGraphSize()
	{
		return m_WorkerData[m_NextWorkerIndex].m_PathfindData.Size;
	}

	public void GetGraphMemory(out uint usedMemory, out uint allocatedMemory)
	{
		usedMemory = 0u;
		allocatedMemory = 0u;
		for (int i = 0; i < m_WorkerData.Count; i++)
		{
			m_WorkerData[i].m_PathfindData.GetMemoryStats(out var used, out var allocated);
			usedMemory += used;
			allocatedMemory += allocated;
		}
	}

	public void GetQueryMemory(out uint usedMemory, out uint allocatedMemory)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		usedMemory = 0u;
		allocatedMemory = 0u;
		for (int i = 0; i < m_ThreadData.Count; i++)
		{
			ref UnsafeLinearAllocator allocator = ref m_ThreadData[i].m_Allocator.Allocator;
			usedMemory += ((UnsafeLinearAllocator)(ref allocator)).Used;
			allocatedMemory += ((UnsafeLinearAllocator)(ref allocator)).Size;
		}
		for (int j = 0; j < m_AllocatorPool.Count; j++)
		{
			ref UnsafeLinearAllocator allocator2 = ref m_AllocatorPool[j].Allocator;
			usedMemory += ((UnsafeLinearAllocator)(ref allocator2)).Used;
			allocatedMemory += ((UnsafeLinearAllocator)(ref allocator2)).Size;
		}
	}

	public void Enqueue(CreateAction action, JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, Entity.Null, dependencies, uint.MaxValue, m_CreateActions, ActionType.Create, null, highPriority: false, modification: true);
	}

	public void Enqueue(UpdateAction action, JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, Entity.Null, dependencies, uint.MaxValue, m_UpdateActions, ActionType.Update, null, highPriority: false, modification: true);
	}

	public void Enqueue(DeleteAction action, JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, Entity.Null, dependencies, uint.MaxValue, m_DeleteActions, ActionType.Delete, null, highPriority: false, modification: true);
	}

	public void Enqueue(PathfindAction action, Entity owner, JobHandle dependencies, uint resultFrame, object system, bool highPriority = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, owner, dependencies, resultFrame, m_PathfindActions, ActionType.Pathfind, system, highPriority, modification: false);
	}

	public void Enqueue(PathfindAction action, Entity owner, JobHandle dependencies, uint resultFrame, object system, PathEventData eventData, bool highPriority = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, owner, dependencies, resultFrame, m_PathfindActions, ActionType.Pathfind, system, highPriority, eventData);
	}

	public void Enqueue(CoverageAction action, Entity owner, JobHandle dependencies, uint resultFrame, object system, bool highPriority = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, owner, dependencies, resultFrame, m_CoverageActions, ActionType.Coverage, system, highPriority, modification: false);
	}

	public void Enqueue(CoverageAction action, Entity owner, JobHandle dependencies, uint resultFrame, object system, PathEventData eventData, bool highPriority = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, owner, dependencies, resultFrame, m_CoverageActions, ActionType.Coverage, system, highPriority, eventData);
	}

	public void Enqueue(AvailabilityAction action, Entity owner, JobHandle dependencies, uint resultFrame, object system)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, owner, dependencies, resultFrame, m_AvailabilityActions, ActionType.Availability, system, highPriority: false, modification: false);
	}

	public void Enqueue(DensityAction action, JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, Entity.Null, dependencies, uint.MaxValue, m_DensityActions, ActionType.Density, null, highPriority: false, modification: true);
	}

	public void Enqueue(TimeAction action, JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, Entity.Null, dependencies, uint.MaxValue, m_TimeActions, ActionType.Time, null, highPriority: false, modification: true);
	}

	public void Enqueue(FlowAction action, JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enqueue(action, Entity.Null, dependencies, uint.MaxValue, m_FlowActions, ActionType.Flow, null, highPriority: false, modification: true);
	}

	public ActionList<CreateAction> GetCreateActions()
	{
		return m_CreateActions;
	}

	public ActionList<UpdateAction> GetUpdateActions()
	{
		return m_UpdateActions;
	}

	public ActionList<DeleteAction> GetDeleteActions()
	{
		return m_DeleteActions;
	}

	public ActionList<PathfindAction> GetPathfindActions()
	{
		return m_PathfindActions;
	}

	public ActionList<CoverageAction> GetCoverageActions()
	{
		return m_CoverageActions;
	}

	public ActionList<AvailabilityAction> GetAvailabilityActions()
	{
		return m_AvailabilityActions;
	}

	public ActionList<DensityAction> GetDensityActions()
	{
		return m_DensityActions;
	}

	public ActionList<TimeAction> GetTimeActions()
	{
		return m_TimeActions;
	}

	public ActionList<FlowAction> GetFlowActions()
	{
		return m_FlowActions;
	}

	private void Enqueue<T>(T action, Entity owner, JobHandle dependencies, uint resultFrame, ActionList<T> list, ActionType type, object system, bool highPriority, bool modification) where T : struct, IDisposable
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		PathFlags flags = PathFlags.Pending;
		if (highPriority)
		{
			list.m_Items.Insert(list.m_NextIndex + list.m_PriorityCount++, new ActionListItem<T>(action, owner, dependencies, flags, resultFrame, default(PathEventData), system));
			m_HighPriorityTypes.Enqueue(type);
		}
		else if (modification)
		{
			list.m_Items.Add(new ActionListItem<T>(action, owner, dependencies, flags, resultFrame, default(PathEventData), system));
			m_ModificationTypes.Enqueue(type);
		}
		else
		{
			list.m_Items.Add(new ActionListItem<T>(action, owner, dependencies, flags, resultFrame, default(PathEventData), system));
			m_ActionTypes.Enqueue(type);
		}
	}

	private void Enqueue<T>(T action, Entity owner, JobHandle dependencies, uint resultFrame, ActionList<T> list, ActionType type, object system, bool highPriority, PathEventData eventData) where T : struct, IDisposable
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		PathFlags flags = PathFlags.Pending | PathFlags.WantsEvent;
		if (highPriority)
		{
			list.m_Items.Insert(list.m_NextIndex + list.m_PriorityCount++, new ActionListItem<T>(action, owner, dependencies, flags, resultFrame, eventData, system));
			m_HighPriorityTypes.Enqueue(type);
		}
		else
		{
			list.m_Items.Add(new ActionListItem<T>(action, owner, dependencies, flags, resultFrame, eventData, system));
			m_ActionTypes.Enqueue(type);
		}
	}

	private JobHandle ScheduleModificationJob<T>(T job) where T : struct, IJob, ModificationJobs.IPathfindModificationJob
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		for (int i = 0; i < m_WorkerData.Count; i++)
		{
			WorkerData workerData = m_WorkerData[i];
			job.SetPathfindData(workerData.m_PathfindData);
			workerData.m_WriteHandle = IJobExtensions.Schedule<T>(job, JobHandle.CombineDependencies(workerData.m_WriteHandle, workerData.m_ReadHandle));
			workerData.m_ReadHandle = default(JobHandle);
			val = JobHandle.CombineDependencies(val, workerData.m_WriteHandle);
		}
		if (m_LastWorkerIndex == m_NextWorkerIndex && ++m_NextWorkerIndex >= m_WorkerData.Count)
		{
			m_NextWorkerIndex = 0;
		}
		return val;
	}

	private void ScheduleWorkerJobs(ref WorkerActions currentActions)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		if (currentActions == null)
		{
			return;
		}
		WorkerData workerData = m_WorkerData[m_NextWorkerIndex];
		int num = math.min(currentActions.m_Actions.Length, math.max(m_MaxThreadCount, currentActions.m_HighPriorityCount));
		int num2 = m_MaxThreadCount + currentActions.m_HighPriorityCount;
		int count = m_ThreadData.Count;
		PathfindWorkerJob pathfindWorkerJob = new PathfindWorkerJob
		{
			m_RandomSeed = RandomSeed.Next(),
			m_PathfindData = workerData.m_PathfindData,
			m_PathfindHeuristicData = m_NetInitializeSystem.GetHeuristicData(),
			m_Actions = currentActions.m_Actions.AsArray(),
			m_ActionIndex = currentActions.m_ActionIndex
		};
		m_TransportLineSystem.GetMaxTransportSpeed(out pathfindWorkerJob.m_MaxPassengerTransportSpeed, out pathfindWorkerJob.m_MaxCargoTransportSpeed);
		for (int i = 0; i < num; i++)
		{
			JobHandle val = workerData.m_WriteHandle;
			ThreadData threadData = default(ThreadData);
			if (m_ThreadData.Count >= num2)
			{
				if (m_DependencyIndex >= count)
				{
					m_DependencyIndex = 0;
				}
				threadData = m_ThreadData[m_DependencyIndex];
				val = JobHandle.CombineDependencies(val, threadData.m_JobHandle);
			}
			else if (m_AllocatorPool.Count != 0)
			{
				threadData.m_Allocator = m_AllocatorPool[m_AllocatorPool.Count - 1];
				m_AllocatorPool.RemoveAt(m_AllocatorPool.Count - 1);
			}
			else
			{
				threadData.m_Allocator = new AllocatorHelper<UnsafeLinearAllocator>(AllocatorHandle.op_Implicit((Allocator)4), false, 0);
				((UnsafeLinearAllocator)(ref threadData.m_Allocator.Allocator)).Initialize(1048576u);
			}
			pathfindWorkerJob.m_Allocator = threadData.m_Allocator;
			threadData.m_JobHandle = IJobExtensions.Schedule<PathfindWorkerJob>(pathfindWorkerJob, val);
			currentActions.m_ReadHandle = JobHandle.CombineDependencies(currentActions.m_ReadHandle, threadData.m_JobHandle);
			if (m_ThreadData.Count >= num2)
			{
				m_ThreadData[m_DependencyIndex++] = threadData;
			}
			else
			{
				m_ThreadData.Add(threadData);
			}
		}
		workerData.m_ReadHandle = JobHandle.CombineDependencies(workerData.m_ReadHandle, currentActions.m_ReadHandle);
		currentActions = null;
		m_LastWorkerIndex = m_NextWorkerIndex;
	}

	private void RequireWorkerActions(ref WorkerActions currentActions)
	{
		if (currentActions == null)
		{
			if (!m_WorkerActionPool.TryDequeue(ref currentActions))
			{
				currentActions = new WorkerActions((Allocator)4);
			}
			m_WorkerActions.Enqueue(currentActions);
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = m_RequireDebug;
		m_RequireDebug = false;
		for (int i = 0; i < m_AllocatorPool.Count; i++)
		{
			((UnsafeLinearAllocator)(ref m_AllocatorPool[i].Allocator)).Rewind(true);
		}
		int num = 0;
		for (int j = 0; j < m_ThreadData.Count; j++)
		{
			ThreadData value = m_ThreadData[j];
			if (((JobHandle)(ref value.m_JobHandle)).IsCompleted)
			{
				((JobHandle)(ref value.m_JobHandle)).Complete();
				m_AllocatorPool.Add(value.m_Allocator);
				if (num < m_DependencyIndex)
				{
					m_DependencyIndex--;
				}
			}
			else
			{
				m_ThreadData[num++] = value;
			}
		}
		if (num < m_ThreadData.Count)
		{
			m_ThreadData.RemoveRange(num, m_ThreadData.Count - num);
		}
		for (int k = 0; k < m_WorkerData.Count; k++)
		{
			WorkerData workerData = m_WorkerData[k];
			if (((JobHandle)(ref workerData.m_WriteHandle)).IsCompleted)
			{
				((JobHandle)(ref workerData.m_WriteHandle)).Complete();
			}
			if (((JobHandle)(ref workerData.m_ReadHandle)).IsCompleted)
			{
				((JobHandle)(ref workerData.m_ReadHandle)).Complete();
			}
		}
		WorkerActions workerActions = default(WorkerActions);
		while (m_WorkerActions.TryPeek(ref workerActions) && ((JobHandle)(ref workerActions.m_ReadHandle)).IsCompleted)
		{
			workerActions.Clear();
			m_WorkerActions.Dequeue();
			m_WorkerActionPool.Enqueue(workerActions);
		}
		m_PathfindSetupSystem.CompleteSetup();
		WorkerActions currentActions = null;
		try
		{
			while (true)
			{
				ActionType actionType;
				bool flag2;
				bool flag3;
				if (m_HighPriorityTypes.Count != 0)
				{
					actionType = m_HighPriorityTypes.Peek();
					flag2 = true;
					flag3 = false;
				}
				else if (m_ModificationTypes.Count != 0)
				{
					actionType = m_ModificationTypes.Peek();
					flag2 = false;
					flag3 = true;
				}
				else
				{
					if (m_ActionTypes.Count == 0)
					{
						break;
					}
					actionType = m_ActionTypes.Peek();
					flag2 = false;
					flag3 = false;
				}
				switch (actionType)
				{
				case ActionType.Create:
				{
					ActionListItem<CreateAction> value5 = m_CreateActions.m_Items[m_CreateActions.m_NextIndex];
					if (!((JobHandle)(ref value5.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value5.m_Dependencies)).Complete();
					ScheduleWorkerJobs(ref currentActions);
					value5.m_Dependencies = ScheduleModificationJob<ModificationJobs.CreateEdgesJob>(new ModificationJobs.CreateEdgesJob
					{
						m_Action = value5.m_Action
					});
					value5.m_Flags = (value5.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_CreateActions.m_Items[m_CreateActions.m_NextIndex++] = value5;
					break;
				}
				case ActionType.Update:
				{
					ActionListItem<UpdateAction> value10 = m_UpdateActions.m_Items[m_UpdateActions.m_NextIndex];
					if (!((JobHandle)(ref value10.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value10.m_Dependencies)).Complete();
					ScheduleWorkerJobs(ref currentActions);
					value10.m_Dependencies = ScheduleModificationJob<ModificationJobs.UpdateEdgesJob>(new ModificationJobs.UpdateEdgesJob
					{
						m_Action = value10.m_Action
					});
					value10.m_Flags = (value10.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_UpdateActions.m_Items[m_UpdateActions.m_NextIndex++] = value10;
					break;
				}
				case ActionType.Delete:
				{
					ActionListItem<DeleteAction> value7 = m_DeleteActions.m_Items[m_DeleteActions.m_NextIndex];
					if (!((JobHandle)(ref value7.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value7.m_Dependencies)).Complete();
					ScheduleWorkerJobs(ref currentActions);
					value7.m_Dependencies = ScheduleModificationJob<ModificationJobs.DeleteEdgesJob>(new ModificationJobs.DeleteEdgesJob
					{
						m_Action = value7.m_Action
					});
					value7.m_Flags = (value7.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_DeleteActions.m_Items[m_DeleteActions.m_NextIndex++] = value7;
					break;
				}
				case ActionType.Pathfind:
				{
					ActionListItem<PathfindAction> value9 = m_PathfindActions.m_Items[m_PathfindActions.m_NextIndex];
					if (!((JobHandle)(ref value9.m_Dependencies)).IsCompleted || (flag && (value9.m_Flags & PathFlags.Debug) == 0))
					{
						return;
					}
					((JobHandle)(ref value9.m_Dependencies)).Complete();
					RequireWorkerActions(ref currentActions);
					currentActions.Add(actionType, flag2, ref value9.m_Action.data);
					value9.m_Flags = (value9.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_PathfindActions.m_Items[m_PathfindActions.m_NextIndex++] = value9;
					if (flag2)
					{
						m_PathfindActions.m_PriorityCount--;
					}
					break;
				}
				case ActionType.Coverage:
				{
					ActionListItem<CoverageAction> value3 = m_CoverageActions.m_Items[m_CoverageActions.m_NextIndex];
					if (!((JobHandle)(ref value3.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value3.m_Dependencies)).Complete();
					RequireWorkerActions(ref currentActions);
					currentActions.Add(actionType, flag2, ref value3.m_Action.data);
					value3.m_Flags = (value3.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_CoverageActions.m_Items[m_CoverageActions.m_NextIndex++] = value3;
					if (flag2)
					{
						m_CoverageActions.m_PriorityCount--;
					}
					break;
				}
				case ActionType.Availability:
				{
					ActionListItem<AvailabilityAction> value8 = m_AvailabilityActions.m_Items[m_AvailabilityActions.m_NextIndex];
					if (!((JobHandle)(ref value8.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value8.m_Dependencies)).Complete();
					RequireWorkerActions(ref currentActions);
					currentActions.Add(actionType, flag2, ref value8.m_Action.data);
					value8.m_Flags = (value8.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_AvailabilityActions.m_Items[m_AvailabilityActions.m_NextIndex++] = value8;
					if (flag2)
					{
						m_CoverageActions.m_PriorityCount--;
					}
					break;
				}
				case ActionType.Density:
				{
					ActionListItem<DensityAction> value6 = m_DensityActions.m_Items[m_DensityActions.m_NextIndex];
					if (!((JobHandle)(ref value6.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value6.m_Dependencies)).Complete();
					ScheduleWorkerJobs(ref currentActions);
					value6.m_Dependencies = ScheduleModificationJob<ModificationJobs.SetDensityJob>(new ModificationJobs.SetDensityJob
					{
						m_Action = value6.m_Action
					});
					value6.m_Flags = (value6.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_DensityActions.m_Items[m_DensityActions.m_NextIndex++] = value6;
					break;
				}
				case ActionType.Time:
				{
					ActionListItem<TimeAction> value4 = m_TimeActions.m_Items[m_TimeActions.m_NextIndex];
					if (!((JobHandle)(ref value4.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value4.m_Dependencies)).Complete();
					ScheduleWorkerJobs(ref currentActions);
					value4.m_Dependencies = ScheduleModificationJob<ModificationJobs.SetTimeJob>(new ModificationJobs.SetTimeJob
					{
						m_Action = value4.m_Action
					});
					value4.m_Flags = (value4.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_TimeActions.m_Items[m_TimeActions.m_NextIndex++] = value4;
					break;
				}
				case ActionType.Flow:
				{
					ActionListItem<FlowAction> value2 = m_FlowActions.m_Items[m_FlowActions.m_NextIndex];
					if (!((JobHandle)(ref value2.m_Dependencies)).IsCompleted)
					{
						return;
					}
					((JobHandle)(ref value2.m_Dependencies)).Complete();
					ScheduleWorkerJobs(ref currentActions);
					value2.m_Dependencies = ScheduleModificationJob<ModificationJobs.SetFlowJob>(new ModificationJobs.SetFlowJob
					{
						m_Action = value2.m_Action
					});
					value2.m_Flags = (value2.m_Flags & ~PathFlags.Pending) | PathFlags.Scheduled;
					m_FlowActions.m_Items[m_FlowActions.m_NextIndex++] = value2;
					break;
				}
				}
				if (flag2)
				{
					m_HighPriorityTypes.Dequeue();
				}
				else if (flag3)
				{
					m_ModificationTypes.Dequeue();
				}
				else
				{
					m_ActionTypes.Dequeue();
				}
			}
		}
		finally
		{
			ScheduleWorkerJobs(ref currentActions);
		}
	}

	[Preserve]
	public PathfindQueueSystem()
	{
	}
}

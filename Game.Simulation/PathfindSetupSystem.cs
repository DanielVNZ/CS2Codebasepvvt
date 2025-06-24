using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Serialization;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class PathfindSetupSystem : GameSystemBase, IPreDeserialize
{
	public struct SetupData
	{
		[ReadOnly]
		private int m_StartIndex;

		[ReadOnly]
		private int m_Length;

		[ReadOnly]
		private NativeList<SetupListItem> m_SetupItems;

		[ReadOnly]
		private PathfindTargetSeekerData m_SeekerData;

		private ParallelWriter<PathfindSetupTarget> m_TargetQueue;

		public int Length => m_Length;

		public SetupData(int startIndex, int endIndex, NativeList<SetupListItem> setupItems, PathfindTargetSeekerData seekerData, ParallelWriter<PathfindSetupTarget> targetQueue)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			m_StartIndex = startIndex;
			m_Length = endIndex - startIndex;
			m_SetupItems = setupItems;
			m_SeekerData = seekerData;
			m_TargetQueue = targetQueue;
		}

		public void GetItem(int index, out Entity entity, out PathfindTargetSeeker<PathfindSetupBuffer> targetSeeker)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			SetupListItem setupListItem = m_SetupItems[m_StartIndex + index];
			entity = ((setupListItem.m_Target.m_Entity != Entity.Null) ? setupListItem.m_Target.m_Entity : setupListItem.m_Owner);
			PathfindSetupBuffer buffer = new PathfindSetupBuffer
			{
				m_Queue = m_TargetQueue,
				m_SetupIndex = index
			};
			targetSeeker = new PathfindTargetSeeker<PathfindSetupBuffer>(m_SeekerData, setupListItem.m_Parameters, setupListItem.m_Target, buffer, setupListItem.m_RandomSeed, setupListItem.m_ActionStart);
		}

		public void GetItem(int index, out Entity entity, out Entity owner, out PathfindTargetSeeker<PathfindSetupBuffer> targetSeeker)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			SetupListItem setupListItem = m_SetupItems[m_StartIndex + index];
			entity = ((setupListItem.m_Target.m_Entity != Entity.Null) ? setupListItem.m_Target.m_Entity : setupListItem.m_Owner);
			owner = setupListItem.m_Owner;
			PathfindSetupBuffer buffer = new PathfindSetupBuffer
			{
				m_Queue = m_TargetQueue,
				m_SetupIndex = index
			};
			targetSeeker = new PathfindTargetSeeker<PathfindSetupBuffer>(m_SeekerData, setupListItem.m_Parameters, setupListItem.m_Target, buffer, setupListItem.m_RandomSeed, setupListItem.m_ActionStart);
		}
	}

	public struct SetupListItem : IComparable<SetupListItem>
	{
		public SetupQueueTarget m_Target;

		public PathfindParameters m_Parameters;

		public UnsafeList<PathTarget> m_Buffer;

		public Entity m_Owner;

		public RandomSeed m_RandomSeed;

		public int m_ActionIndex;

		public bool m_ActionStart;

		public int CompareTo(SetupListItem other)
		{
			return m_Target.m_Type - other.m_Target.m_Type;
		}

		public SetupListItem(SetupQueueTarget target, PathfindParameters parameters, Entity owner, RandomSeed randomSeed, int actionIndex, bool actionStart)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			m_Target = target;
			m_Parameters = parameters;
			m_Buffer = new UnsafeList<PathTarget>(0, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)0);
			m_Owner = owner;
			m_RandomSeed = randomSeed;
			m_ActionIndex = actionIndex;
			m_ActionStart = actionStart;
		}
	}

	private struct ActionListItem
	{
		public PathfindAction m_Action;

		public Entity m_Owner;

		public uint m_ResultFrame;

		public object m_System;

		public ActionListItem(PathfindAction action, Entity owner, uint resultFrame, object system)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Action = action;
			m_Owner = owner;
			m_ResultFrame = resultFrame;
			m_System = system;
		}
	}

	private struct SetupQueue
	{
		public NativeQueue<SetupQueueItem> m_Queue;

		public uint m_ResultFrame;

		public uint m_SpreadFrame;

		public object m_System;
	}

	[BurstCompile]
	private struct DequePathTargetsJob : IJob
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe SetupListItem* m_SetupItems;

		public NativeQueue<PathfindSetupTarget> m_TargetQueue;

		public unsafe void Execute()
		{
			PathfindSetupTarget pathfindSetupTarget = default(PathfindSetupTarget);
			while (m_TargetQueue.TryDequeue(ref pathfindSetupTarget))
			{
				ref SetupListItem reference = ref m_SetupItems[pathfindSetupTarget.m_SetupIndex];
				if ((reference.m_Parameters.m_PathfindFlags & PathfindFlags.SkipPathfind) != 0)
				{
					if (reference.m_Buffer.Length == 0)
					{
						reference.m_Buffer.Add(ref pathfindSetupTarget.m_PathTarget);
						continue;
					}
					ref PathTarget reference2 = ref reference.m_Buffer.ElementAt(0);
					if (pathfindSetupTarget.m_PathTarget.m_Cost < reference2.m_Cost)
					{
						reference2 = pathfindSetupTarget.m_PathTarget;
					}
				}
				else
				{
					reference.m_Buffer.Add(ref pathfindSetupTarget.m_PathTarget);
				}
			}
		}
	}

	private PathfindTargetSeekerData m_TargetSeekerData;

	private CommonPathfindSetup m_CommonPathfindSetup;

	private PostServicePathfindSetup m_PostServicePathfindSetup;

	private GarbagePathfindSetup m_GarbagePathfindSetup;

	private TransportPathfindSetup m_TransportPathfindSetup;

	private PolicePathfindSetup m_PolicePathfindSetup;

	private FirePathfindSetup m_FirePathfindSetup;

	private HealthcarePathfindSetup m_HealthcarePathfindSetup;

	private AreaPathfindSetup m_AreaPathfindSetup;

	private RoadPathfindSetup m_RoadPathfindSetup;

	private CitizenPathfindSetup m_CitizenPathfindSetup;

	private ResourcePathfindSetup m_ResourcePathfindSetup;

	private SimulationSystem m_SimulationSystem;

	private PathfindQueueSystem m_PathfindQueueSystem;

	private AirwaySystem m_AirwaySystem;

	private NativeList<SetupListItem> m_SetupList;

	private List<SetupQueue> m_ActiveQueues;

	private List<SetupQueue> m_FreeQueues;

	private List<ActionListItem> m_ActionList;

	private JobHandle m_QueueDependencies;

	private JobHandle m_SetupDependencies;

	private uint m_QueueSimulationFrameIndex;

	private uint m_SetupSimulationFrameIndex;

	private int m_PendingRequestCount;

	public uint pendingSimulationFrame => math.min(m_QueueSimulationFrameIndex, m_SetupSimulationFrameIndex);

	public int pendingRequestCount => m_PendingRequestCount;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TargetSeekerData = new PathfindTargetSeekerData((SystemBase)(object)this);
		m_CommonPathfindSetup = new CommonPathfindSetup(this);
		m_PostServicePathfindSetup = new PostServicePathfindSetup(this);
		m_GarbagePathfindSetup = new GarbagePathfindSetup(this);
		m_TransportPathfindSetup = new TransportPathfindSetup(this);
		m_PolicePathfindSetup = new PolicePathfindSetup(this);
		m_FirePathfindSetup = new FirePathfindSetup(this);
		m_HealthcarePathfindSetup = new HealthcarePathfindSetup(this);
		m_AreaPathfindSetup = new AreaPathfindSetup(this);
		m_RoadPathfindSetup = new RoadPathfindSetup(this);
		m_CitizenPathfindSetup = new CitizenPathfindSetup(this);
		m_ResourcePathfindSetup = new ResourcePathfindSetup(this);
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_AirwaySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirwaySystem>();
		m_SetupList = new NativeList<SetupListItem>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_ActiveQueues = new List<SetupQueue>(10);
		m_FreeQueues = new List<SetupQueue>(10);
		m_ActionList = new List<ActionListItem>(50);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_QueueDependencies)).Complete();
		for (int i = 0; i < m_ActiveQueues.Count; i++)
		{
			m_ActiveQueues[i].m_Queue.Dispose();
		}
		for (int j = 0; j < m_FreeQueues.Count; j++)
		{
			m_FreeQueues[j].m_Queue.Dispose();
		}
		m_ActiveQueues.Clear();
		m_FreeQueues.Clear();
		((JobHandle)(ref m_SetupDependencies)).Complete();
		for (int k = 0; k < m_SetupList.Length; k++)
		{
			m_SetupList[k].m_Buffer.Dispose();
		}
		for (int l = 0; l < m_ActionList.Count; l++)
		{
			m_ActionList[l].m_Action.Dispose();
		}
		m_SetupList.Dispose();
		m_ActionList.Clear();
		base.OnDestroy();
	}

	public void PreDeserialize(Context context)
	{
		((JobHandle)(ref m_QueueDependencies)).Complete();
		for (int i = 0; i < m_ActiveQueues.Count; i++)
		{
			m_ActiveQueues[i].m_Queue.Dispose();
		}
		for (int j = 0; j < m_FreeQueues.Count; j++)
		{
			m_FreeQueues[j].m_Queue.Dispose();
		}
		m_ActiveQueues.Clear();
		m_FreeQueues.Clear();
		((JobHandle)(ref m_SetupDependencies)).Complete();
		for (int k = 0; k < m_SetupList.Length; k++)
		{
			m_SetupList[k].m_Buffer.Dispose();
		}
		for (int l = 0; l < m_ActionList.Count; l++)
		{
			m_ActionList[l].m_Action.Dispose();
		}
		m_SetupList.Clear();
		m_ActionList.Clear();
		m_QueueSimulationFrameIndex = uint.MaxValue;
		m_SetupSimulationFrameIndex = uint.MaxValue;
		m_PendingRequestCount = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		if (m_ActiveQueues.Count == 0)
		{
			return;
		}
		CompleteSetup();
		m_TargetSeekerData.Update((SystemBase)(object)this, m_AirwaySystem.GetAirwayData());
		((JobHandle)(ref m_QueueDependencies)).Complete();
		m_QueueDependencies = default(JobHandle);
		int num = 0;
		SetupQueueItem setupQueueItem = default(SetupQueueItem);
		for (int i = 0; i < m_ActiveQueues.Count; i++)
		{
			SetupQueue setupQueue = m_ActiveQueues[i];
			int num2 = int.MaxValue;
			if (setupQueue.m_SpreadFrame > m_SimulationSystem.frameIndex)
			{
				float num3 = ((m_SimulationSystem.smoothSpeed == 0f) ? 1f : (Time.deltaTime * m_SimulationSystem.smoothSpeed * 60f));
				float num4 = (float)(setupQueue.m_SpreadFrame - m_SimulationSystem.frameIndex) + num3;
				num2 = (int)math.ceil((float)setupQueue.m_Queue.Count * (num3 / num4));
			}
			while (num2-- != 0 && setupQueue.m_Queue.TryDequeue(ref setupQueueItem))
			{
				if (setupQueueItem.m_Parameters.m_ParkingTarget != Entity.Null)
				{
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ConnectionLane>(setupQueueItem.m_Parameters.m_ParkingTarget))
					{
						setupQueueItem.m_Parameters.m_ParkingDelta = -1f;
					}
				}
				PathfindAction action = new PathfindAction(0, 0, (Allocator)4, setupQueueItem.m_Parameters, setupQueueItem.m_Origin.m_Type, setupQueueItem.m_Destination.m_Type);
				ref NativeList<SetupListItem> reference = ref m_SetupList;
				SetupListItem setupListItem = new SetupListItem(setupQueueItem.m_Origin, setupQueueItem.m_Parameters, setupQueueItem.m_Owner, RandomSeed.Next(), m_ActionList.Count, actionStart: true);
				reference.Add(ref setupListItem);
				ref NativeList<SetupListItem> reference2 = ref m_SetupList;
				setupListItem = new SetupListItem(setupQueueItem.m_Destination, setupQueueItem.m_Parameters, setupQueueItem.m_Owner, RandomSeed.Next(), m_ActionList.Count, actionStart: false);
				reference2.Add(ref setupListItem);
				m_ActionList.Add(new ActionListItem(action, setupQueueItem.m_Owner, setupQueue.m_ResultFrame, setupQueue.m_System));
			}
			if (setupQueue.m_Queue.IsEmpty())
			{
				m_FreeQueues.Add(setupQueue);
			}
			else
			{
				m_ActiveQueues[num++] = setupQueue;
			}
		}
		if (m_ActiveQueues.Count > num)
		{
			m_ActiveQueues.RemoveRange(num, m_ActiveQueues.Count - num);
		}
		if (m_SetupList.Length == 0)
		{
			m_QueueSimulationFrameIndex = uint.MaxValue;
			return;
		}
		NativeSortExtension.Sort<SetupListItem>(m_SetupList);
		m_SetupSimulationFrameIndex = m_QueueSimulationFrameIndex;
		m_QueueSimulationFrameIndex = uint.MaxValue;
		m_PendingRequestCount = m_ActionList.Count;
		int num5 = 0;
		int j = 1;
		SetupTargetType setupTargetType = m_SetupList[num5].m_Target.m_Type;
		for (; j < m_SetupList.Length; j++)
		{
			SetupTargetType type = m_SetupList[j].m_Target.m_Type;
			if (setupTargetType != type)
			{
				FindTargets(num5, j);
				num5 = j;
				setupTargetType = type;
			}
		}
		FindTargets(num5, j);
	}

	public void CompleteSetup()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		m_SetupSimulationFrameIndex = uint.MaxValue;
		m_PendingRequestCount = 0;
		((JobHandle)(ref m_SetupDependencies)).Complete();
		m_SetupDependencies = default(JobHandle);
		for (int i = 0; i < m_SetupList.Length; i++)
		{
			SetupListItem setupListItem = m_SetupList[i];
			ActionListItem value = m_ActionList[setupListItem.m_ActionIndex];
			if (setupListItem.m_ActionStart)
			{
				value.m_Action.data.m_StartTargets = setupListItem.m_Buffer;
			}
			else
			{
				value.m_Action.data.m_EndTargets = setupListItem.m_Buffer;
			}
			m_ActionList[setupListItem.m_ActionIndex] = value;
		}
		for (int j = 0; j < m_ActionList.Count; j++)
		{
			ActionListItem actionListItem = m_ActionList[j];
			m_PathfindQueueSystem.Enqueue(actionListItem.m_Action, actionListItem.m_Owner, m_SetupDependencies, actionListItem.m_ResultFrame, actionListItem.m_System);
		}
		m_SetupList.Clear();
		m_ActionList.Clear();
	}

	public NativeQueue<SetupQueueItem> GetQueue(object system, int maxDelayFrames, int spreadFrames = 0)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		SetupQueue item;
		if (m_FreeQueues.Count != 0)
		{
			item = m_FreeQueues[m_FreeQueues.Count - 1];
			m_FreeQueues.RemoveAt(m_FreeQueues.Count - 1);
		}
		else
		{
			item = new SetupQueue
			{
				m_Queue = new NativeQueue<SetupQueueItem>(AllocatorHandle.op_Implicit((Allocator)4))
			};
		}
		item.m_ResultFrame = m_SimulationSystem.frameIndex + (uint)maxDelayFrames;
		item.m_SpreadFrame = m_SimulationSystem.frameIndex + (uint)spreadFrames;
		if (item.m_ResultFrame < m_SimulationSystem.frameIndex)
		{
			item.m_ResultFrame = uint.MaxValue;
		}
		m_QueueSimulationFrameIndex = math.min(m_QueueSimulationFrameIndex, item.m_ResultFrame);
		item.m_System = system;
		m_ActiveQueues.Add(item);
		return item.m_Queue;
	}

	public void AddQueueWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_QueueDependencies = JobHandle.CombineDependencies(m_QueueDependencies, handle);
	}

	public EntityQuery GetSetupQuery(params EntityQueryDesc[] entityQueryDesc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ((ComponentSystemBase)this).GetEntityQuery(entityQueryDesc);
	}

	public EntityQuery GetSetupQuery(params ComponentType[] componentTypes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ((ComponentSystemBase)this).GetEntityQuery(componentTypes);
	}

	private unsafe void FindTargets(int startIndex, int endIndex)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		SetupListItem setupListItem = m_SetupList[startIndex];
		NativeQueue<PathfindSetupTarget> targetQueue = default(NativeQueue<PathfindSetupTarget>);
		targetQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		SetupData setupData = new SetupData(startIndex, endIndex, m_SetupList, m_TargetSeekerData, targetQueue.AsParallelWriter());
		DequePathTargetsJob obj = new DequePathTargetsJob
		{
			m_SetupItems = NativeListUnsafeUtility.GetUnsafeReadOnlyPtr<SetupListItem>(m_SetupList) + startIndex,
			m_TargetQueue = targetQueue
		};
		JobHandle val = FindTargets(setupListItem.m_Target.m_Type, in setupData);
		JobHandle val2 = IJobExtensions.Schedule<DequePathTargetsJob>(obj, val);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val);
		m_SetupDependencies = JobHandle.CombineDependencies(m_SetupDependencies, val2);
		targetQueue.Dispose(val2);
	}

	private JobHandle FindTargets(SetupTargetType targetType, in SetupData setupData)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		switch (targetType)
		{
		case SetupTargetType.CurrentLocation:
			return m_CommonPathfindSetup.SetupCurrentLocation(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.AccidentLocation:
			return m_CommonPathfindSetup.SetupAccidentLocation(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Safety:
			return m_CommonPathfindSetup.SetupSafety(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.PostVan:
			return m_PostServicePathfindSetup.SetupPostVans(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.MailTransfer:
			return m_PostServicePathfindSetup.SetupMailTransfer(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.MailBox:
			return m_PostServicePathfindSetup.SetupMailBoxes(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.PostVanRequest:
			return m_PostServicePathfindSetup.SetupPostVanRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.GarbageCollector:
			return m_GarbagePathfindSetup.SetupGarbageCollector(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.GarbageTransfer:
			return m_GarbagePathfindSetup.SetupGarbageTransfer(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.GarbageCollectorRequest:
			return m_GarbagePathfindSetup.SetupGarbageCollectorRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Taxi:
			return m_TransportPathfindSetup.SetupTaxi(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.TransportVehicle:
			return m_TransportPathfindSetup.SetupTransportVehicle(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.RouteWaypoints:
			return m_TransportPathfindSetup.SetupRouteWaypoints(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.TransportVehicleRequest:
			return m_TransportPathfindSetup.SetupTransportVehicleRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.TaxiRequest:
			return m_TransportPathfindSetup.SetupTaxiRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.CrimeProducer:
			return m_PolicePathfindSetup.SetupCrimeProducer(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.PolicePatrol:
			return m_PolicePathfindSetup.SetupPolicePatrols(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.PrisonerTransport:
			return m_PolicePathfindSetup.SetupPrisonerTransport(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.PrisonerTransportRequest:
			return m_PolicePathfindSetup.SetupPrisonerTransportRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.PoliceRequest:
			return m_PolicePathfindSetup.SetupPoliceRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.EmergencyShelter:
			return m_FirePathfindSetup.SetupEmergencyShelters(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.EvacuationTransport:
			return m_FirePathfindSetup.SetupEvacuationTransport(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.FireEngine:
			return m_FirePathfindSetup.SetupFireEngines(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.EvacuationRequest:
			return m_FirePathfindSetup.SetupEvacuationRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.FireRescueRequest:
			return m_FirePathfindSetup.SetupFireRescueRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Ambulance:
			return m_HealthcarePathfindSetup.SetupAmbulances(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Hospital:
			return m_HealthcarePathfindSetup.SetupHospitals(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Hearse:
			return m_HealthcarePathfindSetup.SetupHearses(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.HealthcareRequest:
			return m_HealthcarePathfindSetup.SetupHealthcareRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.AreaLocation:
			return m_AreaPathfindSetup.SetupAreaLocation(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.WoodResource:
			return m_AreaPathfindSetup.SetupWoodResource(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Maintenance:
			return m_RoadPathfindSetup.SetupMaintenanceProviders(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.RandomTraffic:
			return m_RoadPathfindSetup.SetupRandomTraffic(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.OutsideConnection:
			return m_RoadPathfindSetup.SetupOutsideConnections(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.MaintenanceRequest:
			return m_RoadPathfindSetup.SetupMaintenanceRequest(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.TouristFindTarget:
			return m_CitizenPathfindSetup.SetupTouristTarget(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Leisure:
			return m_CitizenPathfindSetup.SetupLeisureTarget(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.SchoolSeekerTo:
			return m_CitizenPathfindSetup.SetupSchoolSeekerTo(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.JobSeekerTo:
			return m_CitizenPathfindSetup.SetupJobSeekerTo(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Attraction:
			return m_CitizenPathfindSetup.SetupAttraction(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.HomelessShelter:
			return m_CitizenPathfindSetup.SetupHomeless(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.FindHome:
			return m_CitizenPathfindSetup.SetupFindHome(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.Sightseeing:
			return default(JobHandle);
		case SetupTargetType.ResourceSeller:
			return m_ResourcePathfindSetup.SetupResourceSeller(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.ResourceExport:
			return m_ResourcePathfindSetup.SetupResourceExport(this, setupData, ((SystemBase)this).Dependency);
		case SetupTargetType.StorageTransfer:
			return m_ResourcePathfindSetup.SetupStorageTransfer(this, setupData, ((SystemBase)this).Dependency);
		default:
			Debug.LogWarning((object)("Invalid target type in Pathfind setup " + targetType));
			return default(JobHandle);
		}
	}

	[Preserve]
	public PathfindSetupSystem()
	{
	}
}

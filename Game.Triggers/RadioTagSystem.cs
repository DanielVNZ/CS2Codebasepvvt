using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Audio.Radio;
using Game.Simulation;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Triggers;

public class RadioTagSystem : GameSystemBase
{
	private static readonly int kFrameDelay = 50000;

	private static readonly int kMaxBufferSize = 10;

	private NativeParallelHashMap<Entity, uint> m_RecentTags;

	private NativeQueue<RadioTag> m_InputQueue;

	private NativeQueue<RadioTag> m_EmergencyInputQueue;

	private NativeQueue<RadioTag> m_EmergencyQueue;

	private Dictionary<Radio.SegmentType, List<RadioTag>> m_Events;

	private JobHandle m_InputDependencies;

	private JobHandle m_EmergencyInputDependencies;

	private JobHandle m_EmergencyDependencies;

	private SimulationSystem m_SimulationSystem;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_InputQueue = new NativeQueue<RadioTag>(AllocatorHandle.op_Implicit((Allocator)4));
		m_EmergencyInputQueue = new NativeQueue<RadioTag>(AllocatorHandle.op_Implicit((Allocator)4));
		m_EmergencyQueue = new NativeQueue<RadioTag>(AllocatorHandle.op_Implicit((Allocator)4));
		m_RecentTags = new NativeParallelHashMap<Entity, uint>(0, AllocatorHandle.op_Implicit((Allocator)4));
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_Events = new Dictionary<Radio.SegmentType, List<RadioTag>>();
		((ComponentSystemBase)this).Enabled = false;
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_InputDependencies)).Complete();
		((JobHandle)(ref m_EmergencyInputDependencies)).Complete();
		((JobHandle)(ref m_EmergencyDependencies)).Complete();
		RadioTag item = default(RadioTag);
		uint num = default(uint);
		while (m_InputQueue.TryDequeue(ref item))
		{
			List<RadioTag> list = EnsureList(item.m_SegmentType);
			if (!m_RecentTags.TryGetValue(item.m_Event, ref num) || m_SimulationSystem.frameIndex >= num + kFrameDelay)
			{
				while (list.Contains(item))
				{
					list.Remove(item);
				}
				list.Add(item);
				list.RemoveRange(0, math.max(list.Count - kMaxBufferSize, 0));
				m_RecentTags[item.m_Event] = m_SimulationSystem.frameIndex;
			}
		}
		RadioTag radioTag = default(RadioTag);
		uint num2 = default(uint);
		while (m_EmergencyInputQueue.TryDequeue(ref radioTag))
		{
			if (!m_RecentTags.TryGetValue(radioTag.m_Event, ref num2) || m_SimulationSystem.frameIndex >= num2 + radioTag.m_EmergencyFrameDelay)
			{
				m_EmergencyQueue.Enqueue(radioTag);
				m_RecentTags[radioTag.m_Event] = m_SimulationSystem.frameIndex;
			}
		}
	}

	private List<RadioTag> EnsureList(Radio.SegmentType segmentType)
	{
		if (!m_Events.ContainsKey(segmentType))
		{
			m_Events.Add(segmentType, new List<RadioTag>());
		}
		return m_Events[segmentType];
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		Clear();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_InputQueue.Dispose();
		m_EmergencyInputQueue.Dispose();
		m_EmergencyQueue.Dispose();
		m_RecentTags.Dispose();
		base.OnDestroy();
	}

	public bool TryPopEvent(Radio.SegmentType segmentType, bool newestFirst, out RadioTag radioTag)
	{
		List<RadioTag> list = EnsureList(segmentType);
		if (list.Count > 0)
		{
			int index = (newestFirst ? (list.Count - 1) : 0);
			radioTag = list[index];
			list.RemoveAt(index);
			return true;
		}
		radioTag = default(RadioTag);
		return false;
	}

	public void FlushEvents(Radio.SegmentType segmentType)
	{
		EnsureList(segmentType).Clear();
	}

	public NativeQueue<RadioTag> GetInputQueue(out JobHandle deps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(((ComponentSystemBase)this).Enabled, "Can not write to queue when system isn't running");
		deps = m_InputDependencies;
		return m_InputQueue;
	}

	public NativeQueue<RadioTag> GetEmergencyInputQueue(out JobHandle deps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(((ComponentSystemBase)this).Enabled, "Can not write to queue when system isn't running");
		deps = m_EmergencyInputDependencies;
		return m_EmergencyInputQueue;
	}

	public NativeQueue<RadioTag> GetEmergencyQueue(out JobHandle deps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(((ComponentSystemBase)this).Enabled, "Can not write to queue when system isn't running");
		deps = m_EmergencyDependencies;
		return m_EmergencyQueue;
	}

	public void AddInputQueueWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_InputDependencies = JobHandle.CombineDependencies(m_InputDependencies, handle);
	}

	public void AddEmergencyInputQueueWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_EmergencyInputDependencies = JobHandle.CombineDependencies(m_EmergencyInputDependencies, handle);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		Clear();
	}

	private void Clear()
	{
		((JobHandle)(ref m_InputDependencies)).Complete();
		((JobHandle)(ref m_EmergencyInputDependencies)).Complete();
		((JobHandle)(ref m_EmergencyDependencies)).Complete();
		m_InputQueue.Clear();
		m_EmergencyInputQueue.Clear();
		m_EmergencyQueue.Clear();
		m_RecentTags.Clear();
		m_Events.Clear();
	}

	[Preserve]
	public RadioTagSystem()
	{
	}
}

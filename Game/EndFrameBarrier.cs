using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game;

public class EndFrameBarrier : SafeCommandBufferSystem
{
	private Stopwatch m_Stopwatch;

	public JobHandle producerHandle { get; private set; }

	public float lastElapsedTime { get; private set; }

	public float currentElapsedTime => (float)m_Stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;

	[Preserve]
	protected override void OnCreate()
	{
		((EntityCommandBufferSystem)this).OnCreate();
		m_Stopwatch = new Stopwatch();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Stopwatch.Stop();
		((EntityCommandBufferSystem)this).OnDestroy();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		m_Stopwatch.Stop();
		lastElapsedTime = (float)m_Stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
		m_Stopwatch.Reset();
		JobHandle val = producerHandle;
		((JobHandle)(ref val)).Complete();
		val = (producerHandle = default(JobHandle));
		m_Stopwatch.Start();
		base.OnUpdate();
	}

	public void AddJobHandleForProducer(JobHandle producerJob)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		producerHandle = JobHandle.CombineDependencies(producerHandle, producerJob);
	}

	[Preserve]
	public EndFrameBarrier()
	{
	}
}

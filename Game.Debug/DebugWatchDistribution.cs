using System;
using Unity.Collections;
using Unity.Jobs;

namespace Game.Debug;

public class DebugWatchDistribution : IDisposable
{
	public struct ClearJob : IJob
	{
		public NativeQueue<int> m_RawData;

		public void Execute()
		{
			m_RawData.Clear();
		}
	}

	private NativeQueue<int> m_RawData;

	private JobHandle m_Deps;

	private bool m_Persistent;

	private bool m_Relative;

	public bool Persistent => m_Persistent;

	public bool Relative => m_Relative;

	public bool IsEnabled => m_RawData.IsCreated;

	public DebugWatchDistribution(bool persistent = false, bool relative = false)
	{
		m_Persistent = persistent;
		m_Relative = relative;
	}

	public NativeQueue<int> GetQueue(bool clear, out JobHandle deps)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsEnabled)
		{
			throw new Exception("cannot get data queue from disabled DebugWatchDistribution");
		}
		if (clear)
		{
			ClearJob clearJob = new ClearJob
			{
				m_RawData = m_RawData
			};
			m_Deps = IJobExtensions.Schedule<ClearJob>(clearJob, m_Deps);
		}
		deps = m_Deps;
		return m_RawData;
	}

	public void AddWriter(JobHandle handle)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!IsEnabled)
		{
			throw new Exception("cannot add writer to disabled DebugWatchDistribution");
		}
		m_Deps = JobHandle.CombineDependencies(m_Deps, handle);
	}

	public void Enable()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsEnabled)
		{
			m_RawData = new NativeQueue<int>(AllocatorHandle.op_Implicit((Allocator)4));
			m_Deps = default(JobHandle);
		}
	}

	public void Disable()
	{
		if (IsEnabled)
		{
			((JobHandle)(ref m_Deps)).Complete();
			m_RawData.Dispose();
		}
	}

	public void Dispose()
	{
		Disable();
	}
}

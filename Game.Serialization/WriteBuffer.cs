using System;
using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Jobs;

namespace Game.Serialization;

public class WriteBuffer : IWriteBuffer, IDisposable
{
	private JobHandle m_WriteDependencies;

	private bool m_HasDependencies;

	private bool m_IsDone;

	public NativeList<byte> buffer { get; private set; }

	public bool isCompleted
	{
		get
		{
			if (m_IsDone)
			{
				if (m_HasDependencies)
				{
					return ((JobHandle)(ref m_WriteDependencies)).IsCompleted;
				}
				return true;
			}
			return false;
		}
	}

	public WriteBuffer()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		buffer = new NativeList<byte>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	public void CompleteDependencies()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (m_HasDependencies)
		{
			((JobHandle)(ref m_WriteDependencies)).Complete();
			m_WriteDependencies = default(JobHandle);
			m_HasDependencies = false;
		}
	}

	private void DisposeBuffers()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		CompleteDependencies();
		NativeList<byte> val = buffer;
		if (val.IsCreated)
		{
			val.Dispose();
		}
		buffer = val;
	}

	public void Dispose()
	{
		DisposeBuffers();
	}

	public void Done(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = JobHandle.CombineDependencies(m_WriteDependencies, handle);
		m_HasDependencies = true;
		m_IsDone = true;
	}

	public void Done()
	{
		m_IsDone = true;
	}
}

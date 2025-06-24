using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Game.Rendering.Utilities;

public class ManagedStructuredBuffers<T> : IDisposable where T : unmanaged
{
	private const int k_NumBufferedFrames = 3;

	private int m_CurrFrame;

	private int m_CurrSize;

	private List<ComputeBuffer>[] m_UploadBuffers;

	private List<ComputeBuffer> m_ReuseBuffers;

	public ManagedStructuredBuffers(int InitialSize)
	{
		m_CurrFrame = 0;
		m_CurrSize = InitialSize;
		m_UploadBuffers = new List<ComputeBuffer>[3];
		for (int i = 0; i < 3; i++)
		{
			m_UploadBuffers[i] = new List<ComputeBuffer>();
		}
		m_ReuseBuffers = new List<ComputeBuffer>();
	}

	public ComputeBuffer Request(int AmountNeeded)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		ComputeBuffer val = null;
		if (AmountNeeded > m_CurrSize)
		{
			m_CurrSize = AmountNeeded + 1000;
			ReleaseBuffers(m_CurrFrame);
		}
		if (m_ReuseBuffers.Count > 0)
		{
			val = m_ReuseBuffers[m_ReuseBuffers.Count - 1];
			m_ReuseBuffers.RemoveAt(m_ReuseBuffers.Count - 1);
		}
		else
		{
			val = new ComputeBuffer(m_CurrSize, UnsafeUtility.SizeOf<T>(), (ComputeBufferType)16);
		}
		m_UploadBuffers[m_CurrFrame].Add(val);
		return val;
	}

	public void StartFrame()
	{
		List<ComputeBuffer> list = m_UploadBuffers[m_CurrFrame];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].count < m_CurrSize)
			{
				list[i].Release();
			}
			else
			{
				m_ReuseBuffers.Add(list[i]);
			}
		}
		list.Clear();
	}

	public void EndFrame()
	{
		if (++m_CurrFrame >= 3)
		{
			m_CurrFrame = 0;
		}
	}

	public void Dispose()
	{
		ReleaseBuffers();
	}

	private void ReleaseBuffers(int IgnoreIndex = -1)
	{
		for (int i = 0; i < m_ReuseBuffers.Count; i++)
		{
			m_ReuseBuffers[i].Release();
		}
		m_ReuseBuffers.Clear();
		for (int j = 0; j < 3; j++)
		{
			if (j == IgnoreIndex)
			{
				continue;
			}
			foreach (ComputeBuffer item in m_UploadBuffers[j])
			{
				item.Release();
			}
			m_UploadBuffers[j].Clear();
		}
	}
}

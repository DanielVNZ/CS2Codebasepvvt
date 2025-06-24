using System;
using Unity.Collections;

namespace Game.Pathfind;

public struct TimeAction : IDisposable
{
	public NativeQueue<TimeActionData> m_TimeData;

	public TimeAction(Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		m_TimeData = new NativeQueue<TimeActionData>(AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		m_TimeData.Dispose();
	}
}

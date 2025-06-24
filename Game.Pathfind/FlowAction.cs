using System;
using Unity.Collections;

namespace Game.Pathfind;

public struct FlowAction : IDisposable
{
	public NativeQueue<FlowActionData> m_FlowData;

	public FlowAction(Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		m_FlowData = new NativeQueue<FlowActionData>(AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		m_FlowData.Dispose();
	}
}

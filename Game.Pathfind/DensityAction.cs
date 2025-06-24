using System;
using Unity.Collections;

namespace Game.Pathfind;

public struct DensityAction : IDisposable
{
	public NativeQueue<DensityActionData> m_DensityData;

	public DensityAction(Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		m_DensityData = new NativeQueue<DensityActionData>(AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		m_DensityData.Dispose();
	}
}

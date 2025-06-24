using System;
using Unity.Collections;

namespace Game.Pathfind;

public struct UpdateAction : IDisposable
{
	public NativeArray<UpdateActionData> m_UpdateData;

	public UpdateAction(int size, Allocator allocator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_UpdateData = new NativeArray<UpdateActionData>(size, allocator, (NativeArrayOptions)1);
	}

	public void Dispose()
	{
		m_UpdateData.Dispose();
	}
}

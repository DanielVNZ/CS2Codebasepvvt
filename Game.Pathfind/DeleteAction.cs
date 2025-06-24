using System;
using Unity.Collections;

namespace Game.Pathfind;

public struct DeleteAction : IDisposable
{
	public NativeArray<DeleteActionData> m_DeleteData;

	public DeleteAction(int size, Allocator allocator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_DeleteData = new NativeArray<DeleteActionData>(size, allocator, (NativeArrayOptions)1);
	}

	public void Dispose()
	{
		m_DeleteData.Dispose();
	}
}

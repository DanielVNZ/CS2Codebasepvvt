using System;
using Unity.Collections;

namespace Game.Pathfind;

public struct CreateAction : IDisposable
{
	public NativeArray<CreateActionData> m_CreateData;

	public CreateAction(int size, Allocator allocator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_CreateData = new NativeArray<CreateActionData>(size, allocator, (NativeArrayOptions)1);
	}

	public void Dispose()
	{
		m_CreateData.Dispose();
	}
}

using Unity.Collections;

namespace Game.Simulation;

public struct CarLaneSelectBuffer
{
	private NativeArray<float> m_Buffer;

	public NativeArray<float> Ensure()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Buffer.IsCreated)
		{
			m_Buffer = new NativeArray<float>(64, (Allocator)2, (NativeArrayOptions)0);
		}
		return m_Buffer;
	}

	public void Dispose()
	{
	}
}

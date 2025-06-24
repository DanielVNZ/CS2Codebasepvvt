using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Jobs;

namespace Game.Serialization;

public class ReadBuffer : IReadBuffer
{
	public NativeArray<byte> buffer { get; private set; }

	public NativeReference<int> position { get; private set; }

	public ReadBuffer(int size)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		buffer = new NativeArray<byte>(size, (Allocator)3, (NativeArrayOptions)1);
		position = new NativeReference<int>(0, AllocatorHandle.op_Implicit((Allocator)3));
	}

	public void Done(JobHandle handle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		buffer.Dispose(handle);
		position.Dispose(handle);
	}

	public void Done()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		buffer.Dispose();
		position.Dispose();
	}
}

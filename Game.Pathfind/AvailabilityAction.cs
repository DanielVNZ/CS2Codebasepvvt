using System;
using Colossal.Collections;
using Unity.Collections;

namespace Game.Pathfind;

public struct AvailabilityAction : IDisposable
{
	public NativeReference<AvailabilityActionData> m_Data;

	public ref AvailabilityActionData data => ref CollectionUtils.ValueAsRef<AvailabilityActionData>(m_Data);

	public AvailabilityAction(Allocator allocator, AvailabilityParameters parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		m_Data = new NativeReference<AvailabilityActionData>(new AvailabilityActionData(allocator, parameters), AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		data.Dispose();
		m_Data.Dispose();
	}
}

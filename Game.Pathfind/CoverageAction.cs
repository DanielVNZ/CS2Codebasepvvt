using System;
using Colossal.Collections;
using Unity.Collections;

namespace Game.Pathfind;

public struct CoverageAction : IDisposable
{
	public NativeReference<CoverageActionData> m_Data;

	public ref CoverageActionData data => ref CollectionUtils.ValueAsRef<CoverageActionData>(m_Data);

	public CoverageAction(Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_Data = new NativeReference<CoverageActionData>(new CoverageActionData(allocator), AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		data.Dispose();
		m_Data.Dispose();
	}
}

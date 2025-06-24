using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Game.Pathfind;

public struct CoverageActionData : IDisposable
{
	public UnsafeQueue<PathTarget> m_Sources;

	public UnsafeList<CoverageResult> m_Results;

	public CoverageParameters m_Parameters;

	public PathfindActionState m_State;

	public CoverageActionData(Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		m_Sources = new UnsafeQueue<PathTarget>(AllocatorHandle.op_Implicit(allocator));
		m_Results = new UnsafeList<CoverageResult>(100, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
		m_Parameters = default(CoverageParameters);
		m_State = PathfindActionState.Pending;
	}

	public void Dispose()
	{
		m_Sources.Dispose();
		m_Results.Dispose();
	}
}

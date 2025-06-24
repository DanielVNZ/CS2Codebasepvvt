using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Game.Pathfind;

public struct AvailabilityActionData : IDisposable
{
	public UnsafeQueue<PathTarget> m_Sources;

	public UnsafeQueue<AvailabilityProvider> m_Providers;

	public UnsafeList<AvailabilityResult> m_Results;

	public AvailabilityParameters m_Parameters;

	public PathfindActionState m_State;

	public AvailabilityActionData(Allocator allocator, AvailabilityParameters parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		m_Sources = new UnsafeQueue<PathTarget>(AllocatorHandle.op_Implicit(allocator));
		m_Providers = new UnsafeQueue<AvailabilityProvider>(AllocatorHandle.op_Implicit(allocator));
		m_Results = new UnsafeList<AvailabilityResult>(100, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
		m_Parameters = parameters;
		m_State = PathfindActionState.Pending;
	}

	public void Dispose()
	{
		m_Sources.Dispose();
		m_Providers.Dispose();
		m_Results.Dispose();
	}
}

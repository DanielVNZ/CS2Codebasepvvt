using System;
using Colossal.Collections;
using Unity.Collections;

namespace Game.Pathfind;

public struct PathfindAction : IDisposable
{
	public NativeReference<PathfindActionData> m_Data;

	public ref PathfindActionData data => ref CollectionUtils.ValueAsRef<PathfindActionData>(m_Data);

	public PathfindActionData readOnlyData => m_Data.Value;

	public PathfindAction(int startCount, int endCount, Allocator allocator, PathfindParameters parameters, SetupTargetType originType, SetupTargetType destinationType)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		m_Data = new NativeReference<PathfindActionData>(new PathfindActionData(startCount, endCount, allocator, parameters, originType, destinationType), AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		data.Dispose();
		m_Data.Dispose();
	}
}

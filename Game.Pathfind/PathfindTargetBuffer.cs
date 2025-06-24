using Unity.Collections;

namespace Game.Pathfind;

public struct PathfindTargetBuffer : IPathfindTargetBuffer
{
	private ParallelWriter<PathTarget> m_Queue;

	public void Enqueue(PathTarget pathTarget)
	{
		m_Queue.Enqueue(pathTarget);
	}

	public static implicit operator PathfindTargetBuffer(ParallelWriter<PathTarget> queue)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new PathfindTargetBuffer
		{
			m_Queue = queue
		};
	}
}

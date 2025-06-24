using Unity.Mathematics;

namespace Game.Pathfind;

public struct PathfindWeights
{
	public float4 m_Value;

	public float time => m_Value.x;

	public float money => m_Value.z;

	public PathfindWeights(float time, float behaviour, float money, float comfort)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		m_Value = new float4(time, behaviour, money, comfort);
	}

	public static PathfindWeights operator *(float x, PathfindWeights w)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new PathfindWeights
		{
			m_Value = x * w.m_Value
		};
	}
}

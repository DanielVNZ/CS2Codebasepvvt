using Unity.Mathematics;

namespace Game.Pathfind;

public struct PathfindCosts
{
	public float4 m_Value;

	public PathfindCosts(float time, float behaviour, float money, float comfort)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		m_Value = new float4(time, behaviour, money, comfort);
	}
}

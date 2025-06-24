using Unity.Entities;

namespace Game.Pathfind;

public struct PathTarget
{
	public Entity m_Target;

	public Entity m_Entity;

	public float m_Delta;

	public float m_Cost;

	public EdgeFlags m_Flags;

	public PathTarget(Entity target, Entity entity, float delta, float cost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_Entity = entity;
		m_Delta = delta;
		m_Cost = cost;
		m_Flags = EdgeFlags.DefaultMask;
	}

	public PathTarget(Entity target, Entity entity, float delta, float cost, EdgeFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_Entity = entity;
		m_Delta = delta;
		m_Cost = cost;
		m_Flags = flags;
	}
}

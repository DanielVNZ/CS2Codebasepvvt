using Unity.Entities;

namespace Game.Policies;

public struct Modify : IComponentData, IQueryTypeParameter
{
	public Entity m_Entity;

	public Entity m_Policy;

	public PolicyFlags m_Flags;

	public float m_Adjustment;

	public Modify(Entity entity, Entity policy, bool active, float adjustment)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Entity = entity;
		m_Policy = policy;
		m_Flags = (active ? PolicyFlags.Active : ((PolicyFlags)0));
		m_Adjustment = adjustment;
	}
}

using Unity.Entities;

namespace Game.Objects;

public struct SubObjectsUpdated : IComponentData, IQueryTypeParameter
{
	public Entity m_Owner;

	public SubObjectsUpdated(Entity owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Owner = owner;
	}
}

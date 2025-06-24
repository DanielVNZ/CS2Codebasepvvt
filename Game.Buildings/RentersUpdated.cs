using Unity.Entities;

namespace Game.Buildings;

public struct RentersUpdated : IComponentData, IQueryTypeParameter
{
	public Entity m_Property;

	public RentersUpdated(Entity property)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Property = property;
	}
}

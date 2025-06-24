using Unity.Entities;

namespace Game.Routes;

public struct ColorUpdated : IComponentData, IQueryTypeParameter
{
	public Entity m_Route;

	public ColorUpdated(Entity route)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Route = route;
	}
}

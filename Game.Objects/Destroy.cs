using Unity.Entities;

namespace Game.Objects;

public struct Destroy : IComponentData, IQueryTypeParameter
{
	public Entity m_Object;

	public Entity m_Event;

	public Destroy(Entity _object, Entity _event)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Object = _object;
		m_Event = _event;
	}
}

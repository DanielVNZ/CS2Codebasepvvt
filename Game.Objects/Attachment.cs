using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Attachment : IComponentData, IQueryTypeParameter, IEmptySerializable
{
	public Entity m_Attached;

	public Attachment(Entity attached)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Attached = attached;
	}
}

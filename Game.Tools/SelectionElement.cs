using Unity.Entities;

namespace Game.Tools;

[InternalBufferCapacity(0)]
public struct SelectionElement : IBufferElementData
{
	public Entity m_Entity;

	public SelectionElement(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Entity = entity;
	}
}

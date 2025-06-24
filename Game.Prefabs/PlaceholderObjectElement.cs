using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct PlaceholderObjectElement : IBufferElementData
{
	public Entity m_Object;

	public PlaceholderObjectElement(Entity obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Object = obj;
	}
}

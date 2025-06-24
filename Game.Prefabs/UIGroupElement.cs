using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct UIGroupElement : IBufferElementData
{
	public Entity m_Prefab;

	public UIGroupElement(Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = prefab;
	}
}

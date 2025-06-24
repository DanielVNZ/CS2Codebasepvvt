using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct OverlayElement : IBufferElementData
{
	public Entity m_Overlay;

	public int m_SortOrder;
}

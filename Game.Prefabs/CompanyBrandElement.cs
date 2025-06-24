using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct CompanyBrandElement : IBufferElementData
{
	public Entity m_Brand;

	public CompanyBrandElement(Entity brand)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Brand = brand;
	}
}

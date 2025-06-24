using System;
using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct AffiliatedBrandElement : IBufferElementData, IComparable<AffiliatedBrandElement>
{
	public Entity m_Brand;

	public int CompareTo(AffiliatedBrandElement other)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return m_Brand.Index - other.m_Brand.Index;
	}
}

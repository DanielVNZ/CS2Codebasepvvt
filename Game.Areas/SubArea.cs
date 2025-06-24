using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Areas;

[InternalBufferCapacity(0)]
public struct SubArea : IBufferElementData, IEquatable<SubArea>, IEmptySerializable
{
	public Entity m_Area;

	public SubArea(Entity area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Area = area;
	}

	public bool Equals(SubArea other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Area)).Equals(other.m_Area);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Area)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

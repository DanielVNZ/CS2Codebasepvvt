using System;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace Game.Areas;

public struct AreaSearchItem : IEquatable<AreaSearchItem>
{
	public Entity m_Area;

	public int m_Triangle;

	public AreaSearchItem(Entity area, int triangle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Area = area;
		m_Triangle = triangle;
	}

	public bool Equals(AreaSearchItem other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Area)).Equals(other.m_Area) & m_Triangle.Equals(other.m_Triangle);
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Area)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_Triangle.GetHashCode();
	}
}

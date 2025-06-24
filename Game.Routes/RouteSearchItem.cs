using System;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace Game.Routes;

public struct RouteSearchItem : IEquatable<RouteSearchItem>
{
	public Entity m_Entity;

	public int m_Element;

	public RouteSearchItem(Entity entity, int element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Entity = entity;
		m_Element = element;
	}

	public bool Equals(RouteSearchItem other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Entity)).Equals(other.m_Entity) & m_Element.Equals(other.m_Element);
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Entity)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_Element.GetHashCode();
	}
}

using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Notifications;

[InternalBufferCapacity(4)]
public struct IconElement : IBufferElementData, IEquatable<IconElement>, IEmptySerializable
{
	public Entity m_Icon;

	public IconElement(Entity icon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Icon = icon;
	}

	public bool Equals(IconElement other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Icon)).Equals(other.m_Icon);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Icon)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

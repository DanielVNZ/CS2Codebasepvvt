using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

[InternalBufferCapacity(0)]
public struct SubObject : IBufferElementData, IEquatable<SubObject>, IEmptySerializable
{
	public Entity m_SubObject;

	public SubObject(Entity subObject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SubObject = subObject;
	}

	public bool Equals(SubObject other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_SubObject)).Equals(other.m_SubObject);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SubObject)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

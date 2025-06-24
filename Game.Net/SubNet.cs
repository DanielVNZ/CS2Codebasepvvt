using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

[InternalBufferCapacity(0)]
public struct SubNet : IBufferElementData, IEquatable<SubNet>, IEmptySerializable
{
	public Entity m_SubNet;

	public SubNet(Entity subNet)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SubNet = subNet;
	}

	public bool Equals(SubNet other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_SubNet)).Equals(other.m_SubNet);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SubNet)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

[InternalBufferCapacity(4)]
public struct ConnectedEdge : IBufferElementData, IEquatable<ConnectedEdge>, IEmptySerializable
{
	public Entity m_Edge;

	public ConnectedEdge(Entity edge)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Edge = edge;
	}

	public bool Equals(ConnectedEdge other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Edge)).Equals(other.m_Edge);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Edge)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

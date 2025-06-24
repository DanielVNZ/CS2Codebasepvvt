using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

[InternalBufferCapacity(0)]
public struct ConnectedRoute : IBufferElementData, IEquatable<ConnectedRoute>, IEmptySerializable
{
	public Entity m_Waypoint;

	public ConnectedRoute(Entity waypoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Waypoint = waypoint;
	}

	public bool Equals(ConnectedRoute other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Waypoint)).Equals(other.m_Waypoint);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Waypoint)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

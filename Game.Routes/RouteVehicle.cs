using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

[InternalBufferCapacity(0)]
public struct RouteVehicle : IBufferElementData, IEquatable<RouteVehicle>, IEmptySerializable
{
	public Entity m_Vehicle;

	public RouteVehicle(Entity vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Vehicle = vehicle;
	}

	public bool Equals(RouteVehicle other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Vehicle)).Equals(other.m_Vehicle);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Vehicle)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

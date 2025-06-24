using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

[InternalBufferCapacity(0)]
public struct Passenger : IBufferElementData, IEquatable<Passenger>, IEmptySerializable
{
	public Entity m_Passenger;

	public Passenger(Entity passenger)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Passenger = passenger;
	}

	public bool Equals(Passenger other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Passenger)).Equals(other.m_Passenger);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Passenger)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

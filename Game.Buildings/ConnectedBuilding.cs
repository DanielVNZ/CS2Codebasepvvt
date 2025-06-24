using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

[InternalBufferCapacity(0)]
public struct ConnectedBuilding : IBufferElementData, IEquatable<ConnectedBuilding>, IEmptySerializable
{
	public Entity m_Building;

	public ConnectedBuilding(Entity building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Building = building;
	}

	public bool Equals(ConnectedBuilding other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Building)).Equals(other.m_Building);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Building)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

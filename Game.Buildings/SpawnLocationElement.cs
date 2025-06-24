using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

[InternalBufferCapacity(0)]
public struct SpawnLocationElement : IBufferElementData, IEquatable<SpawnLocationElement>, IEmptySerializable
{
	public Entity m_SpawnLocation;

	public SpawnLocationType m_Type;

	public SpawnLocationElement(Entity spawnLocation, SpawnLocationType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SpawnLocation = spawnLocation;
		m_Type = type;
	}

	public bool Equals(SpawnLocationElement other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_SpawnLocation)).Equals(other.m_SpawnLocation);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SpawnLocation)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

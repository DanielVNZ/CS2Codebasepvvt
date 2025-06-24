using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Creatures;

[InternalBufferCapacity(0)]
public struct OwnedCreature : IBufferElementData, IEquatable<OwnedCreature>, IEmptySerializable
{
	public Entity m_Creature;

	public OwnedCreature(Entity creature)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Creature = creature;
	}

	public bool Equals(OwnedCreature other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Creature)).Equals(other.m_Creature);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Creature)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

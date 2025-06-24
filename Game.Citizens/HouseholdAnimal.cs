using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

[InternalBufferCapacity(1)]
public struct HouseholdAnimal : IBufferElementData, IEquatable<HouseholdAnimal>, IEmptySerializable
{
	public Entity m_HouseholdPet;

	public HouseholdAnimal(Entity householdPet)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_HouseholdPet = householdPet;
	}

	public bool Equals(HouseholdAnimal other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_HouseholdPet)).Equals(other.m_HouseholdPet);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_HouseholdPet)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

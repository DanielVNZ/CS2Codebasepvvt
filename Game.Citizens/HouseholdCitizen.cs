using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

[InternalBufferCapacity(5)]
public struct HouseholdCitizen : IBufferElementData, IEquatable<HouseholdCitizen>, IEmptySerializable
{
	public Entity m_Citizen;

	public HouseholdCitizen(Entity citizen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Citizen = citizen;
	}

	public bool Equals(HouseholdCitizen other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Citizen)).Equals(other.m_Citizen);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Citizen)/*cast due to .constrained prefix*/).GetHashCode();
	}

	public static implicit operator Entity(HouseholdCitizen citizen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return citizen.m_Citizen;
	}
}

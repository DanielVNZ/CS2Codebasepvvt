using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct Renter : IBufferElementData, IEmptySerializable
{
	public Entity m_Renter;

	public static implicit operator Entity(Renter renter)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return renter.m_Renter;
	}
}

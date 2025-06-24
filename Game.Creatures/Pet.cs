using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Creatures;

public struct Pet : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_HouseholdPet;

	public PetFlags m_Flags;

	public Pet(Entity householdPet)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_HouseholdPet = householdPet;
		m_Flags = PetFlags.None;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)m_Flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (PetFlags)flags;
	}
}

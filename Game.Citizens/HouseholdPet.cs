using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct HouseholdPet : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Household;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Household);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Household);
	}
}

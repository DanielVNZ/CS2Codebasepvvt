using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct StreetLight : IComponentData, IQueryTypeParameter, ISerializable
{
	public StreetLightState m_State;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_State);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte state = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		m_State = (StreetLightState)state;
	}
}

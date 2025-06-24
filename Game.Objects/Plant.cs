using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Plant : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Pollution;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Pollution);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Pollution);
	}
}

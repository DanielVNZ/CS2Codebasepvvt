using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Quantity : IComponentData, IQueryTypeParameter, ISerializable
{
	public byte m_Fullness;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Fullness);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Fullness);
	}
}

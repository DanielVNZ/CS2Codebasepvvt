using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct Creditworthiness : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Amount;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Amount);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Amount);
	}
}

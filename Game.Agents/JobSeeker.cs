using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Agents;

public struct JobSeeker : IComponentData, IQueryTypeParameter, ISerializable
{
	public byte m_Level;

	public byte m_Outside;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Level);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Level);
	}
}

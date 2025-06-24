using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct WorkStopData : IComponentData, IQueryTypeParameter, ISerializable
{
	public bool m_WorkLocation;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_WorkLocation);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_WorkLocation);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct HearseData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_CorpseCapacity;

	public HearseData(int corpseCapacity)
	{
		m_CorpseCapacity = corpseCapacity;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_CorpseCapacity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_CorpseCapacity);
	}
}

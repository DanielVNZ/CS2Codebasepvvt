using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct AttractionData : IComponentData, IQueryTypeParameter, ICombineData<AttractionData>, ISerializable
{
	public int m_Attractiveness;

	public void Combine(AttractionData otherData)
	{
		m_Attractiveness += otherData.m_Attractiveness;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Attractiveness);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Attractiveness);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct LaneCondition : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Wear;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Wear);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Wear);
	}
}

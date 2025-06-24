using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct DangerLevel : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_DangerLevel;

	public DangerLevel(float dangerLevel)
	{
		m_DangerLevel = dangerLevel;
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_DangerLevel);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_DangerLevel);
	}
}

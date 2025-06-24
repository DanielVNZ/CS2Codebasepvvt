using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct FloodCounterData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_FloodCounter;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_FloodCounter);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_FloodCounter);
	}
}

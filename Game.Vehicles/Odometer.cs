using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Odometer : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Distance;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Distance);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Distance);
	}
}

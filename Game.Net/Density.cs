using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct Density : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Density;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Density);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Density);
	}
}

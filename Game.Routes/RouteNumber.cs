using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct RouteNumber : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Number;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Number);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Number);
	}
}

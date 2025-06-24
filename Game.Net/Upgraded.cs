using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Net;

public struct Upgraded : IComponentData, IQueryTypeParameter, ISerializable
{
	public CompositionFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write<CompositionFlags>(m_Flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read<CompositionFlags>(ref m_Flags);
	}
}

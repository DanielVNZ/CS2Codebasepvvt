using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct BuildOrder : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_Start;

	public uint m_End;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		uint start = m_Start;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(start);
		uint end = m_End;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(end);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref uint start = ref m_Start;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref start);
		ref uint end = ref m_End;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref end);
	}
}

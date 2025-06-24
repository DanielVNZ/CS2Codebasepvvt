using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.City;

public struct City : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_OptionMask;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_OptionMask);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_OptionMask);
	}
}

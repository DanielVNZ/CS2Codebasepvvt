using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.City;

public struct TaxRates : IBufferElementData, ISerializable
{
	public int m_TaxRate;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_TaxRate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_TaxRate);
	}
}

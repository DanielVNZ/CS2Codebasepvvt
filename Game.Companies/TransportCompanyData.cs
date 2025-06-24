using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct TransportCompanyData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_MaxTransports;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_MaxTransports);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_MaxTransports);
	}
}

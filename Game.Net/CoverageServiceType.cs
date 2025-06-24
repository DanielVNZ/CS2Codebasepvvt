using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct CoverageServiceType : ISharedComponentData, IQueryTypeParameter, ISerializable
{
	public CoverageService m_Service;

	public CoverageServiceType(CoverageService service)
	{
		m_Service = service;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_Service);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte service = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref service);
		m_Service = (CoverageService)service;
	}
}

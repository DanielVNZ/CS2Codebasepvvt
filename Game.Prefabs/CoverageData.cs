using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct CoverageData : IComponentData, IQueryTypeParameter, ISerializable
{
	public CoverageService m_Service;

	public float m_Range;

	public float m_Capacity;

	public float m_Magnitude;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float range = m_Range;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(range);
		float capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		float magnitude = m_Magnitude;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(magnitude);
		CoverageService service = m_Service;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)service);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float range = ref m_Range;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref range);
		ref float capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		ref float magnitude = ref m_Magnitude;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref magnitude);
		byte service = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref service);
		m_Service = (CoverageService)service;
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct ServiceAvailable : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_ServiceAvailable;

	public float m_MeanPriority;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int serviceAvailable = m_ServiceAvailable;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(serviceAvailable);
		float meanPriority = m_MeanPriority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(meanPriority);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int serviceAvailable = ref m_ServiceAvailable;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref serviceAvailable);
		ref float meanPriority = ref m_MeanPriority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref meanPriority);
	}
}

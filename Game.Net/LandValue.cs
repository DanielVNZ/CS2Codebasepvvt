using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct LandValue : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_LandValue;

	public float m_Weight;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float landValue = m_LandValue;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(landValue);
		float weight = m_Weight;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(weight);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float landValue = ref m_LandValue;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref landValue);
		ref float weight = ref m_Weight;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref weight);
	}
}

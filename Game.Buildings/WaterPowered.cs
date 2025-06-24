using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct WaterPowered : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Length;

	public float m_Height;

	public float m_Estimate;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float length = m_Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		float height = m_Height;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(height);
		float estimate = m_Estimate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(estimate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float length = ref m_Length;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref length);
		ref float height = ref m_Height;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref height);
		ref float estimate = ref m_Estimate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref estimate);
	}
}

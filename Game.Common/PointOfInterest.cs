using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Common;

public struct PointOfInterest : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_Position;

	public bool m_IsValid;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float3 position = m_Position;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		bool isValid = m_IsValid;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(isValid);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float3 position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		ref bool isValid = ref m_IsValid;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref isValid);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct CarTractorData : IComponentData, IQueryTypeParameter, ISerializable
{
	public CarTrailerType m_TrailerType;

	public float3 m_AttachPosition;

	public Entity m_FixedTrailer;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_AttachPosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_AttachPosition);
	}
}

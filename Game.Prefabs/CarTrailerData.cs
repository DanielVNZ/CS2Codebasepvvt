using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct CarTrailerData : IComponentData, IQueryTypeParameter, ISerializable
{
	public CarTrailerType m_TrailerType;

	public TrailerMovementType m_MovementType;

	public float3 m_AttachPosition;

	public Entity m_FixedTractor;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		byte num = (byte)m_MovementType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		float3 attachPosition = m_AttachPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(attachPosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte movementType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref movementType);
		ref float3 attachPosition = ref m_AttachPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref attachPosition);
		m_MovementType = (TrailerMovementType)movementType;
	}
}

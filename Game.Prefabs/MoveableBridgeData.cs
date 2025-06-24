using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct MoveableBridgeData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_LiftOffsets;

	public float m_MovingTime;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float3 liftOffsets = m_LiftOffsets;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(liftOffsets);
		float movingTime = m_MovingTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(movingTime);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float3 liftOffsets = ref m_LiftOffsets;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref liftOffsets);
		ref float movingTime = ref m_MovingTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref movingTime);
	}
}

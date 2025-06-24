using Colossal.Serialization.Entities;
using Game.Objects;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct TrainBogiePosition : ISerializable
{
	public float3 m_Position;

	public float3 m_Direction;

	public TrainBogiePosition(Transform transform)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_Position = transform.m_Position;
		m_Direction = math.forward(transform.m_Rotation);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 position = m_Position;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		float3 direction = m_Direction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(direction);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float3 position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		ref float3 direction = ref m_Direction;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref direction);
	}
}

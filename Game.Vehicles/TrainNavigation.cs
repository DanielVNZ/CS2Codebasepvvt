using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct TrainNavigation : IComponentData, IQueryTypeParameter, ISerializable
{
	public TrainBogiePosition m_Front;

	public TrainBogiePosition m_Rear;

	public float m_Speed;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		TrainBogiePosition front = m_Front;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<TrainBogiePosition>(front);
		TrainBogiePosition rear = m_Rear;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<TrainBogiePosition>(rear);
		float speed = m_Speed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(speed);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref TrainBogiePosition front = ref m_Front;
		((IReader)reader/*cast due to .constrained prefix*/).Read<TrainBogiePosition>(ref front);
		ref TrainBogiePosition rear = ref m_Rear;
		((IReader)reader/*cast due to .constrained prefix*/).Read<TrainBogiePosition>(ref rear);
		ref float speed = ref m_Speed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref speed);
	}
}

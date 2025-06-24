using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Notifications;

public struct Animation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Timer;

	public float m_Duration;

	public AnimationType m_Type;

	public Animation(AnimationType type, float timer, float duration)
	{
		m_Timer = timer;
		m_Duration = duration;
		m_Type = type;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float timer = m_Timer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(timer);
		float duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		AnimationType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)type);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float timer = ref m_Timer;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref timer);
		ref float duration = ref m_Duration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
		byte type = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		m_Type = (AnimationType)type;
	}
}

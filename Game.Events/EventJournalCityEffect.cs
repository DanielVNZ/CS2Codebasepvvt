using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct EventJournalCityEffect : IBufferElementData, ISerializable
{
	public EventCityEffectTrackingType m_Type;

	public int m_StartValue;

	public int m_Value;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		int type = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		m_Type = (EventCityEffectTrackingType)type;
		ref int startValue = ref m_StartValue;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startValue);
		ref int value = ref m_Value;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		EventCityEffectTrackingType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)type);
		int startValue = m_StartValue;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startValue);
		int value = m_Value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value);
	}
}

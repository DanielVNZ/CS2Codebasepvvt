using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct EventJournalData : IBufferElementData, ISerializable
{
	public EventDataTrackingType m_Type;

	public int m_Value;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		int type = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		m_Type = (EventDataTrackingType)type;
		ref int value = ref m_Value;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		EventDataTrackingType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)type);
		int value = m_Value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value);
	}
}

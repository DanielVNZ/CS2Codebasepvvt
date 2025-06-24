using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct JournalEvent : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_JournalEntity;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_JournalEntity);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_JournalEntity);
	}
}

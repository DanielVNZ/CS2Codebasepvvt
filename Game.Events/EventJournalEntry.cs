using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct EventJournalEntry : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public uint m_StartFrame;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref uint startFrame = ref m_StartFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startFrame);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		uint startFrame = m_StartFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startFrame);
	}
}

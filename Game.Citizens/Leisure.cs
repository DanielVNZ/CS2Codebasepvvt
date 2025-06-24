using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct Leisure : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetAgent;

	public uint m_LastPossibleFrame;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetAgent = m_TargetAgent;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetAgent);
		uint lastPossibleFrame = m_LastPossibleFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastPossibleFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity targetAgent = ref m_TargetAgent;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetAgent);
		ref uint lastPossibleFrame = ref m_LastPossibleFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastPossibleFrame);
	}
}

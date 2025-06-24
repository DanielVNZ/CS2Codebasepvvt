using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct Duration : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_StartFrame;

	public uint m_EndFrame;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		uint startFrame = m_StartFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startFrame);
		uint endFrame = m_EndFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref uint startFrame = ref m_StartFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startFrame);
		ref uint endFrame = ref m_EndFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref endFrame);
	}
}

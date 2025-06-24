using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct UpdateFrame : ISharedComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_Index;

	public UpdateFrame(uint index)
	{
		m_Index = index;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Index);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Index);
	}
}

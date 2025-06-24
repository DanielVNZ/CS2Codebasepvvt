using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Creatures;

public struct Human : IComponentData, IQueryTypeParameter, ISerializable
{
	public HumanFlags m_Flags;

	public Human(HumanFlags flags)
	{
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)m_Flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (HumanFlags)flags;
	}
}

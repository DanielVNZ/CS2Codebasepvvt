using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Watercraft : IComponentData, IQueryTypeParameter, ISerializable
{
	public WatercraftFlags m_Flags;

	public Watercraft(WatercraftFlags flags)
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
		m_Flags = (WatercraftFlags)flags;
	}
}

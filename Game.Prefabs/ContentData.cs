using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct ContentData : IComponentData, IQueryTypeParameter, ISerializable
{
	public ContentFlags m_Flags;

	public int m_DlcID;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ContentFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		int dlcID = m_DlcID;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dlcID);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref int dlcID = ref m_DlcID;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref dlcID);
		m_Flags = (ContentFlags)flags;
	}
}

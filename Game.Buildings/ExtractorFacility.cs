using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct ExtractorFacility : IComponentData, IQueryTypeParameter, ISerializable
{
	public ExtractorFlags m_Flags;

	public byte m_Timer;

	public BuildingFlags m_MainBuildingFlags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ExtractorFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte timer = m_Timer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(timer);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref byte timer = ref m_Timer;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref timer);
		m_Flags = (ExtractorFlags)flags;
	}
}

using Colossal.Serialization.Entities;
using Game.Zones;
using Unity.Entities;

namespace Game.Prefabs;

public struct ZoneData : IComponentData, IQueryTypeParameter, ISerializable
{
	public ZoneType m_ZoneType;

	public AreaType m_AreaType;

	public ZoneFlags m_ZoneFlags;

	public ushort m_MinOddHeight;

	public ushort m_MinEvenHeight;

	public ushort m_MaxHeight;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ZoneType zoneType = m_ZoneType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<ZoneType>(zoneType);
		AreaType areaType = m_AreaType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)areaType);
		ZoneFlags zoneFlags = m_ZoneFlags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)zoneFlags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref ZoneType zoneType = ref m_ZoneType;
		((IReader)reader/*cast due to .constrained prefix*/).Read<ZoneType>(ref zoneType);
		byte areaType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref areaType);
		byte zoneFlags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref zoneFlags);
		m_AreaType = (AreaType)areaType;
		m_ZoneFlags = (ZoneFlags)zoneFlags;
	}

	public bool IsOffice()
	{
		return (m_ZoneFlags & ZoneFlags.Office) != 0;
	}
}

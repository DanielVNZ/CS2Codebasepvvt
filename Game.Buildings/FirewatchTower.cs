using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct FirewatchTower : IComponentData, IQueryTypeParameter, IEmptySerializable
{
	public FirewatchTowerFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_Flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (FirewatchTowerFlags)flags;
	}
}

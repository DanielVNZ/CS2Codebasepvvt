using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct ParkingFacility : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_ComfortFactor;

	public ParkingFacilityFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float comfortFactor = m_ComfortFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(comfortFactor);
		ParkingFacilityFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float comfortFactor = ref m_ComfortFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref comfortFactor);
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (ParkingFacilityFlags)flags;
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct ParkingFacilityData : IComponentData, IQueryTypeParameter, ICombineData<ParkingFacilityData>, ISerializable
{
	public float m_ComfortFactor;

	public int m_GarageMarkerCapacity;

	public void Combine(ParkingFacilityData otherData)
	{
		m_ComfortFactor += otherData.m_ComfortFactor;
		m_GarageMarkerCapacity += otherData.m_GarageMarkerCapacity;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float comfortFactor = m_ComfortFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(comfortFactor);
		int garageMarkerCapacity = m_GarageMarkerCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(garageMarkerCapacity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float comfortFactor = ref m_ComfortFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref comfortFactor);
		ref int garageMarkerCapacity = ref m_GarageMarkerCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref garageMarkerCapacity);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct GarageLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public ushort m_ParkingFee;

	public ushort m_ComfortFactor;

	public ushort m_VehicleCount;

	public ushort m_VehicleCapacity;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ushort parkingFee = m_ParkingFee;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(parkingFee);
		ushort comfortFactor = m_ComfortFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(comfortFactor);
		ushort vehicleCount = m_VehicleCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleCount);
		ushort vehicleCapacity = m_VehicleCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleCapacity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref ushort parkingFee = ref m_ParkingFee;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref parkingFee);
		ref ushort comfortFactor = ref m_ComfortFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref comfortFactor);
		ref ushort vehicleCount = ref m_VehicleCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleCount);
		ref ushort vehicleCapacity = ref m_VehicleCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleCapacity);
	}
}

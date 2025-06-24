using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PublicTransportVehicleData : IComponentData, IQueryTypeParameter, ISerializable
{
	public TransportType m_TransportType;

	public int m_PassengerCapacity;

	public PublicTransportPurpose m_PurposeMask;

	public float m_MaintenanceRange;

	public PublicTransportVehicleData(TransportType type, int passengerCapacity, PublicTransportPurpose purposeMask, float maintenanceRange)
	{
		m_TransportType = type;
		m_PassengerCapacity = passengerCapacity;
		m_PurposeMask = purposeMask;
		m_MaintenanceRange = maintenanceRange;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		sbyte num = (sbyte)m_TransportType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		PublicTransportPurpose purposeMask = m_PurposeMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)purposeMask);
		int passengerCapacity = m_PassengerCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(passengerCapacity);
		float maintenanceRange = m_MaintenanceRange;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenanceRange);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		sbyte transportType = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref transportType);
		uint purposeMask = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purposeMask);
		ref int passengerCapacity = ref m_PassengerCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref passengerCapacity);
		ref float maintenanceRange = ref m_MaintenanceRange;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceRange);
		m_TransportType = (TransportType)transportType;
		m_PurposeMask = (PublicTransportPurpose)purposeMask;
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct TaxiData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_PassengerCapacity;

	public float m_MaintenanceRange;

	public TaxiData(int passengerCapacity, float maintenanceRange)
	{
		m_PassengerCapacity = passengerCapacity;
		m_MaintenanceRange = maintenanceRange;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int passengerCapacity = m_PassengerCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(passengerCapacity);
		float maintenanceRange = m_MaintenanceRange;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenanceRange);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int passengerCapacity = ref m_PassengerCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref passengerCapacity);
		ref float maintenanceRange = ref m_MaintenanceRange;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceRange);
	}
}

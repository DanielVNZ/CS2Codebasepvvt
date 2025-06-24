using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Prefabs;

public struct CargoTransportVehicleData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Resource m_Resources;

	public int m_CargoCapacity;

	public int m_MaxResourceCount;

	public float m_MaintenanceRange;

	public CargoTransportVehicleData(Resource resources, int cargoCapacity, int maxResourceCount, float maintenanceRange)
	{
		m_Resources = resources;
		m_CargoCapacity = cargoCapacity;
		m_MaxResourceCount = maxResourceCount;
		m_MaintenanceRange = maintenanceRange;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		Resource resources = m_Resources;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)resources);
		int cargoCapacity = m_CargoCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cargoCapacity);
		int maxResourceCount = m_MaxResourceCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxResourceCount);
		float maintenanceRange = m_MaintenanceRange;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenanceRange);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ulong resources = default(ulong);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref resources);
		ref int cargoCapacity = ref m_CargoCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref cargoCapacity);
		ref int maxResourceCount = ref m_MaxResourceCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxResourceCount);
		ref float maintenanceRange = ref m_MaintenanceRange;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceRange);
		m_Resources = (Resource)resources;
	}
}

using Colossal.Serialization.Entities;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

public struct MaintenanceVehicleData : IComponentData, IQueryTypeParameter, ISerializable
{
	public MaintenanceType m_MaintenanceType;

	public int m_MaintenanceCapacity;

	public int m_MaintenanceRate;

	public MaintenanceVehicleData(MaintenanceType maintenanceType, int maintenanceCapacity, int maintenanceRate)
	{
		m_MaintenanceType = maintenanceType;
		m_MaintenanceCapacity = maintenanceCapacity;
		m_MaintenanceRate = maintenanceRate;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		MaintenanceType maintenanceType = m_MaintenanceType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)maintenanceType);
		int maintenanceCapacity = m_MaintenanceCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenanceCapacity);
		int maintenanceRate = m_MaintenanceRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenanceRate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte maintenanceType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceType);
		ref int maintenanceCapacity = ref m_MaintenanceCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceCapacity);
		ref int maintenanceRate = ref m_MaintenanceRate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceRate);
		m_MaintenanceType = (MaintenanceType)maintenanceType;
	}
}

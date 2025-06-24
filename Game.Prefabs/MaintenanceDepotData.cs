using Colossal.Serialization.Entities;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

public struct MaintenanceDepotData : IComponentData, IQueryTypeParameter, ICombineData<MaintenanceDepotData>, ISerializable
{
	public MaintenanceType m_MaintenanceType;

	public int m_VehicleCapacity;

	public float m_VehicleEfficiency;

	public void Combine(MaintenanceDepotData otherData)
	{
		m_MaintenanceType |= otherData.m_MaintenanceType;
		m_VehicleCapacity += otherData.m_VehicleCapacity;
		m_VehicleEfficiency += otherData.m_VehicleEfficiency;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int vehicleCapacity = m_VehicleCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleCapacity);
		float vehicleEfficiency = m_VehicleEfficiency;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleEfficiency);
		MaintenanceType maintenanceType = m_MaintenanceType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)maintenanceType);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int vehicleCapacity = ref m_VehicleCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleCapacity);
		ref float vehicleEfficiency = ref m_VehicleEfficiency;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleEfficiency);
		byte maintenanceType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceType);
		m_MaintenanceType = (MaintenanceType)maintenanceType;
	}
}

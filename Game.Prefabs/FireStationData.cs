using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct FireStationData : IComponentData, IQueryTypeParameter, ICombineData<FireStationData>, ISerializable
{
	public int m_FireEngineCapacity;

	public int m_FireHelicopterCapacity;

	public int m_DisasterResponseCapacity;

	public float m_VehicleEfficiency;

	public void Combine(FireStationData otherData)
	{
		m_FireEngineCapacity += otherData.m_FireEngineCapacity;
		m_FireHelicopterCapacity += otherData.m_FireHelicopterCapacity;
		m_DisasterResponseCapacity += otherData.m_DisasterResponseCapacity;
		m_VehicleEfficiency += otherData.m_VehicleEfficiency;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int fireEngineCapacity = m_FireEngineCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fireEngineCapacity);
		int fireHelicopterCapacity = m_FireHelicopterCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fireHelicopterCapacity);
		int disasterResponseCapacity = m_DisasterResponseCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(disasterResponseCapacity);
		float vehicleEfficiency = m_VehicleEfficiency;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleEfficiency);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int fireEngineCapacity = ref m_FireEngineCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref fireEngineCapacity);
		ref int fireHelicopterCapacity = ref m_FireHelicopterCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref fireHelicopterCapacity);
		ref int disasterResponseCapacity = ref m_DisasterResponseCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref disasterResponseCapacity);
		ref float vehicleEfficiency = ref m_VehicleEfficiency;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleEfficiency);
	}
}

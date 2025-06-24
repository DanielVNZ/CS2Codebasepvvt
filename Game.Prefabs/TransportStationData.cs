using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct TransportStationData : IComponentData, IQueryTypeParameter, ICombineData<TransportStationData>, ISerializable
{
	public float m_ComfortFactor;

	public float m_LoadingFactor;

	public EnergyTypes m_CarRefuelTypes;

	public EnergyTypes m_TrainRefuelTypes;

	public EnergyTypes m_WatercraftRefuelTypes;

	public EnergyTypes m_AircraftRefuelTypes;

	public void Combine(TransportStationData otherData)
	{
		m_ComfortFactor += otherData.m_ComfortFactor;
		m_LoadingFactor += otherData.m_LoadingFactor;
		m_CarRefuelTypes |= otherData.m_CarRefuelTypes;
		m_TrainRefuelTypes |= otherData.m_TrainRefuelTypes;
		m_WatercraftRefuelTypes |= otherData.m_WatercraftRefuelTypes;
		m_AircraftRefuelTypes |= otherData.m_AircraftRefuelTypes;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float comfortFactor = m_ComfortFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(comfortFactor);
		float loadingFactor = m_LoadingFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(loadingFactor);
		EnergyTypes carRefuelTypes = m_CarRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)carRefuelTypes);
		EnergyTypes trainRefuelTypes = m_TrainRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trainRefuelTypes);
		EnergyTypes watercraftRefuelTypes = m_WatercraftRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)watercraftRefuelTypes);
		EnergyTypes aircraftRefuelTypes = m_AircraftRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)aircraftRefuelTypes);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float comfortFactor = ref m_ComfortFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref comfortFactor);
		ref float loadingFactor = ref m_LoadingFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref loadingFactor);
		byte carRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref carRefuelTypes);
		byte trainRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trainRefuelTypes);
		byte watercraftRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref watercraftRefuelTypes);
		byte aircraftRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref aircraftRefuelTypes);
		m_CarRefuelTypes = (EnergyTypes)carRefuelTypes;
		m_TrainRefuelTypes = (EnergyTypes)trainRefuelTypes;
		m_WatercraftRefuelTypes = (EnergyTypes)watercraftRefuelTypes;
		m_AircraftRefuelTypes = (EnergyTypes)aircraftRefuelTypes;
	}
}

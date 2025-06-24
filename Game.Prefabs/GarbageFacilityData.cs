using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct GarbageFacilityData : IComponentData, IQueryTypeParameter, ICombineData<GarbageFacilityData>, ISerializable
{
	public int m_GarbageCapacity;

	public int m_VehicleCapacity;

	public int m_TransportCapacity;

	public int m_ProcessingSpeed;

	public bool m_IndustrialWasteOnly;

	public bool m_LongTermStorage;

	public void Combine(GarbageFacilityData otherData)
	{
		m_GarbageCapacity += otherData.m_GarbageCapacity;
		m_VehicleCapacity += otherData.m_VehicleCapacity;
		m_TransportCapacity += otherData.m_TransportCapacity;
		m_ProcessingSpeed += otherData.m_ProcessingSpeed;
		m_IndustrialWasteOnly |= otherData.m_IndustrialWasteOnly;
		m_LongTermStorage |= otherData.m_LongTermStorage;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int garbageCapacity = m_GarbageCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(garbageCapacity);
		int vehicleCapacity = m_VehicleCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleCapacity);
		int transportCapacity = m_TransportCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(transportCapacity);
		int processingSpeed = m_ProcessingSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(processingSpeed);
		bool industrialWasteOnly = m_IndustrialWasteOnly;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(industrialWasteOnly);
		bool longTermStorage = m_LongTermStorage;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(longTermStorage);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int garbageCapacity = ref m_GarbageCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref garbageCapacity);
		ref int vehicleCapacity = ref m_VehicleCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleCapacity);
		ref int transportCapacity = ref m_TransportCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref transportCapacity);
		ref int processingSpeed = ref m_ProcessingSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref processingSpeed);
		ref bool industrialWasteOnly = ref m_IndustrialWasteOnly;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref industrialWasteOnly);
		ref bool longTermStorage = ref m_LongTermStorage;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref longTermStorage);
	}
}

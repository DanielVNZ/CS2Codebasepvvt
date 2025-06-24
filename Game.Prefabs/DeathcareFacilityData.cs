using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct DeathcareFacilityData : IComponentData, IQueryTypeParameter, ICombineData<DeathcareFacilityData>, ISerializable
{
	public int m_HearseCapacity;

	public int m_StorageCapacity;

	public float m_ProcessingRate;

	public bool m_LongTermStorage;

	public void Combine(DeathcareFacilityData otherData)
	{
		m_HearseCapacity += otherData.m_HearseCapacity;
		m_StorageCapacity += otherData.m_StorageCapacity;
		m_ProcessingRate += otherData.m_ProcessingRate;
		m_LongTermStorage |= otherData.m_LongTermStorage;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int hearseCapacity = m_HearseCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(hearseCapacity);
		int storageCapacity = m_StorageCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(storageCapacity);
		float processingRate = m_ProcessingRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(processingRate);
		bool longTermStorage = m_LongTermStorage;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(longTermStorage);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int hearseCapacity = ref m_HearseCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref hearseCapacity);
		ref int storageCapacity = ref m_StorageCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref storageCapacity);
		ref float processingRate = ref m_ProcessingRate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref processingRate);
		ref bool longTermStorage = ref m_LongTermStorage;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref longTermStorage);
	}
}

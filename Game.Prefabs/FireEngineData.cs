using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct FireEngineData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_ExtinguishingRate;

	public float m_ExtinguishingSpread;

	public float m_ExtinguishingCapacity;

	public float m_DestroyedClearDuration;

	public FireEngineData(float extinguishingRate, float extinguishingSpread, float extinguishingCapacity, float destroyedClearDuration)
	{
		m_ExtinguishingRate = extinguishingRate;
		m_ExtinguishingSpread = extinguishingSpread;
		m_ExtinguishingCapacity = extinguishingCapacity;
		m_DestroyedClearDuration = destroyedClearDuration;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float extinguishingRate = m_ExtinguishingRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(extinguishingRate);
		float extinguishingSpread = m_ExtinguishingSpread;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(extinguishingSpread);
		float extinguishingCapacity = m_ExtinguishingCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(extinguishingCapacity);
		float destroyedClearDuration = m_DestroyedClearDuration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(destroyedClearDuration);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float extinguishingRate = ref m_ExtinguishingRate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref extinguishingRate);
		ref float extinguishingSpread = ref m_ExtinguishingSpread;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref extinguishingSpread);
		ref float extinguishingCapacity = ref m_ExtinguishingCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref extinguishingCapacity);
		ref float destroyedClearDuration = ref m_DestroyedClearDuration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref destroyedClearDuration);
	}
}

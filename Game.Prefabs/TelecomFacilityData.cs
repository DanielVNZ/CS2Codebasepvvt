using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct TelecomFacilityData : IComponentData, IQueryTypeParameter, ICombineData<TelecomFacilityData>, ISerializable
{
	public float m_Range;

	public float m_NetworkCapacity;

	public bool m_PenetrateTerrain;

	public void Combine(TelecomFacilityData otherData)
	{
		m_Range += otherData.m_Range;
		m_NetworkCapacity += otherData.m_NetworkCapacity;
		m_PenetrateTerrain |= otherData.m_PenetrateTerrain;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float range = m_Range;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(range);
		float networkCapacity = m_NetworkCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(networkCapacity);
		bool penetrateTerrain = m_PenetrateTerrain;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(penetrateTerrain);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float range = ref m_Range;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref range);
		ref float networkCapacity = ref m_NetworkCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref networkCapacity);
		ref bool penetrateTerrain = ref m_PenetrateTerrain;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref penetrateTerrain);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct SewageOutletData : IComponentData, IQueryTypeParameter, ICombineData<SewageOutletData>, ISerializable
{
	public int m_Capacity;

	public float m_Purification;

	public void Combine(SewageOutletData otherData)
	{
		m_Capacity += otherData.m_Capacity;
		m_Purification += otherData.m_Purification;
		m_Purification = math.min(1f, m_Purification);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		float purification = m_Purification;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(purification);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		ref float purification = ref m_Purification;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purification);
	}
}

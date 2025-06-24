using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct WaterPumpingStationData : IComponentData, IQueryTypeParameter, ICombineData<WaterPumpingStationData>, ISerializable
{
	public AllowedWaterTypes m_Types;

	public int m_Capacity;

	public float m_Purification;

	public void Combine(WaterPumpingStationData otherData)
	{
		m_Types |= otherData.m_Types;
		m_Capacity += otherData.m_Capacity;
		m_Purification = 1f - (1f - m_Purification) * (1f - otherData.m_Purification);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		float purification = m_Purification;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(purification);
		ushort num = (ushort)m_Types;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		ref float purification = ref m_Purification;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purification);
		ushort types = default(ushort);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref types);
		m_Types = (AllowedWaterTypes)types;
	}
}

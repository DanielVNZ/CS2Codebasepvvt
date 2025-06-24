using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct BatteryData : IComponentData, IQueryTypeParameter, ICombineData<BatteryData>, ISerializable
{
	public int m_Capacity;

	public int m_PowerOutput;

	public long capacityTicks => 85 * m_Capacity;

	public void Combine(BatteryData otherData)
	{
		m_Capacity += otherData.m_Capacity;
		m_PowerOutput += otherData.m_PowerOutput;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		int powerOutput = m_PowerOutput;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(powerOutput);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		ref int powerOutput = ref m_PowerOutput;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref powerOutput);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct GarbagePoweredData : IComponentData, IQueryTypeParameter, ICombineData<GarbagePoweredData>, ISerializable
{
	public int m_Capacity;

	public float m_ProductionPerUnit;

	public void Combine(GarbagePoweredData otherData)
	{
		m_Capacity += otherData.m_Capacity;
		m_ProductionPerUnit = math.max(m_ProductionPerUnit, otherData.m_ProductionPerUnit);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		float productionPerUnit = m_ProductionPerUnit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(productionPerUnit);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		ref float productionPerUnit = ref m_ProductionPerUnit;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref productionPerUnit);
	}
}

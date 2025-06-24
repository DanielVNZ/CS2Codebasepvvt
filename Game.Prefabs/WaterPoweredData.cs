using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct WaterPoweredData : IComponentData, IQueryTypeParameter, ICombineData<WaterPoweredData>, ISerializable
{
	public float m_ProductionFactor;

	public float m_CapacityFactor;

	public void Combine(WaterPoweredData otherData)
	{
		m_ProductionFactor += otherData.m_ProductionFactor;
		m_CapacityFactor += otherData.m_CapacityFactor;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float productionFactor = m_ProductionFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(productionFactor);
		float capacityFactor = m_CapacityFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacityFactor);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float productionFactor = ref m_ProductionFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref productionFactor);
		ref float capacityFactor = ref m_CapacityFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacityFactor);
	}
}

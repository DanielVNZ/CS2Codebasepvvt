using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct GroundWaterPoweredData : IComponentData, IQueryTypeParameter, ICombineData<GroundWaterPoweredData>, ISerializable
{
	public int m_Production;

	public int m_MaximumGroundWater;

	public void Combine(GroundWaterPoweredData otherData)
	{
		m_Production += otherData.m_Production;
		m_MaximumGroundWater += otherData.m_MaximumGroundWater;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int production = m_Production;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(production);
		int maximumGroundWater = m_MaximumGroundWater;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maximumGroundWater);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int production = ref m_Production;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref production);
		ref int maximumGroundWater = ref m_MaximumGroundWater;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maximumGroundWater);
	}
}

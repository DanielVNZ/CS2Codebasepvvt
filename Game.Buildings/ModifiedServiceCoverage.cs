using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Buildings;

public struct ModifiedServiceCoverage : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Range;

	public float m_Capacity;

	public float m_Magnitude;

	public void ReplaceData(ref CoverageData coverage)
	{
		coverage.m_Capacity = m_Capacity;
		coverage.m_Range = m_Range;
		coverage.m_Magnitude = m_Magnitude;
	}

	public ModifiedServiceCoverage(CoverageData coverage)
	{
		m_Capacity = coverage.m_Capacity;
		m_Range = coverage.m_Range;
		m_Magnitude = coverage.m_Magnitude;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		float range = m_Range;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(range);
		float magnitude = m_Magnitude;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(magnitude);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		float num = default(float);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_Capacity = num;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_Range = num;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_Magnitude = num;
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct WindPoweredData : IComponentData, IQueryTypeParameter, ICombineData<WindPoweredData>, ISerializable
{
	public float m_MaximumWind;

	public int m_Production;

	public void Combine(WindPoweredData otherData)
	{
		if (m_Production > 0)
		{
			m_MaximumWind = math.min(m_MaximumWind, otherData.m_MaximumWind);
		}
		else
		{
			m_MaximumWind = otherData.m_MaximumWind;
		}
		m_Production += otherData.m_Production;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float maximumWind = m_MaximumWind;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maximumWind);
		int production = m_Production;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(production);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float maximumWind = ref m_MaximumWind;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maximumWind);
		ref int production = ref m_Production;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref production);
	}
}

using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct PopulationCell : IPopulationCell, IStrideSerializable, ISerializable
{
	public float m_Population;

	public float Get()
	{
		return m_Population;
	}

	public void Add(float amount)
	{
		m_Population += amount;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Population);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Population);
	}

	public int GetStride(Context context)
	{
		return 4;
	}
}

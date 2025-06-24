using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct TrafficAmbienceCell : IStrideSerializable, ISerializable
{
	public float m_Accumulator;

	public float m_Traffic;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float accumulator = m_Accumulator;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accumulator);
		float traffic = m_Traffic;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(traffic);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float accumulator = ref m_Accumulator;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accumulator);
		ref float traffic = ref m_Traffic;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref traffic);
	}

	public int GetStride(Context context)
	{
		return 8;
	}
}

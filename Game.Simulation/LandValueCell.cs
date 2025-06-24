using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct LandValueCell : ILandValueCell, IStrideSerializable, ISerializable
{
	public float m_LandValue;

	public void Add(float amount)
	{
		m_LandValue += amount;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_LandValue);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_LandValue);
	}

	public int GetStride(Context context)
	{
		return 4;
	}
}

using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct SoilWater : IStrideSerializable, ISerializable
{
	public float m_Surface;

	public short m_Amount;

	public short m_Max;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float surface = m_Surface;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(surface);
		short amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		short max = m_Max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float surface = ref m_Surface;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref surface);
		ref short amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		ref short max = ref m_Max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
	}

	public int GetStride(Context context)
	{
		return 8;
	}
}

using Colossal.Serialization.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct AirPollution : IPollution, IStrideSerializable, ISerializable
{
	public short m_Pollution;

	public void Add(short amount)
	{
		m_Pollution = (short)math.min(32767, m_Pollution + amount);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Pollution);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Pollution);
	}

	public int GetStride(Context context)
	{
		return 2;
	}
}

using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct NaturalResourceAmount : IStrideSerializable, ISerializable
{
	public ushort m_Base;

	public ushort m_Used;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ushort num = m_Base;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		ushort used = m_Used;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(used);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref ushort reference = ref m_Base;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref ushort used = ref m_Used;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref used);
	}

	public int GetStride(Context context)
	{
		return 4;
	}
}

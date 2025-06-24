using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct ZoneAmbienceCell : IStrideSerializable, ISerializable
{
	public ZoneAmbiences m_Accumulator;

	public ZoneAmbiences m_Value;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ZoneAmbiences accumulator = m_Accumulator;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<ZoneAmbiences>(accumulator);
		ZoneAmbiences value = m_Value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<ZoneAmbiences>(value);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref ZoneAmbiences accumulator = ref m_Accumulator;
		((IReader)reader/*cast due to .constrained prefix*/).Read<ZoneAmbiences>(ref accumulator);
		ref ZoneAmbiences value = ref m_Value;
		((IReader)reader/*cast due to .constrained prefix*/).Read<ZoneAmbiences>(ref value);
	}

	public int GetStride(Context context)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return m_Accumulator.GetStride(context) + m_Value.GetStride(context);
	}
}

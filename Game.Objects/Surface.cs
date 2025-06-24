using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Surface : IComponentData, IQueryTypeParameter, ISerializable
{
	public byte m_Wetness;

	public byte m_SnowAmount;

	public byte m_AccumulatedWetness;

	public byte m_AccumulatedSnow;

	public byte m_Dirtyness;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte wetness = m_Wetness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(wetness);
		byte snowAmount = m_SnowAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(snowAmount);
		byte accumulatedWetness = m_AccumulatedWetness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accumulatedWetness);
		byte accumulatedSnow = m_AccumulatedSnow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accumulatedSnow);
		byte dirtyness = m_Dirtyness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dirtyness);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref byte wetness = ref m_Wetness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref wetness);
		ref byte snowAmount = ref m_SnowAmount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref snowAmount);
		ref byte accumulatedWetness = ref m_AccumulatedWetness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accumulatedWetness);
		ref byte accumulatedSnow = ref m_AccumulatedSnow;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accumulatedSnow);
		ref byte dirtyness = ref m_Dirtyness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref dirtyness);
	}
}

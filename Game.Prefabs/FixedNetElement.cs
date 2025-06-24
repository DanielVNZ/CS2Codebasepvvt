using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct FixedNetElement : IBufferElementData, ISerializable
{
	public Bounds1 m_LengthRange;

	public int2 m_CountRange;

	public CompositionFlags m_SetState;

	public CompositionFlags m_UnsetState;

	public FixedNetFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float min = m_LengthRange.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_LengthRange.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
		int2 countRange = m_CountRange;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(countRange);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float min = ref m_LengthRange.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_LengthRange.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		ref int2 countRange = ref m_CountRange;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref countRange);
	}
}

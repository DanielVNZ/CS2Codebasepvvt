using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Stack : IComponentData, IQueryTypeParameter, ISerializable
{
	public Bounds1 m_Range;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float min = m_Range.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_Range.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float min = ref m_Range.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_Range.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
	}
}

using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PillarData : IComponentData, IQueryTypeParameter, ISerializable
{
	public PillarType m_Type;

	public Bounds1 m_OffsetRange;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		PillarType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)type);
		float min = m_OffsetRange.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_OffsetRange.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		int type = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		ref float min = ref m_OffsetRange.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_OffsetRange.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		m_Type = (PillarType)type;
	}
}

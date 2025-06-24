using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct ExtractorFacilityData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Bounds1 m_RotationRange;

	public Bounds1 m_HeightOffset;

	public ExtractorRequirementFlags m_Requirements;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float min = m_RotationRange.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_RotationRange.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
		float min2 = m_HeightOffset.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min2);
		float max2 = m_HeightOffset.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float min = ref m_RotationRange.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_RotationRange.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		ref float min2 = ref m_HeightOffset.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min2);
		ref float max2 = ref m_HeightOffset.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max2);
	}
}

using Colossal.Serialization.Entities;

namespace Game.Simulation;

public struct TerrainAttractiveness : IStrideSerializable, ISerializable
{
	public float m_ShoreBonus;

	public float m_ForestBonus;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float shoreBonus = m_ShoreBonus;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(shoreBonus);
		float forestBonus = m_ForestBonus;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(forestBonus);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float shoreBonus = ref m_ShoreBonus;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref shoreBonus);
		ref float forestBonus = ref m_ForestBonus;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref forestBonus);
	}

	public int GetStride(Context context)
	{
		return 8;
	}
}

using Colossal.Serialization.Entities;
using Game.Areas;
using Unity.Entities;

namespace Game.Prefabs;

public struct ExtractorAreaData : IComponentData, IQueryTypeParameter, ISerializable
{
	public MapFeature m_MapFeature;

	public float m_ObjectSpawnFactor;

	public float m_MaxObjectArea;

	public bool m_RequireNaturalResource;

	public float m_WorkAmountFactor;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		sbyte num = (sbyte)m_MapFeature;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		bool requireNaturalResource = m_RequireNaturalResource;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requireNaturalResource);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		sbyte mapFeature = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref mapFeature);
		ref bool requireNaturalResource = ref m_RequireNaturalResource;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref requireNaturalResource);
		m_MapFeature = (MapFeature)mapFeature;
	}
}

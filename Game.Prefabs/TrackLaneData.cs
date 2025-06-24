using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct TrackLaneData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_FallbackPrefab;

	public Entity m_EndObjectPrefab;

	public TrackTypes m_TrackTypes;

	public float m_MaxCurviness;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_TrackTypes);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte trackTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trackTypes);
		m_TrackTypes = (TrackTypes)trackTypes;
		m_MaxCurviness = float.MaxValue;
	}
}

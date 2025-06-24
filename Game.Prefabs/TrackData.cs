using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct TrackData : IComponentData, IQueryTypeParameter, ISerializable
{
	public TrackTypes m_TrackType;

	public float m_SpeedLimit;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		TrackTypes trackType = m_TrackType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trackType);
		float speedLimit = m_SpeedLimit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(speedLimit);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte trackType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trackType);
		ref float speedLimit = ref m_SpeedLimit;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref speedLimit);
		m_TrackType = (TrackTypes)trackType;
	}
}

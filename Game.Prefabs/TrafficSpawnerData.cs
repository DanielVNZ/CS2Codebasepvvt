using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct TrafficSpawnerData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_SpawnRate;

	public RoadTypes m_RoadType;

	public TrackTypes m_TrackType;

	public bool m_NoSlowVehicles;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float spawnRate = m_SpawnRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(spawnRate);
		bool noSlowVehicles = m_NoSlowVehicles;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(noSlowVehicles);
		RoadTypes roadType = m_RoadType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)roadType);
		TrackTypes trackType = m_TrackType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trackType);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float spawnRate = ref m_SpawnRate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref spawnRate);
		ref bool noSlowVehicles = ref m_NoSlowVehicles;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref noSlowVehicles);
		byte roadType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref roadType);
		byte trackType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trackType);
		m_RoadType = (RoadTypes)roadType;
		m_TrackType = (TrackTypes)trackType;
	}
}

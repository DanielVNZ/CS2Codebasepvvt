using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct RoadData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_ZoneBlockPrefab;

	public float m_SpeedLimit;

	public RoadFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float speedLimit = m_SpeedLimit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(speedLimit);
		RoadFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float speedLimit = ref m_SpeedLimit;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref speedLimit);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (RoadFlags)flags;
	}
}

using Colossal.Serialization.Entities;
using Game.Net;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct CarLaneData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_NotTrackLanePrefab;

	public Entity m_NotBusLanePrefab;

	public RoadTypes m_RoadTypes;

	public SizeClass m_MaxSize;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		RoadTypes roadTypes = m_RoadTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)roadTypes);
		SizeClass maxSize = m_MaxSize;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)maxSize);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte roadTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref roadTypes);
		byte maxSize = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSize);
		m_RoadTypes = (RoadTypes)roadTypes;
		m_MaxSize = (SizeClass)maxSize;
	}
}

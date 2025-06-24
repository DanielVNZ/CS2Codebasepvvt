using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct UtilityLaneData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_LocalConnectionPrefab;

	public Entity m_LocalConnectionPrefab2;

	public Entity m_NodeObjectPrefab;

	public float m_VisualCapacity;

	public float m_Hanging;

	public UtilityTypes m_UtilityTypes;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_UtilityTypes);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte utilityTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref utilityTypes);
		m_UtilityTypes = (UtilityTypes)utilityTypes;
	}
}

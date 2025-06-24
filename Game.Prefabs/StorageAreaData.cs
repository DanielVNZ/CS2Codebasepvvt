using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Prefabs;

public struct StorageAreaData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Resource m_Resources;

	public int m_Capacity;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		Resource resources = m_Resources;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)resources);
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ulong resources = default(ulong);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref resources);
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		m_Resources = (Resource)resources;
	}
}

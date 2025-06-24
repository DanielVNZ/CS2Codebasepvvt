using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Triggers;

[InternalBufferCapacity(2)]
public struct ChirpEntity : IBufferElementData, ISerializable
{
	public Entity m_Entity;

	public ChirpEntity(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Entity = entity;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Entity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Entity);
	}
}

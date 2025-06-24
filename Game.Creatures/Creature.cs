using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Creatures;

public struct Creature : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_QueueEntity;

	public Sphere3 m_QueueArea;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		float radius = m_QueueArea.radius;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(radius);
		if (m_QueueArea.radius > 0f)
		{
			Entity queueEntity = m_QueueEntity;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(queueEntity);
			float3 position = m_QueueArea.position;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float radius = ref m_QueueArea.radius;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref radius);
		if (m_QueueArea.radius > 0f)
		{
			ref Entity queueEntity = ref m_QueueEntity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref queueEntity);
			ref float3 position = ref m_QueueArea.position;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		}
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct EvacuationRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public float m_Priority;

	public EvacuationRequest(Entity target, float priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_Priority = priority;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		float priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		ref float priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
	}
}

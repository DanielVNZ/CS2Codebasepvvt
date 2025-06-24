using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct Flooded : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public float m_Depth;

	public Flooded(Entity _event, float depth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_Depth = depth;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		float depth = m_Depth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(depth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref float depth = ref m_Depth;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref depth);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct FacingWeather : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public float m_Severity;

	public FacingWeather(Entity _event, float severity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_Severity = severity;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		float severity = m_Severity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(severity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref float severity = ref m_Severity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref severity);
	}
}

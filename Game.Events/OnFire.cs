using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct OnFire : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public Entity m_RescueRequest;

	public float m_Intensity;

	public uint m_RequestFrame;

	public OnFire(Entity _event, float intensity, uint requestFrame = 0u)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_RescueRequest = Entity.Null;
		m_Intensity = intensity;
		m_RequestFrame = requestFrame;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		Entity rescueRequest = m_RescueRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(rescueRequest);
		float intensity = m_Intensity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(intensity);
		uint requestFrame = m_RequestFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requestFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref Entity rescueRequest = ref m_RescueRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref rescueRequest);
		ref float intensity = ref m_Intensity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref intensity);
		ref uint requestFrame = ref m_RequestFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref requestFrame);
	}
}

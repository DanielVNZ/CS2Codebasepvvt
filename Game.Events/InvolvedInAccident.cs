using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct InvolvedInAccident : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public float m_Severity;

	public uint m_InvolvedFrame;

	public InvolvedInAccident(Entity _event, float severity, uint simulationFrame)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_Severity = severity;
		m_InvolvedFrame = simulationFrame;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		float severity = m_Severity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(severity);
		uint involvedFrame = m_InvolvedFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(involvedFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref float severity = ref m_Severity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref severity);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.accidentInvolvedFrame)
		{
			ref uint involvedFrame = ref m_InvolvedFrame;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref involvedFrame);
		}
	}
}

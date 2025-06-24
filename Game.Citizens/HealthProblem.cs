using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct HealthProblem : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public Entity m_HealthcareRequest;

	public HealthProblemFlags m_Flags;

	public byte m_Timer;

	public HealthProblem(Entity _event, HealthProblemFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_HealthcareRequest = Entity.Null;
		m_Flags = flags;
		m_Timer = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		Entity healthcareRequest = m_HealthcareRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(healthcareRequest);
		HealthProblemFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte timer = m_Timer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(timer);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref Entity healthcareRequest = ref m_HealthcareRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref healthcareRequest);
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.healthcareNotifications)
		{
			ref byte timer = ref m_Timer;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref timer);
		}
		m_Flags = (HealthProblemFlags)flags;
	}
}

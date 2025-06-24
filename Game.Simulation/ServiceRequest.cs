using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct ServiceRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public byte m_FailCount;

	public byte m_Cooldown;

	public ServiceRequestFlags m_Flags;

	public ServiceRequest(bool reversed)
	{
		m_FailCount = 0;
		m_Cooldown = 0;
		m_Flags = (reversed ? ServiceRequestFlags.Reversed : ((ServiceRequestFlags)0));
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte failCount = m_FailCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(failCount);
		byte cooldown = m_Cooldown;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cooldown);
		ServiceRequestFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref byte failCount = ref m_FailCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref failCount);
		ref byte cooldown = ref m_Cooldown;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref cooldown);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (ServiceRequestFlags)flags;
		}
	}
}

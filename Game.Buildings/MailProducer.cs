using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct MailProducer : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_MailRequest;

	public ushort m_SendingMail;

	public ushort m_ReceivingMail;

	public byte m_DispatchIndex;

	public int receivingMail
	{
		get
		{
			return m_ReceivingMail & 0x7FFF;
		}
		set
		{
			m_ReceivingMail = (ushort)((m_ReceivingMail & 0x8000) | value);
		}
	}

	public bool mailDelivered
	{
		get
		{
			return (m_ReceivingMail & 0x8000) != 0;
		}
		set
		{
			if (value)
			{
				m_ReceivingMail |= 32768;
			}
			else
			{
				m_ReceivingMail &= 32767;
			}
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity mailRequest = m_MailRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(mailRequest);
		ushort sendingMail = m_SendingMail;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sendingMail);
		ushort num = m_ReceivingMail;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		byte dispatchIndex = m_DispatchIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref Entity mailRequest = ref m_MailRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref mailRequest);
		ref ushort sendingMail = ref m_SendingMail;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sendingMail);
		ref ushort reference = ref m_ReceivingMail;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.requestDispatchIndex)
		{
			ref byte dispatchIndex = ref m_DispatchIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchIndex);
		}
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct PostFacility : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_MailDeliverRequest;

	public Entity m_MailReceiveRequest;

	public Entity m_TargetRequest;

	public float m_AcceptMailPriority;

	public float m_DeliverMailPriority;

	public PostFacilityFlags m_Flags;

	public byte m_ProcessingFactor;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Entity mailDeliverRequest = m_MailDeliverRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(mailDeliverRequest);
		Entity mailReceiveRequest = m_MailReceiveRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(mailReceiveRequest);
		float acceptMailPriority = m_AcceptMailPriority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(acceptMailPriority);
		float deliverMailPriority = m_DeliverMailPriority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(deliverMailPriority);
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		PostFacilityFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte processingFactor = m_ProcessingFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(processingFactor);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transferRequestRefactoring)
		{
			ref Entity mailDeliverRequest = ref m_MailDeliverRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref mailDeliverRequest);
			ref Entity mailReceiveRequest = ref m_MailReceiveRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref mailReceiveRequest);
			ref float acceptMailPriority = ref m_AcceptMailPriority;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref acceptMailPriority);
			ref float deliverMailPriority = ref m_DeliverMailPriority;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref deliverMailPriority);
		}
		else
		{
			Entity val = default(Entity);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.mailProcessing)
		{
			ref byte processingFactor = ref m_ProcessingFactor;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref processingFactor);
		}
		m_Flags = (PostFacilityFlags)flags;
	}
}

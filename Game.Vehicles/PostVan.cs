using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct PostVan : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public PostVanFlags m_State;

	public int m_RequestCount;

	public float m_PathElementTime;

	public int m_DeliveringMail;

	public int m_CollectedMail;

	public int m_DeliveryEstimate;

	public int m_CollectEstimate;

	public PostVan(PostVanFlags flags, int requestCount, int deliveringMail)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TargetRequest = Entity.Null;
		m_State = flags;
		m_RequestCount = requestCount;
		m_PathElementTime = 0f;
		m_DeliveringMail = deliveringMail;
		m_CollectedMail = 0;
		m_DeliveryEstimate = 0;
		m_CollectEstimate = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		PostVanFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		int requestCount = m_RequestCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requestCount);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
		int deliveringMail = m_DeliveringMail;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(deliveringMail);
		int collectedMail = m_CollectedMail;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(collectedMail);
		int deliveryEstimate = m_DeliveryEstimate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(deliveryEstimate);
		int collectEstimate = m_CollectEstimate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(collectEstimate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref int requestCount = ref m_RequestCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref requestCount);
		ref float pathElementTime = ref m_PathElementTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.taxiDispatchCenter)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		ref int deliveringMail = ref m_DeliveringMail;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref deliveringMail);
		ref int collectedMail = ref m_CollectedMail;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref collectedMail);
		m_State = (PostVanFlags)state;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeShiftEstimate)
		{
			ref int deliveryEstimate = ref m_DeliveryEstimate;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref deliveryEstimate);
			ref int collectEstimate = ref m_CollectEstimate;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref collectEstimate);
		}
	}
}

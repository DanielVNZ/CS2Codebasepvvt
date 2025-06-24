using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct GarbageFacility : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_GarbageDeliverRequest;

	public Entity m_GarbageReceiveRequest;

	public Entity m_TargetRequest;

	public GarbageFacilityFlags m_Flags;

	public float m_AcceptGarbagePriority;

	public float m_DeliverGarbagePriority;

	public int m_ProcessingRate;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity garbageDeliverRequest = m_GarbageDeliverRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(garbageDeliverRequest);
		Entity garbageReceiveRequest = m_GarbageReceiveRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(garbageReceiveRequest);
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		GarbageFacilityFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		float acceptGarbagePriority = m_AcceptGarbagePriority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(acceptGarbagePriority);
		float deliverGarbagePriority = m_DeliverGarbagePriority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(deliverGarbagePriority);
		int processingRate = m_ProcessingRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(processingRate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transferRequestRefactoring)
		{
			ref Entity garbageDeliverRequest = ref m_GarbageDeliverRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref garbageDeliverRequest);
			ref Entity garbageReceiveRequest = ref m_GarbageReceiveRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref garbageReceiveRequest);
		}
		else
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.garbageFacilityRefactor2)
			{
				Entity val = default(Entity);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			}
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
		if (((Context)(ref context)).version >= Version.garbageFacilityRefactor)
		{
			ref float acceptGarbagePriority = ref m_AcceptGarbagePriority;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref acceptGarbagePriority);
			ref float deliverGarbagePriority = ref m_DeliverGarbagePriority;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref deliverGarbagePriority);
		}
		else
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.garbageProcessing)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.powerPlantConsumption)
			{
				float num2 = default(float);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.powerPlantConsumption)
		{
			ref int processingRate = ref m_ProcessingRate;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref processingRate);
		}
		m_Flags = (GarbageFacilityFlags)flags;
	}
}

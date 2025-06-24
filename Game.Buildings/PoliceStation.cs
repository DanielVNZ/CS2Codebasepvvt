using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Buildings;

public struct PoliceStation : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_PrisonerTransportRequest;

	public Entity m_TargetRequest;

	public PoliceStationFlags m_Flags;

	public PolicePurpose m_PurposeMask;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity prisonerTransportRequest = m_PrisonerTransportRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prisonerTransportRequest);
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		PoliceStationFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		PolicePurpose purposeMask = m_PurposeMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)purposeMask);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeImprovement2)
		{
			ref Entity prisonerTransportRequest = ref m_PrisonerTransportRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref prisonerTransportRequest);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (PoliceStationFlags)flags;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeImprovement3)
		{
			int purposeMask = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref purposeMask);
			m_PurposeMask = (PolicePurpose)purposeMask;
		}
		else
		{
			m_PurposeMask = PolicePurpose.Patrol | PolicePurpose.Emergency;
		}
	}
}

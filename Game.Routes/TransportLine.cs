using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Routes;

public struct TransportLine : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_VehicleRequest;

	public float m_VehicleInterval;

	public float m_UnbunchingFactor;

	public TransportLineFlags m_Flags;

	public ushort m_TicketPrice;

	public TransportLine(TransportLineData transportLineData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_VehicleRequest = Entity.Null;
		m_VehicleInterval = transportLineData.m_DefaultVehicleInterval;
		m_UnbunchingFactor = transportLineData.m_DefaultUnbunchingFactor;
		m_Flags = (TransportLineFlags)0;
		m_TicketPrice = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity vehicleRequest = m_VehicleRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleRequest);
		float vehicleInterval = m_VehicleInterval;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleInterval);
		float unbunchingFactor = m_UnbunchingFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(unbunchingFactor);
		TransportLineFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)flags);
		ushort ticketPrice = m_TicketPrice;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(ticketPrice);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		ref Entity vehicleRequest = ref m_VehicleRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleRequest);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.routeVehicleInterval)
		{
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		ref float vehicleInterval = ref m_VehicleInterval;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleInterval);
		ref float unbunchingFactor = ref m_UnbunchingFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref unbunchingFactor);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transportLineFlags)
		{
			ushort flags = default(ushort);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (TransportLineFlags)flags;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transportLinePolicies)
		{
			ref ushort ticketPrice = ref m_TicketPrice;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref ticketPrice);
		}
	}
}

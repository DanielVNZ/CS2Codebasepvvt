using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct TransportDepot : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public TransportDepotFlags m_Flags;

	public byte m_AvailableVehicles;

	public float m_MaintenanceRequirement;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		TransportDepotFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte availableVehicles = m_AvailableVehicles;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(availableVehicles);
		float maintenanceRequirement = m_MaintenanceRequirement;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenanceRequirement);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiDispatchCenter)
		{
			ref byte availableVehicles = ref m_AvailableVehicles;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref availableVehicles);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transportMaintenance)
		{
			ref float maintenanceRequirement = ref m_MaintenanceRequirement;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceRequirement);
		}
		m_Flags = (TransportDepotFlags)flags;
	}
}

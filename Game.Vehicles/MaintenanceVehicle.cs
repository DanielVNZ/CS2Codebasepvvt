using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct MaintenanceVehicle : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public MaintenanceVehicleFlags m_State;

	public int m_Maintained;

	public int m_MaintainEstimate;

	public int m_RequestCount;

	public float m_PathElementTime;

	public float m_Efficiency;

	public MaintenanceVehicle(MaintenanceVehicleFlags flags, int requestCount, float efficiency)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TargetRequest = Entity.Null;
		m_State = flags;
		m_Maintained = 0;
		m_MaintainEstimate = 0;
		m_RequestCount = requestCount;
		m_PathElementTime = 0f;
		m_Efficiency = efficiency;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		MaintenanceVehicleFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		int maintained = m_Maintained;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintained);
		int maintainEstimate = m_MaintainEstimate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintainEstimate);
		int requestCount = m_RequestCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requestCount);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
		float efficiency = m_Efficiency;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(efficiency);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref int maintained = ref m_Maintained;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintained);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeShiftEstimate)
		{
			ref int maintainEstimate = ref m_MaintainEstimate;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintainEstimate);
		}
		ref int requestCount = ref m_RequestCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref requestCount);
		ref float pathElementTime = ref m_PathElementTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.maintenanceImprovement)
		{
			ref float efficiency = ref m_Efficiency;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref efficiency);
		}
		m_State = (MaintenanceVehicleFlags)state;
	}
}

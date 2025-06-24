using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Ambulance : IComponentData, IQueryTypeParameter, ISerializable
{
	public AmbulanceFlags m_State;

	public Entity m_TargetPatient;

	public Entity m_TargetLocation;

	public Entity m_TargetRequest;

	public float m_PathElementTime;

	public Ambulance(Entity targetPatient, Entity targetLocation, AmbulanceFlags state)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		m_State = state;
		m_TargetPatient = targetPatient;
		m_TargetLocation = targetLocation;
		m_TargetRequest = Entity.Null;
		m_PathElementTime = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		AmbulanceFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		Entity targetPatient = m_TargetPatient;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetPatient);
		Entity targetLocation = m_TargetLocation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetLocation);
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref Entity targetPatient = ref m_TargetPatient;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetPatient);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.healthcareImprovement2)
		{
			ref Entity targetLocation = ref m_TargetLocation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetLocation);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		ref float pathElementTime = ref m_PathElementTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		m_State = (AmbulanceFlags)state;
	}
}

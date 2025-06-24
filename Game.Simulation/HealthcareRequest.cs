using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct HealthcareRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Citizen;

	public HealthcareRequestType m_Type;

	public HealthcareRequest(Entity citizen, HealthcareRequestType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Citizen = citizen;
		m_Type = type;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity citizen = m_Citizen;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(citizen);
		HealthcareRequestType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)type);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity citizen = ref m_Citizen;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref citizen);
		byte type = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		m_Type = (HealthcareRequestType)type;
	}
}

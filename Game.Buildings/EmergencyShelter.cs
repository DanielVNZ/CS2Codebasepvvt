using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct EmergencyShelter : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public EmergencyShelterFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		EmergencyShelterFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (EmergencyShelterFlags)flags;
	}
}

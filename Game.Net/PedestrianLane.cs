using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct PedestrianLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_AccessRestriction;

	public PedestrianLaneFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity accessRestriction = m_AccessRestriction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accessRestriction);
		PedestrianLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindAccessRestriction)
		{
			ref Entity accessRestriction = ref m_AccessRestriction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRestriction);
		}
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (PedestrianLaneFlags)flags;
	}
}

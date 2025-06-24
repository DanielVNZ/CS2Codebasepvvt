using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct TrackLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_AccessRestriction;

	public TrackLaneFlags m_Flags;

	public float m_SpeedLimit;

	public float m_Curviness;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity accessRestriction = m_AccessRestriction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accessRestriction);
		TrackLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float speedLimit = m_SpeedLimit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(speedLimit);
		float curviness = m_Curviness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curviness);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindRestrictions)
		{
			ref Entity accessRestriction = ref m_AccessRestriction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRestriction);
		}
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float speedLimit = ref m_SpeedLimit;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref speedLimit);
		ref float curviness = ref m_Curviness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curviness);
		m_Flags = (TrackLaneFlags)flags;
	}
}

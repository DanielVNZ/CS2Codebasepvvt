using System;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct ConnectionLane : IComponentData, IQueryTypeParameter, ISerializable, IEquatable<ConnectionLane>
{
	public Entity m_AccessRestriction;

	public ConnectionLaneFlags m_Flags;

	public TrackTypes m_TrackTypes;

	public RoadTypes m_RoadTypes;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity accessRestriction = m_AccessRestriction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accessRestriction);
		ConnectionLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		TrackTypes trackTypes = m_TrackTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trackTypes);
		RoadTypes roadTypes = m_RoadTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)roadTypes);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindAccessRestriction)
		{
			ref Entity accessRestriction = ref m_AccessRestriction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRestriction);
		}
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		byte trackTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trackTypes);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.shipLanes)
		{
			byte roadTypes = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref roadTypes);
			m_RoadTypes = (RoadTypes)roadTypes;
		}
		m_Flags = (ConnectionLaneFlags)flags;
		m_TrackTypes = (TrackTypes)trackTypes;
	}

	public bool Equals(ConnectionLane other)
	{
		if (m_Flags == other.m_Flags && m_TrackTypes == other.m_TrackTypes)
		{
			return m_RoadTypes == other.m_RoadTypes;
		}
		return false;
	}
}

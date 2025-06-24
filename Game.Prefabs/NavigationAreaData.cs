using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct NavigationAreaData : IComponentData, IQueryTypeParameter, ISerializable
{
	public RouteConnectionType m_ConnectionType;

	public RouteConnectionType m_SecondaryType;

	public TrackTypes m_TrackTypes;

	public RoadTypes m_RoadTypes;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte num = (byte)m_ConnectionType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		byte num2 = (byte)m_SecondaryType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		TrackTypes trackTypes = m_TrackTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trackTypes);
		RoadTypes roadTypes = m_RoadTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)roadTypes);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte connectionType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref connectionType);
		byte secondaryType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref secondaryType);
		byte trackTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trackTypes);
		byte roadTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref roadTypes);
		m_ConnectionType = (RouteConnectionType)connectionType;
		m_SecondaryType = (RouteConnectionType)secondaryType;
		m_TrackTypes = (TrackTypes)trackTypes;
		m_RoadTypes = (RoadTypes)roadTypes;
	}
}

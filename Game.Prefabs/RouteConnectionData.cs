using Colossal.Serialization.Entities;
using Game.Net;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct RouteConnectionData : IComponentData, IQueryTypeParameter, ISerializable
{
	public RouteConnectionType m_AccessConnectionType;

	public RouteConnectionType m_RouteConnectionType;

	public TrackTypes m_AccessTrackType;

	public TrackTypes m_RouteTrackType;

	public RoadTypes m_AccessRoadType;

	public RoadTypes m_RouteRoadType;

	public SizeClass m_RouteSizeClass;

	public float m_StartLaneOffset;

	public float m_EndMargin;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte num = (byte)m_AccessConnectionType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		byte num2 = (byte)m_RouteConnectionType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		TrackTypes accessTrackType = m_AccessTrackType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)accessTrackType);
		TrackTypes routeTrackType = m_RouteTrackType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)routeTrackType);
		RoadTypes accessRoadType = m_AccessRoadType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)accessRoadType);
		RoadTypes routeRoadType = m_RouteRoadType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)routeRoadType);
		SizeClass routeSizeClass = m_RouteSizeClass;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)routeSizeClass);
		float startLaneOffset = m_StartLaneOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startLaneOffset);
		float endMargin = m_EndMargin;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endMargin);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte accessConnectionType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessConnectionType);
		byte routeConnectionType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref routeConnectionType);
		byte accessTrackType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessTrackType);
		byte routeTrackType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref routeTrackType);
		byte accessRoadType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRoadType);
		byte routeRoadType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref routeRoadType);
		byte routeSizeClass = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref routeSizeClass);
		ref float startLaneOffset = ref m_StartLaneOffset;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startLaneOffset);
		ref float endMargin = ref m_EndMargin;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref endMargin);
		m_AccessConnectionType = (RouteConnectionType)accessConnectionType;
		m_RouteConnectionType = (RouteConnectionType)routeConnectionType;
		m_AccessTrackType = (TrackTypes)accessTrackType;
		m_RouteTrackType = (TrackTypes)routeTrackType;
		m_AccessRoadType = (RoadTypes)accessRoadType;
		m_RouteRoadType = (RoadTypes)routeRoadType;
		m_RouteSizeClass = (SizeClass)routeSizeClass;
	}
}

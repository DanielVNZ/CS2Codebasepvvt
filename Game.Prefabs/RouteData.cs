using Colossal.Serialization.Entities;
using Game.Routes;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public struct RouteData : IComponentData, IQueryTypeParameter, ISerializable
{
	public EntityArchetype m_RouteArchetype;

	public EntityArchetype m_WaypointArchetype;

	public EntityArchetype m_ConnectedArchetype;

	public EntityArchetype m_SegmentArchetype;

	public float m_SnapDistance;

	public RouteType m_Type;

	public Color32 m_Color;

	public float m_Width;

	public float m_SegmentLength;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		sbyte num = (sbyte)m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		float width = m_Width;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(width);
		float segmentLength = m_SegmentLength;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(segmentLength);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		sbyte type = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		ref float width = ref m_Width;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref width);
		ref float segmentLength = ref m_SegmentLength;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref segmentLength);
		m_Type = (RouteType)type;
		m_Color = new Color32((byte)128, (byte)128, (byte)128, byte.MaxValue);
		m_SnapDistance = m_Width;
	}
}

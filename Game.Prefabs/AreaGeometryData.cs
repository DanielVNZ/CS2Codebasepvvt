using Colossal.Serialization.Entities;
using Game.Areas;
using Unity.Entities;

namespace Game.Prefabs;

public struct AreaGeometryData : IComponentData, IQueryTypeParameter, ISerializable
{
	public AreaType m_Type;

	public GeometryFlags m_Flags;

	public float m_SnapDistance;

	public float m_MaxHeight;

	public float m_LodBias;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		sbyte num = (sbyte)m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		GeometryFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float snapDistance = m_SnapDistance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(snapDistance);
		float maxHeight = m_MaxHeight;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxHeight);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		sbyte type = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float snapDistance = ref m_SnapDistance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref snapDistance);
		ref float maxHeight = ref m_MaxHeight;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxHeight);
		m_Type = (AreaType)type;
		m_Flags = (GeometryFlags)flags;
	}
}

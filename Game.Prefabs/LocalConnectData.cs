using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct LocalConnectData : IComponentData, IQueryTypeParameter, ISerializable
{
	public LocalConnectFlags m_Flags;

	public Layer m_Layers;

	public Bounds1 m_HeightRange;

	public float m_SearchDistance;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		LocalConnectFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		Layer layers = m_Layers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)layers);
		float min = m_HeightRange.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_HeightRange.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
		float searchDistance = m_SearchDistance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(searchDistance);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		uint layers = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref layers);
		ref float min = ref m_HeightRange.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_HeightRange.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		ref float searchDistance = ref m_SearchDistance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref searchDistance);
		m_Flags = (LocalConnectFlags)flags;
		m_Layers = (Layer)layers;
	}
}

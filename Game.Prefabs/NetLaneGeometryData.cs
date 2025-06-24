using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct NetLaneGeometryData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_Size;

	public int m_MinLod;

	public MeshLayer m_GameLayers;

	public MeshLayer m_EditorLayers;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Size);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Size);
		m_MinLod = 255;
		m_GameLayers = MeshLayer.Default;
		m_EditorLayers = MeshLayer.Default;
	}
}

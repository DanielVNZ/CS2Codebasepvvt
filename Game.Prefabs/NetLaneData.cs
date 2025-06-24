using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct NetLaneData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_PathfindPrefab;

	public LaneFlags m_Flags;

	public float m_Width;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity pathfindPrefab = m_PathfindPrefab;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathfindPrefab);
		LaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float width = m_Width;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(width);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity pathfindPrefab = ref m_PathfindPrefab;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathfindPrefab);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float width = ref m_Width;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref width);
		m_Flags = (LaneFlags)flags;
	}
}

using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct StackData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Bounds1 m_FirstBounds;

	public Bounds1 m_MiddleBounds;

	public Bounds1 m_LastBounds;

	public StackDirection m_Direction;

	public bool3 m_DontScale;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_Direction);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		byte direction = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref direction);
		m_MiddleBounds = new Bounds1(-1f, 1f);
		m_Direction = (StackDirection)direction;
	}
}

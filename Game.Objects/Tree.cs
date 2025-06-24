using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Tree : IComponentData, IQueryTypeParameter, ISerializable
{
	public TreeState m_State;

	public byte m_Growth;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		TreeState state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)state);
		byte growth = m_Growth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(growth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte state = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref byte growth = ref m_Growth;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref growth);
		m_State = (TreeState)state;
	}
}

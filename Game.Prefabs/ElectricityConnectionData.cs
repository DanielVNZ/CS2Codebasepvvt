using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct ElectricityConnectionData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Capacity;

	public FlowDirection m_Direction;

	public ElectricityConnection.Voltage m_Voltage;

	public CompositionFlags m_CompositionAll;

	public CompositionFlags m_CompositionAny;

	public CompositionFlags m_CompositionNone;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		FlowDirection direction = m_Direction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)direction);
		ElectricityConnection.Voltage voltage = m_Voltage;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)voltage);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		byte direction = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref direction);
		byte voltage = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref voltage);
		m_Direction = (FlowDirection)direction;
		m_Voltage = (ElectricityConnection.Voltage)voltage;
	}
}

using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Vehicles;

public struct DeliveryTruck : IComponentData, IQueryTypeParameter, ISerializable
{
	public DeliveryTruckFlags m_State;

	public Resource m_Resource;

	public int m_Amount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		DeliveryTruckFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		sbyte index = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		m_State = (DeliveryTruckFlags)state;
		m_Resource = EconomyUtils.GetResource(index);
	}
}

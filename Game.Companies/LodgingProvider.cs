using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct LodgingProvider : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_FreeRooms;

	public int m_Price;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int freeRooms = m_FreeRooms;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freeRooms);
		int price = m_Price;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(price);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int freeRooms = ref m_FreeRooms;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref freeRooms);
		ref int price = ref m_Price;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref price);
	}
}

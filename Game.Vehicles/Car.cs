using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Car : IComponentData, IQueryTypeParameter, ISerializable
{
	public CarFlags m_Flags;

	public Car(CarFlags flags)
	{
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)m_Flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (CarFlags)flags;
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PersonalCarData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_PassengerCapacity;

	public int m_BaggageCapacity;

	public int m_CostToDrive;

	public int m_Probability;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int passengerCapacity = m_PassengerCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(passengerCapacity);
		int baggageCapacity = m_BaggageCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(baggageCapacity);
		int costToDrive = m_CostToDrive;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(costToDrive);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int passengerCapacity = ref m_PassengerCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref passengerCapacity);
		ref int baggageCapacity = ref m_BaggageCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref baggageCapacity);
		ref int costToDrive = ref m_CostToDrive;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref costToDrive);
	}
}

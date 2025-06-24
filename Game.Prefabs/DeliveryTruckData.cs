using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Prefabs;

public struct DeliveryTruckData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_CargoCapacity;

	public int m_CostToDrive;

	public Resource m_TransportedResources;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		Resource transportedResources = m_TransportedResources;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)transportedResources);
		int cargoCapacity = m_CargoCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cargoCapacity);
		int costToDrive = m_CostToDrive;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(costToDrive);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ulong transportedResources = default(ulong);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref transportedResources);
		ref int cargoCapacity = ref m_CargoCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref cargoCapacity);
		ref int costToDrive = ref m_CostToDrive;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref costToDrive);
		m_TransportedResources = (Resource)transportedResources;
	}
}

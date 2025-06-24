using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct TransportStopData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_ComfortFactor;

	public float m_LoadingFactor;

	public float m_AccessDistance;

	public float m_BoardingTime;

	public TransportType m_TransportType;

	public bool m_PassengerTransport;

	public bool m_CargoTransport;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		sbyte num = (sbyte)m_TransportType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		bool passengerTransport = m_PassengerTransport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(passengerTransport);
		bool cargoTransport = m_CargoTransport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cargoTransport);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		sbyte transportType = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref transportType);
		ref bool passengerTransport = ref m_PassengerTransport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref passengerTransport);
		ref bool cargoTransport = ref m_CargoTransport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref cargoTransport);
		m_TransportType = (TransportType)transportType;
	}
}

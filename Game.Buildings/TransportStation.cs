using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Buildings;

public struct TransportStation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_ComfortFactor;

	public float m_LoadingFactor;

	public EnergyTypes m_CarRefuelTypes;

	public EnergyTypes m_TrainRefuelTypes;

	public EnergyTypes m_WatercraftRefuelTypes;

	public EnergyTypes m_AircraftRefuelTypes;

	public TransportStationFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float comfortFactor = m_ComfortFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(comfortFactor);
		float loadingFactor = m_LoadingFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(loadingFactor);
		EnergyTypes carRefuelTypes = m_CarRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)carRefuelTypes);
		EnergyTypes trainRefuelTypes = m_TrainRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trainRefuelTypes);
		EnergyTypes watercraftRefuelTypes = m_WatercraftRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)watercraftRefuelTypes);
		EnergyTypes aircraftRefuelTypes = m_AircraftRefuelTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)aircraftRefuelTypes);
		TransportStationFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ref float comfortFactor = ref m_ComfortFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref comfortFactor);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transportLoadingFactor)
		{
			ref float loadingFactor = ref m_LoadingFactor;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref loadingFactor);
		}
		byte carRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref carRefuelTypes);
		byte trainRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trainRefuelTypes);
		byte watercraftRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref watercraftRefuelTypes);
		byte aircraftRefuelTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref aircraftRefuelTypes);
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_CarRefuelTypes = (EnergyTypes)carRefuelTypes;
		m_TrainRefuelTypes = (EnergyTypes)trainRefuelTypes;
		m_WatercraftRefuelTypes = (EnergyTypes)watercraftRefuelTypes;
		m_AircraftRefuelTypes = (EnergyTypes)aircraftRefuelTypes;
		m_Flags = (TransportStationFlags)flags;
	}
}

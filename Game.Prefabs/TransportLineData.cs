using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct TransportLineData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_PathfindPrefab;

	public TransportType m_TransportType;

	public float m_DefaultVehicleInterval;

	public float m_DefaultUnbunchingFactor;

	public float m_StopDuration;

	public SizeClass m_SizeClass;

	public bool m_PassengerTransport;

	public bool m_CargoTransport;

	public Entity m_VehicleNotification;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity pathfindPrefab = m_PathfindPrefab;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathfindPrefab);
		sbyte num = (sbyte)m_TransportType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		float defaultVehicleInterval = m_DefaultVehicleInterval;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(defaultVehicleInterval);
		float defaultUnbunchingFactor = m_DefaultUnbunchingFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(defaultUnbunchingFactor);
		float stopDuration = m_StopDuration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(stopDuration);
		bool passengerTransport = m_PassengerTransport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(passengerTransport);
		bool cargoTransport = m_CargoTransport;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cargoTransport);
		SizeClass sizeClass = m_SizeClass;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)sizeClass);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		ref Entity pathfindPrefab = ref m_PathfindPrefab;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathfindPrefab);
		sbyte transportType = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref transportType);
		ref float defaultVehicleInterval = ref m_DefaultVehicleInterval;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref defaultVehicleInterval);
		ref float defaultUnbunchingFactor = ref m_DefaultUnbunchingFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref defaultUnbunchingFactor);
		ref float stopDuration = ref m_StopDuration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref stopDuration);
		ref bool passengerTransport = ref m_PassengerTransport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref passengerTransport);
		ref bool cargoTransport = ref m_CargoTransport;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref cargoTransport);
		m_TransportType = (TransportType)transportType;
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.BpPrefabData))
		{
			byte sizeClass = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref sizeClass);
			m_SizeClass = (SizeClass)sizeClass;
		}
		else
		{
			m_SizeClass = SizeClass.Large;
		}
	}
}

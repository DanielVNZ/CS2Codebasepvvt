using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct TrafficLights : IComponentData, IQueryTypeParameter, ISerializable
{
	public TrafficLightState m_State;

	public TrafficLightFlags m_Flags;

	public byte m_SignalGroupCount;

	public byte m_CurrentSignalGroup;

	public byte m_NextSignalGroup;

	public byte m_Timer;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		TrafficLightState state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)state);
		TrafficLightFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte signalGroupCount = m_SignalGroupCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(signalGroupCount);
		byte currentSignalGroup = m_CurrentSignalGroup;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(currentSignalGroup);
		byte nextSignalGroup = m_NextSignalGroup;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nextSignalGroup);
		byte timer = m_Timer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(timer);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		byte state = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref byte signalGroupCount = ref m_SignalGroupCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref signalGroupCount);
		ref byte currentSignalGroup = ref m_CurrentSignalGroup;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref currentSignalGroup);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.nextLaneSignal)
		{
			ref byte nextSignalGroup = ref m_NextSignalGroup;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref nextSignalGroup);
		}
		ref byte timer = ref m_Timer;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref timer);
		m_State = (TrafficLightState)state;
		m_Flags = (TrafficLightFlags)flags;
	}
}

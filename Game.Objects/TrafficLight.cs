using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct TrafficLight : IComponentData, IQueryTypeParameter, ISerializable
{
	public TrafficLightState m_State;

	public ushort m_GroupMask0;

	public ushort m_GroupMask1;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		TrafficLightState state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)state);
		ushort groupMask = m_GroupMask0;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groupMask);
		ushort groupMask2 = m_GroupMask1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groupMask2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		ushort state = default(ushort);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trafficLightGroups)
		{
			ref ushort groupMask = ref m_GroupMask0;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref groupMask);
			ref ushort groupMask2 = ref m_GroupMask1;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref groupMask2);
		}
		m_State = (TrafficLightState)state;
	}
}

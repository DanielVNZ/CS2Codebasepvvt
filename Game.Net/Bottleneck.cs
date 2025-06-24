using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct Bottleneck : IComponentData, IQueryTypeParameter, ISerializable
{
	public byte m_Position;

	public byte m_MinPos;

	public byte m_MaxPos;

	public byte m_Timer;

	public Bottleneck(byte minPos, byte maxPos, byte timer)
	{
		m_Position = (byte)(minPos + maxPos + 1 >> 1);
		m_MinPos = minPos;
		m_MaxPos = maxPos;
		m_Timer = timer;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte position = m_Position;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		byte minPos = m_MinPos;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(minPos);
		byte maxPos = m_MaxPos;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxPos);
		byte timer = m_Timer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(timer);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref byte position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trafficBottleneckPosition)
		{
			ref byte minPos = ref m_MinPos;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref minPos);
			ref byte maxPos = ref m_MaxPos;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxPos);
		}
		else
		{
			m_MinPos = m_Position;
			m_MaxPos = m_Position;
		}
		ref byte timer = ref m_Timer;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref timer);
	}
}

using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct LaneReservation : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Blocker;

	public ReservationData m_Next;

	public ReservationData m_Prev;

	public float GetOffset()
	{
		return (float)math.max((int)m_Next.m_Offset, (int)m_Prev.m_Offset) * 0.003921569f;
	}

	public int GetPriority()
	{
		return math.max((int)m_Next.m_Priority, (int)m_Prev.m_Priority);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity blocker = m_Blocker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(blocker);
		byte offset = m_Next.m_Offset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(offset);
		byte priority = m_Next.m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		byte offset2 = m_Prev.m_Offset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(offset2);
		byte priority2 = m_Prev.m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.stuckTrainFix)
		{
			ref Entity blocker = ref m_Blocker;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref blocker);
		}
		ref byte offset = ref m_Next.m_Offset;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref offset);
		ref byte priority = ref m_Next.m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		ref byte offset2 = ref m_Prev.m_Offset;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref offset2);
		ref byte priority2 = ref m_Prev.m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority2);
	}
}

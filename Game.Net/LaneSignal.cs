using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct LaneSignal : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Petitioner;

	public Entity m_Blocker;

	public ushort m_GroupMask;

	public sbyte m_Priority;

	public sbyte m_Default;

	public LaneSignalType m_Signal;

	public LaneSignalFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity petitioner = m_Petitioner;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(petitioner);
		Entity blocker = m_Blocker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(blocker);
		ushort groupMask = m_GroupMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groupMask);
		sbyte priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		sbyte num = m_Default;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		LaneSignalType signal = m_Signal;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)signal);
		LaneSignalFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trafficImprovements)
		{
			ref Entity petitioner = ref m_Petitioner;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref petitioner);
			ref Entity blocker = ref m_Blocker;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref blocker);
		}
		ref ushort groupMask = ref m_GroupMask;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref groupMask);
		ref sbyte priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.levelCrossing)
		{
			ref sbyte reference = ref m_Default;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		}
		byte signal = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref signal);
		m_Signal = (LaneSignalType)signal;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trafficFlowFixes)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (LaneSignalFlags)flags;
		}
	}
}

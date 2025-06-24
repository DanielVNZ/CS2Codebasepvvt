using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Simulation;

public struct ElectricityFlowEdge : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Index;

	public Entity m_Start;

	public Entity m_End;

	public int m_Capacity;

	public int m_Flow;

	public ElectricityFlowEdgeFlags m_Flags;

	public FlowDirection direction
	{
		get
		{
			return (FlowDirection)(m_Flags & ElectricityFlowEdgeFlags.ForwardBackward);
		}
		set
		{
			m_Flags &= ~ElectricityFlowEdgeFlags.ForwardBackward;
			m_Flags |= (ElectricityFlowEdgeFlags)value;
		}
	}

	public bool isBottleneck => (m_Flags & ElectricityFlowEdgeFlags.Bottleneck) != 0;

	public bool isBeyondBottleneck => (m_Flags & ElectricityFlowEdgeFlags.BeyondBottleneck) != 0;

	public bool isDisconnected => (m_Flags & ElectricityFlowEdgeFlags.Disconnected) != 0;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity start = m_Start;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(start);
		Entity end = m_End;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(end);
		int flow = m_Flow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flow);
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		ElectricityFlowEdgeFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		ref Entity start = ref m_Start;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref start);
		ref Entity end = ref m_End;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref end);
		ref int flow = ref m_Flow;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flow);
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.electricityImprovements2)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (ElectricityFlowEdgeFlags)flags;
			return;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.electricityImprovements)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_Flags = ElectricityFlowEdgeFlags.ForwardBackward;
		}
		else
		{
			m_Flags = ElectricityFlowEdgeFlags.ForwardBackward;
		}
	}
}

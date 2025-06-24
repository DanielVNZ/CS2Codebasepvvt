using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct WaterPipeEdge : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Index;

	public Entity m_Start;

	public Entity m_End;

	public int m_FreshFlow;

	public float m_FreshPollution;

	public int m_SewageFlow;

	public int m_FreshCapacity;

	public int m_SewageCapacity;

	public WaterPipeEdgeFlags m_Flags;

	public int2 flow => new int2(m_FreshFlow, m_SewageFlow);

	public int2 capacity => new int2(m_FreshCapacity, m_SewageCapacity);

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity start = m_Start;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(start);
		Entity end = m_End;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(end);
		int freshFlow = m_FreshFlow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freshFlow);
		float freshPollution = m_FreshPollution;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freshPollution);
		int sewageFlow = m_SewageFlow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sewageFlow);
		int freshCapacity = m_FreshCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freshCapacity);
		int sewageCapacity = m_SewageCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sewageCapacity);
		WaterPipeEdgeFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		ref Entity start = ref m_Start;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref start);
		ref Entity end = ref m_End;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref end);
		ref int freshFlow = ref m_FreshFlow;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref freshFlow);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterPipePollution)
		{
			ref float freshPollution = ref m_FreshPollution;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref freshPollution);
		}
		else
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		ref int sewageFlow = ref m_SewageFlow;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sewageFlow);
		ref int freshCapacity = ref m_FreshCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref freshCapacity);
		ref int sewageCapacity = ref m_SewageCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sewageCapacity);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.stormWater)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.waterPipeFlowSim)
			{
				int num2 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
				int num3 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterPipeFlags)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (WaterPipeEdgeFlags)flags;
		}
	}
}

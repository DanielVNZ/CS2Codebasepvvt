using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct Road : IComponentData, IQueryTypeParameter, ISerializable
{
	public float4 m_TrafficFlowDuration0;

	public float4 m_TrafficFlowDuration1;

	public float4 m_TrafficFlowDistance0;

	public float4 m_TrafficFlowDistance1;

	public RoadFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float4 trafficFlowDuration = m_TrafficFlowDuration0;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(trafficFlowDuration);
		float4 trafficFlowDuration2 = m_TrafficFlowDuration1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(trafficFlowDuration2);
		float4 trafficFlowDistance = m_TrafficFlowDistance0;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(trafficFlowDistance);
		float4 trafficFlowDistance2 = m_TrafficFlowDistance1;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(trafficFlowDistance2);
		RoadFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.netInfoviewImprovements)
		{
			ref float4 trafficFlowDuration = ref m_TrafficFlowDuration0;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trafficFlowDuration);
			ref float4 trafficFlowDuration2 = ref m_TrafficFlowDuration1;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trafficFlowDuration2);
			ref float4 trafficFlowDistance = ref m_TrafficFlowDistance0;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trafficFlowDistance);
			ref float4 trafficFlowDistance2 = ref m_TrafficFlowDistance1;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trafficFlowDistance2);
		}
		else
		{
			ref float4 trafficFlowDuration3 = ref m_TrafficFlowDuration0;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trafficFlowDuration3);
			ref float4 trafficFlowDistance3 = ref m_TrafficFlowDistance0;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trafficFlowDistance3);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.trafficFlowFixes)
			{
				float4 val = m_TrafficFlowDuration0 + 1f;
				m_TrafficFlowDistance0 *= 0.01f;
				m_TrafficFlowDuration0 = math.select(m_TrafficFlowDistance0 / val, float4.op_Implicit(0f), val <= 0f);
			}
			m_TrafficFlowDuration0 *= 0.5f;
			m_TrafficFlowDistance0 *= 0.5f;
			m_TrafficFlowDuration1 = m_TrafficFlowDuration0;
			m_TrafficFlowDistance1 = m_TrafficFlowDistance0;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.roadFlags)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (RoadFlags)flags;
		}
	}
}

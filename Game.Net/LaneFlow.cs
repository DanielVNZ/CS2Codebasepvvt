using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct LaneFlow : IComponentData, IQueryTypeParameter, ISerializable
{
	public float4 m_Duration;

	public float4 m_Distance;

	public float2 m_Next;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float4 duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		float4 distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
		float2 next = m_Next;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(next);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		ref float4 duration = ref m_Duration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
		ref float4 distance = ref m_Distance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
		ref float2 next = ref m_Next;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref next);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.trafficFlowFixes)
		{
			float4 val = m_Duration + 1f;
			m_Distance *= 0.01f;
			m_Duration = math.select(m_Distance / val, float4.op_Implicit(0f), val <= 0f);
			float num = m_Next.x + m_Next.y;
			m_Next.x = math.select(m_Next.y / num, 0f, num <= 0f);
			m_Next.y *= 0.01f;
			m_Next.x *= m_Next.y;
		}
	}
}

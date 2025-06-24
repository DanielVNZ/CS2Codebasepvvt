using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct EdgeLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public float2 m_EdgeDelta;

	public byte m_ConnectedStartCount;

	public byte m_ConnectedEndCount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		bool3 val = m_EdgeDelta.x == new float3(0f, 0.5f, 1f);
		bool3 val2 = m_EdgeDelta.y == new float3(0f, 0.5f, 1f);
		int3 val3 = math.select(int3.op_Implicit(0), new int3(1, 2, 3), ((bool3)(ref val)).xyx & ((bool3)(ref val2)).yzz);
		int3 val4 = math.select(int3.op_Implicit(0), new int3(4, 5, 6), ((bool3)(ref val)).zyz & ((bool3)(ref val2)).yxx);
		byte b = (byte)math.csum(val3 + val4);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(b);
		if (b == 0)
		{
			float2 edgeDelta = m_EdgeDelta;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(edgeDelta);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.saveOptimizations)
		{
			byte b = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
			switch (b)
			{
			case 1:
				m_EdgeDelta = new float2(0f, 0.5f);
				break;
			case 2:
				m_EdgeDelta = new float2(0.5f, 1f);
				break;
			case 3:
				m_EdgeDelta = new float2(0f, 1f);
				break;
			case 4:
				m_EdgeDelta = new float2(1f, 0.5f);
				break;
			case 5:
				m_EdgeDelta = new float2(0.5f, 0f);
				break;
			case 6:
				m_EdgeDelta = new float2(1f, 0f);
				break;
			default:
			{
				ref float2 edgeDelta = ref m_EdgeDelta;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref edgeDelta);
				break;
			}
			}
		}
		else
		{
			ref float2 edgeDelta2 = ref m_EdgeDelta;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref edgeDelta2);
		}
	}
}

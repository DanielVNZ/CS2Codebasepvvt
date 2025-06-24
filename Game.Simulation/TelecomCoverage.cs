using Colossal.Serialization.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct TelecomCoverage : IStrideSerializable, ISerializable
{
	public byte m_SignalStrength;

	public byte m_NetworkLoad;

	public int networkQuality => m_SignalStrength * 510 / (255 + (m_NetworkLoad << 1));

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte signalStrength = m_SignalStrength;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(signalStrength);
		byte networkLoad = m_NetworkLoad;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(networkLoad);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref byte signalStrength = ref m_SignalStrength;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref signalStrength);
		ref byte networkLoad = ref m_NetworkLoad;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref networkLoad);
	}

	public int GetStride(Context context)
	{
		return 2;
	}

	public static float SampleNetworkQuality(CellMapData<TelecomCoverage> coverage, float3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		float2 val = ((float3)(ref position)).xz / coverage.m_CellSize + float2.op_Implicit(coverage.m_TextureSize) * 0.5f - 0.5f;
		int2 val2 = (int2)math.floor(val);
		int4 val3 = ((int2)(ref val2)).xyxy;
		((int4)(ref val3)).zw = ((int4)(ref val3)).zw + 1;
		val3 = math.clamp(val3, int4.op_Implicit(0), ((int2)(ref coverage.m_TextureSize)).xyxy - 1);
		int4 val4 = ((int4)(ref val3)).xzxz + coverage.m_TextureSize.x * ((int4)(ref val3)).yyww;
		TelecomCoverage telecomCoverage = coverage.m_Buffer[val4.x];
		TelecomCoverage telecomCoverage2 = coverage.m_Buffer[val4.y];
		TelecomCoverage telecomCoverage3 = coverage.m_Buffer[val4.z];
		TelecomCoverage telecomCoverage4 = coverage.m_Buffer[val4.w];
		float4 val5 = default(float4);
		((float4)(ref val5))._002Ector((float)(int)telecomCoverage.m_SignalStrength, (float)(int)telecomCoverage2.m_SignalStrength, (float)(int)telecomCoverage3.m_SignalStrength, (float)(int)telecomCoverage4.m_SignalStrength);
		float4 val6 = default(float4);
		((float4)(ref val6))._002Ector((float)(int)telecomCoverage.m_NetworkLoad, (float)(int)telecomCoverage2.m_NetworkLoad, (float)(int)telecomCoverage3.m_NetworkLoad, (float)(int)telecomCoverage4.m_NetworkLoad);
		float4 val7 = math.min(float4.op_Implicit(1f), val5 / (127.5f + val6));
		float2 val8 = math.saturate(val - float2.op_Implicit(((int4)(ref val3)).xy));
		float2 val9 = math.lerp(((float4)(ref val7)).xz, ((float4)(ref val7)).yw, val8.x);
		return math.lerp(val9.x, val9.y, val8.y);
	}
}

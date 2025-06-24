using System;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

[InternalBufferCapacity(0)]
public struct LaneOverlap : IBufferElementData, IEmptySerializable, IComparable<LaneOverlap>
{
	public Entity m_Other;

	public OverlapFlags m_Flags;

	public byte m_ThisStart;

	public byte m_ThisEnd;

	public byte m_OtherStart;

	public byte m_OtherEnd;

	public byte m_Parallelism;

	public sbyte m_PriorityDelta;

	public LaneOverlap(Entity other, float4 overlap, OverlapFlags flags, float parallelism, int priorityDelta)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		int4 val = math.clamp((int4)math.round(overlap * 255f), int4.op_Implicit(0), int4.op_Implicit(255));
		m_Other = other;
		m_ThisStart = (byte)val.x;
		m_ThisEnd = (byte)val.y;
		m_OtherStart = (byte)val.z;
		m_OtherEnd = (byte)val.w;
		m_Flags = flags;
		m_Parallelism = (byte)math.clamp((int)math.round(parallelism * 128f), 0, 255);
		m_PriorityDelta = (sbyte)priorityDelta;
	}

	public int CompareTo(LaneOverlap other)
	{
		return ((m_ThisStart << 8) | m_ThisEnd) - ((other.m_ThisStart << 8) | other.m_ThisEnd);
	}
}

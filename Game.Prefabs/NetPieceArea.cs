using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct NetPieceArea : IBufferElementData, IComparable<NetPieceArea>
{
	public NetAreaFlags m_Flags;

	public float3 m_Position;

	public float m_Width;

	public float3 m_SnapPosition;

	public float m_SnapWidth;

	public int CompareTo(NetPieceArea other)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return math.select(0, math.select(-1, 1, m_Position.x > other.m_Position.x), m_Position.x != other.m_Position.x);
	}
}

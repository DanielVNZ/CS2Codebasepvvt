using System;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Pathfind;

public struct PathNode : IEquatable<PathNode>, IStrideSerializable, ISerializable
{
	private const float FLOAT_TO_INT = 32767f;

	private const float INT_TO_FLOAT = 3.051851E-05f;

	private const ulong CURVEPOS_INCLUDE = 2147418112uL;

	private const ulong CURVEPOS_EXCLUDE = 18446744071562133503uL;

	private const ulong SECONDARY_NODE = 2147483648uL;

	private ulong m_SearchKey;

	public PathNode(Entity owner, byte laneIndex, byte segmentIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		m_SearchKey = (ulong)((long)owner.Index << 32) | ((ulong)segmentIndex << 8) | laneIndex;
	}

	public PathNode(Entity owner, byte laneIndex, byte segmentIndex, float curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		m_SearchKey = (ulong)((long)owner.Index << 32) | ((ulong)(curvePosition * 32767f) << 16) | ((ulong)segmentIndex << 8) | laneIndex;
	}

	public PathNode(Entity owner, ushort laneIndex, float curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		m_SearchKey = (ulong)((long)owner.Index << 32) | ((ulong)(curvePosition * 32767f) << 16) | laneIndex;
	}

	public PathNode(Entity owner, ushort laneIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		m_SearchKey = (ulong)(((long)owner.Index << 32) | laneIndex);
	}

	public PathNode(PathTarget pathTarget)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SearchKey = (ulong)((long)pathTarget.m_Entity.Index << 32) | ((ulong)(pathTarget.m_Delta * 32767f) << 16);
	}

	public PathNode(PathNode pathNode, float curvePosition)
	{
		m_SearchKey = (pathNode.m_SearchKey & 0xFFFFFFFF8000FFFFuL) | ((ulong)(curvePosition * 32767f) << 16);
	}

	public PathNode(PathNode pathNode, bool secondaryNode)
	{
		m_SearchKey = math.select(pathNode.m_SearchKey & 0xFFFFFFFF7FFFFFFFuL, pathNode.m_SearchKey | 0x80000000u, secondaryNode);
	}

	public bool IsSecondary()
	{
		return (m_SearchKey & 0x80000000u) != 0;
	}

	public bool Equals(PathNode other)
	{
		return m_SearchKey == other.m_SearchKey;
	}

	public bool EqualsIgnoreCurvePos(PathNode other)
	{
		return ((m_SearchKey ^ other.m_SearchKey) & 0xFFFFFFFF8000FFFFuL) == 0;
	}

	public bool OwnerEquals(PathNode other)
	{
		return (uint)(m_SearchKey >> 32) == (uint)(other.m_SearchKey >> 32);
	}

	public override int GetHashCode()
	{
		return m_SearchKey.GetHashCode();
	}

	public PathNode StripCurvePos()
	{
		return new PathNode
		{
			m_SearchKey = (m_SearchKey & 0xFFFFFFFF8000FFFFuL)
		};
	}

	public void ReplaceOwner(Entity oldOwner, Entity newOwner)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if ((int)(m_SearchKey >> 32) == oldOwner.Index)
		{
			m_SearchKey = (ulong)((long)newOwner.Index << 32) | (m_SearchKey & 0xFFFFFFFFu);
		}
	}

	public void SetOwner(Entity newOwner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		m_SearchKey = (ulong)((long)newOwner.Index << 32) | (m_SearchKey & 0xFFFFFFFFu);
	}

	public void SetSegmentIndex(byte segmentIndex)
	{
		m_SearchKey = ((ulong)segmentIndex << 8) | (m_SearchKey & 0xFFFFFFFFFFFF00FFuL);
	}

	public float GetCurvePos()
	{
		return (float)((m_SearchKey & 0x7FFF0000) >> 16) * 3.051851E-05f;
	}

	public int GetOwnerIndex()
	{
		return (int)(m_SearchKey >> 32);
	}

	public ushort GetLaneIndex()
	{
		return (ushort)(m_SearchKey & 0xFFFF);
	}

	public bool GetOrder(PathNode other)
	{
		return other.m_SearchKey < m_SearchKey;
	}

	public int GetCurvePosOrder(PathNode other)
	{
		return (int)(m_SearchKey & 0x7FFF0000) - (int)(other.m_SearchKey & 0x7FFF0000);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Entity val = new Entity
		{
			Index = (int)(m_SearchKey >> 32)
		};
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val, true);
		int num = (int)m_SearchKey;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Entity val = default(Entity);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
		uint num = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_SearchKey = (ulong)(((long)val.Index << 32) | num);
	}

	public int GetStride(Context context)
	{
		return 8;
	}
}

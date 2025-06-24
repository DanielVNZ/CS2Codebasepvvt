using System;
using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;

namespace Game.Net;

public struct Lane : IComponentData, IQueryTypeParameter, IEquatable<Lane>, IStrideSerializable, ISerializable
{
	public PathNode m_StartNode;

	public PathNode m_MiddleNode;

	public PathNode m_EndNode;

	public bool Equals(Lane other)
	{
		if (m_StartNode.Equals(other.m_StartNode) && m_MiddleNode.Equals(other.m_MiddleNode))
		{
			return m_EndNode.Equals(other.m_EndNode);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return m_MiddleNode.GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		PathNode startNode = m_StartNode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<PathNode>(startNode);
		PathNode middleNode = m_MiddleNode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<PathNode>(middleNode);
		PathNode endNode = m_EndNode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<PathNode>(endNode);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref PathNode startNode = ref m_StartNode;
		((IReader)reader/*cast due to .constrained prefix*/).Read<PathNode>(ref startNode);
		ref PathNode middleNode = ref m_MiddleNode;
		((IReader)reader/*cast due to .constrained prefix*/).Read<PathNode>(ref middleNode);
		ref PathNode endNode = ref m_EndNode;
		((IReader)reader/*cast due to .constrained prefix*/).Read<PathNode>(ref endNode);
	}

	public int GetStride(Context context)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return m_StartNode.GetStride(context) * 3;
	}
}

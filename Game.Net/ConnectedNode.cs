using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

[InternalBufferCapacity(0)]
public struct ConnectedNode : IBufferElementData, IEquatable<ConnectedNode>, ISerializable
{
	public Entity m_Node;

	public float m_CurvePosition;

	public ConnectedNode(Entity node, float curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Node = node;
		m_CurvePosition = curvePosition;
	}

	public bool Equals(ConnectedNode other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Node)).Equals(other.m_Node);
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Node)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_CurvePosition.GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity node = m_Node;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(node);
		float curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity node = ref m_Node;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref node);
		ref float curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
	}
}

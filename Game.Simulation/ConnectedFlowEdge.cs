using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct ConnectedFlowEdge : IBufferElementData, IEmptySerializable, IEquatable<ConnectedFlowEdge>
{
	public Entity m_Edge;

	public ConnectedFlowEdge(Entity edge)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Edge = edge;
	}

	public bool Equals(ConnectedFlowEdge other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Edge)).Equals(other.m_Edge);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Edge)/*cast due to .constrained prefix*/).GetHashCode();
	}

	public static implicit operator Entity(ConnectedFlowEdge element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return element.m_Edge;
	}
}

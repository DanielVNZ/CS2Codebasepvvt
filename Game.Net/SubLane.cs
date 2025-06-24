using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;

namespace Game.Net;

[InternalBufferCapacity(0)]
public struct SubLane : IBufferElementData, IEquatable<SubLane>, IEmptySerializable
{
	public Entity m_SubLane;

	public PathMethod m_PathMethods;

	public SubLane(Entity lane, PathMethod pathMethods)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SubLane = lane;
		m_PathMethods = pathMethods;
	}

	public bool Equals(SubLane other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_SubLane)).Equals(other.m_SubLane);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SubLane)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

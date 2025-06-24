using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

[InternalBufferCapacity(0)]
public struct SubRoute : IBufferElementData, IEquatable<SubRoute>, IEmptySerializable
{
	public Entity m_Route;

	public SubRoute(Entity route)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Route = route;
	}

	public bool Equals(SubRoute other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Route)).Equals(other.m_Route);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Route)/*cast due to .constrained prefix*/).GetHashCode();
	}
}

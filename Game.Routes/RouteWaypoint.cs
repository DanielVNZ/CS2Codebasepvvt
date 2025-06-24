using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

[InternalBufferCapacity(0)]
public struct RouteWaypoint : IBufferElementData, IEmptySerializable
{
	public Entity m_Waypoint;

	public RouteWaypoint(Entity waypoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Waypoint = waypoint;
	}
}

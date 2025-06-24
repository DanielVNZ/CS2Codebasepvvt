using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

[InternalBufferCapacity(0)]
public struct RouteSegment : IBufferElementData, IEmptySerializable
{
	public Entity m_Segment;

	public RouteSegment(Entity segment)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Segment = segment;
	}
}

using Unity.Entities;
using Unity.Mathematics;

namespace Game.Routes;

[InternalBufferCapacity(0)]
public struct WaypointDefinition : IBufferElementData
{
	public float3 m_Position;

	public Entity m_Connection;

	public Entity m_Original;

	public WaypointDefinition(float3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_Position = position;
		m_Connection = Entity.Null;
		m_Original = Entity.Null;
	}
}

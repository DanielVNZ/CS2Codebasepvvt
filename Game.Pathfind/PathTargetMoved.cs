using Unity.Entities;
using Unity.Mathematics;

namespace Game.Pathfind;

public struct PathTargetMoved : IComponentData, IQueryTypeParameter
{
	public Entity m_Target;

	public float3 m_OldLocation;

	public float3 m_NewLocation;

	public PathTargetMoved(Entity target, float3 oldLocation, float3 newLocation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_OldLocation = oldLocation;
		m_NewLocation = newLocation;
	}
}

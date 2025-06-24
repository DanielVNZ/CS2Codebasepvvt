using Unity.Entities;
using Unity.Mathematics;

namespace Game.Objects;

public struct Damage : IComponentData, IQueryTypeParameter
{
	public Entity m_Object;

	public float3 m_Delta;

	public Damage(Entity _object, float3 delta)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Object = _object;
		m_Delta = delta;
	}
}

using Unity.Entities;

namespace Game.Prefabs;

public struct Unlock : IComponentData, IQueryTypeParameter
{
	public Entity m_Prefab;

	public Unlock(Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = prefab;
	}
}

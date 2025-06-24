using Unity.Entities;

namespace Game.Pathfind;

public struct CoverageUpdated : IComponentData, IQueryTypeParameter
{
	public Entity m_Owner;

	public PathEventData m_Data;

	public CoverageUpdated(Entity owner, PathEventData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Owner = owner;
		m_Data = data;
	}
}

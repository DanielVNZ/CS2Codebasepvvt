using Unity.Entities;

namespace Game.Pathfind;

public struct SetupQueueItem
{
	public Entity m_Owner;

	public PathfindParameters m_Parameters;

	public SetupQueueTarget m_Origin;

	public SetupQueueTarget m_Destination;

	public SetupQueueItem(Entity owner, PathfindParameters parameters, SetupQueueTarget origin, SetupQueueTarget destination)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Owner = owner;
		m_Parameters = parameters;
		m_Origin = origin;
		m_Destination = destination;
	}
}

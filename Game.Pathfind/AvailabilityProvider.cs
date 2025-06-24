using Unity.Entities;

namespace Game.Pathfind;

public struct AvailabilityProvider
{
	public Entity m_Provider;

	public float m_Capacity;

	public float m_Cost;

	public AvailabilityProvider(Entity provider, float capacity, float cost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Provider = provider;
		m_Capacity = capacity;
		m_Cost = cost;
	}
}

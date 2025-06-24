using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(1)]
public struct ServiceUpgradeBuilding : IBufferElementData
{
	public Entity m_Building;

	public ServiceUpgradeBuilding(Entity building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Building = building;
	}
}

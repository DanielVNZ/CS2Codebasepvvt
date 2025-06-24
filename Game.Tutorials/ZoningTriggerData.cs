using Unity.Entities;

namespace Game.Tutorials;

public struct ZoningTriggerData : IBufferElementData
{
	public Entity m_Zone;

	public ZoningTriggerData(Entity zone)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Zone = zone;
	}
}

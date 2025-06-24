using Unity.Entities;

namespace Game.Tutorials;

public struct AreaTriggerData : IBufferElementData
{
	public Entity m_Prefab;

	public AreaTriggerFlags m_Flags;

	public AreaTriggerData(Entity prefab, AreaTriggerFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = prefab;
		m_Flags = flags;
	}
}

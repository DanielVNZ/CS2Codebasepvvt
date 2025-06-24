using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct DefaultPolicyData : IBufferElementData
{
	public Entity m_Policy;

	public DefaultPolicyData(Entity policy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Policy = policy;
	}
}

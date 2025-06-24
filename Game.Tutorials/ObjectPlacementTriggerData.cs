using Unity.Entities;

namespace Game.Tutorials;

public struct ObjectPlacementTriggerData : IBufferElementData
{
	public Entity m_Object;

	public ObjectPlacementTriggerFlags m_Flags;

	public ObjectPlacementTriggerData(Entity obj, ObjectPlacementTriggerFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Object = obj;
		m_Flags = flags;
	}
}

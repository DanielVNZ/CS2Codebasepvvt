using Unity.Entities;

namespace Game.Tutorials;

public struct ObjectSelectionActivationData : IBufferElementData
{
	public Entity m_Prefab;

	public bool m_AllowTool;

	public ObjectSelectionActivationData(Entity prefab, bool allowTool)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = prefab;
		m_AllowTool = allowTool;
	}
}

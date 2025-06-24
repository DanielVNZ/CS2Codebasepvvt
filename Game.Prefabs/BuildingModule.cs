using Unity.Entities;

namespace Game.Prefabs;

public struct BuildingModule : IBufferElementData
{
	public Entity m_Module;

	public BuildingModule(Entity module)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Module = module;
	}
}

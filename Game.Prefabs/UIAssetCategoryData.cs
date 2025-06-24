using Unity.Entities;

namespace Game.Prefabs;

public struct UIAssetCategoryData : IComponentData, IQueryTypeParameter
{
	public Entity m_Menu;

	public UIAssetCategoryData(Entity menu)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Menu = menu;
	}
}

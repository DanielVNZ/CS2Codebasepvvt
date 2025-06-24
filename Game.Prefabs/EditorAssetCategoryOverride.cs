using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class EditorAssetCategoryOverride : ComponentBase
{
	public string[] m_IncludeCategories;

	public string[] m_ExcludeCategories;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if ((m_IncludeCategories != null && m_IncludeCategories.Length != 0) || (m_ExcludeCategories != null && m_ExcludeCategories.Length != 0))
		{
			components.Add(ComponentType.ReadWrite<EditorAssetCategoryOverrideData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}

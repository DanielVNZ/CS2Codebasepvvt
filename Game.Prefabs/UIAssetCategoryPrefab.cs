using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UIAssetCategoryPrefab : UIGroupPrefab
{
	public UIAssetMenuPrefab m_Menu;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		if ((Object)(object)m_Menu != (Object)null)
		{
			components.Add(ComponentType.ReadWrite<UIAssetCategoryData>());
		}
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_Menu != (Object)null)
		{
			prefabs.Add(m_Menu);
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if ((Object)(object)m_Menu != (Object)null)
		{
			Entity entity2 = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>().GetEntity(m_Menu);
			((EntityManager)(ref entityManager)).SetComponentData<UIAssetCategoryData>(entity, new UIAssetCategoryData(entity2));
			m_Menu.AddElement(entityManager, entity);
		}
	}
}

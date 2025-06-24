using System;
using System.Collections.Generic;
using Game.UI.Editor;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UIObject : ComponentBase
{
	public UIGroupPrefab m_Group;

	public int m_Priority;

	[CustomField(typeof(UIIconField))]
	public string m_Icon;

	public bool m_IsDebugObject;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			UIGroupPrefab uIGroupPrefab = m_Group;
			if (uIGroupPrefab is UIAssetCategoryPrefab category)
			{
				yield return "UI" + ((Object)category).name;
				if (((Object)category).name.StartsWith("Props"))
				{
					yield return "UIProps";
				}
				if ((Object)(object)category.m_Menu != (Object)null)
				{
					yield return "UI" + ((Object)category.m_Menu).name;
				}
			}
		}
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_Group != (Object)null)
		{
			prefabs.Add(m_Group);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsDebugObject || Debug.isDebugBuild)
		{
			components.Add(ComponentType.ReadWrite<UIObjectData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (!m_IsDebugObject || Debug.isDebugBuild)
		{
			Entity val = Entity.Null;
			if ((Object)(object)m_Group != (Object)null)
			{
				val = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>().GetEntity(m_Group);
				m_Group.AddElement(entityManager, entity);
			}
			((EntityManager)(ref entityManager)).SetComponentData<UIObjectData>(entity, new UIObjectData
			{
				m_Group = val,
				m_Priority = m_Priority
			});
		}
	}
}

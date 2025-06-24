using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UIAvatarColors : PrefabBase
{
	public Color32[] m_AvatarColors;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UIAvatarColorData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<UIAvatarColorData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<UIAvatarColorData>(entity, false);
		for (int i = 0; i < m_AvatarColors.Length; i++)
		{
			buffer.Add(new UIAvatarColorData(m_AvatarColors[i]));
		}
		base.LateInitialize(entityManager, entity);
	}
}

using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Notifications/", new Type[] { typeof(NotificationIconPrefab) })]
public class SimulationWarning : ComponentBase
{
	public IconCategory[] m_Categories;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Categories != null)
		{
			NotificationIconDisplayData componentData = ((EntityManager)(ref entityManager)).GetComponentData<NotificationIconDisplayData>(entity);
			for (int i = 0; i < m_Categories.Length; i++)
			{
				componentData.m_CategoryMask |= (uint)(1 << (int)m_Categories[i]);
			}
			((EntityManager)(ref entityManager)).SetComponentData<NotificationIconDisplayData>(entity, componentData);
		}
	}
}

using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Policies/", new Type[] { typeof(PolicyPrefab) })]
public class RouteModifiers : ComponentBase
{
	public RouteModifierInfo[] m_Modifiers;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<RouteModifierData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Modifiers != null)
		{
			DynamicBuffer<RouteModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RouteModifierData>(entity, false);
			for (int i = 0; i < m_Modifiers.Length; i++)
			{
				RouteModifierInfo routeModifierInfo = m_Modifiers[i];
				buffer.Add(new RouteModifierData(routeModifierInfo.m_Type, routeModifierInfo.m_Mode, routeModifierInfo.m_Range));
			}
		}
	}
}

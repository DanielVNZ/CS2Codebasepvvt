using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Policies/", new Type[] { typeof(PolicyPrefab) })]
public class DistrictModifiers : ComponentBase
{
	public DistrictModifierInfo[] m_Modifiers;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<DistrictModifierData>());
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
			DynamicBuffer<DistrictModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<DistrictModifierData>(entity, false);
			for (int i = 0; i < m_Modifiers.Length; i++)
			{
				DistrictModifierInfo districtModifierInfo = m_Modifiers[i];
				buffer.Add(new DistrictModifierData(districtModifierInfo.m_Type, districtModifierInfo.m_Mode, districtModifierInfo.m_Range));
			}
		}
	}
}

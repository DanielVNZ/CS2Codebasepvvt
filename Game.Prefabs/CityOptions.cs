using System;
using System.Collections.Generic;
using Game.City;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Policies/", new Type[] { typeof(PolicyPrefab) })]
public class CityOptions : ComponentBase
{
	public CityOption[] m_Options;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CityOptionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Options != null)
		{
			CityOptionData cityOptionData = default(CityOptionData);
			for (int i = 0; i < m_Options.Length; i++)
			{
				cityOptionData.m_OptionMask |= (uint)(1 << (int)m_Options[i]);
			}
			((EntityManager)(ref entityManager)).SetComponentData<CityOptionData>(entity, cityOptionData);
		}
	}
}

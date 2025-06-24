using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Companies/", new Type[] { })]
public class BrandPrefab : PrefabBase
{
	public CompanyPrefab[] m_Companies;

	public Color[] m_BrandColors;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Companies.Length; i++)
		{
			prefabs.Add(m_Companies[i]);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<BrandData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		BrandData brandData = default(BrandData);
		for (int i = 0; i < m_BrandColors.Length; i++)
		{
			brandData.m_ColorSet[i] = m_BrandColors[i];
		}
		((EntityManager)(ref entityManager)).SetComponentData<BrandData>(entity, brandData);
	}
}

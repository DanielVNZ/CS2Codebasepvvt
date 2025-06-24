using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class CityEffects : ComponentBase, IServiceUpgrade
{
	public CityEffectInfo[] m_Effects;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CityModifierData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<CityEffectProvider>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CityEffectProvider>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Effects != null)
		{
			DynamicBuffer<CityModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CityModifierData>(entity, false);
			for (int i = 0; i < m_Effects.Length; i++)
			{
				CityEffectInfo cityEffectInfo = m_Effects[i];
				buffer.Add(new CityModifierData(cityEffectInfo.m_Type, cityEffectInfo.m_Mode, new Bounds1(0f, cityEffectInfo.m_Delta)));
			}
		}
	}
}

using System;
using System.Collections.Generic;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class Attraction : ComponentBase, IServiceUpgrade
{
	public int m_Attractiveness;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AttractionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null && m_Attractiveness > 0)
		{
			components.Add(ComponentType.ReadWrite<AttractivenessProvider>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (m_Attractiveness > 0)
		{
			components.Add(ComponentType.ReadWrite<AttractivenessProvider>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		AttractionData attractionData = new AttractionData
		{
			m_Attractiveness = m_Attractiveness
		};
		((EntityManager)(ref entityManager)).SetComponentData<AttractionData>(entity, attractionData);
	}
}

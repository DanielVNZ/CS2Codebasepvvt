using System;
using System.Collections.Generic;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab),
	typeof(NetPrefab),
	typeof(RoutePrefab)
})]
public class ServiceUpgrade : ComponentBase
{
	public BuildingPrefab[] m_Buildings;

	public uint m_UpgradeCost = 100u;

	public int m_XPReward;

	public int m_MaxPlacementOffset = -1;

	public float m_MaxPlacementDistance;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Buildings != null)
		{
			for (int i = 0; i < m_Buildings.Length; i++)
			{
				prefabs.Add(m_Buildings[i]);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ServiceUpgradeData>());
		components.Add(ComponentType.ReadWrite<ServiceUpgradeBuilding>());
		if ((Object)(object)GetComponent<BuildingPrefab>() != (Object)null)
		{
			components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
			components.Add(ComponentType.ReadWrite<PlaceableInfoviewItem>());
		}
		if (base.prefab.TryGet<ServiceConsumption>(out var component) && component.m_Upkeep > 0)
		{
			components.Add(ComponentType.ReadWrite<ServiceUpkeepData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.ServiceUpgrade>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<ServiceUpgradeData>(entity, new ServiceUpgradeData
		{
			m_UpgradeCost = m_UpgradeCost,
			m_XPReward = m_XPReward,
			m_MaxPlacementOffset = m_MaxPlacementOffset,
			m_MaxPlacementDistance = m_MaxPlacementDistance
		});
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (m_Buildings == null)
		{
			return;
		}
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_Buildings.Length; i++)
		{
			BuildingPrefab buildingPrefab = m_Buildings[i];
			if (!((Object)(object)buildingPrefab == (Object)null))
			{
				((EntityManager)(ref entityManager)).GetBuffer<ServiceUpgradeBuilding>(entity, false).Add(new ServiceUpgradeBuilding(existingSystemManaged.GetEntity(buildingPrefab)));
				buildingPrefab.AddUpgrade(entityManager, this);
			}
		}
	}
}

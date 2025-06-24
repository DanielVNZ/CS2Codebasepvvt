using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Economy;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
[RequireComponent(typeof(CityServiceBuilding))]
public class ResourceProducer : ComponentBase, IServiceUpgrade
{
	public ResourceProductionInfo[] m_Resources;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ResourceProductionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Resources>());
			components.Add(ComponentType.ReadWrite<Game.Buildings.ResourceProducer>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Resources>());
		components.Add(ComponentType.ReadWrite<Game.Buildings.ResourceProducer>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Resources != null)
		{
			DynamicBuffer<ResourceProductionData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ResourceProductionData>(entity, false);
			buffer.ResizeUninitialized(m_Resources.Length);
			for (int i = 0; i < m_Resources.Length; i++)
			{
				ResourceProductionInfo resourceProductionInfo = m_Resources[i];
				buffer[i] = new ResourceProductionData(EconomyUtils.GetResource(resourceProductionInfo.m_Resource), resourceProductionInfo.m_ProductionRate, resourceProductionInfo.m_StorageCapacity);
			}
		}
	}
}

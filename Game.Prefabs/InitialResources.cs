using System;
using System.Collections.Generic;
using Game.Economy;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class InitialResources : ComponentBase, IServiceUpgrade
{
	public InitialResourceItem[] m_InitialResources;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<InitialResourceData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Resources>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Resources>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DynamicBuffer<InitialResourceData> val = ((EntityManager)(ref entityManager)).AddBuffer<InitialResourceData>(entity);
		if (m_InitialResources != null)
		{
			for (int i = 0; i < m_InitialResources.Length; i++)
			{
				val.Add(new InitialResourceData
				{
					m_Value = new ResourceStack
					{
						m_Resource = EconomyUtils.GetResource(m_InitialResources[i].m_Value.m_Resource),
						m_Amount = m_InitialResources[i].m_Value.m_Amount
					}
				});
			}
		}
	}
}

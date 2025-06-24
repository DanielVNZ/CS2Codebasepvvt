using System;
using System.Collections.Generic;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[]
{
	typeof(NetPrefab),
	typeof(ObjectPrefab)
})]
public class ResourceConnection : ComponentBase
{
	public ResourceInEditor m_Resource;

	public NotificationIconPrefab m_ConnectionWarningNotification;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_ConnectionWarningNotification != (Object)null)
		{
			prefabs.Add(m_ConnectionWarningNotification);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ResourceConnectionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (components.Contains(ComponentType.ReadWrite<Node>()))
		{
			components.Add(ComponentType.ReadWrite<Game.Net.ResourceConnection>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			components.Add(ComponentType.ReadWrite<Game.Net.ResourceConnection>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Game.Objects.Object>()))
		{
			components.Add(ComponentType.ReadWrite<Game.Net.ResourceConnection>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		ResourceConnectionData resourceConnectionData = new ResourceConnectionData
		{
			m_Resource = EconomyUtils.GetResource(m_Resource)
		};
		if ((Object)(object)m_ConnectionWarningNotification != (Object)null)
		{
			resourceConnectionData.m_ConnectionWarningNotification = orCreateSystemManaged.GetEntity(m_ConnectionWarningNotification);
		}
		((EntityManager)(ref entityManager)).SetComponentData<ResourceConnectionData>(entity, resourceConnectionData);
	}
}

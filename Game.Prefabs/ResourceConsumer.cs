using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
[RequireComponent(typeof(CityServiceBuilding))]
public class ResourceConsumer : ComponentBase
{
	[CanBeNull]
	public NotificationIconPrefab m_NoResourceNotificationPrefab;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_NoResourceNotificationPrefab != (Object)null)
		{
			prefabs.Add(m_NoResourceNotificationPrefab);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ResourceConsumerData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.ResourceConsumer>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<ResourceConsumerData>(entity, new ResourceConsumerData
		{
			m_NoResourceNotificationPrefab = (((Object)(object)m_NoResourceNotificationPrefab != (Object)null) ? orCreateSystemManaged.GetEntity(m_NoResourceNotificationPrefab) : Entity.Null)
		});
	}
}

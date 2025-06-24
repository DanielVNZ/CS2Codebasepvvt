using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Services/", new Type[]
{
	typeof(ObjectPrefab),
	typeof(NetPrefab),
	typeof(AreaPrefab),
	typeof(ZonePrefab),
	typeof(RoutePrefab),
	typeof(TerraformingPrefab)
})]
public class ServiceObject : ComponentBase
{
	public ServicePrefab m_Service;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ServiceObjectData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<ServiceObjectData>(entity, new ServiceObjectData
		{
			m_Service = existingSystemManaged.GetEntity(m_Service)
		});
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_Service);
	}
}

using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Economy;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { typeof(LotPrefab) })]
public class StorageArea : ComponentBase
{
	public ResourceInEditor[] m_StoredResources;

	public int m_Capacity;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<StorageAreaData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Storage>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		Resource resource = Resource.NoResource;
		if (m_StoredResources != null)
		{
			for (int i = 0; i < m_StoredResources.Length; i++)
			{
				resource |= EconomyUtils.GetResource(m_StoredResources[i]);
			}
		}
		StorageAreaData storageAreaData = default(StorageAreaData);
		storageAreaData.m_Resources = resource;
		storageAreaData.m_Capacity = m_Capacity;
		((EntityManager)(ref entityManager)).SetComponentData<StorageAreaData>(entity, storageAreaData);
	}
}

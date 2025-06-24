using System;
using System.Collections.Generic;
using Game.City;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
public class ServiceFeeCollector : ComponentBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.City.ServiceFeeCollector>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
	}
}

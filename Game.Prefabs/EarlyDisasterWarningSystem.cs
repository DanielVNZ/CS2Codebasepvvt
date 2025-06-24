using System;
using System.Collections.Generic;
using Game.Buildings;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class EarlyDisasterWarningSystem : ComponentBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.EarlyDisasterWarningSystem>());
	}
}

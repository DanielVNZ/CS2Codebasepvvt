using System;
using System.Collections.Generic;
using Game.Buildings;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
public class BuildingEfficiency : ComponentBase, IServiceUpgrade
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!base.prefab.Has<ServiceUpgrade>())
		{
			components.Add(ComponentType.ReadWrite<Efficiency>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Efficiency>());
	}
}

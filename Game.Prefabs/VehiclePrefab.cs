using System.Collections.Generic;
using Game.Objects;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public abstract class VehiclePrefab : MovingObjectPrefab
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<VehicleData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Vehicle>());
		components.Add(ComponentType.ReadWrite<Color>());
		components.Add(ComponentType.ReadWrite<Surface>());
	}
}

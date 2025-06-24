using System;
using System.Collections.Generic;
using Game.Areas;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { })]
public class SurfacePrefab : AreaPrefab
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<SurfaceData>());
		components.Add(ComponentType.ReadWrite<AreaGeometryData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Surface>());
		components.Add(ComponentType.ReadWrite<Geometry>());
		components.Add(ComponentType.ReadWrite<Expand>());
	}
}

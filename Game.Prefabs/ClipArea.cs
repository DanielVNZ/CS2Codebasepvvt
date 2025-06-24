using System;
using System.Collections.Generic;
using Game.Areas;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { typeof(SurfacePrefab) })]
public class ClipArea : ComponentBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Clip>());
	}
}

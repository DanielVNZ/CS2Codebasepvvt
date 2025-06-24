using System;
using System.Collections.Generic;
using Game.Routes;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Routes/", new Type[] { })]
public class LivePathPrefab : RoutePrefab
{
	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		base.GetPrefabComponents(components);
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Route>()))
		{
			components.Add(ComponentType.ReadWrite<LivePath>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Waypoint>()))
		{
			components.Add(ComponentType.ReadWrite<LivePath>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Segment>()))
		{
			components.Add(ComponentType.ReadWrite<LivePath>());
			components.Add(ComponentType.ReadWrite<PathSource>());
			components.Add(ComponentType.ReadWrite<CurveSource>());
		}
	}
}

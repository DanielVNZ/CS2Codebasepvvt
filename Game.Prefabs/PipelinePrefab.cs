using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/Prefab/", new Type[] { })]
public class PipelinePrefab : NetGeometryPrefab
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<PipelineData>());
		components.Add(ComponentType.ReadWrite<LocalConnectData>());
		components.Add(ComponentType.ReadWrite<DefaultNetLane>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			components.Add(ComponentType.ReadWrite<EdgeColor>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Node>()))
		{
			components.Add(ComponentType.ReadWrite<NodeColor>());
		}
	}
}

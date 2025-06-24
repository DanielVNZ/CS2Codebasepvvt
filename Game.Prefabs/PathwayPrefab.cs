using System;
using System.Collections.Generic;
using Game.Net;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/Prefab/", new Type[] { })]
public class PathwayPrefab : NetGeometryPrefab
{
	public float m_SpeedLimit = 40f;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<PathwayData>());
		components.Add(ComponentType.ReadWrite<LocalConnectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
			components.Add(ComponentType.ReadWrite<EdgeColor>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Node>()))
		{
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
			components.Add(ComponentType.ReadWrite<NodeColor>());
		}
		else if (components.Contains(ComponentType.ReadWrite<NetCompositionData>()))
		{
			components.Add(ComponentType.ReadWrite<PathwayComposition>());
		}
	}
}

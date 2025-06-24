using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetLanePrefab) })]
public class PedestrianLane : ComponentBase
{
	public float m_Width;

	public bool m_OnWater;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PedestrianLaneData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Net.PedestrianLane>());
		components.Add(ComponentType.ReadWrite<LaneObject>());
		components.Add(ComponentType.ReadWrite<LaneOverlap>());
	}
}

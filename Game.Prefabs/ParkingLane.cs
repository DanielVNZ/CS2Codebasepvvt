using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetLanePrefab) })]
public class ParkingLane : ComponentBase
{
	public RoadTypes m_RoadType = RoadTypes.Car;

	public float2 m_SlotSize;

	public float m_SlotAngle;

	public bool m_SpecialVehicles;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ParkingLaneData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Net.ParkingLane>());
		components.Add(ComponentType.ReadWrite<LaneObject>());
		components.Add(ComponentType.ReadWrite<LaneOverlap>());
	}
}

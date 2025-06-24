using System;
using System.Collections.Generic;
using Game.Net;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetLanePrefab) })]
public class CarLane : ComponentBase
{
	public NetLanePrefab m_NotTrackLane;

	public NetLanePrefab m_NotBusLane;

	public RoadTypes m_RoadType = RoadTypes.Car;

	public SizeClass m_MaxSize = SizeClass.Large;

	public float m_Width = 3f;

	public bool m_StartingLane;

	public bool m_EndingLane;

	public bool m_Twoway;

	public bool m_BusLane;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_NotTrackLane != (Object)null)
		{
			prefabs.Add(m_NotTrackLane);
		}
		if ((Object)(object)m_NotBusLane != (Object)null)
		{
			prefabs.Add(m_NotBusLane);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CarLaneData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Net.CarLane>());
		if (!components.Contains(ComponentType.ReadWrite<MasterLane>()))
		{
			components.Add(ComponentType.ReadWrite<LaneObject>());
			components.Add(ComponentType.ReadWrite<LaneReservation>());
			components.Add(ComponentType.ReadWrite<LaneFlow>());
			components.Add(ComponentType.ReadWrite<LaneOverlap>());
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
		}
	}
}

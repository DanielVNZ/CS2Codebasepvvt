using System;
using System.Collections.Generic;
using Game.Net;
using Game.Simulation;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetLanePrefab) })]
public class TrackLane : ComponentBase
{
	public NetLanePrefab m_FallbackLane;

	public ObjectPrefab m_EndObject;

	public TrackTypes m_TrackType = TrackTypes.Train;

	public float m_Width = 4f;

	public float m_MaxCurviness = 1.8f;

	public bool m_Twoway;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_FallbackLane != (Object)null)
		{
			prefabs.Add(m_FallbackLane);
		}
		if ((Object)(object)m_EndObject != (Object)null)
		{
			prefabs.Add(m_EndObject);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TrackLaneData>());
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
		components.Add(ComponentType.ReadWrite<Game.Net.TrackLane>());
		if (!components.Contains(ComponentType.ReadWrite<MasterLane>()))
		{
			components.Add(ComponentType.ReadWrite<LaneObject>());
			components.Add(ComponentType.ReadWrite<LaneReservation>());
			components.Add(ComponentType.ReadWrite<LaneColor>());
			components.Add(ComponentType.ReadWrite<LaneOverlap>());
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
		}
	}
}

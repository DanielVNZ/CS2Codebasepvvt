using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetLanePrefab) })]
public class UtilityLane : ComponentBase
{
	public UtilityTypes m_UtilityType = UtilityTypes.WaterPipe;

	public NetLanePrefab m_LocalConnectionLane;

	public NetLanePrefab m_LocalConnectionLane2;

	public ObjectPrefab m_NodeObject;

	public float m_Width;

	public float m_VisualCapacity;

	public float m_Hanging;

	public bool m_Underground;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_LocalConnectionLane != (Object)null)
		{
			prefabs.Add(m_LocalConnectionLane);
		}
		if ((Object)(object)m_LocalConnectionLane2 != (Object)null)
		{
			prefabs.Add(m_LocalConnectionLane2);
		}
		if ((Object)(object)m_NodeObject != (Object)null)
		{
			prefabs.Add(m_NodeObject);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<UtilityLaneData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Net.UtilityLane>());
		components.Add(ComponentType.ReadWrite<LaneColor>());
		if ((m_UtilityType & ~(UtilityTypes.StormwaterPipe | UtilityTypes.Fence | UtilityTypes.Catenary)) != UtilityTypes.None)
		{
			components.Add(ComponentType.ReadWrite<EdgeMapping>());
			components.Add(ComponentType.ReadWrite<SubFlow>());
		}
		if (m_Hanging != 0f)
		{
			components.Add(ComponentType.ReadWrite<HangingLane>());
		}
	}
}

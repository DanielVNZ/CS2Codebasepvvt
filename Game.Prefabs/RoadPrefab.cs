using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Game.Net;
using Game.Objects;
using Game.Simulation;
using Game.Zones;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/Prefab/", new Type[] { })]
public class RoadPrefab : NetGeometryPrefab
{
	public RoadType m_RoadType;

	public float m_SpeedLimit = 100f;

	public ZoneBlockPrefab m_ZoneBlock;

	public bool m_TrafficLights;

	public bool m_HighwayRules;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			yield return "Roads";
		}
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_ZoneBlock != (Object)null)
		{
			prefabs.Add(m_ZoneBlock);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<RoadData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			components.Add(ComponentType.ReadWrite<Road>());
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
			components.Add(ComponentType.ReadWrite<LandValue>());
			components.Add(ComponentType.ReadWrite<EdgeColor>());
			components.Add(ComponentType.ReadWrite<NetCondition>());
			components.Add(ComponentType.ReadWrite<MaintenanceConsumer>());
			components.Add(ComponentType.ReadWrite<BorderDistrict>());
			if ((Object)(object)m_ZoneBlock != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<SubBlock>());
				components.Add(ComponentType.ReadWrite<ConnectedBuilding>());
				components.Add(ComponentType.ReadWrite<Game.Net.ServiceCoverage>());
				components.Add(ComponentType.ReadWrite<ResourceAvailability>());
				components.Add(ComponentType.ReadWrite<Density>());
			}
			else if (!m_HighwayRules)
			{
				components.Add(ComponentType.ReadWrite<ConnectedBuilding>());
			}
		}
		else if (components.Contains(ComponentType.ReadWrite<Game.Net.Node>()))
		{
			components.Add(ComponentType.ReadWrite<Road>());
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
			components.Add(ComponentType.ReadWrite<LandValue>());
			components.Add(ComponentType.ReadWrite<NodeColor>());
			components.Add(ComponentType.ReadWrite<NetCondition>());
			components.Add(ComponentType.ReadWrite<Game.Objects.Surface>());
		}
		else if (components.Contains(ComponentType.ReadWrite<NetCompositionData>()))
		{
			components.Add(ComponentType.ReadWrite<RoadComposition>());
		}
	}
}

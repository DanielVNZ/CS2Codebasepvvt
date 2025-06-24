using System;
using System.Collections.Generic;
using Game.Net;
using Game.PSI;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/Prefab/", new Type[] { })]
public class TrackPrefab : NetGeometryPrefab
{
	public TrackTypes m_TrackType = TrackTypes.Train;

	public float m_SpeedLimit = 100f;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			foreach (string enumFlagTag in ModTags.GetEnumFlagTags(m_TrackType, TrackTypes.Train))
			{
				yield return enumFlagTag + "Track";
			}
		}
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TrackData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
			components.Add(ComponentType.ReadWrite<EdgeColor>());
			AddTrackType(components);
		}
		else if (components.Contains(ComponentType.ReadWrite<Node>()))
		{
			components.Add(ComponentType.ReadWrite<UpdateFrame>());
			components.Add(ComponentType.ReadWrite<NodeColor>());
			AddTrackType(components);
		}
		else if (components.Contains(ComponentType.ReadWrite<NetCompositionData>()))
		{
			components.Add(ComponentType.ReadWrite<TrackComposition>());
		}
	}

	private void AddTrackType(HashSet<ComponentType> components)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		switch (m_TrackType)
		{
		case TrackTypes.Train:
			components.Add(ComponentType.ReadWrite<TrainTrack>());
			break;
		case TrackTypes.Tram:
			components.Add(ComponentType.ReadWrite<TramTrack>());
			break;
		case TrackTypes.Subway:
			components.Add(ComponentType.ReadWrite<SubwayTrack>());
			break;
		case TrackTypes.Train | TrackTypes.Tram:
			break;
		}
	}
}

using System;
using System.Collections.Generic;
using Game.Objects;
using Game.Rendering;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[] { })]
public class CarTrailerPrefab : CarBasePrefab
{
	public CarTrailerType m_TrailerType = CarTrailerType.Towbar;

	public TrailerMovementType m_MovementType;

	public float3 m_AttachOffset = new float3(0f, 0.5f, 0f);

	public CarBasePrefab m_FixedTractor;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_FixedTractor != (Object)null)
		{
			prefabs.Add(m_FixedTractor);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<CarData>());
		components.Add(ComponentType.ReadWrite<CarTrailerData>());
		components.Add(ComponentType.ReadWrite<SwayingData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<CarTrailer>());
		components.Add(ComponentType.ReadWrite<Controller>());
		components.Add(ComponentType.ReadWrite<BlockedLane>());
		if (components.Contains(ComponentType.ReadWrite<Stopped>()))
		{
			components.Add(ComponentType.ReadWrite<ParkedCar>());
		}
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<CarTrailerLane>());
			components.Add(ComponentType.ReadWrite<Swaying>());
		}
	}
}

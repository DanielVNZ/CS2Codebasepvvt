using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[]
{
	typeof(CarPrefab),
	typeof(CarTrailerPrefab)
})]
public class CarTractor : ComponentBase
{
	public CarTrailerType m_TrailerType = CarTrailerType.Towbar;

	public float3 m_AttachOffset = new float3(0f, 0.5f, 0f);

	public CarTrailerPrefab m_FixedTrailer;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_FixedTrailer != (Object)null)
		{
			prefabs.Add(m_FixedTrailer);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CarTractorData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}

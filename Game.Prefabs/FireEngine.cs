using System;
using System.Collections.Generic;
using Game.Objects;
using Game.Pathfind;
using Game.PSI;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ExcludeGeneratedModTag]
[ComponentMenu("Vehicles/", new Type[]
{
	typeof(CarPrefab),
	typeof(AircraftPrefab)
})]
public class FireEngine : ComponentBase
{
	public float m_ExtinguishingRate = 7f;

	public float m_ExtinguishingSpread = 20f;

	public float m_ExtinguishingCapacity;

	public float m_DestroyedClearDuration = 10f;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			if ((Object)(object)GetComponent<AircraftPrefab>() != (Object)null)
			{
				yield return "FireEngineAircraft";
			}
			else
			{
				yield return "FireEngine";
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<FireEngineData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Vehicles.FireEngine>());
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<PathInformation>());
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<FireEngineData>(entity, new FireEngineData(m_ExtinguishingRate, m_ExtinguishingSpread, m_ExtinguishingCapacity, m_DestroyedClearDuration));
		if (((EntityManager)(ref entityManager)).HasComponent<CarData>(entity))
		{
			((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(4));
		}
	}
}

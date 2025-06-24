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
public class Ambulance : ComponentBase
{
	public int m_PatientCapacity = 1;

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
				yield return "AmbulanceAircraft";
			}
			else
			{
				yield return "Ambulance";
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AmbulanceData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Vehicles.Ambulance>());
		components.Add(ComponentType.ReadWrite<Passenger>());
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<PathInformation>());
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<AmbulanceData>(entity, new AmbulanceData(m_PatientCapacity));
		if (((EntityManager)(ref entityManager)).HasComponent<CarData>(entity))
		{
			((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(0));
		}
	}
}

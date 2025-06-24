using System;
using System.Collections.Generic;
using Game.Common;
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
public class PoliceCar : ComponentBase
{
	public int m_CriminalCapacity = 2;

	public float m_CrimeReductionRate = 10000f;

	public float m_ShiftDuration = 1f;

	[EnumFlag]
	public PolicePurpose m_Purposes = PolicePurpose.Patrol | PolicePurpose.Emergency;

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
				yield return "PoliceCarAircraft";
			}
			else
			{
				yield return "PoliceCar";
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PoliceCarData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Vehicles.PoliceCar>());
		components.Add(ComponentType.ReadWrite<Passenger>());
		components.Add(ComponentType.ReadWrite<PointOfInterest>());
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<PathInformation>());
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		uint shiftDuration = (uint)(m_ShiftDuration * 262144f);
		((EntityManager)(ref entityManager)).SetComponentData<PoliceCarData>(entity, new PoliceCarData(m_CriminalCapacity, m_CrimeReductionRate, shiftDuration, m_Purposes));
		if (((EntityManager)(ref entityManager)).HasComponent<CarData>(entity))
		{
			((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(5));
		}
	}
}

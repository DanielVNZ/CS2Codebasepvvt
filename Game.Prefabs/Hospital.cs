using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab),
	typeof(MarkerObjectPrefab)
})]
public class Hospital : ComponentBase, IServiceUpgrade
{
	public int m_AmbulanceCapacity = 10;

	public int m_MedicalHelicopterCapacity;

	public int m_PatientCapacity = 10;

	public int m_TreatmentBonus = 3;

	public int2 m_HealthRange = new int2(0, 100);

	public bool m_TreatDiseases = true;

	public bool m_TreatInjuries = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<HospitalData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Game.Buildings.Hospital>());
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
				components.Add(ComponentType.ReadWrite<ServiceUsage>());
			}
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			if ((Object)(object)GetComponent<UniqueObject>() == (Object)null)
			{
				components.Add(ComponentType.ReadWrite<ServiceDistrict>());
			}
			if (m_PatientCapacity != 0)
			{
				components.Add(ComponentType.ReadWrite<Patient>());
			}
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.Hospital>());
		components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		components.Add(ComponentType.ReadWrite<ServiceUsage>());
		if (m_PatientCapacity != 0)
		{
			components.Add(ComponentType.ReadWrite<Patient>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<HospitalData>(entity, new HospitalData
		{
			m_AmbulanceCapacity = m_AmbulanceCapacity,
			m_MedicalHelicopterCapacity = m_MedicalHelicopterCapacity,
			m_PatientCapacity = m_PatientCapacity,
			m_TreatmentBonus = m_TreatmentBonus,
			m_HealthRange = m_HealthRange,
			m_TreatDiseases = m_TreatDiseases,
			m_TreatInjuries = m_TreatInjuries
		});
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(1));
	}
}

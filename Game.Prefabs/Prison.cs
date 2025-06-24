using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class Prison : ComponentBase, IServiceUpgrade
{
	public int m_PrisonVanCapacity = 10;

	public int m_PrisonerCapacity = 500;

	public sbyte m_PrisonerWellbeing;

	public sbyte m_PrisonerHealth;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PrisonData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.Prison>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			if ((Object)(object)GetComponent<UniqueObject>() == (Object)null)
			{
				components.Add(ComponentType.ReadWrite<ServiceDistrict>());
			}
			if (m_PrisonerCapacity != 0)
			{
				components.Add(ComponentType.ReadWrite<Occupant>());
			}
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.Prison>());
		components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		if (m_PrisonerCapacity != 0)
		{
			components.Add(ComponentType.ReadWrite<Occupant>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		PrisonData prisonData = default(PrisonData);
		prisonData.m_PrisonVanCapacity = m_PrisonVanCapacity;
		prisonData.m_PrisonerCapacity = m_PrisonerCapacity;
		prisonData.m_PrisonerWellbeing = m_PrisonerWellbeing;
		prisonData.m_PrisonerHealth = m_PrisonerHealth;
		((EntityManager)(ref entityManager)).SetComponentData<PrisonData>(entity, prisonData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(3));
	}
}

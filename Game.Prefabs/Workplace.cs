using System;
using System.Collections.Generic;
using Game.Companies;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class Workplace : ComponentBase, IServiceUpgrade
{
	[Tooltip("The max amount of workers for City Service Buildings. ATTENTION: the other companies' max amount of workers changed dynamically, the max amount of workers of them depend on the m_MaxWorkersPerCell of each company prefab data")]
	public int m_Workplaces;

	[Tooltip("The minimum amount of workers of this workplace")]
	public int m_MinimumWorkersLimit;

	public WorkplaceComplexity m_Complexity;

	public float m_EveningShiftProbability;

	public float m_NightShiftProbability;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WorkplaceData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null && m_Workplaces > 0)
		{
			components.Add(ComponentType.ReadWrite<WorkProvider>());
			components.Add(ComponentType.ReadWrite<Employee>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (m_Workplaces > 0)
		{
			components.Add(ComponentType.ReadWrite<WorkProvider>());
			components.Add(ComponentType.ReadWrite<Employee>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<WorkplaceData>(entity, new WorkplaceData
		{
			m_MaxWorkers = m_Workplaces,
			m_MinimumWorkersLimit = m_MinimumWorkersLimit,
			m_Complexity = m_Complexity,
			m_EveningShiftProbability = m_EveningShiftProbability,
			m_NightShiftProbability = m_NightShiftProbability
		});
	}
}

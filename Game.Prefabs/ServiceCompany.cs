using System;
using System.Collections.Generic;
using Game.Companies;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Companies/", new Type[] { typeof(CompanyPrefab) })]
public class ServiceCompany : ComponentBase
{
	public int m_MaxService;

	public float m_MaxWorkersPerCell;

	[Tooltip("The service consumed per leisure tick")]
	public int m_ServiceConsuming = 1;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ServiceCompanyData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ServiceAvailable>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<ServiceCompanyData>(entity, new ServiceCompanyData
		{
			m_MaxService = m_MaxService,
			m_WorkPerUnit = 0,
			m_MaxWorkersPerCell = m_MaxWorkersPerCell,
			m_ServiceConsuming = m_ServiceConsuming
		});
	}
}

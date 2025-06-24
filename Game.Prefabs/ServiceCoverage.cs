using System;
using System.Collections.Generic;
using Game.Net;
using Game.Pathfind;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(StaticObjectPrefab) })]
public class ServiceCoverage : ComponentBase
{
	public float m_Range = 1000f;

	public float m_Capacity = 3000f;

	public float m_Magnitude = 1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CoverageData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CoverageServiceType>());
		components.Add(ComponentType.ReadWrite<CoverageElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		CoverageData coverageData = new CoverageData
		{
			m_Range = m_Range,
			m_Capacity = m_Capacity,
			m_Magnitude = m_Magnitude
		};
		if (((EntityManager)(ref entityManager)).HasComponent<HospitalData>(entity))
		{
			coverageData.m_Service = CoverageService.Healthcare;
		}
		else if (((EntityManager)(ref entityManager)).HasComponent<FireStationData>(entity))
		{
			coverageData.m_Service = CoverageService.FireRescue;
		}
		else if (((EntityManager)(ref entityManager)).HasComponent<PoliceStationData>(entity))
		{
			coverageData.m_Service = CoverageService.Police;
		}
		else if (((EntityManager)(ref entityManager)).HasComponent<ParkData>(entity))
		{
			coverageData.m_Service = CoverageService.Park;
		}
		else if (((EntityManager)(ref entityManager)).HasComponent<PostFacilityData>(entity) || ((EntityManager)(ref entityManager)).HasComponent<MailBoxData>(entity))
		{
			coverageData.m_Service = CoverageService.PostService;
		}
		else if (((EntityManager)(ref entityManager)).HasComponent<SchoolData>(entity))
		{
			coverageData.m_Service = CoverageService.Education;
		}
		else if (((EntityManager)(ref entityManager)).HasComponent<EmergencyShelterData>(entity))
		{
			coverageData.m_Service = CoverageService.EmergencyShelter;
		}
		else if (((EntityManager)(ref entityManager)).HasComponent<WelfareOfficeData>(entity))
		{
			coverageData.m_Service = CoverageService.Welfare;
		}
		else
		{
			ComponentBase.baseLog.ErrorFormat((Object)(object)base.prefab, "Unknown coverage service type: {0}", (object)((Object)base.prefab).name);
		}
		((EntityManager)(ref entityManager)).SetComponentData<CoverageData>(entity, coverageData);
	}
}

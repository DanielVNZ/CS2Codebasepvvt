using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab),
	typeof(MarkerObjectPrefab)
})]
public class School : ComponentBase, IServiceUpgrade
{
	public int m_StudentCapacity = 80;

	public SchoolLevel m_Level;

	public float m_GraduationModifier;

	public sbyte m_StudentWellbeing;

	public sbyte m_StudentHealth;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SchoolData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.School>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			components.Add(ComponentType.ReadWrite<Student>());
			if ((Object)(object)GetComponent<UniqueObject>() == (Object)null)
			{
				components.Add(ComponentType.ReadWrite<ServiceDistrict>());
			}
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.School>());
		components.Add(ComponentType.ReadWrite<Student>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		SchoolData schoolData = default(SchoolData);
		schoolData.m_EducationLevel = (byte)m_Level;
		schoolData.m_StudentCapacity = m_StudentCapacity;
		schoolData.m_GraduationModifier = m_GraduationModifier;
		schoolData.m_StudentWellbeing = m_StudentWellbeing;
		schoolData.m_StudentHealth = m_StudentHealth;
		((EntityManager)(ref entityManager)).SetComponentData<SchoolData>(entity, schoolData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(6));
	}
}

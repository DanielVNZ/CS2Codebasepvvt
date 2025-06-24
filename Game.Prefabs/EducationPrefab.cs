using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class EducationPrefab : PrefabBase
{
	public PrefabBase m_EducationServicePrefab;

	[Tooltip("Probability that a student leaves a disabled school (1024 rolls per day)")]
	[Range(0f, 1f)]
	public float m_InoperableSchoolLeaveProbability = 0.1f;

	[Tooltip("Probability that a student enter the high school")]
	[Range(0f, 1f)]
	public float m_EnterHighSchoolProbability = 0.75f;

	[Tooltip("Probability for adult to enter the high school")]
	[Range(0f, 1f)]
	public float m_AdultEnterHighSchoolProbability = 0.1f;

	[Tooltip("Probability for worker to enter the high school")]
	[Range(0f, 1f)]
	public float m_WorkerContinueEducationProbability = 0.1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<EducationParameterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<EducationParameterData>(entity, new EducationParameterData
		{
			m_EducationServicePrefab = orCreateSystemManaged.GetEntity(m_EducationServicePrefab),
			m_InoperableSchoolLeaveProbability = m_InoperableSchoolLeaveProbability,
			m_EnterHighSchoolProbability = m_EnterHighSchoolProbability,
			m_AdultEnterHighSchoolProbability = m_AdultEnterHighSchoolProbability,
			m_WorkerContinueEducationProbability = m_WorkerContinueEducationProbability
		});
	}
}

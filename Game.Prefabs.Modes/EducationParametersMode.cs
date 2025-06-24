using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class EducationParametersMode : EntityQueryModePrefab
{
	[Range(0f, 1f)]
	public float m_InoperableSchoolLeaveProbability;

	[Range(0f, 1f)]
	public float m_EnterHighSchoolProbability;

	[Range(0f, 1f)]
	public float m_AdultEnterHighSchoolProbability;

	[Range(0f, 1f)]
	public float m_WorkerContinueEducationProbability;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<EducationParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		EducationParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<EducationParameterData>(singletonEntity);
		componentData.m_InoperableSchoolLeaveProbability = m_InoperableSchoolLeaveProbability;
		componentData.m_EnterHighSchoolProbability = m_EnterHighSchoolProbability;
		componentData.m_AdultEnterHighSchoolProbability = m_AdultEnterHighSchoolProbability;
		componentData.m_WorkerContinueEducationProbability = m_WorkerContinueEducationProbability;
		((EntityManager)(ref entityManager)).SetComponentData<EducationParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		EducationPrefab educationPrefab = prefabSystem.GetPrefab<EducationPrefab>(val);
		EducationParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<EducationParameterData>(val);
		componentData.m_InoperableSchoolLeaveProbability = educationPrefab.m_InoperableSchoolLeaveProbability;
		componentData.m_EnterHighSchoolProbability = educationPrefab.m_EnterHighSchoolProbability;
		componentData.m_AdultEnterHighSchoolProbability = educationPrefab.m_AdultEnterHighSchoolProbability;
		componentData.m_WorkerContinueEducationProbability = educationPrefab.m_WorkerContinueEducationProbability;
		((EntityManager)(ref entityManager)).SetComponentData<EducationParameterData>(val, componentData);
	}
}

using System;
using Colossal.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class HealthcareParametersMode : EntityQueryModePrefab
{
	public float m_TransportWarningTime = 15f;

	public float m_NoResourceTreatmentPenalty = 0.5f;

	public float m_BuildingDestoryDeathRate = 0.5f;

	public AnimationCurve m_DeathRate;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<HealthcareParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		HealthcareParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<HealthcareParameterData>(singletonEntity);
		componentData.m_TransportWarningTime = m_TransportWarningTime;
		componentData.m_NoResourceTreatmentPenalty = m_NoResourceTreatmentPenalty;
		componentData.m_BuildingDestoryDeathRate = m_BuildingDestoryDeathRate;
		componentData.m_DeathRate = new AnimationCurve1(m_DeathRate);
		((EntityManager)(ref entityManager)).SetComponentData<HealthcareParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		HealthcarePrefab healthcarePrefab = prefabSystem.GetPrefab<HealthcarePrefab>(val);
		HealthcareParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<HealthcareParameterData>(val);
		componentData.m_TransportWarningTime = healthcarePrefab.m_TransportWarningTime;
		componentData.m_NoResourceTreatmentPenalty = healthcarePrefab.m_NoResourceTreatmentPenalty;
		componentData.m_BuildingDestoryDeathRate = healthcarePrefab.m_BuildingDestoryDeathRate;
		componentData.m_DeathRate = new AnimationCurve1(healthcarePrefab.m_DeathRate);
		((EntityManager)(ref entityManager)).SetComponentData<HealthcareParameterData>(val, componentData);
	}
}

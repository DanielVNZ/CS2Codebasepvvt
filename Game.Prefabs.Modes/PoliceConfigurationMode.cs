using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class PoliceConfigurationMode : EntityQueryModePrefab
{
	public float m_MaxCrimeAccumulationMultiplier;

	public float m_CrimeAccumulationToleranceMultiplier;

	public int m_HomeCrimeEffectMultiplier;

	public int m_WorkplaceCrimeEffectMultiplier;

	public float m_WelfareCrimeRecurrenceFactor;

	public float m_CrimePoliceCoverageFactorMultiflier;

	public float m_CrimePopulationReductionMultiplier;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<PoliceConfigurationData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		PoliceConfigurationData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PoliceConfigurationData>(singletonEntity);
		componentData.m_MaxCrimeAccumulation *= m_MaxCrimeAccumulationMultiplier;
		componentData.m_CrimeAccumulationTolerance *= m_CrimeAccumulationToleranceMultiplier;
		componentData.m_HomeCrimeEffect *= m_HomeCrimeEffectMultiplier;
		componentData.m_WorkplaceCrimeEffect *= m_WorkplaceCrimeEffectMultiplier;
		componentData.m_WelfareCrimeRecurrenceFactor = m_WelfareCrimeRecurrenceFactor;
		componentData.m_CrimePoliceCoverageFactor *= m_CrimePoliceCoverageFactorMultiflier;
		componentData.m_CrimePopulationReduction *= m_CrimePopulationReductionMultiplier;
		((EntityManager)(ref entityManager)).SetComponentData<PoliceConfigurationData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		PoliceConfigurationPrefab policeConfigurationPrefab = prefabSystem.GetPrefab<PoliceConfigurationPrefab>(val);
		PoliceConfigurationData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PoliceConfigurationData>(val);
		componentData.m_MaxCrimeAccumulation = policeConfigurationPrefab.m_MaxCrimeAccumulation;
		componentData.m_CrimeAccumulationTolerance = policeConfigurationPrefab.m_CrimeAccumulationTolerance;
		componentData.m_HomeCrimeEffect = policeConfigurationPrefab.m_HomeCrimeEffect;
		componentData.m_WorkplaceCrimeEffect = policeConfigurationPrefab.m_WorkplaceCrimeEffect;
		componentData.m_WelfareCrimeRecurrenceFactor = policeConfigurationPrefab.m_WelfareCrimeRecurrenceFactor;
		componentData.m_CrimePoliceCoverageFactor = policeConfigurationPrefab.m_CrimePoliceCoverageFactor;
		componentData.m_CrimePopulationReduction = policeConfigurationPrefab.m_CrimePopulationReduction;
		((EntityManager)(ref entityManager)).SetComponentData<PoliceConfigurationData>(val, componentData);
	}
}

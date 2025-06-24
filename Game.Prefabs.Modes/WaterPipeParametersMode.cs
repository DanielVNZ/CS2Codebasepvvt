using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class WaterPipeParametersMode : EntityQueryModePrefab
{
	public float m_GroundwaterReplenish;

	public int m_GroundwaterPurification;

	public float m_GroundwaterUsageMultiplier;

	public float m_GroundwaterPumpEffectiveAmount;

	public float m_SurfaceWaterUsageMultiplier;

	public float m_SurfaceWaterPumpEffectiveDepth;

	[Range(0f, 1f)]
	public float m_MaxToleratedPollution;

	[Range(1f, 32f)]
	public int m_WaterPipePollutionSpreadInterval;

	[Range(0f, 1f)]
	public float m_StaleWaterPipePurification;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<WaterPipeParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		WaterPipeParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WaterPipeParameterData>(singletonEntity);
		componentData.m_GroundwaterReplenish = m_GroundwaterReplenish;
		componentData.m_GroundwaterPurification = m_GroundwaterPurification;
		componentData.m_GroundwaterUsageMultiplier = m_GroundwaterUsageMultiplier;
		componentData.m_GroundwaterPumpEffectiveAmount = m_GroundwaterPumpEffectiveAmount;
		componentData.m_SurfaceWaterUsageMultiplier = m_SurfaceWaterUsageMultiplier;
		componentData.m_SurfaceWaterPumpEffectiveDepth = m_SurfaceWaterPumpEffectiveDepth;
		componentData.m_MaxToleratedPollution = m_MaxToleratedPollution;
		componentData.m_WaterPipePollutionSpreadInterval = m_WaterPipePollutionSpreadInterval;
		componentData.m_StaleWaterPipePurification = m_StaleWaterPipePurification;
		((EntityManager)(ref entityManager)).SetComponentData<WaterPipeParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		WaterPipeParametersPrefab waterPipeParametersPrefab = prefabSystem.GetPrefab<WaterPipeParametersPrefab>(val);
		WaterPipeParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WaterPipeParameterData>(val);
		componentData.m_GroundwaterReplenish = waterPipeParametersPrefab.m_GroundwaterReplenish;
		componentData.m_GroundwaterPurification = waterPipeParametersPrefab.m_GroundwaterPurification;
		componentData.m_GroundwaterUsageMultiplier = waterPipeParametersPrefab.m_GroundwaterUsageMultiplier;
		componentData.m_GroundwaterPumpEffectiveAmount = waterPipeParametersPrefab.m_GroundwaterPumpEffectiveAmount;
		componentData.m_SurfaceWaterUsageMultiplier = waterPipeParametersPrefab.m_SurfaceWaterUsageMultiplier;
		componentData.m_SurfaceWaterPumpEffectiveDepth = waterPipeParametersPrefab.m_SurfaceWaterPumpEffectiveDepth;
		componentData.m_MaxToleratedPollution = waterPipeParametersPrefab.m_MaxToleratedPollution;
		componentData.m_WaterPipePollutionSpreadInterval = waterPipeParametersPrefab.m_WaterPipePollutionSpreadInterval;
		componentData.m_StaleWaterPipePurification = waterPipeParametersPrefab.m_StaleWaterPipePurification;
		((EntityManager)(ref entityManager)).SetComponentData<WaterPipeParameterData>(val, componentData);
	}
}

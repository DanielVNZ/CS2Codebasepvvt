using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class SoilWaterMode : EntityQueryModePrefab
{
	public float m_RainMultiplier;

	public float m_HeightEffect;

	public float m_MaxDiffusion;

	public float m_WaterPerUnit;

	public float m_MoistureUnderWater;

	public float m_MaximumWaterDepth;

	public float m_OverflowRate;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SoilWaterParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<SoilWaterParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		SoilWaterParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SoilWaterParameterData>(singletonEntity);
		componentData.m_RainMultiplier = m_RainMultiplier;
		componentData.m_HeightEffect = m_HeightEffect;
		componentData.m_MaxDiffusion = m_MaxDiffusion;
		componentData.m_WaterPerUnit = m_WaterPerUnit;
		componentData.m_MoistureUnderWater = m_MoistureUnderWater;
		componentData.m_MaximumWaterDepth = m_MaximumWaterDepth;
		componentData.m_OverflowRate = m_OverflowRate;
		((EntityManager)(ref entityManager)).SetComponentData<SoilWaterParameterData>(singletonEntity, componentData);
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
		SoilWaterPrefab soilWaterPrefab = prefabSystem.GetPrefab<SoilWaterPrefab>(val);
		SoilWaterParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SoilWaterParameterData>(val);
		componentData.m_RainMultiplier = soilWaterPrefab.m_RainMultiplier;
		componentData.m_HeightEffect = soilWaterPrefab.m_HeightEffect;
		componentData.m_MaxDiffusion = soilWaterPrefab.m_MaxDiffusion;
		componentData.m_WaterPerUnit = soilWaterPrefab.m_WaterPerUnit;
		componentData.m_MoistureUnderWater = soilWaterPrefab.m_MoistureUnderWater;
		componentData.m_MaximumWaterDepth = soilWaterPrefab.m_MaximumWaterDepth;
		componentData.m_OverflowRate = soilWaterPrefab.m_OverflowRate;
		((EntityManager)(ref entityManager)).SetComponentData<SoilWaterParameterData>(val, componentData);
	}
}

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class AttractivenessParametersMode : EntityQueryModePrefab
{
	public float m_ForestEffect;

	public float m_ForestDistance;

	public float m_ShoreEffect;

	public float m_ShoreDistance;

	public float3 m_HeightBonus;

	public float2 m_AttractiveTemperature;

	public float2 m_ExtremeTemperature;

	public float2 m_TemperatureAffect;

	public float2 m_RainEffectRange;

	public float2 m_SnowEffectRange;

	public float3 m_SnowRainExtremeAffect;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<AttractivenessParameterData>(entity);
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		AttractivenessParametersPrefab attractivenessParametersPrefab = prefabSystem.GetPrefab<AttractivenessParametersPrefab>(val);
		AttractivenessParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AttractivenessParameterData>(val);
		componentData.m_ForestEffect = attractivenessParametersPrefab.m_ForestEffect;
		componentData.m_ForestDistance = attractivenessParametersPrefab.m_ForestDistance;
		componentData.m_ShoreEffect = attractivenessParametersPrefab.m_ShoreEffect;
		componentData.m_ShoreDistance = attractivenessParametersPrefab.m_ShoreDistance;
		componentData.m_HeightBonus = attractivenessParametersPrefab.m_HeightBonus;
		componentData.m_AttractiveTemperature = attractivenessParametersPrefab.m_AttractiveTemperature;
		componentData.m_ExtremeTemperature = attractivenessParametersPrefab.m_ExtremeTemperature;
		componentData.m_TemperatureAffect = attractivenessParametersPrefab.m_TemperatureAffect;
		componentData.m_RainEffectRange = attractivenessParametersPrefab.m_RainEffectRange;
		componentData.m_SnowEffectRange = attractivenessParametersPrefab.m_SnowEffectRange;
		componentData.m_SnowRainExtremeAffect = attractivenessParametersPrefab.m_SnowRainExtremeAffect;
		((EntityManager)(ref entityManager)).SetComponentData<AttractivenessParameterData>(val, componentData);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		AttractivenessParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AttractivenessParameterData>(singletonEntity);
		componentData.m_ForestEffect = m_ForestEffect;
		componentData.m_ForestDistance = m_ForestDistance;
		componentData.m_ShoreEffect = m_ShoreEffect;
		componentData.m_ShoreDistance = m_ShoreDistance;
		componentData.m_HeightBonus = m_HeightBonus;
		componentData.m_AttractiveTemperature = m_AttractiveTemperature;
		componentData.m_ExtremeTemperature = m_ExtremeTemperature;
		componentData.m_TemperatureAffect = m_TemperatureAffect;
		componentData.m_RainEffectRange = m_RainEffectRange;
		componentData.m_SnowEffectRange = m_SnowEffectRange;
		componentData.m_SnowRainExtremeAffect = m_SnowRainExtremeAffect;
		((EntityManager)(ref entityManager)).SetComponentData<AttractivenessParameterData>(singletonEntity, componentData);
		return deps;
	}
}

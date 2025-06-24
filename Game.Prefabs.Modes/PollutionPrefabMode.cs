using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class PollutionPrefabMode : EntityQueryModePrefab
{
	public float m_GroundMultiplier;

	public float m_AirMultiplier;

	public float m_NoiseMultiplier;

	public float m_NetAirMultiplier;

	public float m_NetNoiseMultiplier;

	public float m_GroundRadius;

	public float m_AirRadius;

	public float m_NoiseRadius;

	public float m_NetNoiseRadius;

	public float m_WindAdvectionSpeed;

	public short m_AirFade;

	public short m_GroundFade;

	public float m_PlantAirMultiplier;

	public float m_PlantGroundMultiplier;

	public float m_PlantFade;

	public float m_FertilityGroundMultiplier;

	public float m_DistanceExponent;

	public int m_AirPollutionNotificationLimit;

	public int m_NoisePollutionNotificationLimit;

	public int m_GroundPollutionNotificationLimit;

	public float m_AbandonedNoisePollutionMultiplier;

	public int m_HomelessNoisePollution;

	public int m_GroundPollutionLandValueDivisor;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<PollutionParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		PollutionParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PollutionParameterData>(singletonEntity);
		componentData.m_GroundMultiplier = m_GroundMultiplier;
		componentData.m_AirMultiplier = m_AirMultiplier;
		componentData.m_NoiseMultiplier = m_NoiseMultiplier;
		componentData.m_NetAirMultiplier = m_NetAirMultiplier;
		componentData.m_NetNoiseMultiplier = m_NetNoiseMultiplier;
		componentData.m_GroundRadius = m_GroundRadius;
		componentData.m_AirRadius = m_AirRadius;
		componentData.m_NoiseRadius = m_NoiseRadius;
		componentData.m_NetNoiseRadius = m_NetNoiseRadius;
		componentData.m_WindAdvectionSpeed = m_WindAdvectionSpeed;
		componentData.m_AirFade = m_AirFade;
		componentData.m_GroundFade = m_GroundFade;
		componentData.m_PlantAirMultiplier = m_PlantAirMultiplier;
		componentData.m_PlantGroundMultiplier = m_PlantGroundMultiplier;
		componentData.m_PlantFade = m_PlantFade;
		componentData.m_FertilityGroundMultiplier = m_FertilityGroundMultiplier;
		componentData.m_DistanceExponent = m_DistanceExponent;
		componentData.m_AirPollutionNotificationLimit = m_AirPollutionNotificationLimit;
		componentData.m_NoisePollutionNotificationLimit = m_NoisePollutionNotificationLimit;
		componentData.m_GroundPollutionNotificationLimit = m_GroundPollutionNotificationLimit;
		componentData.m_AbandonedNoisePollutionMultiplier = m_AbandonedNoisePollutionMultiplier;
		componentData.m_HomelessNoisePollution = m_HomelessNoisePollution;
		componentData.m_GroundPollutionLandValueDivisor = m_GroundPollutionLandValueDivisor;
		((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		PollutionPrefab pollutionPrefab = prefabSystem.GetPrefab<PollutionPrefab>(val);
		PollutionParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PollutionParameterData>(val);
		componentData.m_GroundMultiplier = pollutionPrefab.m_GroundMultiplier;
		componentData.m_AirMultiplier = pollutionPrefab.m_AirMultiplier;
		componentData.m_NoiseMultiplier = pollutionPrefab.m_NoiseMultiplier;
		componentData.m_NetAirMultiplier = pollutionPrefab.m_NetAirMultiplier;
		componentData.m_NetNoiseMultiplier = pollutionPrefab.m_NetNoiseMultiplier;
		componentData.m_GroundRadius = pollutionPrefab.m_GroundRadius;
		componentData.m_AirRadius = pollutionPrefab.m_AirRadius;
		componentData.m_NoiseRadius = pollutionPrefab.m_NoiseRadius;
		componentData.m_NetNoiseRadius = pollutionPrefab.m_NetNoiseRadius;
		componentData.m_WindAdvectionSpeed = pollutionPrefab.m_WindAdvectionSpeed;
		componentData.m_AirFade = pollutionPrefab.m_AirFade;
		componentData.m_GroundFade = pollutionPrefab.m_GroundFade;
		componentData.m_PlantAirMultiplier = pollutionPrefab.m_PlantAirMultiplier;
		componentData.m_PlantGroundMultiplier = pollutionPrefab.m_PlantGroundMultiplier;
		componentData.m_PlantFade = pollutionPrefab.m_PlantFade;
		componentData.m_FertilityGroundMultiplier = pollutionPrefab.m_FertilityGroundMultiplier;
		componentData.m_DistanceExponent = pollutionPrefab.m_DistanceExponent;
		componentData.m_AirPollutionNotificationLimit = pollutionPrefab.m_AirPollutionNotificationLimit;
		componentData.m_NoisePollutionNotificationLimit = pollutionPrefab.m_NoisePollutionNotificationLimit;
		componentData.m_GroundPollutionNotificationLimit = pollutionPrefab.m_GroundPollutionNotificationLimit;
		componentData.m_AbandonedNoisePollutionMultiplier = pollutionPrefab.m_AbandonedNoisePollutionMultiplier;
		componentData.m_HomelessNoisePollution = pollutionPrefab.m_HomelessNoisePollution;
		componentData.m_GroundPollutionLandValueDivisor = pollutionPrefab.m_GroundPollutionLandValueDivisor;
		((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(val, componentData);
	}
}

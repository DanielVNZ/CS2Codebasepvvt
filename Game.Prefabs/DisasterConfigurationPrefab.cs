using System;
using System.Collections.Generic;
using Colossal.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class DisasterConfigurationPrefab : PrefabBase
{
	public NotificationIconPrefab m_WeatherDamageNotificationPrefab;

	public NotificationIconPrefab m_WeatherDestroyedNotificationPrefab;

	public NotificationIconPrefab m_WaterDamageNotificationPrefab;

	public NotificationIconPrefab m_WaterDestroyedNotificationPrefab;

	public NotificationIconPrefab m_DestroyedNotificationPrefab;

	public float m_FloodDamageRate = 200f;

	[Tooltip("Correlation between the general danger level (0.0-1.0) in the city and the probability that the cim will exit the shelter if there is no imminent danger to their home, workplace or school (1024 rolls per day).\nThe y value at 0.0 determines how quickly cims will leave the shelter when there is no danger.")]
	public AnimationCurve m_EmergencyShelterDangerLevelExitProbability;

	[Tooltip("Probability that a cim will exit an inoperable emergency shelter (1024 rolls per day)")]
	[Range(0f, 1f)]
	public float m_InoperableEmergencyShelterExitProbability = 0.1f;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_WeatherDamageNotificationPrefab);
		prefabs.Add(m_WeatherDestroyedNotificationPrefab);
		prefabs.Add(m_WaterDamageNotificationPrefab);
		prefabs.Add(m_WaterDestroyedNotificationPrefab);
		prefabs.Add(m_DestroyedNotificationPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<DisasterConfigurationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<DisasterConfigurationData>(entity, new DisasterConfigurationData
		{
			m_WeatherDamageNotificationPrefab = existingSystemManaged.GetEntity(m_WeatherDamageNotificationPrefab),
			m_WeatherDestroyedNotificationPrefab = existingSystemManaged.GetEntity(m_WeatherDestroyedNotificationPrefab),
			m_WaterDamageNotificationPrefab = existingSystemManaged.GetEntity(m_WaterDamageNotificationPrefab),
			m_WaterDestroyedNotificationPrefab = existingSystemManaged.GetEntity(m_WaterDestroyedNotificationPrefab),
			m_DestroyedNotificationPrefab = existingSystemManaged.GetEntity(m_DestroyedNotificationPrefab),
			m_FloodDamageRate = m_FloodDamageRate,
			m_EmergencyShelterDangerLevelExitProbability = new AnimationCurve1(m_EmergencyShelterDangerLevelExitProbability),
			m_InoperableEmergencyShelterExitProbability = m_InoperableEmergencyShelterExitProbability
		});
	}
}

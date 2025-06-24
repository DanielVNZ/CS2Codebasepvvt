using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class FireConfigurationPrefab : PrefabBase
{
	public NotificationIconPrefab m_FireNotificationPrefab;

	public NotificationIconPrefab m_BurnedDownNotificationPrefab;

	public float m_DefaultStructuralIntegrity = 3000f;

	public float m_BuildingStructuralIntegrity = 15000f;

	public float m_StructuralIntegrityLevel1 = 12000f;

	public float m_StructuralIntegrityLevel2 = 13000f;

	public float m_StructuralIntegrityLevel3 = 14000f;

	public float m_StructuralIntegrityLevel4 = 15000f;

	public float m_StructuralIntegrityLevel5 = 16000f;

	public Bounds1 m_ResponseTimeRange = new Bounds1(3f, 30f);

	public float m_TelecomResponseTimeModifier = -0.15f;

	public float m_DarknessResponseTimeModifier = 0.1f;

	public AnimationCurve m_TemperatureForestFireHazard;

	public AnimationCurve m_NoRainForestFireHazard;

	public float m_DeathRateOfFireAccident = 0.01f;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_FireNotificationPrefab);
		prefabs.Add(m_BurnedDownNotificationPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<FireConfigurationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		FireConfigurationData fireConfigurationData = default(FireConfigurationData);
		fireConfigurationData.m_FireNotificationPrefab = existingSystemManaged.GetEntity(m_FireNotificationPrefab);
		fireConfigurationData.m_BurnedDownNotificationPrefab = existingSystemManaged.GetEntity(m_BurnedDownNotificationPrefab);
		fireConfigurationData.m_DefaultStructuralIntegrity = m_DefaultStructuralIntegrity;
		fireConfigurationData.m_BuildingStructuralIntegrity = m_BuildingStructuralIntegrity;
		fireConfigurationData.m_StructuralIntegrityLevel1 = m_StructuralIntegrityLevel1;
		fireConfigurationData.m_StructuralIntegrityLevel2 = m_StructuralIntegrityLevel2;
		fireConfigurationData.m_StructuralIntegrityLevel3 = m_StructuralIntegrityLevel3;
		fireConfigurationData.m_StructuralIntegrityLevel4 = m_StructuralIntegrityLevel4;
		fireConfigurationData.m_StructuralIntegrityLevel5 = m_StructuralIntegrityLevel5;
		fireConfigurationData.m_ResponseTimeRange = m_ResponseTimeRange;
		fireConfigurationData.m_TelecomResponseTimeModifier = m_TelecomResponseTimeModifier;
		fireConfigurationData.m_DarknessResponseTimeModifier = m_DarknessResponseTimeModifier;
		fireConfigurationData.m_DeathRateOfFireAccident = m_DeathRateOfFireAccident;
		((EntityManager)(ref entityManager)).SetComponentData<FireConfigurationData>(entity, fireConfigurationData);
	}
}

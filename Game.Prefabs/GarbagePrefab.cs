using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class GarbagePrefab : PrefabBase
{
	public ServicePrefab m_GarbageServicePrefab;

	public NotificationIconPrefab m_GarbageNotificationPrefab;

	public NotificationIconPrefab m_FacilityFullNotificationPrefab;

	[Tooltip("The garbage produce amount of homeless")]
	public int m_HomelessGarbageProduce = 25;

	public int m_CollectionGarbageLimit = 20;

	public int m_RequestGarbageLimit = 100;

	public int m_WarningGarbageLimit = 500;

	public int m_MaxGarbageAccumulation = 2000;

	public float m_BuildingLevelBalance = 1.25f;

	public float m_EducationBalance = 2.5f;

	[Tooltip("The baseline of garbage accumulated amount to affect happiness")]
	public int m_HappinessEffectBaseline = 100;

	[Tooltip("The step of garbage accumulated amount to affect happiness, e.g. baseline(100)+step(65) = 165 accumulated garbage to have -1 happiness bonus")]
	public int m_HappinessEffectStep = 65;

	public override bool ignoreUnlockDependencies => true;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_GarbageServicePrefab);
		prefabs.Add(m_GarbageNotificationPrefab);
		prefabs.Add(m_FacilityFullNotificationPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<GarbageParameterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		GarbageParameterData garbageParameterData = default(GarbageParameterData);
		garbageParameterData.m_GarbageServicePrefab = orCreateSystemManaged.GetEntity(m_GarbageServicePrefab);
		garbageParameterData.m_GarbageNotificationPrefab = orCreateSystemManaged.GetEntity(m_GarbageNotificationPrefab);
		garbageParameterData.m_FacilityFullNotificationPrefab = orCreateSystemManaged.GetEntity(m_FacilityFullNotificationPrefab);
		garbageParameterData.m_HomelessGarbageProduce = m_HomelessGarbageProduce;
		garbageParameterData.m_CollectionGarbageLimit = m_CollectionGarbageLimit;
		garbageParameterData.m_RequestGarbageLimit = m_RequestGarbageLimit;
		garbageParameterData.m_WarningGarbageLimit = m_WarningGarbageLimit;
		garbageParameterData.m_MaxGarbageAccumulation = m_MaxGarbageAccumulation;
		garbageParameterData.m_BuildingLevelBalance = m_BuildingLevelBalance;
		garbageParameterData.m_EducationBalance = m_EducationBalance;
		garbageParameterData.m_HappinessEffectBaseline = m_HappinessEffectBaseline;
		garbageParameterData.m_HappinessEffectStep = m_HappinessEffectStep;
		((EntityManager)(ref entityManager)).SetComponentData<GarbageParameterData>(entity, garbageParameterData);
	}
}

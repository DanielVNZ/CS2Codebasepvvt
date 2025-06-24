using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class WorkProviderParameterPrefab : PrefabBase
{
	public NotificationIconPrefab m_UneducatedNotificationPrefab;

	public NotificationIconPrefab m_EducatedNotificationPrefab;

	[Tooltip("Delay in ticks for the 'missing uneducated workers' notification to appear (512 ticks per day)")]
	[Min(1f)]
	public short m_UneducatedNotificationDelay = 128;

	[Tooltip("Delay in ticks for the 'missing educated workers' notification to appear (512 ticks per day)")]
	[Min(1f)]
	public short m_EducatedNotificationDelay = 128;

	[Tooltip("Percentage of uneducated workers missing for the 'missing uneducated workers' notification to show up")]
	[Range(0f, 1f)]
	public float m_UneducatedNotificationLimit = 0.6f;

	[Tooltip("Percentage of educated workers missing for the 'missing educated workers' notification to show up")]
	[Range(0f, 1f)]
	public float m_EducatedNotificationLimit = 0.7f;

	public int m_SeniorEmployeeLevel = 3;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_UneducatedNotificationPrefab);
		prefabs.Add(m_EducatedNotificationPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<WorkProviderParameterData>());
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
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<WorkProviderParameterData>(entity, new WorkProviderParameterData
		{
			m_EducatedNotificationPrefab = orCreateSystemManaged.GetEntity(m_EducatedNotificationPrefab),
			m_UneducatedNotificationPrefab = orCreateSystemManaged.GetEntity(m_UneducatedNotificationPrefab),
			m_EducatedNotificationDelay = m_EducatedNotificationDelay,
			m_EducatedNotificationLimit = m_EducatedNotificationLimit,
			m_UneducatedNotificationDelay = m_UneducatedNotificationDelay,
			m_UneducatedNotificationLimit = m_UneducatedNotificationLimit,
			m_SeniorEmployeeLevel = m_SeniorEmployeeLevel
		});
	}
}

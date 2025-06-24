using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class CompanyNotificationParameterPrefab : PrefabBase
{
	public NotificationIconPrefab m_NoInputsNotificationPrefab;

	public NotificationIconPrefab m_NoCustomersNotificationPrefab;

	public float m_NoInputCostLimit = 5f;

	public float m_NoCustomersServiceLimit = 0.9f;

	[Tooltip("The limit of empty rooms percentage of total room amount, 0.9 means 90% rooms are empty")]
	public float m_NoCustomersHotelLimit = 0.9f;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_NoInputsNotificationPrefab);
		prefabs.Add(m_NoCustomersNotificationPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<CompanyNotificationParameterData>());
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
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		CompanyNotificationParameterData companyNotificationParameterData = default(CompanyNotificationParameterData);
		companyNotificationParameterData.m_NoCustomersNotificationPrefab = orCreateSystemManaged.GetEntity(m_NoCustomersNotificationPrefab);
		companyNotificationParameterData.m_NoInputsNotificationPrefab = orCreateSystemManaged.GetEntity(m_NoInputsNotificationPrefab);
		companyNotificationParameterData.m_NoCustomersServiceLimit = m_NoCustomersServiceLimit;
		companyNotificationParameterData.m_NoInputCostLimit = m_NoInputCostLimit;
		companyNotificationParameterData.m_NoCustomersHotelLimit = m_NoCustomersHotelLimit;
		((EntityManager)(ref entityManager)).SetComponentData<CompanyNotificationParameterData>(entity, companyNotificationParameterData);
	}
}

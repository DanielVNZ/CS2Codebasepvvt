using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class WorkProviderParameterMode : EntityQueryModePrefab
{
	[Min(1f)]
	public short m_UneducatedNotificationDelay;

	[Min(1f)]
	public short m_EducatedNotificationDelay;

	[Range(0f, 1f)]
	public float m_UneducatedNotificationLimit;

	[Range(0f, 1f)]
	public float m_EducatedNotificationLimit;

	public int m_SeniorEmployeeLevel;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WorkProviderParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<WorkProviderParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		WorkProviderParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WorkProviderParameterData>(singletonEntity);
		componentData.m_UneducatedNotificationDelay = m_UneducatedNotificationDelay;
		componentData.m_EducatedNotificationDelay = m_EducatedNotificationDelay;
		componentData.m_UneducatedNotificationLimit = m_UneducatedNotificationLimit;
		componentData.m_EducatedNotificationLimit = m_EducatedNotificationLimit;
		componentData.m_SeniorEmployeeLevel = m_SeniorEmployeeLevel;
		((EntityManager)(ref entityManager)).SetComponentData<WorkProviderParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		WorkProviderParameterPrefab workProviderParameterPrefab = prefabSystem.GetPrefab<WorkProviderParameterPrefab>(val);
		WorkProviderParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WorkProviderParameterData>(val);
		componentData.m_UneducatedNotificationDelay = workProviderParameterPrefab.m_UneducatedNotificationDelay;
		componentData.m_EducatedNotificationDelay = workProviderParameterPrefab.m_EducatedNotificationDelay;
		componentData.m_UneducatedNotificationLimit = workProviderParameterPrefab.m_UneducatedNotificationLimit;
		componentData.m_EducatedNotificationLimit = workProviderParameterPrefab.m_EducatedNotificationLimit;
		componentData.m_SeniorEmployeeLevel = workProviderParameterPrefab.m_SeniorEmployeeLevel;
		((EntityManager)(ref entityManager)).SetComponentData<WorkProviderParameterData>(val, componentData);
	}
}

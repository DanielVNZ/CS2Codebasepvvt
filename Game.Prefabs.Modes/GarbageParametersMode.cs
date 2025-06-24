using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class GarbageParametersMode : EntityQueryModePrefab
{
	public int m_HomelessGarbageProduce = 25;

	public int m_CollectionGarbageLimit = 20;

	public int m_RequestGarbageLimit = 100;

	public int m_WarningGarbageLimit = 500;

	public int m_MaxGarbageAccumulation = 2000;

	public float m_BuildingLevelBalance = 1.25f;

	public float m_EducationBalance = 2.5f;

	public int m_HappinessEffectBaseline = 100;

	public int m_HappinessEffectStep = 65;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<GarbageParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		GarbageParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<GarbageParameterData>(singletonEntity);
		componentData.m_HomelessGarbageProduce = m_HomelessGarbageProduce;
		componentData.m_CollectionGarbageLimit = m_CollectionGarbageLimit;
		componentData.m_RequestGarbageLimit = m_RequestGarbageLimit;
		componentData.m_WarningGarbageLimit = m_WarningGarbageLimit;
		componentData.m_MaxGarbageAccumulation = m_MaxGarbageAccumulation;
		componentData.m_BuildingLevelBalance = m_BuildingLevelBalance;
		componentData.m_EducationBalance = m_EducationBalance;
		componentData.m_HappinessEffectBaseline = m_HappinessEffectBaseline;
		componentData.m_HappinessEffectStep = m_HappinessEffectStep;
		((EntityManager)(ref entityManager)).SetComponentData<GarbageParameterData>(singletonEntity, componentData);
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
		GarbagePrefab garbagePrefab = prefabSystem.GetPrefab<GarbagePrefab>(val);
		GarbageParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<GarbageParameterData>(val);
		componentData.m_HomelessGarbageProduce = garbagePrefab.m_HomelessGarbageProduce;
		componentData.m_CollectionGarbageLimit = garbagePrefab.m_CollectionGarbageLimit;
		componentData.m_RequestGarbageLimit = garbagePrefab.m_RequestGarbageLimit;
		componentData.m_WarningGarbageLimit = garbagePrefab.m_WarningGarbageLimit;
		componentData.m_MaxGarbageAccumulation = garbagePrefab.m_MaxGarbageAccumulation;
		componentData.m_BuildingLevelBalance = garbagePrefab.m_BuildingLevelBalance;
		componentData.m_EducationBalance = garbagePrefab.m_EducationBalance;
		componentData.m_HappinessEffectBaseline = garbagePrefab.m_HappinessEffectBaseline;
		componentData.m_HappinessEffectStep = garbagePrefab.m_HappinessEffectStep;
		((EntityManager)(ref entityManager)).SetComponentData<GarbageParameterData>(val, componentData);
	}
}

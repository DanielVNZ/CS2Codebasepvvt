using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class LeisureParametersMode : EntityQueryModePrefab
{
	public int m_LeisureRandomFactor;

	public int m_TouristLodgingConsumePerDay;

	public int m_TouristServiceConsumePerDay;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LeisureParametersData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<LeisureParametersData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		LeisureParametersData componentData = ((EntityManager)(ref entityManager)).GetComponentData<LeisureParametersData>(singletonEntity);
		componentData.m_LeisureRandomFactor = m_LeisureRandomFactor;
		componentData.m_TouristLodgingConsumePerDay = m_TouristLodgingConsumePerDay;
		componentData.m_TouristServiceConsumePerDay = m_TouristServiceConsumePerDay;
		((EntityManager)(ref entityManager)).SetComponentData<LeisureParametersData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		LeisureParametersPrefab leisureParametersPrefab = prefabSystem.GetPrefab<LeisureParametersPrefab>(val);
		LeisureParametersData componentData = ((EntityManager)(ref entityManager)).GetComponentData<LeisureParametersData>(val);
		componentData.m_LeisureRandomFactor = leisureParametersPrefab.m_LeisureRandomFactor;
		componentData.m_TouristLodgingConsumePerDay = leisureParametersPrefab.m_TouristLodgingConsumePerDay;
		componentData.m_TouristServiceConsumePerDay = leisureParametersPrefab.m_TouristServiceConsumePerDay;
		((EntityManager)(ref entityManager)).SetComponentData<LeisureParametersData>(val, componentData);
	}
}

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class HappinessFactorParameterMode : EntityQueryModePrefab
{
	public int m_TaxBaseLevel;

	public int taxIndex => 17;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HappinessFactorParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetBuffer<HappinessFactorParameterData>(entity, false);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		DynamicBuffer<HappinessFactorParameterData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HappinessFactorParameterData>(singletonEntity, false);
		HappinessFactorParameterData happinessFactorParameterData = buffer[taxIndex];
		happinessFactorParameterData.m_BaseLevel = m_TaxBaseLevel;
		buffer[taxIndex] = happinessFactorParameterData;
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		HappinessFactorParameterPrefab happinessFactorParameterPrefab = prefabSystem.GetPrefab<HappinessFactorParameterPrefab>(val);
		DynamicBuffer<HappinessFactorParameterData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HappinessFactorParameterData>(val, false);
		HappinessFactorParameterData happinessFactorParameterData = buffer[taxIndex];
		happinessFactorParameterData.m_BaseLevel = happinessFactorParameterPrefab.m_BaseLevels[taxIndex];
		buffer[taxIndex] = happinessFactorParameterData;
	}
}

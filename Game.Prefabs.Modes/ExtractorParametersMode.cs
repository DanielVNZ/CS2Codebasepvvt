using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class ExtractorParametersMode : EntityQueryModePrefab
{
	public float m_FertilityConsumption;

	public float m_OreConsumption;

	public float m_ForestConsumption;

	public float m_OilConsumption;

	public float m_FullFertility;

	public float m_FullOre;

	public float m_FullOil;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ExtractorParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<ExtractorParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		ExtractorParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ExtractorParameterData>(singletonEntity);
		componentData.m_FertilityConsumption = m_FertilityConsumption;
		componentData.m_OreConsumption = m_OreConsumption;
		componentData.m_ForestConsumption = m_ForestConsumption;
		componentData.m_OilConsumption = m_OilConsumption;
		componentData.m_FullFertility = m_FullFertility;
		componentData.m_FullOre = m_FullOre;
		componentData.m_FullOil = m_FullOil;
		((EntityManager)(ref entityManager)).SetComponentData<ExtractorParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		ExtractorParameterPrefab extractorParameterPrefab = prefabSystem.GetPrefab<ExtractorParameterPrefab>(val);
		ExtractorParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ExtractorParameterData>(val);
		componentData.m_FertilityConsumption = extractorParameterPrefab.m_FertilityConsumption;
		componentData.m_OreConsumption = extractorParameterPrefab.m_OreConsumption;
		componentData.m_ForestConsumption = extractorParameterPrefab.m_ForestConsumption;
		componentData.m_OilConsumption = extractorParameterPrefab.m_OilConsumption;
		componentData.m_FullFertility = extractorParameterPrefab.m_FullFertility;
		componentData.m_FullOre = extractorParameterPrefab.m_FullOre;
		componentData.m_FullOil = extractorParameterPrefab.m_FullOil;
		((EntityManager)(ref entityManager)).SetComponentData<ExtractorParameterData>(val, componentData);
	}
}

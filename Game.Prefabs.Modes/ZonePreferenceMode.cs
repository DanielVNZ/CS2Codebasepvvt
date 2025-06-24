using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class ZonePreferenceMode : EntityQueryModePrefab
{
	public float m_ResidentialSignificanceServices;

	public float m_ResidentialSignificanceWorkplaces;

	public float m_ResidentialSignificanceLandValue;

	public float m_ResidentialSignificancePollution;

	public float m_ResidentialNeutralLandValue;

	public float m_CommercialSignificanceConsumers;

	public float m_CommercialSignificanceCompetitors;

	public float m_CommercialSignificanceWorkplaces;

	public float m_CommercialSignificanceLandValue;

	public float m_CommercialNeutralLandValue;

	public float m_IndustrialSignificanceInput;

	public float m_IndustrialSignificanceOutside;

	public float m_IndustrialSignificanceLandValue;

	public float m_IndustrialNeutralLandValue;

	public float m_OfficeSignificanceEmployees;

	public float m_OfficeSignificanceServices;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ZonePreferenceData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<ZonePreferenceData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		ZonePreferenceData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ZonePreferenceData>(singletonEntity);
		componentData.m_ResidentialSignificanceServices = m_ResidentialSignificanceServices;
		componentData.m_ResidentialSignificanceWorkplaces = m_ResidentialSignificanceWorkplaces;
		componentData.m_ResidentialSignificanceLandValue = m_ResidentialSignificanceLandValue;
		componentData.m_ResidentialSignificancePollution = m_ResidentialSignificancePollution;
		componentData.m_ResidentialNeutralLandValue = m_ResidentialNeutralLandValue;
		componentData.m_CommercialSignificanceConsumers = m_CommercialSignificanceConsumers;
		componentData.m_CommercialSignificanceCompetitors = m_CommercialSignificanceCompetitors;
		componentData.m_CommercialSignificanceWorkplaces = m_CommercialSignificanceWorkplaces;
		componentData.m_CommercialSignificanceLandValue = m_CommercialSignificanceLandValue;
		componentData.m_CommercialNeutralLandValue = m_CommercialNeutralLandValue;
		componentData.m_IndustrialSignificanceInput = m_IndustrialSignificanceInput;
		componentData.m_IndustrialSignificanceOutside = m_IndustrialSignificanceOutside;
		componentData.m_IndustrialSignificanceLandValue = m_IndustrialSignificanceLandValue;
		componentData.m_IndustrialNeutralLandValue = m_IndustrialNeutralLandValue;
		componentData.m_OfficeSignificanceEmployees = m_OfficeSignificanceEmployees;
		componentData.m_OfficeSignificanceServices = m_OfficeSignificanceServices;
		((EntityManager)(ref entityManager)).SetComponentData<ZonePreferenceData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		ZonePreferencePrefab zonePreferencePrefab = prefabSystem.GetPrefab<ZonePreferencePrefab>(val);
		ZonePreferenceData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ZonePreferenceData>(val);
		componentData.m_ResidentialSignificanceServices = zonePreferencePrefab.m_ResidentialSignificanceServices;
		componentData.m_ResidentialSignificanceWorkplaces = zonePreferencePrefab.m_ResidentialSignificanceWorkplaces;
		componentData.m_ResidentialSignificanceLandValue = zonePreferencePrefab.m_ResidentialSignificanceLandValue;
		componentData.m_ResidentialSignificancePollution = zonePreferencePrefab.m_ResidentialSignificancePollution;
		componentData.m_ResidentialNeutralLandValue = zonePreferencePrefab.m_ResidentialNeutralLandValue;
		componentData.m_CommercialSignificanceConsumers = zonePreferencePrefab.m_CommercialSignificanceConsumers;
		componentData.m_CommercialSignificanceCompetitors = zonePreferencePrefab.m_CommercialSignificanceCompetitors;
		componentData.m_CommercialSignificanceWorkplaces = zonePreferencePrefab.m_CommercialSignificanceWorkplaces;
		componentData.m_CommercialSignificanceLandValue = zonePreferencePrefab.m_CommercialSignificanceLandValue;
		componentData.m_CommercialNeutralLandValue = zonePreferencePrefab.m_CommercialNeutralLandValue;
		componentData.m_IndustrialSignificanceInput = zonePreferencePrefab.m_IndustrialSignificanceInput;
		componentData.m_IndustrialSignificanceOutside = zonePreferencePrefab.m_IndustrialSignificanceOutside;
		componentData.m_IndustrialSignificanceLandValue = zonePreferencePrefab.m_IndustrialSignificanceLandValue;
		componentData.m_IndustrialNeutralLandValue = zonePreferencePrefab.m_IndustrialNeutralLandValue;
		componentData.m_OfficeSignificanceEmployees = zonePreferencePrefab.m_OfficeSignificanceEmployees;
		componentData.m_OfficeSignificanceServices = zonePreferencePrefab.m_OfficeSignificanceServices;
		((EntityManager)(ref entityManager)).SetComponentData<ZonePreferenceData>(val, componentData);
	}
}

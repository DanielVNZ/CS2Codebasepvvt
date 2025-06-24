using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class DemandParameterMode : EntityQueryModePrefab
{
	public int m_MinimumHappiness;

	public float m_HappinessEffect;

	public float3 m_TaxEffect;

	public float m_StudentEffect;

	public float m_AvailableWorkplaceEffect;

	public float m_HomelessEffect;

	public int m_NeutralHappiness;

	public float m_NeutralUnemployment;

	public float m_NeutralAvailableWorkplacePercentage;

	public int m_NeutralHomelessness;

	public int3 m_FreeResidentialRequirement;

	public float m_CommercialBaseDemand;

	public float m_IndustrialBaseDemand;

	public float m_ExtractorBaseDemand;

	public int m_CommuterWorkerRatioLimit;

	public int m_CommuterSlowSpawnFactor;

	public float4 m_CommuterOCSpawnParameters;

	public float4 m_TouristOCSpawnParameters;

	public float4 m_CitizenOCSpawnParameters;

	public float m_TeenSpawnPercentage;

	public int3 m_FrameIntervalForSpawning;

	public float m_HouseholdSpawnSpeedFactor;

	public float m_HotelRoomPercentRequirement;

	public float4 m_NewCitizenEducationParameters;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<DemandParameterData>(entity);
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		DemandPrefab demandPrefab = prefabSystem.GetPrefab<DemandPrefab>(val);
		DemandParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<DemandParameterData>(val);
		componentData.m_MinimumHappiness = demandPrefab.m_MinimumHappiness;
		componentData.m_HappinessEffect = demandPrefab.m_HappinessEffect;
		componentData.m_TaxEffect = demandPrefab.m_TaxEffect;
		componentData.m_StudentEffect = demandPrefab.m_StudentEffect;
		componentData.m_AvailableWorkplaceEffect = demandPrefab.m_AvailableWorkplaceEffect;
		componentData.m_HomelessEffect = demandPrefab.m_HomelessEffect;
		componentData.m_NeutralHappiness = demandPrefab.m_NeutralHappiness;
		componentData.m_NeutralUnemployment = demandPrefab.m_NeutralUnemployment;
		componentData.m_NeutralAvailableWorkplacePercentage = demandPrefab.m_NeutralAvailableWorkplacePercentage;
		componentData.m_NeutralHomelessness = demandPrefab.m_NeutralHomelessness;
		componentData.m_FreeResidentialRequirement = demandPrefab.m_FreeResidentialRequirement;
		componentData.m_CommercialBaseDemand = demandPrefab.m_CommercialBaseDemand;
		componentData.m_IndustrialBaseDemand = demandPrefab.m_IndustrialBaseDemand;
		componentData.m_ExtractorBaseDemand = demandPrefab.m_ExtractorBaseDemand;
		componentData.m_CommuterWorkerRatioLimit = demandPrefab.m_CommuterWorkerRatioLimit;
		componentData.m_CommuterSlowSpawnFactor = demandPrefab.m_CommuterSlowSpawnFactor;
		componentData.m_CommuterOCSpawnParameters = demandPrefab.m_CommuterOCSpawnParameters;
		componentData.m_TouristOCSpawnParameters = demandPrefab.m_TouristOCSpawnParameters;
		componentData.m_CitizenOCSpawnParameters = demandPrefab.m_CitizenOCSpawnParameters;
		componentData.m_TeenSpawnPercentage = demandPrefab.m_TeenSpawnPercentage;
		componentData.m_FrameIntervalForSpawning = demandPrefab.m_FrameIntervalForSpawning;
		componentData.m_HouseholdSpawnSpeedFactor = demandPrefab.m_HouseholdSpawnSpeedFactor;
		componentData.m_HotelRoomPercentRequirement = demandPrefab.m_HotelRoomPercentRequirement;
		componentData.m_NewCitizenEducationParameters = demandPrefab.m_NewCitizenEducationParameters;
		((EntityManager)(ref entityManager)).SetComponentData<DemandParameterData>(val, componentData);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		DemandParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<DemandParameterData>(singletonEntity);
		componentData.m_MinimumHappiness = m_MinimumHappiness;
		componentData.m_HappinessEffect = m_HappinessEffect;
		componentData.m_TaxEffect = m_TaxEffect;
		componentData.m_StudentEffect = m_StudentEffect;
		componentData.m_AvailableWorkplaceEffect = m_AvailableWorkplaceEffect;
		componentData.m_HomelessEffect = m_HomelessEffect;
		componentData.m_NeutralHappiness = m_NeutralHappiness;
		componentData.m_NeutralUnemployment = m_NeutralUnemployment;
		componentData.m_NeutralAvailableWorkplacePercentage = m_NeutralAvailableWorkplacePercentage;
		componentData.m_NeutralHomelessness = m_NeutralHomelessness;
		componentData.m_FreeResidentialRequirement = m_FreeResidentialRequirement;
		componentData.m_CommercialBaseDemand = m_CommercialBaseDemand;
		componentData.m_IndustrialBaseDemand = m_IndustrialBaseDemand;
		componentData.m_ExtractorBaseDemand = m_ExtractorBaseDemand;
		componentData.m_CommuterWorkerRatioLimit = m_CommuterWorkerRatioLimit;
		componentData.m_CommuterSlowSpawnFactor = m_CommuterSlowSpawnFactor;
		componentData.m_CommuterOCSpawnParameters = m_CommuterOCSpawnParameters;
		componentData.m_TouristOCSpawnParameters = m_TouristOCSpawnParameters;
		componentData.m_CitizenOCSpawnParameters = m_CitizenOCSpawnParameters;
		componentData.m_TeenSpawnPercentage = m_TeenSpawnPercentage;
		componentData.m_FrameIntervalForSpawning = m_FrameIntervalForSpawning;
		componentData.m_HouseholdSpawnSpeedFactor = m_HouseholdSpawnSpeedFactor;
		componentData.m_HotelRoomPercentRequirement = m_HotelRoomPercentRequirement;
		componentData.m_NewCitizenEducationParameters = m_NewCitizenEducationParameters;
		((EntityManager)(ref entityManager)).SetComponentData<DemandParameterData>(singletonEntity, componentData);
		return deps;
	}
}

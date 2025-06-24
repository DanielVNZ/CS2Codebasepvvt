using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class OutsideTradeParametersMode : EntityQueryModePrefab
{
	public float m_ElectricityImportPrice;

	public float m_ElectricityExportPrice;

	public float m_WaterImportPrice;

	public float m_WaterExportPrice;

	public float m_WaterExportPollutionTolerance;

	public float m_SewageExportPrice;

	public float m_AirWeightMultiplierOverridden;

	public float m_RoadWeightMultiplierOverridden;

	public float m_TrainWeightMultiplierOverridden;

	public float m_ShipWeightMultiplierOverridden;

	public float m_AirDistanceMultiplierOverridden;

	public float m_RoadDistanceMultiplierOverridden;

	public float m_TrainDistanceMultiplierOverridden;

	public float m_ShipDistanceMultiplierOverridden;

	public float m_AmbulanceImportServiceFee;

	public float m_HearseImportServiceFee;

	public float m_FireEngineImportServiceFee;

	public float m_GarbageImportServiceFee;

	public float m_PoliceImportServiceFee;

	public int m_OCServiceTradePopulationRange;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OutsideTradeParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<OutsideTradeParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		OutsideTradeParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<OutsideTradeParameterData>(singletonEntity);
		componentData.m_ElectricityImportPrice = m_ElectricityImportPrice;
		componentData.m_ElectricityExportPrice = m_ElectricityExportPrice;
		componentData.m_WaterImportPrice = m_WaterImportPrice;
		componentData.m_WaterExportPrice = m_WaterExportPrice;
		componentData.m_WaterExportPollutionTolerance = m_WaterExportPollutionTolerance;
		componentData.m_SewageExportPrice = m_SewageExportPrice;
		componentData.m_AirWeightMultiplier = m_AirWeightMultiplierOverridden;
		componentData.m_RoadWeightMultiplier = m_RoadWeightMultiplierOverridden;
		componentData.m_TrainWeightMultiplier = m_TrainWeightMultiplierOverridden;
		componentData.m_ShipWeightMultiplier = m_ShipWeightMultiplierOverridden;
		componentData.m_AirDistanceMultiplier = m_AirDistanceMultiplierOverridden;
		componentData.m_RoadDistanceMultiplier = m_RoadDistanceMultiplierOverridden;
		componentData.m_TrainDistanceMultiplier = m_TrainDistanceMultiplierOverridden;
		componentData.m_ShipDistanceMultiplier = m_ShipDistanceMultiplierOverridden;
		componentData.m_AmbulanceImportServiceFee = m_AmbulanceImportServiceFee;
		componentData.m_HearseImportServiceFee = m_HearseImportServiceFee;
		componentData.m_FireEngineImportServiceFee = m_FireEngineImportServiceFee;
		componentData.m_GarbageImportServiceFee = m_GarbageImportServiceFee;
		componentData.m_PoliceImportServiceFee = m_PoliceImportServiceFee;
		componentData.m_OCServiceTradePopulationRange = m_OCServiceTradePopulationRange;
		((EntityManager)(ref entityManager)).SetComponentData<OutsideTradeParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		OutsideTradeParameterPrefab outsideTradeParameterPrefab = prefabSystem.GetPrefab<OutsideTradeParameterPrefab>(val);
		OutsideTradeParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<OutsideTradeParameterData>(val);
		componentData.m_ElectricityImportPrice = outsideTradeParameterPrefab.m_ElectricityImportPrice;
		componentData.m_ElectricityExportPrice = outsideTradeParameterPrefab.m_ElectricityExportPrice;
		componentData.m_WaterImportPrice = outsideTradeParameterPrefab.m_WaterImportPrice;
		componentData.m_WaterExportPrice = outsideTradeParameterPrefab.m_WaterExportPrice;
		componentData.m_WaterExportPollutionTolerance = outsideTradeParameterPrefab.m_WaterExportPollutionTolerance;
		componentData.m_SewageExportPrice = outsideTradeParameterPrefab.m_SewageExportPrice;
		componentData.m_AirWeightMultiplier = outsideTradeParameterPrefab.m_AirWeightMultiplier;
		componentData.m_RoadWeightMultiplier = outsideTradeParameterPrefab.m_RoadWeightMultiplier;
		componentData.m_TrainWeightMultiplier = outsideTradeParameterPrefab.m_TrainWeightMultiplier;
		componentData.m_ShipWeightMultiplier = outsideTradeParameterPrefab.m_ShipWeightMultiplier;
		componentData.m_AirDistanceMultiplier = outsideTradeParameterPrefab.m_AirDistanceMultiplier;
		componentData.m_RoadDistanceMultiplier = outsideTradeParameterPrefab.m_RoadDistanceMultiplier;
		componentData.m_TrainDistanceMultiplier = outsideTradeParameterPrefab.m_TrainDistanceMultiplier;
		componentData.m_ShipDistanceMultiplier = outsideTradeParameterPrefab.m_ShipDistanceMultiplier;
		componentData.m_AmbulanceImportServiceFee = outsideTradeParameterPrefab.m_AmbulanceImportServiceFee;
		componentData.m_HearseImportServiceFee = outsideTradeParameterPrefab.m_HearseImportServiceFee;
		componentData.m_FireEngineImportServiceFee = outsideTradeParameterPrefab.m_FireEngineImportServiceFee;
		componentData.m_GarbageImportServiceFee = outsideTradeParameterPrefab.m_GarbageImportServiceFee;
		componentData.m_PoliceImportServiceFee = outsideTradeParameterPrefab.m_PoliceImportServiceFee;
		componentData.m_OCServiceTradePopulationRange = outsideTradeParameterPrefab.m_OCServiceTradePopulationRange;
		((EntityManager)(ref entityManager)).SetComponentData<OutsideTradeParameterData>(val, componentData);
	}
}

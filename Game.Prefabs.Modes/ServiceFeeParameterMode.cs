using System;
using Colossal.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class ServiceFeeParameterMode : EntityQueryModePrefab
{
	public FeeParameters m_ElectricityFee;

	public AnimationCurve m_ElectricityFeeConsumptionMultiplier;

	public FeeParameters m_HealthcareFee;

	public FeeParameters m_BasicEducationFee;

	public FeeParameters m_SecondaryEducationFee;

	public FeeParameters m_HigherEducationFee;

	public FeeParameters m_WaterFee;

	public AnimationCurve m_WaterFeeConsumptionMultiplier;

	public FeeParameters m_GarbageFee;

	public int4 m_GarbageFeeRCIO;

	public FeeParameters m_FireResponseFee;

	public FeeParameters m_PoliceFee;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceFeeParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<ServiceFeeParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		ServiceFeeParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ServiceFeeParameterData>(singletonEntity);
		componentData.m_ElectricityFee = m_ElectricityFee;
		componentData.m_ElectricityFeeConsumptionMultiplier = new AnimationCurve1(m_ElectricityFeeConsumptionMultiplier);
		componentData.m_HealthcareFee = m_HealthcareFee;
		componentData.m_BasicEducationFee = m_BasicEducationFee;
		componentData.m_SecondaryEducationFee = m_SecondaryEducationFee;
		componentData.m_HigherEducationFee = m_HigherEducationFee;
		componentData.m_WaterFee = m_WaterFee;
		componentData.m_WaterFeeConsumptionMultiplier = new AnimationCurve1(m_WaterFeeConsumptionMultiplier);
		componentData.m_GarbageFee = m_GarbageFee;
		componentData.m_GarbageFeeRCIO = m_GarbageFeeRCIO;
		componentData.m_FireResponseFee = m_FireResponseFee;
		componentData.m_PoliceFee = m_PoliceFee;
		((EntityManager)(ref entityManager)).SetComponentData<ServiceFeeParameterData>(singletonEntity, componentData);
		return deps;
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		ServiceFeeParameterPrefab serviceFeeParameterPrefab = prefabSystem.GetPrefab<ServiceFeeParameterPrefab>(val);
		ServiceFeeParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ServiceFeeParameterData>(val);
		componentData.m_ElectricityFee = serviceFeeParameterPrefab.m_ElectricityFee;
		componentData.m_ElectricityFeeConsumptionMultiplier = new AnimationCurve1(serviceFeeParameterPrefab.m_ElectricityFeeConsumptionMultiplier);
		componentData.m_HealthcareFee = serviceFeeParameterPrefab.m_HealthcareFee;
		componentData.m_BasicEducationFee = serviceFeeParameterPrefab.m_BasicEducationFee;
		componentData.m_SecondaryEducationFee = serviceFeeParameterPrefab.m_SecondaryEducationFee;
		componentData.m_HigherEducationFee = serviceFeeParameterPrefab.m_HigherEducationFee;
		componentData.m_WaterFee = serviceFeeParameterPrefab.m_WaterFee;
		componentData.m_WaterFeeConsumptionMultiplier = new AnimationCurve1(serviceFeeParameterPrefab.m_WaterFeeConsumptionMultiplier);
		componentData.m_GarbageFee = serviceFeeParameterPrefab.m_GarbageFee;
		componentData.m_GarbageFeeRCIO = serviceFeeParameterPrefab.m_GarbageFeeRCIO;
		componentData.m_FireResponseFee = serviceFeeParameterPrefab.m_FireResponseFee;
		componentData.m_PoliceFee = serviceFeeParameterPrefab.m_PoliceFee;
		((EntityManager)(ref entityManager)).SetComponentData<ServiceFeeParameterData>(val, componentData);
	}
}

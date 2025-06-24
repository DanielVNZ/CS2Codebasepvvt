using System;
using Colossal.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Parameters/", new Type[] { })]
public class ElectricityParametersMode : EntityQueryModePrefab
{
	[Range(0f, 1f)]
	public float m_InitialBatteryCharge = 0.1f;

	public AnimationCurve m_TemperatureConsumptionMultiplier;

	[Range(0f, 1f)]
	public float m_CloudinessSolarPenalty = 0.25f;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityParameterData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<ElectricityParameterData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref requestedQuery)).GetSingletonEntity();
		ElectricityParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ElectricityParameterData>(singletonEntity);
		componentData.m_InitialBatteryCharge = m_InitialBatteryCharge;
		componentData.m_TemperatureConsumptionMultiplier = new AnimationCurve1(m_TemperatureConsumptionMultiplier);
		componentData.m_CloudinessSolarPenalty = m_CloudinessSolarPenalty;
		((EntityManager)(ref entityManager)).SetComponentData<ElectricityParameterData>(singletonEntity, componentData);
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Entity val = entities[0];
		ElectricityParametersPrefab electricityParametersPrefab = prefabSystem.GetPrefab<ElectricityParametersPrefab>(val);
		ElectricityParameterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ElectricityParameterData>(val);
		componentData.m_InitialBatteryCharge = electricityParametersPrefab.m_InitialBatteryCharge;
		componentData.m_TemperatureConsumptionMultiplier = new AnimationCurve1(electricityParametersPrefab.m_TemperatureConsumptionMultiplier);
		componentData.m_CloudinessSolarPenalty = electricityParametersPrefab.m_CloudinessSolarPenalty;
		((EntityManager)(ref entityManager)).SetComponentData<ElectricityParameterData>(val, componentData);
	}
}

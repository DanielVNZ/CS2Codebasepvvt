using System;
using System.Collections.Generic;
using Colossal.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class ElectricityParametersPrefab : PrefabBase
{
	[Tooltip("Initial charge of batteries")]
	[Range(0f, 1f)]
	public float m_InitialBatteryCharge = 0.1f;

	[Tooltip("Correlation between temperature (in °C) and electricity consumption")]
	public AnimationCurve m_TemperatureConsumptionMultiplier;

	[Tooltip("How much solar power plant electricity output is reduced when it is cloudy")]
	[Range(0f, 1f)]
	public float m_CloudinessSolarPenalty = 0.25f;

	public ServicePrefab m_ElectricityServicePrefab;

	public NotificationIconPrefab m_ElectricityNotificationPrefab;

	public NotificationIconPrefab m_LowVoltageNotConnectedPrefab;

	public NotificationIconPrefab m_HighVoltageNotConnectedPrefab;

	public NotificationIconPrefab m_BottleneckNotificationPrefab;

	public NotificationIconPrefab m_BuildingBottleneckNotificationPrefab;

	public NotificationIconPrefab m_NotEnoughProductionNotificationPrefab;

	public NotificationIconPrefab m_TransformerNotificationPrefab;

	public NotificationIconPrefab m_NotEnoughConnectedNotificationPrefab;

	public NotificationIconPrefab m_BatteryEmptyNotificationPrefab;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_ElectricityServicePrefab);
		prefabs.Add(m_ElectricityNotificationPrefab);
		prefabs.Add(m_LowVoltageNotConnectedPrefab);
		prefabs.Add(m_HighVoltageNotConnectedPrefab);
		prefabs.Add(m_BottleneckNotificationPrefab);
		prefabs.Add(m_BuildingBottleneckNotificationPrefab);
		prefabs.Add(m_NotEnoughProductionNotificationPrefab);
		prefabs.Add(m_TransformerNotificationPrefab);
		prefabs.Add(m_NotEnoughConnectedNotificationPrefab);
		prefabs.Add(m_BatteryEmptyNotificationPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ElectricityParameterData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<ElectricityParameterData>(entity, new ElectricityParameterData
		{
			m_InitialBatteryCharge = m_InitialBatteryCharge,
			m_TemperatureConsumptionMultiplier = new AnimationCurve1(m_TemperatureConsumptionMultiplier),
			m_CloudinessSolarPenalty = m_CloudinessSolarPenalty,
			m_ElectricityServicePrefab = orCreateSystemManaged.GetEntity(m_ElectricityServicePrefab),
			m_ElectricityNotificationPrefab = orCreateSystemManaged.GetEntity(m_ElectricityNotificationPrefab),
			m_LowVoltageNotConnectedPrefab = orCreateSystemManaged.GetEntity(m_LowVoltageNotConnectedPrefab),
			m_HighVoltageNotConnectedPrefab = orCreateSystemManaged.GetEntity(m_HighVoltageNotConnectedPrefab),
			m_BottleneckNotificationPrefab = orCreateSystemManaged.GetEntity(m_BottleneckNotificationPrefab),
			m_BuildingBottleneckNotificationPrefab = orCreateSystemManaged.GetEntity(m_BuildingBottleneckNotificationPrefab),
			m_NotEnoughProductionNotificationPrefab = orCreateSystemManaged.GetEntity(m_NotEnoughProductionNotificationPrefab),
			m_TransformerNotificationPrefab = orCreateSystemManaged.GetEntity(m_TransformerNotificationPrefab),
			m_NotEnoughConnectedNotificationPrefab = orCreateSystemManaged.GetEntity(m_NotEnoughConnectedNotificationPrefab),
			m_BatteryEmptyNotificationPrefab = orCreateSystemManaged.GetEntity(m_BatteryEmptyNotificationPrefab)
		});
	}
}

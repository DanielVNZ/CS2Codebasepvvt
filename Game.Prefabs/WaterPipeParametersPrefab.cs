using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class WaterPipeParametersPrefab : PrefabBase
{
	public PrefabBase m_WaterService;

	public NotificationIconPrefab m_WaterNotification;

	public NotificationIconPrefab m_DirtyWaterNotification;

	public NotificationIconPrefab m_SewageNotification;

	public NotificationIconPrefab m_WaterPipeNotConnectedNotification;

	public NotificationIconPrefab m_SewagePipeNotConnectedNotification;

	public NotificationIconPrefab m_NotEnoughWaterCapacityNotification;

	public NotificationIconPrefab m_NotEnoughSewageCapacityNotification;

	public NotificationIconPrefab m_NotEnoughGroundwaterNotification;

	public NotificationIconPrefab m_NotEnoughSurfaceWaterNotification;

	public NotificationIconPrefab m_DirtyWaterPumpNotification;

	public float m_GroundwaterReplenish = 0.004f;

	[Tooltip("How much the groundwater cell purifies itself per tick (2048 ticks per day)")]
	public int m_GroundwaterPurification = 1;

	public float m_GroundwaterUsageMultiplier = 0.1f;

	public float m_GroundwaterPumpEffectiveAmount = 4000f;

	public float m_SurfaceWaterUsageMultiplier = 5E-05f;

	public float m_SurfaceWaterPumpEffectiveDepth = 4f;

	[Tooltip("If the fresh water pollution exceeds this percentage, notifications will be shown on the pump/consumer")]
	[Range(0f, 1f)]
	public float m_MaxToleratedPollution = 0.1f;

	[Tooltip("The interval at which pollution spreads in pipes. Higher numbers = slower spread and faster cleanup")]
	[Range(1f, 32f)]
	public int m_WaterPipePollutionSpreadInterval = 5;

	[Tooltip("How much pollution is removed from water pipes without any flow, per tick")]
	[Range(0f, 1f)]
	public float m_StaleWaterPipePurification = 0.001f;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_WaterService);
		prefabs.Add(m_WaterNotification);
		prefabs.Add(m_DirtyWaterNotification);
		prefabs.Add(m_SewageNotification);
		prefabs.Add(m_WaterPipeNotConnectedNotification);
		prefabs.Add(m_SewagePipeNotConnectedNotification);
		prefabs.Add(m_NotEnoughWaterCapacityNotification);
		prefabs.Add(m_NotEnoughSewageCapacityNotification);
		prefabs.Add(m_NotEnoughGroundwaterNotification);
		prefabs.Add(m_NotEnoughSurfaceWaterNotification);
		prefabs.Add(m_DirtyWaterPumpNotification);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<WaterPipeParameterData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<WaterPipeParameterData>(entity, new WaterPipeParameterData
		{
			m_WaterService = orCreateSystemManaged.GetEntity(m_WaterService),
			m_WaterNotification = orCreateSystemManaged.GetEntity(m_WaterNotification),
			m_DirtyWaterNotification = orCreateSystemManaged.GetEntity(m_DirtyWaterNotification),
			m_SewageNotification = orCreateSystemManaged.GetEntity(m_SewageNotification),
			m_WaterPipeNotConnectedNotification = orCreateSystemManaged.GetEntity(m_WaterPipeNotConnectedNotification),
			m_SewagePipeNotConnectedNotification = orCreateSystemManaged.GetEntity(m_SewagePipeNotConnectedNotification),
			m_NotEnoughWaterCapacityNotification = orCreateSystemManaged.GetEntity(m_NotEnoughWaterCapacityNotification),
			m_NotEnoughSewageCapacityNotification = orCreateSystemManaged.GetEntity(m_NotEnoughSewageCapacityNotification),
			m_NotEnoughGroundwaterNotification = orCreateSystemManaged.GetEntity(m_NotEnoughGroundwaterNotification),
			m_NotEnoughSurfaceWaterNotification = orCreateSystemManaged.GetEntity(m_NotEnoughSurfaceWaterNotification),
			m_DirtyWaterPumpNotification = orCreateSystemManaged.GetEntity(m_DirtyWaterPumpNotification),
			m_GroundwaterReplenish = m_GroundwaterReplenish,
			m_GroundwaterPurification = m_GroundwaterPurification,
			m_GroundwaterUsageMultiplier = m_GroundwaterUsageMultiplier,
			m_GroundwaterPumpEffectiveAmount = m_GroundwaterPumpEffectiveAmount,
			m_SurfaceWaterUsageMultiplier = m_SurfaceWaterUsageMultiplier,
			m_SurfaceWaterPumpEffectiveDepth = m_SurfaceWaterPumpEffectiveDepth,
			m_MaxToleratedPollution = m_MaxToleratedPollution,
			m_WaterPipePollutionSpreadInterval = m_WaterPipePollutionSpreadInterval,
			m_StaleWaterPipePurification = m_StaleWaterPipePurification
		});
	}
}

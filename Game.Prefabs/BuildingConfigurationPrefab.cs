using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class BuildingConfigurationPrefab : PrefabBase
{
	[Tooltip("The building condition increase when building received enough upkeep cost, change 16 times per game day")]
	public int m_BuildingConditionIncrement = 30;

	[Tooltip("The building condition decrease when building can't pay enough upkeep cost, change 16 times per game day")]
	public int m_BuildingConditionDecrement = 1;

	public NotificationIconPrefab m_AbandonedCollapsedNotification;

	public NotificationIconPrefab m_AbandonedNotification;

	public NotificationIconPrefab m_CondemnedNotification;

	public NotificationIconPrefab m_LevelUpNotification;

	public NotificationIconPrefab m_TurnedOffNotification;

	public NetLanePrefab m_ElectricityConnectionLane;

	public NetLanePrefab m_SewageConnectionLane;

	public NetLanePrefab m_WaterConnectionLane;

	public uint m_AbandonedDestroyDelay;

	public NotificationIconPrefab m_HighRentNotification;

	public BrandPrefab m_DefaultRenterBrand;

	public AreaPrefab m_ConstructionSurface;

	public NetLanePrefab m_ConstructionBorder;

	public ObjectPrefab m_ConstructionObject;

	public ObjectPrefab m_CollapsedObject;

	public EffectPrefab m_CollapseVFX;

	public EffectPrefab m_CollapseSFX;

	public float m_CollapseSFXDensity = 0.1f;

	public AreaPrefab m_CollapsedSurface;

	public EffectPrefab m_FireLoopSFX;

	public EffectPrefab m_FireSpotSFX;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_AbandonedCollapsedNotification);
		prefabs.Add(m_AbandonedNotification);
		prefabs.Add(m_CondemnedNotification);
		prefabs.Add(m_LevelUpNotification);
		prefabs.Add(m_TurnedOffNotification);
		prefabs.Add(m_ElectricityConnectionLane);
		prefabs.Add(m_SewageConnectionLane);
		prefabs.Add(m_WaterConnectionLane);
		prefabs.Add(m_HighRentNotification);
		prefabs.Add(m_DefaultRenterBrand);
		prefabs.Add(m_ConstructionSurface);
		prefabs.Add(m_ConstructionBorder);
		prefabs.Add(m_ConstructionObject);
		prefabs.Add(m_CollapsedObject);
		prefabs.Add(m_CollapseVFX);
		prefabs.Add(m_CollapseSFX);
		prefabs.Add(m_CollapsedSurface);
		prefabs.Add(m_FireLoopSFX);
		prefabs.Add(m_FireSpotSFX);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<BuildingConfigurationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<BuildingConfigurationData>(entity, new BuildingConfigurationData
		{
			m_BuildingConditionIncrement = m_BuildingConditionIncrement,
			m_BuildingConditionDecrement = m_BuildingConditionDecrement,
			m_AbandonedCollapsedNotification = orCreateSystemManaged.GetEntity(m_AbandonedCollapsedNotification),
			m_AbandonedNotification = orCreateSystemManaged.GetEntity(m_AbandonedNotification),
			m_CondemnedNotification = orCreateSystemManaged.GetEntity(m_CondemnedNotification),
			m_LevelUpNotification = orCreateSystemManaged.GetEntity(m_LevelUpNotification),
			m_TurnedOffNotification = orCreateSystemManaged.GetEntity(m_TurnedOffNotification),
			m_ElectricityConnectionLane = orCreateSystemManaged.GetEntity(m_ElectricityConnectionLane),
			m_SewageConnectionLane = orCreateSystemManaged.GetEntity(m_SewageConnectionLane),
			m_WaterConnectionLane = orCreateSystemManaged.GetEntity(m_WaterConnectionLane),
			m_AbandonedDestroyDelay = m_AbandonedDestroyDelay,
			m_HighRentNotification = orCreateSystemManaged.GetEntity(m_HighRentNotification),
			m_DefaultRenterBrand = orCreateSystemManaged.GetEntity(m_DefaultRenterBrand),
			m_ConstructionSurface = orCreateSystemManaged.GetEntity(m_ConstructionSurface),
			m_ConstructionBorder = orCreateSystemManaged.GetEntity(m_ConstructionBorder),
			m_ConstructionObject = orCreateSystemManaged.GetEntity(m_ConstructionObject),
			m_CollapsedObject = orCreateSystemManaged.GetEntity(m_CollapsedObject),
			m_CollapseVFX = orCreateSystemManaged.GetEntity(m_CollapseVFX),
			m_CollapseSFX = orCreateSystemManaged.GetEntity(m_CollapseSFX),
			m_CollapseSFXDensity = m_CollapseSFXDensity,
			m_CollapsedSurface = orCreateSystemManaged.GetEntity(m_CollapsedSurface),
			m_FireLoopSFX = orCreateSystemManaged.GetEntity(m_FireLoopSFX),
			m_FireSpotSFX = orCreateSystemManaged.GetEntity(m_FireSpotSFX)
		});
	}
}

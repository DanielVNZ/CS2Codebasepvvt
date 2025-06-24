using System;
using System.Collections.Generic;
using Game.Zones;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/Unlocking/", new Type[] { })]
public class ZoneBuiltRequirementPrefab : UnlockRequirementPrefab
{
	public ThemePrefab m_RequiredTheme;

	public ZonePrefab m_RequiredZone;

	public AreaType m_RequiredType;

	public int m_MinimumSquares = 2500;

	public int m_MinimumCount;

	public int m_MinimumLevel = 1;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_RequiredTheme != (Object)null)
		{
			prefabs.Add(m_RequiredTheme);
		}
		if ((Object)(object)m_RequiredZone != (Object)null)
		{
			prefabs.Add(m_RequiredZone);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ZoneBuiltRequirementData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).GetBuffer<UnlockRequirement>(entity, false).Add(new UnlockRequirement(entity, UnlockFlags.RequireAll));
		ZoneBuiltRequirementData zoneBuiltRequirementData = default(ZoneBuiltRequirementData);
		if ((Object)(object)m_RequiredTheme != (Object)null)
		{
			zoneBuiltRequirementData.m_RequiredTheme = existingSystemManaged.GetEntity(m_RequiredTheme);
		}
		if ((Object)(object)m_RequiredZone != (Object)null)
		{
			zoneBuiltRequirementData.m_RequiredZone = existingSystemManaged.GetEntity(m_RequiredZone);
		}
		zoneBuiltRequirementData.m_RequiredType = m_RequiredType;
		zoneBuiltRequirementData.m_MinimumSquares = m_MinimumSquares;
		zoneBuiltRequirementData.m_MinimumCount = m_MinimumCount;
		zoneBuiltRequirementData.m_MinimumLevel = (byte)m_MinimumLevel;
		((EntityManager)(ref entityManager)).SetComponentData<ZoneBuiltRequirementData>(entity, zoneBuiltRequirementData);
	}
}

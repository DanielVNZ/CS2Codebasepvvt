using System;
using System.Collections.Generic;
using Game.Common;
using Game.Simulation;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class HappinessFactorParameterPrefab : PrefabBase
{
	[EnumValue(typeof(CitizenHappinessSystem.HappinessFactor))]
	public int[] m_BaseLevels = new int[25];

	[EnumValue(typeof(CitizenHappinessSystem.HappinessFactor))]
	public PrefabBase[] m_LockedEntities = new PrefabBase[25];

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_LockedEntities.Length; i++)
		{
			if ((Object)(object)m_LockedEntities[i] != (Object)null)
			{
				prefabs.Add(m_LockedEntities[i]);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<HappinessFactorParameterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
		DynamicBuffer<HappinessFactorParameterData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HappinessFactorParameterData>(entity, false);
		for (int i = 0; i < m_BaseLevels.Length; i++)
		{
			Entity lockedEntity = (((Object)(object)m_LockedEntities[i] != (Object)null) ? orCreateSystemManaged.GetEntity(m_LockedEntities[i]) : Entity.Null);
			buffer.Add(new HappinessFactorParameterData
			{
				m_BaseLevel = m_BaseLevels[i],
				m_LockedEntity = lockedEntity
			});
		}
	}
}

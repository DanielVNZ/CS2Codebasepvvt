using System;
using System.Collections.Generic;
using Game.City;
using Game.Triggers;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Triggers/", new Type[] { })]
public class StatisticTriggerPrefab : PrefabBase
{
	public StatisticTriggerType m_Type;

	public StatisticsPrefab m_StatisticPrefab;

	public int m_StatisticParameter;

	public StatisticsPrefab m_NormalizeWithPrefab;

	public int m_NormalizeWithParameter;

	public int m_TimeFrame = 1;

	public int m_MinSamples = 1;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_StatisticPrefab != (Object)null)
		{
			prefabs.Add(m_StatisticPrefab);
		}
		if ((Object)(object)m_NormalizeWithPrefab != (Object)null)
		{
			prefabs.Add(m_NormalizeWithPrefab);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TriggerData>());
		components.Add(ComponentType.ReadWrite<StatisticTriggerData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		StatisticTriggerData statisticTriggerData = new StatisticTriggerData
		{
			m_Type = m_Type
		};
		if ((Object)(object)m_StatisticPrefab != (Object)null)
		{
			statisticTriggerData.m_StatisticEntity = orCreateSystemManaged.GetEntity(m_StatisticPrefab);
		}
		statisticTriggerData.m_StatisticParameter = m_StatisticParameter;
		if ((Object)(object)m_NormalizeWithPrefab != (Object)null)
		{
			statisticTriggerData.m_NormalizeWithPrefab = orCreateSystemManaged.GetEntity(m_NormalizeWithPrefab);
		}
		statisticTriggerData.m_NormalizeWithParameter = m_NormalizeWithParameter;
		statisticTriggerData.m_TimeFrame = m_TimeFrame;
		statisticTriggerData.m_MinSamples = m_MinSamples;
		if (((Object)(object)m_StatisticPrefab != (Object)null && m_StatisticPrefab.m_CollectionType == StatisticCollectionType.Daily) || ((Object)(object)m_NormalizeWithPrefab != (Object)null && m_NormalizeWithPrefab.m_CollectionType == StatisticCollectionType.Daily))
		{
			statisticTriggerData.m_MinSamples = math.max(statisticTriggerData.m_MinSamples, 32 + math.max(0, m_TimeFrame - 1));
		}
		((EntityManager)(ref entityManager)).SetComponentData<StatisticTriggerData>(entity, statisticTriggerData);
		((EntityManager)(ref entityManager)).GetBuffer<TriggerData>(entity, false).Add(new TriggerData
		{
			m_TriggerType = TriggerType.StatisticsValue,
			m_TargetTypes = TargetType.Nothing,
			m_TriggerPrefab = entity
		});
	}
}

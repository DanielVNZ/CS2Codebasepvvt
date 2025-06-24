using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Triggers/", new Type[]
{
	typeof(TriggerPrefab),
	typeof(StatisticTriggerPrefab)
})]
public class TriggerCondition : ComponentBase
{
	[SerializeField]
	public TriggerConditionData[] m_Conditions;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (m_Conditions != null && m_Conditions.Length != 0)
		{
			components.Add(ComponentType.ReadWrite<TriggerConditionData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (m_Conditions != null && m_Conditions.Length != 0)
		{
			DynamicBuffer<TriggerConditionData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TriggerConditionData>(entity, false);
			for (int i = 0; i < m_Conditions.Length; i++)
			{
				buffer.Add(m_Conditions[i]);
			}
		}
	}
}

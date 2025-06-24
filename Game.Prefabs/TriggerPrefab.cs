using System;
using System.Collections.Generic;
using Game.Triggers;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Triggers/", new Type[] { })]
public class TriggerPrefab : PrefabBase
{
	public TriggerType m_TriggerType;

	public PrefabBase[] m_TriggerPrefabs;

	[EnumFlag]
	public TargetType m_TargetTypes;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_TriggerPrefabs == null)
		{
			return;
		}
		PrefabBase[] triggerPrefabs = m_TriggerPrefabs;
		foreach (PrefabBase prefabBase in triggerPrefabs)
		{
			if ((Object)(object)prefabBase != (Object)null)
			{
				prefabs.Add(prefabBase);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TriggerData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<TriggerData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TriggerData>(entity, false);
		if (m_TriggerPrefabs != null && m_TriggerPrefabs.Length != 0)
		{
			PrefabBase[] triggerPrefabs = m_TriggerPrefabs;
			foreach (PrefabBase prefabBase in triggerPrefabs)
			{
				if ((Object)(object)prefabBase != (Object)null)
				{
					buffer.Add(new TriggerData
					{
						m_TriggerType = m_TriggerType,
						m_TargetTypes = m_TargetTypes,
						m_TriggerPrefab = existingSystemManaged.GetEntity(prefabBase)
					});
				}
			}
		}
		else
		{
			buffer.Add(new TriggerData
			{
				m_TriggerType = m_TriggerType,
				m_TargetTypes = m_TargetTypes
			});
		}
	}
}

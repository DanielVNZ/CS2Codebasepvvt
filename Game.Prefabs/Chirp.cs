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
public class Chirp : ComponentBase
{
	[Tooltip("When the trigger happens, one of these chirps will be selected randomly")]
	public PrefabBase[] m_Chirps;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TriggerChirpData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<TriggerChirpData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TriggerChirpData>(entity, false);
		if (m_Chirps == null || m_Chirps.Length == 0)
		{
			return;
		}
		PrefabBase[] chirps = m_Chirps;
		foreach (PrefabBase prefabBase in chirps)
		{
			if ((Object)(object)prefabBase != (Object)null)
			{
				buffer.Add(new TriggerChirpData
				{
					m_Chirp = existingSystemManaged.GetEntity(prefabBase)
				});
			}
		}
	}
}

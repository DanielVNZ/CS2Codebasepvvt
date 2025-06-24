using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[RequireComponent(typeof(UnlockableBase))]
[ComponentMenu("Prefabs/Unlocking/", new Type[]
{
	typeof(TutorialPrefab),
	typeof(TutorialPhasePrefab),
	typeof(TutorialTriggerPrefabBase),
	typeof(TutorialListPrefab)
})]
public class ForceUIGroupUnlock : ComponentBase
{
	[Tooltip("UIGroups listed here will unlock whenever this prefab unlocks, regardless if their own unlock requirements have been met.")]
	public UIGroupPrefab[] m_Unlocks;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ForceUIGroupUnlockData>());
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
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<ForceUIGroupUnlockData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ForceUIGroupUnlockData>(entity, false);
		for (int i = 0; i < m_Unlocks.Length; i++)
		{
			Entity entity2 = existingSystemManaged.GetEntity(m_Unlocks[i]);
			buffer.Add(new ForceUIGroupUnlockData
			{
				m_Entity = entity2
			});
		}
	}
}

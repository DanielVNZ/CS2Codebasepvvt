using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/Unlocking/", new Type[] { typeof(UIGroupPrefab) })]
public class ForceUnlockRequirement : ComponentBase
{
	public PrefabBase m_Prefab;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ForceUnlockRequirementData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if ((Object)(object)m_Prefab != (Object)null && ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>().TryGetEntity(m_Prefab, out var entity2))
		{
			ForceUnlockRequirementData forceUnlockRequirementData = default(ForceUnlockRequirementData);
			forceUnlockRequirementData.m_Prefab = entity2;
			((EntityManager)(ref entityManager)).SetComponentData<ForceUnlockRequirementData>(entity, forceUnlockRequirementData);
		}
	}
}

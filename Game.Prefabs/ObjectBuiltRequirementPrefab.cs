using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/Unlocking/", new Type[] { })]
public class ObjectBuiltRequirementPrefab : UnlockRequirementPrefab
{
	public int m_MinimumCount = 1;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ObjectBuiltRequirementData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).GetBuffer<UnlockRequirement>(entity, false).Add(new UnlockRequirement(entity, UnlockFlags.RequireAll));
		ObjectBuiltRequirementData objectBuiltRequirementData = new ObjectBuiltRequirementData
		{
			m_MinimumCount = m_MinimumCount
		};
		((EntityManager)(ref entityManager)).SetComponentData<ObjectBuiltRequirementData>(entity, objectBuiltRequirementData);
	}
}

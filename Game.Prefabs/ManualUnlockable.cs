using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/Unlocking/", new Type[] { })]
public class ManualUnlockable : UnlockableBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity, List<PrefabBase> dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetBuffer<UnlockRequirement>(entity, false).Add(new UnlockRequirement(entity, UnlockFlags.RequireAll));
		base.LateInitialize(entityManager, entity, dependencies);
	}
}

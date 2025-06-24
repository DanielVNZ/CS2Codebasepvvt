using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public abstract class UIGroupPrefab : PrefabBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UIGroupElement>());
		components.Add(ComponentType.ReadWrite<UnlockRequirement>());
		components.Add(ComponentType.ReadWrite<Locked>());
	}

	public void AddElement(EntityManager entityManager, Entity entity)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Entity entity2 = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>().GetEntity(this);
		((EntityManager)(ref entityManager)).GetBuffer<UIGroupElement>(entity2, false).Add(new UIGroupElement(entity));
		((EntityManager)(ref entityManager)).GetBuffer<UnlockRequirement>(entity2, false).Add(new UnlockRequirement(entity, UnlockFlags.RequireAny));
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetBuffer<UnlockRequirement>(entity, false).Add(new UnlockRequirement(entity, UnlockFlags.RequireAny));
		base.LateInitialize(entityManager, entity);
	}
}

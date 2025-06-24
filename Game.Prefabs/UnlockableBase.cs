using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

public abstract class UnlockableBase : ComponentBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public virtual void LateInitialize(EntityManager entityManager, Entity entity, List<PrefabBase> dependencies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		LateInitialize(entityManager, entity);
	}

	public static void DefaultLateInitialize(EntityManager entityManager, Entity entity, List<PrefabBase> dependencies)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<UnlockRequirement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<UnlockRequirement>(entity, false);
		for (int i = 0; i < dependencies.Count; i++)
		{
			PrefabBase prefabBase = dependencies[i];
			if (existingSystemManaged.IsUnlockable(prefabBase))
			{
				Entity entity2 = existingSystemManaged.GetEntity(prefabBase);
				buffer.Add(new UnlockRequirement(entity2, UnlockFlags.RequireAll));
			}
		}
	}
}

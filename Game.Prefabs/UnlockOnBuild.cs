using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/Unlocking/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab),
	typeof(NetPrefab),
	typeof(StaticObjectPrefab)
})]
public class UnlockOnBuild : ComponentBase
{
	public ObjectBuiltRequirementPrefab[] m_Unlocks;

	public override bool ignoreUnlockDependencies => true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<UnlockOnBuildData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Unlocks.Length; i++)
		{
			prefabs.Add(m_Unlocks[i]);
		}
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
		DynamicBuffer<UnlockOnBuildData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<UnlockOnBuildData>(entity, false);
		for (int i = 0; i < m_Unlocks.Length; i++)
		{
			Entity entity2 = existingSystemManaged.GetEntity(m_Unlocks[i]);
			buffer.Add(new UnlockOnBuildData
			{
				m_Entity = entity2
			});
		}
	}
}

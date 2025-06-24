using System;
using System.Collections.Generic;
using Game.Creatures;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(ObjectPrefab) })]
public class CreatureSpawner : ComponentBase
{
	public int m_MaxGroupCount = 3;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceholderObjectElement>());
		components.Add(ComponentType.ReadWrite<CreatureSpawnData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Creatures.CreatureSpawner>());
		components.Add(ComponentType.ReadWrite<OwnedCreature>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		CreatureSpawnData creatureSpawnData = default(CreatureSpawnData);
		creatureSpawnData.m_MaxGroupCount = m_MaxGroupCount;
		((EntityManager)(ref entityManager)).SetComponentData<CreatureSpawnData>(entity, creatureSpawnData);
	}
}

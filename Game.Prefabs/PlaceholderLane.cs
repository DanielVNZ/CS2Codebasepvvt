using System;
using System.Collections.Generic;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetLanePrefab) })]
public class PlaceholderLane : ComponentBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceholderObjectElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Placeholder>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (base.prefab.Has<SpawnableLane>())
		{
			ComponentBase.baseLog.WarnFormat((Object)(object)base.prefab, "PlaceholderLane is SpawnableLane: {0}", (object)((Object)base.prefab).name);
		}
	}
}

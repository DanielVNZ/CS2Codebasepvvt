using System;
using System.Collections.Generic;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(ObjectPrefab) })]
public class PlaceholderObject : ComponentBase
{
	public bool m_RandomizeGroupIndex;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceholderObjectElement>());
		components.Add(ComponentType.ReadWrite<PlaceholderObjectData>());
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
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (base.prefab.Has<SpawnableObject>())
		{
			ComponentBase.baseLog.WarnFormat((Object)(object)base.prefab, "PlaceholderObject is SpawnableObject: {0}", (object)((Object)base.prefab).name);
		}
		PlaceholderObjectData placeholderObjectData = new PlaceholderObjectData
		{
			m_RandomizeGroupIndex = m_RandomizeGroupIndex
		};
		((EntityManager)(ref entityManager)).SetComponentData<PlaceholderObjectData>(entity, placeholderObjectData);
	}
}

using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/", new Type[] { })]
public class ToBeRemoved : ComponentBase
{
	public PrefabBase m_ReplaceWith;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		ComponentBase.baseLog.WarnFormat((Object)(object)base.prefab, "Loading prefab that is set to be removed ({0})", (object)((Object)base.prefab).name);
	}
}

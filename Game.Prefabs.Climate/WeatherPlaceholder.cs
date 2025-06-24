using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Climate;

[ComponentMenu("Weather/", new Type[] { typeof(WeatherPrefab) })]
public class WeatherPlaceholder : ComponentBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceholderObjectElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (base.prefab.Has<WeatherOverride>())
		{
			ComponentBase.baseLog.WarnFormat((Object)(object)base.prefab, "WeatherPlaceholder is WeatherOverride: {0}", (object)((Object)base.prefab).name);
		}
	}
}

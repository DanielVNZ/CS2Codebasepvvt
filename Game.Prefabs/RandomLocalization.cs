using System;
using System.Collections.Generic;
using Game.Common;
using Game.SceneFlow;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Localization/", new Type[] { })]
public class RandomLocalization : Localization
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<LocalizationCount>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<RandomLocalizationIndex>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		int localizationCount = GetLocalizationCount();
		((EntityManager)(ref entityManager)).GetBuffer<LocalizationCount>(entity, false).Add(new LocalizationCount
		{
			m_Count = localizationCount
		});
	}

	protected virtual int GetLocalizationCount()
	{
		return GetLocalizationIndexCount(base.prefab, m_LocalizationID);
	}

	public static int GetLocalizationIndexCount(PrefabBase prefab, string id)
	{
		int num = -1;
		if (id != null && GameManager.instance.localizationManager.activeDictionary.indexCounts.TryGetValue(id, out var value))
		{
			num = value;
		}
		if (num < 1)
		{
			ComponentBase.baseLog.WarnFormat((Object)(object)prefab, "Warning: localizationID {0} not found for {1}", (object)id, (object)((Object)prefab).name);
		}
		return num;
	}
}

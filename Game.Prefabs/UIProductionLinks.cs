using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { typeof(ResourcePrefab) })]
public class UIProductionLinks : ComponentBase
{
	public UIProductionLinkPrefab m_Producer;

	[CanBeNull]
	public UIProductionLinkPrefab[] m_FinalConsumers;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_Producer != (Object)null)
		{
			prefabs.Add(m_Producer);
		}
		if (m_FinalConsumers == null)
		{
			return;
		}
		for (int i = 0; i < m_FinalConsumers.Length; i++)
		{
			UIProductionLinkPrefab uIProductionLinkPrefab = m_FinalConsumers[i];
			if ((Object)(object)uIProductionLinkPrefab != (Object)null)
			{
				prefabs.Add(uIProductionLinkPrefab);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<UIProductionLinksData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}

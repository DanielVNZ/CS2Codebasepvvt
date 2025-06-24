using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.Annotations;
using Colossal.OdinSerializer.Utilities;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UIMultiTagPrefab : PrefabBase
{
	[Tooltip("If set, the override is used as a UI tag instead of the generated tag.")]
	[CanBeNull]
	public string m_Override;

	[Tooltip("This prefab allows adding multiple tags in Tutorials, Tutorial Phases and Tutorial Triggers. The tags are treated as separate but hierarchically equal by the Tutorials system.")]
	public PrefabBase[] m_UITagProviders;

	public override string uiTag
	{
		get
		{
			if (!StringExtensions.IsNullOrWhitespace(m_Override))
			{
				return m_Override;
			}
			if (m_UITagProviders == null)
			{
				return base.uiTag;
			}
			return string.Join("|", m_UITagProviders.Select((PrefabBase t) => t.uiTag));
		}
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_UITagProviders != null)
		{
			for (int i = 0; i < m_UITagProviders.Length; i++)
			{
				prefabs.Add(m_UITagProviders[i]);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> prefabComponents)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(prefabComponents);
		prefabComponents.Add(ComponentType.ReadWrite<UITagPrefabData>());
	}
}

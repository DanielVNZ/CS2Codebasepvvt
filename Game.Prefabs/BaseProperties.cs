using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { typeof(RenderPrefab) })]
public class BaseProperties : ComponentBase
{
	public RenderPrefab m_BaseType;

	public bool m_UseMinBounds;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_BaseType != (Object)null)
		{
			prefabs.Add(m_BaseType);
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}
}

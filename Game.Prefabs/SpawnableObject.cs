using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(ObjectPrefab) })]
public class SpawnableObject : ComponentBase
{
	public ObjectPrefab[] m_Placeholders;

	[Range(0f, 100f)]
	public int m_Probability = 100;

	public GroupPrefab m_RandomizationGroup;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Placeholders.Length; i++)
		{
			prefabs.Add(m_Placeholders[i]);
		}
		if ((Object)(object)m_RandomizationGroup != (Object)null)
		{
			prefabs.Add(m_RandomizationGroup);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SpawnableObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}

using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Themes/", new Type[]
{
	typeof(ZonePrefab),
	typeof(ObjectPrefab),
	typeof(NetPrefab),
	typeof(AreaPrefab),
	typeof(RoutePrefab),
	typeof(NetLanePrefab)
})]
public class ThemeObject : ComponentBase
{
	public ThemePrefab m_Theme;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			if ((Object)(object)m_Theme != (Object)null)
			{
				yield return ((Object)m_Theme).name;
			}
		}
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_Theme);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ObjectRequirementElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<ObjectRequirementElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ObjectRequirementElement>(entity, false);
		int length = buffer.Length;
		buffer.Add(new ObjectRequirementElement(existingSystemManaged.GetEntity(m_Theme), length, ObjectRequirementType.IgnoreExplicit));
	}
}

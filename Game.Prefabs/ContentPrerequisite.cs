using System;
using System.Collections.Generic;
using Game.UI.Editor;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[HideInEditor]
[ComponentMenu("Prefabs/Content/", new Type[] { })]
public class ContentPrerequisite : ComponentBase
{
	public ContentPrefab m_ContentPrerequisite;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_ContentPrerequisite);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ContentPrerequisiteData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if ((Object)(object)m_ContentPrerequisite != (Object)null)
		{
			PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
			((EntityManager)(ref entityManager)).SetComponentData<ContentPrerequisiteData>(entity, new ContentPrerequisiteData
			{
				m_ContentPrerequisite = existingSystemManaged.GetEntity(m_ContentPrerequisite)
			});
		}
	}
}

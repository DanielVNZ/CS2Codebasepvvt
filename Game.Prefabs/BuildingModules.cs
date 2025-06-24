using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
public class BuildingModules : ComponentBase
{
	public PrefabBase[] m_Modules;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Modules != null)
		{
			for (int i = 0; i < m_Modules.Length; i++)
			{
				prefabs.Add(m_Modules[i]);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<BuildingModule>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (m_Modules == null)
		{
			return;
		}
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_Modules.Length; i++)
		{
			PrefabBase prefabBase = m_Modules[i];
			if (!((Object)(object)prefabBase == (Object)null))
			{
				Entity entity2 = existingSystemManaged.GetEntity(prefabBase);
				((EntityManager)(ref entityManager)).AddComponent<BuildingModuleData>(entity2);
				((EntityManager)(ref entityManager)).GetBuffer<BuildingModule>(entity, false).Add(new BuildingModule(entity2));
			}
		}
	}
}

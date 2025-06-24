using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Activation/", new Type[]
{
	typeof(TutorialPrefab),
	typeof(TutorialListPrefab)
})]
public class TutorialAutoActivation : TutorialActivation
{
	[CanBeNull]
	public PrefabBase m_RequiredUnlock;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_RequiredUnlock != (Object)null)
		{
			prefabs.Add(m_RequiredUnlock);
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AutoActivationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		Entity requiredUnlock = Entity.Null;
		if ((Object)(object)m_RequiredUnlock != (Object)null)
		{
			requiredUnlock = existingSystemManaged.GetEntity(m_RequiredUnlock);
		}
		((EntityManager)(ref entityManager)).SetComponentData<AutoActivationData>(entity, new AutoActivationData
		{
			m_RequiredUnlock = requiredUnlock
		});
	}
}

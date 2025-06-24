using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/", new Type[] { typeof(TutorialPrefab) })]
public class TutorialAlternative : ComponentBase
{
	[Tooltip("If one of the alternatives is completed, this is skipped in tutorial list.")]
	[NotNull]
	public TutorialPrefab[] m_Alternatives;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Tutorials.TutorialAlternative>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		TutorialPrefab[] alternatives = m_Alternatives;
		foreach (TutorialPrefab item in alternatives)
		{
			prefabs.Add(item);
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		DynamicBuffer<Game.Tutorials.TutorialAlternative> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Game.Tutorials.TutorialAlternative>(entity, false);
		TutorialPrefab[] alternatives = m_Alternatives;
		foreach (TutorialPrefab tutorialPrefab in alternatives)
		{
			buffer.Add(new Game.Tutorials.TutorialAlternative
			{
				m_Alternative = orCreateSystemManaged.GetEntity(tutorialPrefab)
			});
		}
	}
}

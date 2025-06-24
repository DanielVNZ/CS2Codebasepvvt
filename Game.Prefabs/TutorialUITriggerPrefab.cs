using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Triggers/", new Type[] { })]
public class TutorialUITriggerPrefab : TutorialTriggerPrefabBase
{
	[Serializable]
	public class UITriggerInfo
	{
		[NotNull]
		public PrefabBase m_UITagProvider;

		[CanBeNull]
		[Tooltip("If set, advances the tutorial to the specified phase when triggered.\n\nOverrides TutorialPhasePrefab GoToPhase.")]
		public TutorialPhasePrefab m_GoToPhase;

		public bool m_DisableBlinking;

		public bool m_CompleteManually;
	}

	[NotNull]
	public UITriggerInfo[] m_UITriggers;

	public override bool phaseBranching => m_UITriggers.Any((UITriggerInfo t) => (Object)(object)t.m_GoToPhase != (Object)null);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UITriggerData>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_UITriggers.Length; i++)
		{
			prefabs.Add(m_UITriggers[i].m_UITagProvider);
			if ((Object)(object)m_UITriggers[i].m_GoToPhase != (Object)null)
			{
				prefabs.Add(m_UITriggers[i].m_GoToPhase);
			}
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (m_UITriggers.Length <= 1)
		{
			return;
		}
		PrefabSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_UITriggers.Length; i++)
		{
			TutorialPhasePrefab goToPhase = m_UITriggers[i].m_GoToPhase;
			if ((Object)(object)goToPhase != (Object)null)
			{
				((EntityManager)(ref entityManager)).AddComponent<TutorialPhaseBranch>(existingSystemManaged.GetEntity(goToPhase));
			}
		}
	}

	protected override void GenerateBlinkTags()
	{
		base.GenerateBlinkTags();
		for (int i = 0; i < m_UITriggers.Length; i++)
		{
			if (!m_UITriggers[i].m_DisableBlinking)
			{
				string[] array = m_UITriggers[i].m_UITagProvider.uiTag.Split('|', StringSplitOptions.None);
				foreach (string text in array)
				{
					AddBlinkTag(text.Trim());
				}
			}
		}
	}

	public override void GenerateTutorialLinks(EntityManager entityManager, NativeParallelHashSet<Entity> linkedPrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.GenerateTutorialLinks(entityManager, linkedPrefabs);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_UITriggers.Length; i++)
		{
			linkedPrefabs.Add(existingSystemManaged.GetEntity(m_UITriggers[i].m_UITagProvider));
		}
	}
}

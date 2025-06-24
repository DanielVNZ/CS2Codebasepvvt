using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Prefabs;

public abstract class TutorialPhasePrefab : PrefabBase
{
	[Flags]
	public enum ControlScheme
	{
		KeyboardAndMouse = 1,
		Gamepad = 2,
		All = 3
	}

	public string m_Image;

	public string m_OverrideImagePS;

	public string m_OverrideImageXBox;

	public string m_Icon;

	public bool m_TitleVisible = true;

	[FormerlySerializedAs("m_ShowDescription")]
	public bool m_DescriptionVisible = true;

	public bool m_CanDeactivate;

	public ControlScheme m_ControlScheme = ControlScheme.All;

	[CanBeNull]
	public TutorialTriggerPrefabBase m_Trigger;

	public float m_OverrideCompletionDelay = -1f;

	public override bool ignoreUnlockDependencies => true;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_Trigger != (Object)null)
		{
			prefabs.Add(m_Trigger);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TutorialPhaseData>());
		if ((Object)(object)m_Trigger != (Object)null)
		{
			components.Add(ComponentType.ReadWrite<TutorialTrigger>());
		}
		if (m_CanDeactivate)
		{
			components.Add(ComponentType.ReadWrite<TutorialPhaseCanDeactivate>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		if (((EntityManager)(ref entityManager)).HasComponent<TutorialTrigger>(entity))
		{
			((EntityManager)(ref entityManager)).SetComponentData<TutorialTrigger>(entity, new TutorialTrigger
			{
				m_Trigger = existingSystemManaged.GetEntity(m_Trigger)
			});
		}
	}

	public virtual void GenerateTutorialLinks(EntityManager entityManager, NativeParallelHashSet<Entity> linkedPrefabs)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_Trigger != (Object)null)
		{
			m_Trigger.GenerateTutorialLinks(entityManager, linkedPrefabs);
		}
	}
}

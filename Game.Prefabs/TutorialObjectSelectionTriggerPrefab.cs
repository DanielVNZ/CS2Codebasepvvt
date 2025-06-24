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
public class TutorialObjectSelectionTriggerPrefab : TutorialTriggerPrefabBase
{
	[Serializable]
	public class ObjectSelectionTriggerInfo
	{
		[NotNull]
		public PrefabBase m_Trigger;

		[CanBeNull]
		public TutorialPhasePrefab m_GoToPhase;
	}

	[NotNull]
	public ObjectSelectionTriggerInfo[] m_Triggers;

	public override bool phaseBranching => m_Triggers.Any((ObjectSelectionTriggerInfo t) => (Object)(object)t.m_GoToPhase != (Object)null);

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Triggers.Length; i++)
		{
			prefabs.Add(m_Triggers[i].m_Trigger);
			if ((Object)(object)m_Triggers[i].m_GoToPhase != (Object)null)
			{
				prefabs.Add(m_Triggers[i].m_GoToPhase);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ObjectSelectionTriggerData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<ObjectSelectionTriggerData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ObjectSelectionTriggerData>(entity, false);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_Triggers.Length; i++)
		{
			ObjectSelectionTriggerInfo objectSelectionTriggerInfo = m_Triggers[i];
			Entity entity2 = existingSystemManaged.GetEntity(objectSelectionTriggerInfo.m_Trigger);
			Entity goToPhase = (((Object)(object)objectSelectionTriggerInfo.m_GoToPhase == (Object)null) ? Entity.Null : existingSystemManaged.GetEntity(objectSelectionTriggerInfo.m_GoToPhase));
			buffer.Add(new ObjectSelectionTriggerData
			{
				m_Prefab = entity2,
				m_GoToPhase = goToPhase
			});
		}
		if (m_Triggers.Length <= 1)
		{
			return;
		}
		for (int j = 0; j < m_Triggers.Length; j++)
		{
			TutorialPhasePrefab goToPhase2 = m_Triggers[j].m_GoToPhase;
			if ((Object)(object)goToPhase2 != (Object)null)
			{
				((EntityManager)(ref entityManager)).AddComponent<TutorialPhaseBranch>(existingSystemManaged.GetEntity(goToPhase2));
			}
		}
	}

	public override void GenerateTutorialLinks(EntityManager entityManager, NativeParallelHashSet<Entity> linkedPrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.GenerateTutorialLinks(entityManager, linkedPrefabs);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_Triggers.Length; i++)
		{
			ObjectSelectionTriggerInfo objectSelectionTriggerInfo = m_Triggers[i];
			linkedPrefabs.Add(existingSystemManaged.GetEntity(objectSelectionTriggerInfo.m_Trigger));
		}
	}
}

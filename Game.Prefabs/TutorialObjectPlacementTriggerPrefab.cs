using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Triggers/", new Type[] { })]
public class TutorialObjectPlacementTriggerPrefab : TutorialTriggerPrefabBase
{
	[Serializable]
	public class ObjectPlacementTarget
	{
		[NotNull]
		public PrefabBase m_Target;

		public ObjectPlacementTriggerFlags m_Flags;
	}

	[NotNull]
	public ObjectPlacementTarget[] m_Targets;

	public int m_RequiredCount;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		ObjectPlacementTarget[] targets = m_Targets;
		foreach (ObjectPlacementTarget objectPlacementTarget in targets)
		{
			prefabs.Add(objectPlacementTarget.m_Target);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ObjectPlacementTriggerData>());
		components.Add(ComponentType.ReadWrite<ObjectPlacementTriggerCountData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<ObjectPlacementTriggerData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ObjectPlacementTriggerData>(entity, false);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		ObjectPlacementTarget[] targets = m_Targets;
		foreach (ObjectPlacementTarget objectPlacementTarget in targets)
		{
			if (existingSystemManaged.TryGetEntity(objectPlacementTarget.m_Target, out var entity2))
			{
				buffer.Add(new ObjectPlacementTriggerData(entity2, objectPlacementTarget.m_Flags));
			}
		}
		((EntityManager)(ref entityManager)).SetComponentData<ObjectPlacementTriggerCountData>(entity, new ObjectPlacementTriggerCountData(math.max(m_RequiredCount, 1)));
	}

	protected override void GenerateBlinkTags()
	{
		base.GenerateBlinkTags();
		ObjectPlacementTarget[] targets = m_Targets;
		foreach (ObjectPlacementTarget objectPlacementTarget in targets)
		{
			if (objectPlacementTarget.m_Target.TryGet<UIObject>(out var component) && component.m_Group is UIAssetCategoryPrefab uIAssetCategoryPrefab && (Object)(object)uIAssetCategoryPrefab.m_Menu != (Object)null)
			{
				AddBlinkTagAtPosition(objectPlacementTarget.m_Target.uiTag, 0);
				AddBlinkTagAtPosition(uIAssetCategoryPrefab.uiTag, 1);
				AddBlinkTagAtPosition(uIAssetCategoryPrefab.m_Menu.uiTag, 2);
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
		for (int i = 0; i < m_Targets.Length; i++)
		{
			linkedPrefabs.Add(existingSystemManaged.GetEntity(m_Targets[i].m_Target));
		}
	}
}

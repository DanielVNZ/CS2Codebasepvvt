using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Triggers/", new Type[] { })]
public class TutorialAreaTriggerPrefab : TutorialTriggerPrefabBase
{
	[NotNull]
	public AreaPrefab[] m_Targets;

	public AreaTriggerFlags m_Flags = AreaTriggerFlags.Created | AreaTriggerFlags.Modified;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		AreaPrefab[] targets = m_Targets;
		foreach (AreaPrefab item in targets)
		{
			prefabs.Add(item);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AreaTriggerData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<AreaTriggerData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AreaTriggerData>(entity, false);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		AreaPrefab[] targets = m_Targets;
		foreach (AreaPrefab areaPrefab in targets)
		{
			if (existingSystemManaged.TryGetEntity(areaPrefab, out var entity2))
			{
				buffer.Add(new AreaTriggerData(entity2, m_Flags));
			}
		}
	}

	protected override void GenerateBlinkTags()
	{
		base.GenerateBlinkTags();
		AreaPrefab[] targets = m_Targets;
		foreach (AreaPrefab areaPrefab in targets)
		{
			if (areaPrefab.TryGet<UIObject>(out var component) && component.m_Group is UIAssetCategoryPrefab uIAssetCategoryPrefab && (Object)(object)uIAssetCategoryPrefab.m_Menu != (Object)null)
			{
				AddBlinkTagAtPosition(areaPrefab.uiTag, 0);
				AddBlinkTagAtPosition(uIAssetCategoryPrefab.uiTag, 1);
				AddBlinkTagAtPosition(uIAssetCategoryPrefab.m_Menu.uiTag, 2);
			}
		}
	}

	public override void GenerateTutorialLinks(EntityManager entityManager, NativeParallelHashSet<Entity> linkedPrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.GenerateTutorialLinks(entityManager, linkedPrefabs);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		AreaPrefab[] targets = m_Targets;
		foreach (AreaPrefab areaPrefab in targets)
		{
			linkedPrefabs.Add(existingSystemManaged.GetEntity(areaPrefab));
		}
	}
}

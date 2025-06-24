using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Triggers/", new Type[] { })]
public class TutorialZoningTriggerPrefab : TutorialTriggerPrefabBase
{
	[NotNull]
	public ZonePrefab[] m_Zones;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		ZonePrefab[] zones = m_Zones;
		foreach (ZonePrefab zonePrefab in zones)
		{
			if ((Object)(object)zonePrefab != (Object)null)
			{
				prefabs.Add(zonePrefab);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ZoningTriggerData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<ZoningTriggerData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ZoningTriggerData>(entity, false);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		ZonePrefab[] zones = m_Zones;
		foreach (ZonePrefab zonePrefab in zones)
		{
			buffer.Add(new ZoningTriggerData(existingSystemManaged.GetEntity(zonePrefab)));
		}
	}

	protected override void GenerateBlinkTags()
	{
		base.GenerateBlinkTags();
		ZonePrefab[] zones = m_Zones;
		foreach (ZonePrefab zonePrefab in zones)
		{
			if (zonePrefab.TryGet<UIObject>(out var component) && component.m_Group is UIAssetCategoryPrefab uIAssetCategoryPrefab && (Object)(object)uIAssetCategoryPrefab.m_Menu != (Object)null)
			{
				AddBlinkTagAtPosition(zonePrefab.uiTag, 0);
				AddBlinkTagAtPosition(uIAssetCategoryPrefab.uiTag, 1);
				AddBlinkTagAtPosition(uIAssetCategoryPrefab.m_Menu.uiTag, 2);
			}
		}
	}

	public override void GenerateTutorialLinks(EntityManager entityManager, NativeParallelHashSet<Entity> linkedPrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.GenerateTutorialLinks(entityManager, linkedPrefabs);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_Zones.Length; i++)
		{
			linkedPrefabs.Add(existingSystemManaged.GetEntity(m_Zones[i]));
		}
	}
}

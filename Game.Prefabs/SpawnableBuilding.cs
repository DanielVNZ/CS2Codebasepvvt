using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Common;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
public class SpawnableBuilding : ComponentBase
{
	public ZonePrefab m_ZoneType;

	[Range(1f, 5f)]
	public byte m_Level;

	public override bool ignoreUnlockDependencies => true;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			if ((Object)(object)m_ZoneType != (Object)null && m_ZoneType.TryGet<UIObject>(out var component) && (Object)(object)component.m_Group != (Object)null && component.m_Group is UIAssetCategoryPrefab uIAssetCategoryPrefab)
			{
				yield return "SpawnableBuilding" + ((Object)uIAssetCategoryPrefab).name;
			}
		}
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_ZoneType);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SpawnableBuildingData>());
		components.Add(ComponentType.ReadWrite<BuildingSpawnGroupData>());
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			m_ZoneType.GetBuildingPrefabComponents(components, (BuildingPrefab)base.prefab, m_Level);
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<BuildingCondition>());
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			if (m_ZoneType.Has<RandomLocalization>())
			{
				components.Add(ComponentType.ReadWrite<RandomLocalizationIndex>());
			}
			m_ZoneType.GetBuildingArchetypeComponents(components, (BuildingPrefab)base.prefab, m_Level);
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			m_ZoneType.InitializeBuilding(entityManager, entity, (BuildingPrefab)base.prefab, m_Level);
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		SpawnableBuildingData spawnableBuildingData = new SpawnableBuildingData
		{
			m_Level = m_Level
		};
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			spawnableBuildingData.m_ZonePrefab = existingSystemManaged.GetEntity(m_ZoneType);
		}
		((EntityManager)(ref entityManager)).SetComponentData<SpawnableBuildingData>(entity, spawnableBuildingData);
	}
}

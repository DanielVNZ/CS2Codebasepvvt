using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class TutorialsConfigurationPrefab : PrefabBase
{
	[NotNull]
	public TutorialListPrefab m_TutorialsIntroList;

	[NotNull]
	public FeaturePrefab m_MapTilesPrefab;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_TutorialsIntroList);
		prefabs.Add(m_MapTilesPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TutorialsConfigurationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		Entity entity2 = existingSystemManaged.GetEntity(m_TutorialsIntroList);
		Entity entity3 = existingSystemManaged.GetEntity(m_MapTilesPrefab);
		((EntityManager)(ref entityManager)).SetComponentData<TutorialsConfigurationData>(entity, new TutorialsConfigurationData
		{
			m_TutorialsIntroList = entity2,
			m_MapTilesFeature = entity3
		});
	}
}

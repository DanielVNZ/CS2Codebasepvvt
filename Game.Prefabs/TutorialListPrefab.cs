using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/", new Type[] { })]
public class TutorialListPrefab : PrefabBase
{
	public int m_Priority;

	[NotNull]
	public TutorialPrefab[] m_Tutorials;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		TutorialPrefab[] tutorials = m_Tutorials;
		foreach (TutorialPrefab item in tutorials)
		{
			prefabs.Add(item);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TutorialListData>());
		components.Add(ComponentType.ReadWrite<TutorialRef>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<TutorialListData>(entity, new TutorialListData(m_Priority));
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<TutorialRef> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TutorialRef>(entity, false);
		TutorialPrefab[] tutorials = m_Tutorials;
		foreach (TutorialPrefab tutorialPrefab in tutorials)
		{
			Entity entity2 = existingSystemManaged.GetEntity(tutorialPrefab);
			TutorialRef tutorialRef = new TutorialRef
			{
				m_Tutorial = entity2
			};
			buffer.Add(tutorialRef);
		}
	}
}

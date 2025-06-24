using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Activation/", new Type[] { typeof(TutorialPrefab) })]
public class TutorialInfoviewActivation : TutorialActivation
{
	[NotNull]
	public InfoviewPrefab m_Infoview;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_Infoview);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewActivationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>().TryGetEntity(m_Infoview, out var entity2))
		{
			((EntityManager)(ref entityManager)).SetComponentData<InfoviewActivationData>(entity, new InfoviewActivationData(entity2));
		}
	}

	public override void GenerateTutorialLinks(EntityManager entityManager, NativeParallelHashSet<Entity> linkedPrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.GenerateTutorialLinks(entityManager, linkedPrefabs);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		linkedPrefabs.Add(existingSystemManaged.GetEntity(m_Infoview));
	}
}

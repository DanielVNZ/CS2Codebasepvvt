using System.Collections.Generic;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;

namespace Game.Prefabs;

public abstract class TutorialActivation : ComponentBase
{
	public override bool ignoreUnlockDependencies => true;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TutorialActivationData>());
	}

	public virtual void GenerateTutorialLinks(EntityManager entityManager, NativeParallelHashSet<Entity> linkedPrefabs)
	{
	}
}

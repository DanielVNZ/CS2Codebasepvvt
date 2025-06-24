using System;
using System.Collections.Generic;
using Game.Tutorials;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/", new Type[] { typeof(TutorialPhasePrefab) })]
public class ForceTutorialCompletion : ComponentBase
{
	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Tutorials.ForceTutorialCompletion>());
	}
}

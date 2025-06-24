using System;
using System.Collections.Generic;
using Game.Tutorials;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Triggers/", new Type[] { })]
public class TutorialUpgradeTriggerPrefab : TutorialTriggerPrefabBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UpgradeTriggerData>());
	}
}

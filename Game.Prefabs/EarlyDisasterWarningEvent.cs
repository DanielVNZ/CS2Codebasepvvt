using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class EarlyDisasterWarningEvent : ComponentBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<EarlyDisasterWarningEventData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}

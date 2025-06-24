using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

public abstract class UnlockRequirementPrefab : PrefabBase
{
	public string m_LabelID;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UnlockRequirementData>());
	}
}

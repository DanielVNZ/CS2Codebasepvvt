using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

public class ChirperAccount : PrefabBase
{
	public InfoviewPrefab m_InfoView;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ChirperAccountData>());
	}
}

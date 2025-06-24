using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UIProductionLinkPrefab : PrefabBase
{
	public ProductionChainActorType m_Type;

	public string m_Icon;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UIProductionLinkData>());
	}
}

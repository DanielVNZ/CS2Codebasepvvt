using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetPrefab) })]
public class NetUpgrade : ComponentBase
{
	public NetPieceRequirements[] m_SetState;

	public NetPieceRequirements[] m_UnsetState;

	public bool m_Standalone;

	public bool m_Underground;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceableNetData>());
		components.Add(ComponentType.ReadWrite<PlaceableInfoviewItem>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (components.Contains(ComponentType.ReadWrite<NetCompositionData>()))
		{
			components.Add(ComponentType.ReadWrite<PlaceableNetComposition>());
		}
	}
}
